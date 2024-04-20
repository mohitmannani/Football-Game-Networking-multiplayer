
using UnityEngine;
using System.Collections;


public class Sphere : MonoBehaviour {
	
	public GameObject owner;	// 
	public GameObject inputPlayer;	// 
	public GameObject lastInputPlayer;  
    private GameObject[] players;
	private GameObject[] oponents;
	public Transform shadowBall;
	public Transform blobPlayerSelected;
	public float timeToSelectAgain = 0.0f;
	public GameObject lastCandidatePlayer;
	
	[HideInInspector]	
	public float fHorizontal;
	[HideInInspector]	
	public float fVertical;
	[HideInInspector]	
	public bool bPassButton;
	[HideInInspector]	
	public bool bShootButton;
	[HideInInspector]
	public bool bShootButtonFinished;
	[HideInInspector]		
	public bool pressingShootButton = false;
	[HideInInspector]	
	public bool pressingPassButton = false;
	[HideInInspector]	
	public bool pressingShootButtonEnded = false;
	
	public InGameState_Script inGame;
	public float timeShootButtonPressed = 0.0f;

    public bool bStealButton;


    // Use this for initialization
    void Start () {
		players = GameObject.FindGameObjectsWithTag("PlayerTeam1");		
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");    
		inGame = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InGameState_Script>();
		blobPlayerSelected = GameObject.FindGameObjectWithTag("PlayerSelected").transform;        	
    }


	void LateUpdate() {
	
		shadowBall.position = new Vector3( transform.position.x, 0.35f ,transform.position.z );
		shadowBall.rotation = Quaternion.identity;

    }
	
	
	void Update () {

		
		// get input
		fVertical = Input.GetAxis("Vertical");
		fHorizontal = Input.GetAxis("Horizontal");

		bPassButton = Input.GetKey(KeyCode.S) || pressingPassButton; 
		bShootButton = Input.GetKey(KeyCode.D) || pressingShootButton; 



        if ( Input.GetKeyUp(KeyCode.D) || pressingShootButtonEnded) {
            bShootButtonFinished = true;
		}



        if ( bShootButton ) {
			timeShootButtonPressed += Time.deltaTime;
		} else {
			timeShootButtonPressed = 0.0f;
		}

        
        if (owner)
        {
            transform.position = owner.transform.position + owner.transform.forward / 1.5f + owner.transform.up / 5.0f;
            float velocity = owner.GetComponent<Player_Script>().actualVelocityPlayer.magnitude; 
            
            if (fVertical == 0.0f && fHorizontal == 0.0f && owner.tag == "PlayerTeam1")
            {
                velocity = 0.0f;
                gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            }

            transform.Rotate(owner.transform.right, velocity * 200.0f);

        }


       
        if (inGame.state == InGameState_Script.InGameState.PLAYING)
        {
            ActivateNearestPlayer();
            if (!owner || owner.tag == "PlayerTeam1") 
                ActivateNearestOponent();
        }
        else if (owner && owner.tag == "PlayerTeam1") // Check if the player controlled by the user is near the ball
        {
            float distanceToBall = Vector3.Distance(owner.transform.position, transform.position);
            if (distanceToBall < 2.0f) 
            {
                ActivateNearestPlayer();
            }
        }


        bStealButton = Input.GetKeyDown(KeyCode.Space);

        if (inGame.state == InGameState_Script.InGameState.PLAYING)
        {
            ActivateNearestPlayer();
            if (!owner || owner.tag == "PlayerTeam1")
                ActivateNearestOponent();
        }
        else if (owner && owner.tag == "PlayerTeam1")
        {
            float distanceToBall = Vector3.Distance(owner.transform.position, transform.position);
            if (distanceToBall < 2.0f)
            {
                ActivateNearestPlayer();
            }
        }

        if (bStealButton && owner && owner.tag == "PlayerTeam1")
        {
            StealBallFromOpponent();
        }




    }



    void StealBallFromOpponent()
    {
        if (owner)
        {
            Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, 2.0f); 
            foreach (Collider collider in hitColliders)
            {
                if (collider.CompareTag("OponentTeam"))
                {
                    GameObject opponent = collider.gameObject;
                    opponent.GetComponent<Player_Script>().state = Player_Script.Player_State.STOLE_BALL;
                    break;
                }
            }
        }
    }


    void ActivateNearestOponent()
    {
        float distance = 100000.0f;
        GameObject candidatePlayer = null;
        foreach (GameObject oponent in oponents)
        {

            if (!oponent.GetComponent<Player_Script>().temporallyUnselectable) 
            {

                oponent.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;

                Vector3 relativePos = transform.InverseTransformPoint(oponent.transform.position); 
                
                float newdistance = relativePos.magnitude; 
               
                if (newdistance < distance)
                {

                    distance = newdistance;
                    candidatePlayer = oponent;

                }
            }

        }


        if (candidatePlayer) 
            candidatePlayer.GetComponent<Player_Script>().state = Player_Script.Player_State.STOLE_BALL;


    }
   

    void ActivateNearestPlayer()
    {

       
        lastInputPlayer = inputPlayer;

        float distance = 1000000.0f;
        GameObject candidatePlayer = null;
        foreach (GameObject player in players)
        {

            if (!player.GetComponent<Player_Script>().temporallyUnselectable)
            {

                Vector3 relativePos = transform.InverseTransformPoint(player.transform.position);
                    
                float newdistance = relativePos.magnitude;

                if (newdistance < distance)
                {
                    distance = newdistance;
                    candidatePlayer = player;
                }
            }

        }




        timeToSelectAgain += Time.deltaTime;  
        if (timeToSelectAgain > 0.5f)
        {
            inputPlayer = candidatePlayer;
            timeToSelectAgain = 0.0f;
        }else 
        {
            candidatePlayer = lastCandidatePlayer;
        }

        lastCandidatePlayer = candidatePlayer;


        if (inputPlayer != null) 
        {
          
            blobPlayerSelected.transform.position = new Vector3(candidatePlayer.transform.position.x, candidatePlayer.transform.position.y + 0.1f, candidatePlayer.transform.position.z);
            Quaternion quat = Quaternion.identity;
            blobPlayerSelected.transform.LookAt(new Vector3(blobPlayerSelected.position.x + fHorizontal, blobPlayerSelected.position.y, blobPlayerSelected.position.z + fVertical));


            
            if (inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.PASSING &&
                 inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.SHOOTING &&
                 inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.PICK_BALL &&
                inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.CHANGE_DIRECTION
                )
            {
                inputPlayer.GetComponent<Player_Script>().state = Player_Script.Player_State.CONTROLLING;
            }
        }
    }
}


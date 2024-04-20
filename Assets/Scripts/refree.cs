using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class refree : MonoBehaviour
{
    public Transform ball;
    public float moveSpeed = 1f;
    public float distanceToBall = .9f;

    private Animation refereeAnimation;

    private void Start()
    {
        refereeAnimation = GetComponent<Animation>();
        refereeAnimation["run"].speed = 1.2f;
        refereeAnimation["repose"].speed = 1.0f;
        refereeAnimation["change_sense"].speed = 1.3f;
    }

    private void Update()
    {
        Vector3 targetDirection = (ball.position + new Vector3(5,5,5) ) - transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();

        Debug.Log("direction " +targetDirection);

        if (targetDirection.magnitude > distanceToBall)
        {

            transform.position += targetDirection * moveSpeed * Time.deltaTime;

            refereeAnimation.CrossFade("run", 0.1f, PlayMode.StopAll);


            refereeAnimation.CrossFade("repose", 0.1f, PlayMode.StopAll);


            refereeAnimation.CrossFadeQueued("change_sense", 0.1f, QueueMode.CompleteOthers);

            Debug.Log(transform.position);
        }

            // Update referee's rotation
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.5f);
        
    }
}
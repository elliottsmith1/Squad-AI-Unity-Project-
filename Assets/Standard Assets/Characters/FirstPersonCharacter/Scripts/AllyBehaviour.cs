using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBehaviour : MonoBehaviour {

	private Vector3 targetTransform = new Vector3(0, 1.2f, 0);
	public float speed = 2.0f;
	private UnityEngine.AI.NavMeshAgent allyAI;
    public float minRange = 15;


    // Use this for initialization
    void Start () {
		targetTransform = transform.position;

		allyAI = GetComponent<UnityEngine.AI.NavMeshAgent> ();  
	}
	
	// Update is called once per frame
	void Update () {

        //if (transform.position != targetTransform) 
        if (Vector3.Distance(transform.position, targetTransform) > minRange)
        {
            allyAI.SetDestination(targetTransform);
		}
			
	}

	public void newPosition(Vector3 newPos)
	{
		targetTransform = newPos;
	}
}

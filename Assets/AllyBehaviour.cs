using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBehaviour : MonoBehaviour {

	private Vector3 targetTransform = new Vector3(0, 2, 0);
	public float speed = 2.0f;
	private UnityEngine.AI.NavMeshAgent allyAI;

	// Use this for initialization
	void Start () {
		targetTransform = transform.position;

		allyAI = GetComponent<UnityEngine.AI.NavMeshAgent> ();  
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position != targetTransform) 
		{
			float step = speed * Time.deltaTime;
			//transform.position = Vector3.MoveTowards(transform.position, targetTransform, step);
		}
			
	}

	void newPosition(Vector3 newPos)
	{
		targetTransform = newPos;
	}
}

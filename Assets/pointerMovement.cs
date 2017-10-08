using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointerMovement : MonoBehaviour {

    private bool dir = false;
    private float speed = 0.3f;
    private Vector3 targetPos;

	// Use this for initialization
	void Start () {
        targetPos = transform.position;
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (dir)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }

        else
        {
            transform.Translate(-Vector2.up * speed * Time.deltaTime);
        }

        if (transform.position.y >= 0.6f)
        {
            dir = false;
        }

        if (transform.position.y <= 0.3f)
        {
            dir = true;
        }
    }

    void FixedUpdate()
    {
        //RaycastHit hit;

        //Ray ray = Camera.main.transform.forward(Input.mousePosition);

        //if (Physics.Raycast(ray, hit))
        //{
        //    targetPos = hit.transform.position;

        //    targetPos.y = transform.position.y;

        //    transform.position = targetPos;
        //}

        targetPos = Camera.main.transform.position + Camera.main.transform.forward * 5;

        targetPos.y = transform.position.y;

        transform.position = targetPos;
    }
}

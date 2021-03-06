﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointerMovement : MonoBehaviour {

    private bool dir = false;
    private float speed = 0.3f;
    private Vector3 targetPos;
    private int numObjects = 0;

	// Use this for initialization
	void Start ()
    {
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
        targetPos = Camera.main.transform.position + Camera.main.transform.forward * 7;

        targetPos.y = transform.position.y;

        transform.position = targetPos;
    }

    void OnTriggerEnter(Collider c)
    {
        if ((c.tag == "Ally") || (c.tag == "Enemy") || (c.tag == "Cover") || (c.tag == "Environment"))
        {
            numObjects++;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if ((c.tag == "Ally") || (c.tag == "Enemy") || (c.tag == "Cover") || (c.tag == "Environment"))
        {
            numObjects--;
        }
    }

    public bool GetEmptySpace()
    {
        if (numObjects > 0)
        {
            return false;
        }

        else
        {
            return true;
        }
    }
}

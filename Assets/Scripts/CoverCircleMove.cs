using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCircleMove : MonoBehaviour {

    private bool dir = false;
    private float speed = 0.5f;
    [SerializeField] float height = 1;

    // Use this for initialization
    void Start()
    {
        Material colourMat = GetComponent<Renderer>().material;

        Color colour = colourMat.color;

        colour.a = 0.01f;

        GetComponent<Renderer>().material.color = colour;
    }

    // Update is called once per frame
    void Update()
    {
        if (dir)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }

        else
        {
            transform.Translate(-Vector2.up * speed * Time.deltaTime);
        }

        if (transform.position.y >= height)
        {
            dir = false;
        }

        if (transform.position.y <= 0)
        {
            dir = true;
        }
    }
}

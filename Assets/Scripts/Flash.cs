using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour {

	private Renderer rend_ref;

	private float alph = 1;

	private bool up = false;

	// Use this for initialization
	void Start () {
		rend_ref = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Color newCol = rend_ref.material.color;

		newCol.a = alph;

		rend_ref.material.color = newCol;

		if (up) 
		{
			alph += Time.deltaTime * 0.4f;

			if (alph > 0.4) 
			{
				up = false;
			}
		} 

		else 
		{
			alph -= Time.deltaTime * 0.4f;

			if (alph < 0) 
			{
				up = true;
			}
		}
	}
}

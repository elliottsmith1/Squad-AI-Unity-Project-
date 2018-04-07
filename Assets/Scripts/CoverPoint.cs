using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour {

    public bool Occupied = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Ally")
        {
            Occupied = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAfterTime : MonoBehaviour {

    private float deathTimer = 0;
    public float timeToDeath = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        deathTimer += Time.deltaTime;

        if (deathTimer > timeToDeath)
        {
            Destroy(this.gameObject);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c)
    {
        if (((c.tag == "Enemy") && (tag == "Bullet")) || ((c.tag == "Ally") && (tag == "EnemyBullet")))
        {
            if ((c.GetComponent<AllyBehaviour>().state == AllyState.COVER) || (c.GetComponent<AllyBehaviour>().state == AllyState.COVERSHOOTING))
            {
                c.GetComponent<AllyBehaviour>().NewHealth(-5);
            }

            else
            {
                c.GetComponent<AllyBehaviour>().NewHealth(-10);
            }

            Destroy(this.gameObject);
        }

        else if ((c.tag == "Cover") || (c.tag == "Environment"))
        {
            Destroy(this.gameObject);
        }
    }
}

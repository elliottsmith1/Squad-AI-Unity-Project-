using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDetection : MonoBehaviour {

    void OnTriggerEnter(Collider c)
    {
		if ((tag == "EnemyBullet") && (c.gameObject.tag == "Target"))
		{
			if (Vector3.Distance (c.gameObject.transform.position, transform.position) < 3.0f) 
			{
				c.gameObject.GetComponent<Target> ().DamageTarget (1);
				Destroy (this.gameObject);
			}
		}

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

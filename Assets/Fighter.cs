using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour {

    public GameObject enemy;
    public Animator anim;

    private int lookSpeed = 8;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (enemy)
        {
            //transform.LookAt(enemy.transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(enemy.transform.position - transform.position)), Time.deltaTime * lookSpeed);
        }
    }

    public void newEnemy(GameObject enmy)
    {
        enemy = enmy;
        anim.SetBool("CombatShoot", true);
    }

    public void NoEnemy()
    {
        anim.SetBool("CombatShoot", false);
    }
}

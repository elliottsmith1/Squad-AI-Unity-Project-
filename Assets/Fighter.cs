using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour {

    private GameObject enemy;
    private AllyBehaviour soldier;

    private int lookSpeed = 8;

    // Use this for initialization
    void Start ()
    {
        soldier = GetComponent<AllyBehaviour>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (enemy)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(enemy.transform.position - transform.position)), Time.deltaTime * lookSpeed);
        }
    }

    public void newEnemy(GameObject enmy)
    {
        enemy = enmy;

        soldier.NewEnemy();        
    }

    public void NoEnemy()
    {
        enemy = null;

        soldier.NoEnemy();
    }
}

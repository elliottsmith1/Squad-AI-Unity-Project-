using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour {

    public GameObject enemy;
    private AllyBehaviour soldier;

    private int lookSpeed = 10;

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
            if ((soldier.state == AllyState.SHOOTING) || (soldier.state == AllyState.COVERSHOOTING))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(enemy.transform.position - transform.position)), Time.deltaTime * lookSpeed);                
            }
        }

        else
        {
            if ((soldier.state == AllyState.SHOOTING) || (soldier.state == AllyState.COVERSHOOTING))
            {
                soldier.NoEnemy();
            }
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

    public GameObject GetEnemy()
    {
        return enemy;
    }

    public bool GetMovingToCover()
    {
        return soldier.movingToCover;
    }
}

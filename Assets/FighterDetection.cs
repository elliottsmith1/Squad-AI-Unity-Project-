using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDetection : MonoBehaviour {

    GameObject parentGameobject;

	// Use this for initialization
	void Start ()
    {
        parentGameobject = transform.parent.gameObject;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //void OnTriggerEnter(Collider c)
    //{
    //    if ((parentGameobject.tag == "Enemy") && (c.tag == "Ally"))
    //    {
    //        parentGameobject.GetComponent<Fighter>().newEnemy(c.transform.gameObject);
    //    }

    //    else if ((parentGameobject.tag == "Ally") && (c.tag == "Enemy"))
    //    {
    //        parentGameobject.GetComponent<Fighter>().newEnemy(c.transform.gameObject);
    //    }
    //}

    void OnTriggerExit(Collider c)
    {
        if ((parentGameobject.tag == "Enemy") && (c.tag == "Ally"))
        {
            parentGameobject.GetComponent<Fighter>().newEnemy(null);
            parentGameobject.GetComponent<Fighter>().NoEnemy();
        }

        else if ((parentGameobject.tag == "Ally") && (c.tag == "Enemy"))
        {
            parentGameobject.GetComponent<Fighter>().newEnemy(null);
            parentGameobject.GetComponent<Fighter>().NoEnemy();
        }
    }

    void OnTriggerStay(Collider c)
    {
        if (parentGameobject.GetComponent<Fighter>().GetEnemy() == null)
        {
            if (!parentGameobject.GetComponent<Fighter>().GetMovingToCover())
            {
                if ((parentGameobject.tag == "Enemy") && (c.tag == "Ally"))
                {
                    parentGameobject.GetComponent<Fighter>().newEnemy(c.transform.gameObject);
                }

                else if ((parentGameobject.tag == "Ally") && (c.tag == "Enemy"))
                {
                    parentGameobject.GetComponent<Fighter>().newEnemy(c.transform.gameObject);
                }
            }
        }
    }
 }
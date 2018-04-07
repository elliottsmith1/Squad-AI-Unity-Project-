using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDetection : MonoBehaviour {

    GameObject parentGameobject;
	List<GameObject> nearby_covers = new List<GameObject>();

	// Use this for initialization
	void Start ()
    {
        parentGameobject = transform.parent.gameObject;		
	}

    void OnTriggerExit(Collider c)
    {
		if (c.gameObject.tag == "Cover") 
		{
			nearby_covers.Remove (c.gameObject);
		}	
    }

    void OnTriggerStay(Collider c)
    {
        if (parentGameobject.GetComponent<Fighter>().GetEnemy() == null)
        {
            if (!parentGameobject.GetComponent<Fighter>().GetMovingToCover())
            {
                if ((parentGameobject.tag == "Enemy") && ((c.tag == "Ally") || (c.tag == "Target")))
                {
                    parentGameobject.GetComponent<Fighter>().newEnemy(c.transform.gameObject);
                }

                else if ((parentGameobject.tag == "Ally") && (c.tag == "Enemy"))
                {
					if (nearby_covers.Count > 0)
					{
						if (parentGameobject.GetComponent<AllyBehaviour> ().state != AllyState.COVER) 
						{
							bool getCover = false;

							for (int i = 0; i < nearby_covers.Count; i++) 
							{
								if (Vector3.Distance (nearby_covers [i].transform.position, transform.position) < 10) {
									getCover = true;
								}
							}
								if (getCover) 
								{
									parentGameobject.GetComponent<Fighter> ().NoEnemy ();			

									parentGameobject.GetComponent<AllyBehaviour> ().FindCover ();									
								}
							return;
						}
					}

                    parentGameobject.GetComponent<Fighter>().newEnemy(c.transform.gameObject);
                }
            }
        }
    }

	void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.tag == "Cover") 
		{
			nearby_covers.Add(c.gameObject);
		}
	}
 }
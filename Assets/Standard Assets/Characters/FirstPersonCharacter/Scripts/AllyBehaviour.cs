using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBehaviour : MonoBehaviour {

	public Vector3 targetTransform = new Vector3(0, 1.0f, 0);
	public float speed = 2.0f;
	private UnityEngine.AI.NavMeshAgent allyAI;
    public float minRange = 5;
    public bool following = false;
    private float allyDistance = 0.2f;

    private GameObject player;
    List<GameObject> allies = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        foreach (GameObject ally in GameObject.FindGameObjectsWithTag("Ally"))
        {
            if (ally != this)
            {
                allies.Add(ally);
            }
        }

        stopMoving();

        allyAI = GetComponent<UnityEngine.AI.NavMeshAgent> ();

        player = GameObject.FindGameObjectWithTag("Player");        
    }
	
	// Update is called once per frame
	void Update () {

        foreach (GameObject ally in allies)
        {
            if (Vector3.Distance(transform.position, ally.transform.position) < allyDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, ally.transform.position, -1 * speed * Time.deltaTime);
            }
        }

        if (following)
        {
            targetTransform = player.transform.position;
        }

        if (Vector3.Distance(transform.position, targetTransform) > minRange)
        {
            allyAI.SetDestination(targetTransform);
		}

        else
        {
            stopMoving();
        }		
	}

	public void newPosition(Vector3 newPos)
	{
		targetTransform = newPos;
	}

    public void stopMoving()
    {
        targetTransform = transform.position;
    }

    public void FindCover()
    {
        GameObject[] covers;
        covers = GameObject.FindGameObjectsWithTag("Cover");

        GameObject closest = null;
        float closestDis = 1000.0f; ;

        foreach (GameObject cover in covers)
        {
            float dis = Vector3.Distance(transform.position, cover.transform.position);

            if (dis < closestDis)
            {
                int spots = 3;
                foreach (Transform child in cover.transform)
                {
                    if (child.tag == "CoverPoint")
                    {
                        if (child.GetComponent<CoverPoint>().Occupied == true)
                        {
                            spots--;
                        }
                    }
                }

                if (spots > 0)
                {
                    closestDis = dis;
                    closest = cover;
                }
            }
        }

        if (closest != null)
        {
            foreach (Transform child in closest.transform)
            {
                if (child.tag == "CoverPoint")
                {
                    if (child.GetComponent<CoverPoint>().Occupied == false)
                    {
                        child.GetComponent<CoverPoint>().Occupied = true;
                        newPosition(child.transform.position);
                        return;
                    }
                }
            }
        }
    }    
}

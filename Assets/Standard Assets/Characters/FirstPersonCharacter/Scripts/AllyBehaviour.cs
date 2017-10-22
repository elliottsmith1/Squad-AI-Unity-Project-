using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyState
{
    COVER,
    FOLLOWING,
    STOPPED,
    MOVING,
    SHOOTING,
    COVERSHOOTING
}

public class AllyBehaviour : MonoBehaviour {

	public Vector3 targetTransform = new Vector3(0, 1.0f, 0);
	public float speed = 3.0f;
	private UnityEngine.AI.NavMeshAgent allyAI;
    public float minRange = 1;
    private float allyDistance = 1.0f;
    public bool movingToCover = false;
    private float movementSpeed = 0.0f;
    private Vector3 lastPosition = Vector3.zero;
    private bool firing = false;

    private Animator anim;

    public AllyState state;

    public ParticleSystem gunLight;

    private GameObject player;
    List<GameObject> allies = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        state = AllyState.STOPPED;

        anim = GetComponent<Animator>();

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

        float move = GetComponent<Rigidbody>().velocity.magnitude;

        if (firing)
        {
            if ((state != AllyState.COVERSHOOTING) && (state != AllyState.SHOOTING))
            {
                firing = false;

                //CancelInvoke();
            }
        }

        foreach (GameObject ally in allies)
        {
            if (Vector3.Distance(transform.position, ally.transform.position) < allyDistance)
            {                
                transform.position = Vector3.MoveTowards(transform.position, ally.transform.position, -1 * speed * Time.deltaTime);
            }
        }

        if (state == AllyState.FOLLOWING)
        {
            targetTransform = player.transform.position;
        }

        else if ((state == AllyState.SHOOTING) || (state == AllyState.COVERSHOOTING))
        {
            if (!firing)
            {
                firing = true;

                //InvokeRepeating("GunFlash", 1, 1);
            }
        }

        if (Vector3.Distance(transform.position, targetTransform) > minRange)
        {            
            allyAI.SetDestination(targetTransform);
		}

        else
        {
            if ((state != AllyState.COVER) && (state != AllyState.COVERSHOOTING) && (state != AllyState.SHOOTING))
            {
                stopMoving();
            }
        }		
	}

    void FixedUpdate()
    {
        movementSpeed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;

        anim.SetFloat("Speed", movementSpeed);
    }

	public void newPosition(Vector3 newPos)
	{
		targetTransform = newPos;
	}

    public void stopMoving()
    {
        if (movingToCover == true)
        {
            movingToCover = false;
            state = AllyState.COVER;
            anim.SetTrigger("CombatIdle");
        }

        else
        {
            state = AllyState.STOPPED;
        }

        targetTransform = transform.position;
    }

    public void FindCover()
    {
        state = AllyState.MOVING;

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
                        movingToCover = true;
                        return;
                    }
                }
            }
        }
    }

    public void MoveToCover(GameObject newCover)
    {
        GameObject closest = null;

        state = AllyState.MOVING;

        int spots = 3;
        foreach (Transform child in newCover.transform)
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
            closest = newCover;
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
                        movingToCover = true;
                        return;
                    }
                }
            }
        }
    }

    public void NewEnemy()
    {
        stopMoving();        

        if (state == AllyState.COVER)
        {
            state = AllyState.COVERSHOOTING;
        }

        else
        {
            state = AllyState.SHOOTING;
        }

        anim.SetBool("CombatShoot", true);

        InvokeRepeating("GunFlash", 1, 1);
    }

    public void NoEnemy()
    {
        anim.SetBool("CombatShoot", false);

        CancelInvoke();

        if (state == AllyState.COVERSHOOTING)
        {
            state = AllyState.COVER;
        }

        else
        {
            state = AllyState.STOPPED;
        }
    }

    private void GunFlash()
    {
        gunLight.Play();
    }
}

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
    public float minRange = 0.1f;
    private float allyDistance = 1.0f;
    public bool movingToCover = false;
    private float movementSpeed = 0.0f;
    private Vector3 lastPosition = Vector3.zero;
    public GameObject bullet;
    public int health = 50;
    private float accuracy = 5;


    private Animator anim;

    public AllyState state;

    public ParticleSystem gunLight;

    public ParticleSystem deathExplosion;

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

        allyAI = GetComponent<UnityEngine.AI.NavMeshAgent> ();

        stopMoving();

        player = GameObject.FindGameObjectWithTag("Player");

        if (tag == "Ally")
        {
            accuracy /= 2.5f;
        }
    }
	
	// Update is called once per frame
	void Update () {

        float move = GetComponent<Rigidbody>().velocity.magnitude;

        //foreach (GameObject ally in allies)
        //{
        //    if (Vector3.Distance(transform.position, ally.transform.position) < allyDistance)
        //    {                
        //        transform.position = Vector3.MoveTowards(transform.position, ally.transform.position, -1 * speed * Time.deltaTime);
        //    }
        //}

        if (state == AllyState.FOLLOWING)
        {
            targetTransform = player.transform.position;
        }

        if ((movingToCover) && (movementSpeed == 0))
        {
            stopMoving();
        }	

        if (health <= 0)
        {
            Instantiate(deathExplosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
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
        allyAI.SetDestination(targetTransform);
    }

    public void stopMoving()
    {
        if (movingToCover)
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

        allyAI.ResetPath();
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
        //stopMoving();        

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

        Vector3 bulletPos = gunLight.transform.position;
        
        GameObject clone = Instantiate(bullet, bulletPos, transform.rotation);

        // start with a perfect shot
        Vector3 divergence = Vector3.zero;
        // then we want to randomize the rotation around the X axis
        divergence.x = (1 - 2 * Random.value) * accuracy;
        // and the rotation around the Y axis
        divergence.y = (1 - 2 * Random.value) * accuracy;

        clone.transform.Rotate(divergence);

        clone.transform.localScale *= 2;

        clone.GetComponent<Rigidbody>().AddForce(clone.transform.forward * 2000);
    }

    public void NewHealth(int hlth)
    {
        health += hlth;
    }
}

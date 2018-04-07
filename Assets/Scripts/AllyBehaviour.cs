using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyState
{
    COVER,
    STOPPED,
    MOVING,
    SHOOTING,
    COVERSHOOTING,
    BUILDING
}

public class AllyBehaviour : MonoBehaviour {

	public Vector3 targetTransform = new Vector3(0, 1.0f, 0);
	public float speed = 3.0f;
	private UnityEngine.AI.NavMeshAgent allyAI;
    public float minRange = 0.1f;
    private float allyDistance = 3.0f;
    public bool movingToCover = false;
	public bool following = false;
    private float movementSpeed = 0.0f;
    private Vector3 lastPosition = Vector3.zero;
    public GameObject bullet;
    public int health = 50;
    private float accuracy = 5;
    [SerializeField]GameObject coverPrefab;
    private Vector3 TargetPos;

    private Animator anim;

    public AllyState state;

    [SerializeField] ParticleSystem gunLight;

    [SerializeField] ParticleSystem deathExplosion;

    private GameObject player;
    public List<GameObject> allies = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        state = AllyState.STOPPED;

        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");

        TargetPos = GameObject.FindGameObjectWithTag("Target").transform.position;

        if (tag == "Ally")
        {
            accuracy /= 2.5f;

            foreach (GameObject ally in GameObject.FindGameObjectsWithTag("Ally"))
            {
                if (ally != this.gameObject)
                {
                    allies.Add(ally);
                }
            }
        }

        else if (tag == "Enemy")
        {
            foreach (GameObject ally in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (ally != this.gameObject)
                {
                    allies.Add(ally);
                }
            }            
        }

        allyAI = GetComponent<UnityEngine.AI.NavMeshAgent> ();

        stopMoving();

        if (tag == "Enemy")
        {
            newPosition(TargetPos);
        }               
    }
	
	// Update is called once per frame
	void Update () {

        allies.RemoveAll(item => item == null);

        float move = GetComponent<Rigidbody>().velocity.magnitude;

		if (!movingToCover) 
		{
			if ((state == AllyState.STOPPED) || (state == AllyState.SHOOTING) || (state == AllyState.MOVING)) 
			{
				foreach (GameObject ally in allies)
				{
					if (Vector3.Distance (transform.position, ally.transform.position) < allyDistance) 
					{
						Vector3 pos = transform.position - ally.transform.position;

						Vector3 runTo = transform.position + ((transform.position - ally.transform.position) * 1);

						pos *= -1;

						pos.y = transform.position.y;

						newPosition (runTo);
					}
				}
			}	
		}

		if (state == AllyState.STOPPED) 
		{
			if (following) 
			{
				if (Vector3.Distance (transform.position, player.transform.position) < 10) 
				{
					Vector3 newPos = Vector3.Lerp (transform.position, player.transform.position, 0.2f);
					newPosition (newPos);
				}
			}
		}

		if ((state == AllyState.BUILDING) || (state == AllyState.MOVING))
        {
			if (!allyAI.pathPending) 
			{
				if (allyAI.remainingDistance <= allyAI.stoppingDistance) 
				{
					if ((!allyAI.hasPath) || (allyAI.velocity.sqrMagnitude == 0.0f)) 
					{
						stopMoving ();
					}
				}
			}
        }

        if (health <= 0)
        {
			if (tag == "Enemy") 
			{
				player.GetComponent<Score> ().UpdateScore (50);
			}

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

    public void NewAlly(GameObject newAl)
    {
        allies.Add(newAl);
    }

	public void newPosition(Vector3 newPos)
	{
		targetTransform = newPos;
        allyAI.SetDestination(targetTransform);
    }

    public void stopMoving()
    {
        if (tag == "Enemy")
        {
			if (Vector3.Distance (transform.position, TargetPos) < 10) 
			{
				newPosition (TargetPos);
				return;
			}
        }

        if (state == AllyState.BUILDING)
        {            
            NewCover();
            return;
        }

        if (movingToCover)
        {
            allyAI.stoppingDistance = 3;
            movingToCover = false;
            state = AllyState.COVER;
            anim.SetTrigger("CombatIdle");
        }

        else
        {
            state = AllyState.STOPPED;
        }

        newPosition(this.gameObject.transform.position);
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
			movingToCover = true;

			MoveToCover (closest);
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
						allyAI.stoppingDistance = 0.1f;
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
        if (!movingToCover)
        {
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

    public void NewCover()
    {
        Vector3 relativePos = allyAI.destination - TargetPos;
        Quaternion rot = Quaternion.LookRotation(relativePos);

		rot.z = 0;
		rot.x = 0;

        GameObject obj = Instantiate(coverPrefab, allyAI.destination, Quaternion.identity);

        obj.transform.rotation = rot;

        state = AllyState.STOPPED;

        newPosition(this.gameObject.transform.position);        
    }
}

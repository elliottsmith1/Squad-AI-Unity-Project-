using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Target : MonoBehaviour {

	[SerializeField] int health = 50;
	[SerializeField] GameObject enemyPrefab;

	private int spawnNum = 1;
	private int wave = 1;
	private float spawnTimer = 0.0f;
	private float spawnThreshold = 5.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		spawnTimer += Time.deltaTime;

		if (spawnTimer > spawnThreshold) 
		{
			if (wave % 5 == 0) 
			{
				spawnNum++;
			}

			spawnTimer = 0.0f;

			for (int i = 0; i < spawnNum; i++) 
			{
				Vector3 spawnPos = transform.position;

				int spawnSide = Random.Range (0, 2);

				if (spawnSide == 0) 
				{
					spawnPos.z += Random.Range (35, 50);
				} 

				else 
				{
					spawnPos.z -= Random.Range (35, 50);
				}

				spawnPos.x += Random.Range (10, -100);

				GameObject obj = Instantiate (enemyPrefab, spawnPos, Quaternion.identity);
			}
			wave++;
		}

		if (health < 1) 
		{
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.tag == "Player") 
		{
			c.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().SetCanSpawn (true);
		}
	}

	void OnTriggerExit(Collider c)
	{
		if (c.gameObject.tag == "Player") 
		{
			c.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().SetCanSpawn (false);
		}
	}

	public void DamageTarget(int _dmg)
	{
		health -= _dmg;

		Color newCol = GetComponent<Renderer> ().material.color;

		newCol.r += 0.05f;

		GetComponent<Renderer> ().material.color = newCol;
	}
}

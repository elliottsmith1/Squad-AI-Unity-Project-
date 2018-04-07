using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	[SerializeField] Text scoreText;
	private int score = 500;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		scoreText.text = score.ToString ();
	}

	public void UpdateScore(int _score)
	{
		score += _score;
	}

	public int GetScore()
	{
		return score;
	}
}

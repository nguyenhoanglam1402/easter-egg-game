using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPlayer
{
	public int score;
	public string name;
	public string uid = "";
	public Vector3 position;
	public DataPlayer(string name, string uid)
	{
		this.uid = uid;
		this.name = name;
		position = new Vector3(0, 0, 0);
	}

	public void SetScore(int newScore)
	{
		score = newScore;
	}

	public void SetVectorState(Vector3 newState)
	{
		position = newState;
	}
}

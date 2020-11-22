﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public int totalPoint;
	public int stagePoint;
	public int health;
	public PlayerMove player;
	public int stageIndex;

	public void NextStage () {
		stageIndex++;
		totalPoint += stagePoint;
		stagePoint = 0;
	}

	public void HealthDown()
	{
		if (health > 1)
			health--;
		else {
			player.OnDie();
			Debug.Log("죽었습니다!");
		}
	}
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (health > 1)
			{
				collision.attachedRigidbody.velocity = Vector2.zero;
				collision.transform.position = new Vector3(0, 0, -1);
			}
			HealthDown();
		}
	}
}

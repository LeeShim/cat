﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
	public GameManager gameManager;
	public float maxSpeed;
	public float jumpPower;
	Rigidbody2D rigid;
	CapsuleCollider2D collider;
	SpriteRenderer spriteRenderer;
	Animator anim;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		collider = GetComponent<CapsuleCollider2D>();

	}

	void Update()
	{
		//점프
		if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
		{
			rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
		}

			//키보드 안누르면멈추는 부분
			if (Input.GetButtonUp("Horizontal"))
		{
			rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
		}

		//방향전환
		if (Input.GetButton("Horizontal"))
		{
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		//애니메이션
		if (Mathf.Abs( rigid.velocity.x ) < 0.3)
			anim.SetBool("isWalking", false);
		else
			anim.SetBool("isWalking", true);
	}

	void FixedUpdate()
	{
		//키보드로 움직임
		float h = Input.GetAxisRaw("Horizontal");

		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		if (rigid.velocity.x > maxSpeed) //Right Max Speed
			rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);

		if (rigid.velocity.x < maxSpeed*(-1)) //Left Max Speed
			rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

		//랜딩 플렛폼
		if (rigid.velocity.y < 0)
		{
			Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

			RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

			if (rayHit.collider != null)
			{
				if (rayHit.distance < 0.5f)
				{
					anim.SetBool("isJumping", false);
				}
			}
		}
	}
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Item")
		{
			gameManager.stagePoint += 100;
			collision.gameObject.SetActive(false);
		}
		else if (collision.gameObject.tag == "Finish")
		{
			gameManager.NextStage();
		}
	}

	void OnAttack(Transform enemy)
	{
		gameManager.stagePoint += 100;
		EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
		enemyMove.OnDamaged();
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
			{
				OnAttack(collision.transform);
			}
			else 
				OnDamaged(collision.transform.position);
		}
	}

	void OnDamaged(Vector2 targetPos)
	{
		gameManager.HealthDown();
		gameObject.layer = 11;

		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
		rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

		//
		anim.SetTrigger("doDamaged");
		Invoke("OffDamaged", 3);
	}

	void OffDamaged()
	{
		gameObject.layer = 10;
		spriteRenderer.color = new Color(1, 1, 1, 1);
	}

	public void OnDie()
	{
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		spriteRenderer.flipY = true;
		collider.enabled = false;
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
	}
}

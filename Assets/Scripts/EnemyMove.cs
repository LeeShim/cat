using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour {

	Rigidbody2D rigid;
	Animator anim;
	SpriteRenderer spriteRenderer;
	CapsuleCollider2D collider;

	public int nextMove;

	void Start () {
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<CapsuleCollider2D>();
		Invoke("Think", 5);
	}
	
	void FixedUpdate () {
		//이동
		rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

		//플랫폼 체크
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		if (rayHit.collider == null)
		{
			Turn();
		}
	}


	void Think()
	{
		//다음 활동
		nextMove = Random.Range(-1, 2);


		//에니메이션
		anim.SetInteger("WalkSpeed", nextMove);

		//방향 
		if(nextMove != 0)
		spriteRenderer.flipX = nextMove == 1;

		//Recursive
		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);
	}

	void Turn()
	{
		nextMove = nextMove * -1;
		spriteRenderer.flipX = nextMove == 1;

		CancelInvoke();
		Invoke("Think", 2);
	}
	public void OnDamaged()
	{
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		spriteRenderer.flipY = true;
		collider.enabled = false;
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
		Invoke("DeActive", 5);
	}

	void DeActive()
	{
		gameObject.SetActive(false);
	}
}

using System.Collections;
using UnityEngine;

public class EnemyGrower : EnemyBase
{
	[Header("Settings")]
	public float spookDistance;
	public float bigScale, growDuration, bigDuration, cooldownDuration;

	[Header("Scene references")]
	public Transform model;

	Vector3 initialScale;
	float cooldownTimer;
	bool isGrowing;

	// what do we need here ?

	// grows when it's close enough to the player

	protected override void Awake()
	{
		base.Awake();

		initialScale = model.localScale;
		cooldownTimer = cooldownDuration;
		currentState = AIState.Follow;
	}

	void Update()
	{
		switch (currentState)
		{
			case AIState.Follow:
				StateFollow();
				break;

			case AIState.Attacking:
				StateAttack();
				break;
		}
	}

	void StateFollow()
	{
		agent.SetDestination(player.position);

		RaycastHit hit;
		Physics.Raycast(transform.position, player.position - transform.position, out hit);

		cooldownTimer += Time.deltaTime;

		if (cooldownTimer >= cooldownDuration && hit.transform == player && hit.distance < spookDistance)
			currentState = AIState.Attacking;
	}

	void StateAttack()
	{
		if (!isGrowing)
			StartCoroutine(GrowRoutine());

		agent.SetDestination(player.position);
	}

	IEnumerator GrowRoutine()
	{
		isGrowing = true;
		float timer = 0;

		while (timer < growDuration)
		{
			timer += Time.deltaTime;
			float percent = timer / growDuration;

			model.localScale = Vector3.Lerp(initialScale, initialScale.normalized * bigScale, percent);
			yield return null;
		}

		yield return new WaitForSeconds(bigDuration);

		timer = 0;

		while (timer < (growDuration / 2))
		{
			timer += Time.deltaTime;
			float percent = 1 - (timer / (growDuration / 2));

			model.localScale = Vector3.Lerp(initialScale, initialScale.normalized * bigScale, percent);
			yield return null;
		}

		currentState = AIState.Follow;
		cooldownTimer = 0;
		isGrowing = false;
	}
	
	protected override void OnCollisionEnter(Collision other)
	{
		base.OnCollisionEnter(other);
	}
}
using UnityEngine;

public class EnemyTaurus : EnemyBase
{
	[Header("Settings")]
	public float movementSpeed;
	public float attackDistance, attackDelay, dashDuration, cooldownDelay, dashSpeed;

	float idleTimer, dashTimer, extraCooldownTime, cooldownTimer;

	protected override void Awake()
	{
		base.Awake();

		idleTimer = 0;
		dashTimer = 0;
		extraCooldownTime = 0;
		cooldownTimer = 0;

		currentState = AIState.Follow;
	}

	void Update()
	{
		if (!isAlive)
			return;

		if (agent.speed != movementSpeed)
			agent.speed = movementSpeed;

		switch (currentState)
		{
			case AIState.Follow:
				StateFollow();
				break;

			case AIState.Charging:
				StateCharging();
				break;

			case AIState.Attacking:
				StateAttacking();
				break;

			case AIState.Cooldown:
				StateCooldown();
				break;
		}
	}

	void StateFollow()
	{
		agent.enabled = true;
		agent.isStopped = false;
		agent.SetDestination(player.position);

		RaycastHit hit;
		Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit);

		if (hit.transform == player && hit.distance <= attackDistance)
			currentState = AIState.Charging;
	}

	void StateCharging()
	{
		agent.isStopped = true;
		idleTimer += Time.deltaTime;

		Vector3 dir = player.position - transform.position;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), agent.angularSpeed * Time.deltaTime);

		if (idleTimer >= attackDelay)
		{
			idleTimer = 0;
			extraCooldownTime = 0;
			currentState = AIState.Attacking;
		}
	}

	void StateAttacking()
	{
		agent.enabled = false;
		rigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
		rigid.velocity = transform.forward * dashSpeed;

		dashTimer += Time.deltaTime;

		if (dashTimer >= dashDuration)
		{
			dashTimer = 0;
			extraCooldownTime = 0;
			currentState = AIState.Cooldown;
		}
	}

	void StateCooldown()
	{
		rigid.constraints = RigidbodyConstraints.FreezeAll;
		rigid.velocity = Vector3.zero;
		cooldownTimer += Time.deltaTime;

		if (cooldownTimer > cooldownDelay + extraCooldownTime)
		{
			currentState = AIState.Follow;
			cooldownTimer = 0;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (currentState == AIState.Attacking && other.transform.CompareTag("Wall"))
		{
			rigid.velocity = Vector3.zero;

			extraCooldownTime = dashDuration - dashTimer;
			dashTimer = 0;
			currentState = AIState.Cooldown;
		}
	}
}
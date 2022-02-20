using UnityEngine;

public class EnemyBasic : EnemyBase
{
	[Header("Settings")]
	public float movementSpeed;
	public float trajectoryUpdateDelay;

	float trajectoryUpdateTimer;

	protected override void Awake()
	{
		base.Awake();

		currentState = AIState.Follow;
		trajectoryUpdateTimer = 0;
	}

	protected virtual void Update()
	{
		if (!isAlive)
			return;

		if (agent.speed != movementSpeed)
			agent.speed = movementSpeed;

		trajectoryUpdateTimer += Time.deltaTime;

		if (trajectoryUpdateTimer >= trajectoryUpdateDelay)
		{
			agent.SetDestination(player.position);
			trajectoryUpdateTimer = 0;
		}
	}
}
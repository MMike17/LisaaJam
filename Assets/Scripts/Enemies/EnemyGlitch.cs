using UnityEngine;

public class EnemyGlitch : EnemyBase
{
	[Header("Settings")]
	public float attackDelay;
	public float attackDuration, maxGlitchDistance, noiseMagnitude;
	public int maxPosMemory;

	[Header("Scene references")]
	public Transform model;
	public Transform modelTarget;

	float memoryStep => (float)maxPosMemory / attackDelay;

	Vector3[] positions;
	Quaternion[] rotations;
	float attackTimer, idleTimer, statePercent;

	protected override void Awake()
	{
		base.Awake();

		positions = new Vector3[maxPosMemory];
		rotations = new Quaternion[maxPosMemory];

		model.SetParent(null);
	}

	void Update()
	{
		if (!isAlive)
			return;

		if (agent.speed != maxGlitchDistance)
			agent.speed = maxGlitchDistance / attackDelay;

		switch (currentState)
		{
			case AIState.Charging:
				StateCharging();
				break;

			case AIState.Attacking:
				StateAttack();
				break;
		}
	}

	void StateCharging()
	{
		agent.isStopped = false;
		agent.SetDestination(player.position);

		idleTimer += Time.deltaTime;

		statePercent = idleTimer / attackDelay;
		int currentIndex = PercentToIndex();

		if (positions[currentIndex] == default(Vector3))
			positions[currentIndex] = modelTarget.position + new Vector3((Random.value * 2 - 1) * noiseMagnitude, 0, (Random.value * 2 - 1) * noiseMagnitude);

		if (rotations[currentIndex] == default(Quaternion))
			rotations[currentIndex] = modelTarget.rotation;

		if (statePercent > 1)
		{
			currentState = AIState.Attacking;
			idleTimer = 0;
		}
	}

	int PercentToIndex()
	{
		return Mathf.Clamp(Mathf.FloorToInt(statePercent * maxPosMemory), 0, maxPosMemory - 1);
	}

	void StateAttack()
	{
		agent.isStopped = true;

		attackTimer += Time.deltaTime;

		statePercent = attackTimer / attackDuration;
		int currentIndex = PercentToIndex();

		model.position = positions[currentIndex];
		model.rotation = rotations[currentIndex];

		if (statePercent > 1)
		{
			model.position = modelTarget.position;
			model.rotation = modelTarget.rotation;

			currentState = AIState.Charging;
			attackTimer = 0;

			positions = new Vector3[maxPosMemory];
			rotations = new Quaternion[maxPosMemory];
		}
	}
	
	protected override void OnCollisionEnter(Collision other)
	{
		base.OnCollisionEnter(other);
	}
}
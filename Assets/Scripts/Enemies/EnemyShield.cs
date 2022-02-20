using UnityEngine;

public class EnemyShield : EnemyBasic
{
	[Header("Settings")]
	public float invulnerabilityDuration;

	float invulnerabilityTimer;
	bool wasHit;

	protected override void Awake()
	{
		base.Awake();

		wasHit = false;
		invulnerabilityTimer = 0;
		currentState = AIState.Follow;
	}

	protected override void Update()
	{
		base.Update();

		if (wasHit)
		{
			invulnerabilityTimer += Time.deltaTime;

			if (invulnerabilityTimer >= invulnerabilityDuration)
			{
				foreach (Collider collider in colliders)
					collider.enabled = true;
			}
		}
	}

	public override void Die()
	{
		if (wasHit)
		{
			// play die animation

			// this.DelayAction(() =>
			// {
			// 	isAlive = false;
			// foreach (Collider collider in colliders)
			// 	collider.enabled = false;
			// }, /*animation duration*/);
		}
		else
		{
			wasHit = true;

			foreach (Collider collider in colliders)
				collider.enabled = false;
		}
	}
	
	protected override void OnCollisionEnter(Collision other)
	{
		base.OnCollisionEnter(other);
	}
}
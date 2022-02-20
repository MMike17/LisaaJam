using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(NavMeshAgent))]
public partial class EnemyBase : MonoBehaviour
{
	public enum AIState
	{
		Charging,
		Follow,
		Attacking,
		Cooldown
	}

	protected Rigidbody rigid;
	protected NavMeshAgent agent;
	protected AIState currentState;
	protected Collider[] colliders;
	public Transform player;
	protected bool isAlive;
	protected Animator anim;

	RailPlayer railPlayer => player.GetComponent<RailPlayer>();

	protected virtual void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		colliders = GetComponentsInChildren<Collider>();
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		isAlive = true;
	}

	public virtual void Die()
	{
		// play die animation

		// this.DelayAction(() =>
		// {
		isAlive = false;

		foreach (Collider collider in colliders)
			collider.enabled = false;

		gameObject.SetActive(false);
		// }, /*animation duration*/);
	}

	protected virtual void OnCollisionEnter(Collision other)
	{
		if (other.transform == player)
		{
			// detect player dash
			if (railPlayer.isDashing)
				Die();
			else
				if (SceneLoader.Instance != null) SceneLoader.Instance.ReloadScene();
		}
	}
}
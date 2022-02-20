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

	protected virtual void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		colliders = GetComponentsInChildren<Collider>();
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		// get player reference
		// player = ;

		isAlive = true;
	}

	public virtual void Die()
	{
		// play die animation

		// this.DelayAction(() =>
		// {
		// 	isAlive = false;
		// foreach (Collider collider in colliders)
		// 	collider.enabled = false;
		// }, /*animation duration*/);
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.transform == player)
		{
			// detect player dash
			// if ()
			// 	Die();
			// else
			// 	SceneLoader.Instance.ReloadScene();
		}
	}
}
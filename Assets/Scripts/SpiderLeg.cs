using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
	[Header("Scene references")]
	public Transform target;
	public Transform leg, calf;
	[Space]
	public Transform legElbow;
	public Transform calfElbow;

	[HideInInspector] public bool isMoving;

	Transform hint;

	public void DrawGizmos(float targetSize, Vector3? hintPos)
	{
		if (target != null)
			Gizmos.DrawWireSphere(target.position, targetSize);

		if (leg != null)
		{
			Gizmos.DrawSphere(leg.position, 0.2f);

			if (legElbow != null)
				Gizmos.DrawLine(leg.position, legElbow.position);
		}

		if (calf != null)
		{
			Gizmos.DrawSphere(calf.position, 0.2f);

			if (calfElbow != null)
			{
				Gizmos.DrawLine(calf.position, calfElbow.position);
				Gizmos.DrawLine(calfElbow.position, hintPos.Value);
			}
		}
	}

	public void Init(Transform legHint)
	{
		hint = legHint;

		calf.SetParent(null);
	}

	public bool ShouldMove(float distance)
	{
		return Vector3.Distance(calf.position, target.position) > distance;
	}

	public void ComputeIK(int iterations)
	{
		for (int i = 0; i < iterations; i++)
		{
			if (i == 0)
			{
				leg.LookAt(hint, Vector3.up);
				calf.LookAt(hint, Vector3.up);
			}
			else
			{
				leg.LookAt(calfElbow, Vector3.up);
				calf.LookAt(legElbow, Vector3.up);
			}
		}
	}
}
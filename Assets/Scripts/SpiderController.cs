using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderController : MonoBehaviour
{
	// TODO : Body adapt to terrain
	// TODO : lazer
	// TODO : adapts to leg meshes (different lengths)

	[Header("Settings - Legs")]
	public float legsIKMaxDistance;
	public float legsMoveDuration, legMoveHeight;
	public int IKIterations, maxSyncLegMove;
	public AnimationCurve legMoveHeightCurve;

	[Header("Settings - Movement")]
	public float bodyHeight;
	public float forwardSpeed, backwardSpeed, sidewaysSpeed, turnSpeed;

	[Header("Scene references - Legs")]
	public SpiderLeg[] frontLegs;
	public SpiderLeg[] middleLegs, backLegs;
	public Transform frontLegsHint, middleLegsHint, backLegsHint;

	[Header("Scene references - Movement")]
	public Transform groundRayStart;

	bool canMoveLeg => movingLegsCount < maxSyncLegMove;

	SpiderLeg[] allLegs;
	Rigidbody rigid;
	int movingLegsCount;

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0.3f, 0, 0.5f);

		if (frontLegs != null)
		{
			foreach (SpiderLeg leg in frontLegs)
				leg.DrawGizmos(legsIKMaxDistance, frontLegsHint?.position);

			foreach (SpiderLeg leg in middleLegs)
				leg.DrawGizmos(legsIKMaxDistance, middleLegsHint?.position);

			foreach (SpiderLeg leg in backLegs)
				leg.DrawGizmos(legsIKMaxDistance, backLegsHint?.position);
		}

		if (frontLegsHint != null)
			Gizmos.DrawSphere(frontLegsHint.position, 0.1f);

		if (middleLegsHint != null)
			Gizmos.DrawSphere(middleLegsHint.position, 0.1f);

		if (backLegsHint != null)
			Gizmos.DrawSphere(backLegsHint.position, 0.1f);

		if (groundRayStart != null)
			Gizmos.DrawLine(groundRayStart.position, groundRayStart.position - transform.up * bodyHeight);
	}

	void Awake()
	{
		rigid = GetComponent<Rigidbody>();

		foreach (SpiderLeg leg in frontLegs)
			leg.Init(frontLegsHint);

		foreach (SpiderLeg leg in middleLegs)
			leg.Init(middleLegsHint);

		foreach (SpiderLeg leg in backLegs)
			leg.Init(backLegsHint);

		List<SpiderLeg> allLegsList = new List<SpiderLeg>();

		allLegsList.AddRange(frontLegs);
		allLegsList.AddRange(middleLegs);
		allLegsList.AddRange(backLegs);

		// randomize legs
		allLegsList.Sort((x, y) => (int)Mathf.Sign(Random.value * 2 - 1));
		allLegs = allLegsList.ToArray();

		movingLegsCount = 0;
	}

	void Update()
	{
		float rotateInput = Input.GetAxis("Mouse X");
		transform.Rotate(Vector3.up * turnSpeed * rotateInput * Time.deltaTime);
		CameraManager.Instance.AddVerticalRotation(-Input.GetAxis("Mouse Y") * Time.deltaTime);

		RaycastHit hit;
		Physics.Raycast(groundRayStart.position, -transform.up, out hit);

		transform.position = hit.point + hit.normal * bodyHeight;

		foreach (SpiderLeg leg in allLegs)
		{
			if (canMoveLeg && !leg.isMoving && leg.ShouldMove(legsIKMaxDistance))
				StartCoroutine(MoveLeg(leg));
		}

		ComputeIK();
	}

	void LateUpdate()
	{
		Vector3 velocity = Vector3.zero;

		if (Input.GetKey(Config.Instance.moveForwardKey))
			velocity += transform.forward * forwardSpeed;

		if (Input.GetKey(Config.Instance.moveBackwardsKey))
			velocity -= transform.forward * backwardSpeed;

		if (Input.GetKey(Config.Instance.moveRightKey))
			velocity += transform.right * sidewaysSpeed;

		if (Input.GetKey(Config.Instance.moveLeftKey))
			velocity -= transform.right * sidewaysSpeed;

		rigid.velocity = velocity;
	}

	void ComputeIK()
	{
		foreach (SpiderLeg leg in allLegs)
			leg.ComputeIK(IKIterations);
	}

	// TODO : How are we going to apply that ?
	// angular difference on YZ plane ? => regular Rotate
	Vector3 GetCrossNormal()
	{
		// we suppose the legs are always paired
		Vector3[] crossVectors = new Vector3[2];

		for (int index = 0; index < 2; index++)
		{
			int reverseIndex = (allLegs.Length - 1) - index;
			crossVectors[index] = allLegs[index].target.position - allLegs[reverseIndex].transform.position;
		}

		return Vector3.Cross(crossVectors[0], crossVectors[1]);
	}

	IEnumerator MoveLeg(SpiderLeg leg)
	{
		leg.isMoving = true;
		movingLegsCount++;

		Vector3 initialPos = leg.calf.position;
		Vector3 legTarget = leg.target.position + rigid.velocity.normalized * legsIKMaxDistance;

		RaycastHit hit;
		Physics.Raycast(legTarget + transform.up * 3, -transform.up, out hit);

		legTarget = hit.point;

		float timer = 0;

		while (leg.calf.position != legTarget)
		{
			timer += Time.deltaTime;
			float percent = Mathf.Clamp01(timer / legsMoveDuration);

			Vector3 currentTarget = Vector3.Lerp(initialPos, legTarget, percent);
			currentTarget += transform.up * legMoveHeightCurve.Evaluate(percent) * legMoveHeight;

			leg.calf.position = currentTarget;
			yield return null;
		}

		leg.isMoving = false;
		movingLegsCount--;
	}
}
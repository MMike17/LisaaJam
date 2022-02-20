using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderController : MonoBehaviour
{
	[Header("Settings - Legs")]
	public float legsIKMaxDistance;
	public float legsMoveDuration, legMoveHeight;
	[Range(0, 0.1f)]
	public float swayAmount;
	public int IKIterations, maxSyncLegMove;
	public AnimationCurve legMoveHeightCurve;

	[Header("Settings - Movement")]
	public float bodyHeight;
	public float forwardSpeed, backwardSpeed, sidewaysSpeed, turnSpeed;

	[Header("Settings - Lazer")]
	public float maxLazerRange;
	public int normalCameraIndex, lazerCameraIndex;

	[Header("Scene references - Legs")]
	public SpiderLeg[] frontLegs;
	public SpiderLeg[] middleLegs, backLegs;
	public Transform frontLegsHint, middleLegsHint, backLegsHint;

	[Header("Scene references - Movement")]
	public Transform groundRayStart;
	public Animator anim;

	[Header("Scene references - Movement")]
	public Transform lazerPoint;
	public Transform lazerModel;

	bool canMoveLeg => movingLegsCount < maxSyncLegMove;

	SpiderLeg[] allLegs;
	Transform[] lazers;
	Rigidbody rigid;
	int movingLegsCount;
	bool lazerMode, isDancing;

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

		anim.transform.SetParent(null);

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
		if (!lazerMode && !isDancing && Input.GetKeyDown(Config.Instance.danceKey))
		{
			anim.transform.position = transform.position;
			anim.transform.rotation = transform.rotation;

			isDancing = true;
			anim.Play("Dance");

			this.DelayAction(() =>
			{
				isDancing = false;
				anim.Play("Idle");
			}, 10);
		}

		Vector3 position;

		if (isDancing)
		{
			position = transform.position;
			position.y = GetAverageLegHeight() + bodyHeight;
			transform.position = position;

			foreach (SpiderLeg leg in allLegs)
			{
				if (canMoveLeg && !leg.isMoving && leg.ShouldMove(legsIKMaxDistance))
					StartCoroutine(MoveLeg(leg));
			}

			ComputeIK();
			return;
		}

		transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.Lerp(Vector3.up, GetCrossNormal(), swayAmount));
		transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime, Vector3.up);

		CameraManager.Instance.AddVerticalRotation(-Input.GetAxis("Mouse Y") * Time.deltaTime);

		position = transform.position;
		position.y = GetAverageLegHeight() + bodyHeight;
		transform.position = position;

		foreach (SpiderLeg leg in allLegs)
		{
			if (canMoveLeg && !leg.isMoving && leg.ShouldMove(legsIKMaxDistance))
				StartCoroutine(MoveLeg(leg));
		}

		if (Input.GetKeyDown(Config.Instance.lazerModeKey))
		{
			lazerMode = !lazerMode;
			CameraManager.Instance.SetCameraPreset(lazerMode ? lazerCameraIndex : normalCameraIndex);
		}

		if (lazerMode)
		{
			RaycastHit hit;
			Physics.Raycast(lazerPoint.position, lazerPoint.forward, out hit, maxLazerRange);

			float lazerSize = hit.collider != null ? hit.distance : maxLazerRange;
			lazerModel.localScale = Vector3.one + Vector3.forward * lazerSize;
		}

		lazerModel.gameObject.SetActive(lazerMode);

		ComputeIK();
	}

	float GetAverageLegHeight()
	{
		float totalHeight = 0;

		foreach (SpiderLeg leg in allLegs)
			totalHeight += leg.calf.position.y;

		return totalHeight / allLegs.Length;
	}

	void LateUpdate()
	{
		Vector3 velocity = Vector3.zero;

		if (!isDancing)
		{
			if (Input.GetKey(Config.Instance.moveForwardKey))
				velocity += transform.forward * forwardSpeed;

			if (Input.GetKey(Config.Instance.moveBackwardsKey))
				velocity -= transform.forward * backwardSpeed;

			if (Input.GetKey(Config.Instance.moveRightKey))
				velocity += transform.right * sidewaysSpeed;

			if (Input.GetKey(Config.Instance.moveLeftKey))
				velocity -= transform.right * sidewaysSpeed;
		}
		else
			velocity = transform.forward * 1f;

		rigid.velocity = velocity;
	}

	void ComputeIK()
	{
		foreach (SpiderLeg leg in allLegs)
			leg.ComputeIK(IKIterations);
	}

	Vector3 GetCrossNormal()
	{
		// we suppose the legs are always paired
		Vector3[] crossVectors = new Vector3[2];

		for (int index = 0; index < 2; index++)
		{
			int reverseIndex = (allLegs.Length - 1) - index;
			crossVectors[index] = allLegs[index].calf.position - allLegs[reverseIndex].calf.position;

			Debug.DrawLine(allLegs[index].calf.position, allLegs[reverseIndex].calf.position, Color.blue);
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

	void OnTriggerEnter(Collider other)
	{
		InterractionPoint point = other.GetComponent<InterractionPoint>();

		if (point != null)
			point.Notify(true);
	}

	void OnTriggerExit(Collider other)
	{
		InterractionPoint point = other.GetComponent<InterractionPoint>();

		if (point != null)
			point.Notify(false);
	}

	void OnTriggerStay(Collider other)
	{
		InterractionPoint point = other.GetComponent<InterractionPoint>();

		if (Input.GetKeyDown(Config.Instance.connectKey) && point != null)
			point.OpenDigital();
	}
}
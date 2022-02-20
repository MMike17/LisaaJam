using UnityEngine;

public class RailPlayer : MonoBehaviour
{
	[SerializeField] private RailNode _currentNode;
	public float baseSpeed = 2.0f;
	public float baseRotSpeed = 5.0f;
	public float dashDuration, dashSpeed, decelerationRate;

	[SerializeField] private RailMovementDirection cachedInput = RailMovementDirection.Forward;

	private RailNode CurrentNode
	{
		get => _currentNode;
		set
		{
			if (value == null) Debug.LogWarning($"Assigning null segment, previous segment: {_currentNode}");
			_currentNode = value;
		}
	}

	public bool isDashing => currentSpeed > baseSpeed;

	float currentSpeed, targetSpeed, dashTimer;
	public bool canDash;

	private void Start()
	{
		var entrance = FindObjectOfType<Entrance>();
		transform.position = entrance.transform.position;
		CurrentNode = entrance.startingNode;
		CurrentNode.Init(this, RailMovementDirection.Forward, RailMovementHeading.North, true);

		currentSpeed = baseSpeed;
	}

	private void Update()
	{
		if (SceneLoader.Instance != null && SceneLoader.Instance.isLoading) return;

		if (Input.GetAxis("Horizontal") < 0)
		{
			cachedInput = RailMovementDirection.Left;
		}
		if (Input.GetAxis("Horizontal") > 0)
		{
			cachedInput = RailMovementDirection.Right;
		}
		if (Input.GetAxis("Vertical") > 0)
		{
			cachedInput = RailMovementDirection.Forward;
		}

		if (Input.GetKeyDown(Config.Instance.dashKey) && canDash)
		{
			currentSpeed = dashSpeed;
			dashTimer = 0;
		}

		if (currentSpeed > baseSpeed)
		{
			dashTimer += Time.deltaTime;

			if (dashTimer > dashDuration)
				currentSpeed = Mathf.MoveTowards(currentSpeed, baseSpeed, decelerationRate * Time.deltaTime);
		}

		if (CurrentNode == null) return;
		CurrentNode.Advance(this, currentSpeed);
		if (CurrentNode.GetPositionPercent(transform.position) > 0.9f)
		{
			CurrentNode = CurrentNode.Handoff(this, cachedInput);
			cachedInput = RailMovementDirection.Forward;
		}

	}

	public void RotateTowards(Vector3 target)
	{
		var pos = transform.position;
		Vector3 targetDir = target - pos;
		targetDir.y = 0.0f;
		var speed = baseRotSpeed * Time.deltaTime;
		if (targetDir == Vector3.zero) return;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDir), speed);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		if (CurrentNode != null) Gizmos.DrawSphere(CurrentNode.transform.position, 0.5f);
	}
}

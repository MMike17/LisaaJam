using UnityEngine;

public class Config : Singleton<Config>
{
	[Header("Settings")]
	public KeyCode moveForwardKey;
	public KeyCode moveBackwardsKey, moveLeftKey, moveRightKey;

	public override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);
	}
}
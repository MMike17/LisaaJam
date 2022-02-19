using UnityEngine;

public class Config : Singleton<Config>
{
	[Header("Settings")]
	public KeyCode moveForwardKey;
	public KeyCode moveBackwardsKey, moveLeftKey, moveRightKey;

	float globalSoundMultiplier;

	public override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);
	}

	public void SetSoundMultiplier(float mult)
	{
		globalSoundMultiplier = mult;
	}

	public float GetAudioVolume(float sourceVolume)
	{
		return sourceVolume * globalSoundMultiplier;
	}
}
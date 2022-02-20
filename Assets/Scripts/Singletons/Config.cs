using System;
using UnityEngine;

public class Config : Singleton<Config>
{
	[Header("Settings")]
	public KeyCode moveForwardKey;
	public KeyCode moveBackwardsKey, moveLeftKey, moveRightKey, lazerModeKey, dashKey, pauseKey, connectKey, danceKey;

	public bool canPause { get; private set; }

	float globalSoundMultiplier;

	protected override void Awake()
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

	public void SetCanPause(bool state)
	{
		canPause = state;
	}
}
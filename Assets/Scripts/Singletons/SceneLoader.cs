using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneLoader.GameScene;

public class SceneLoader : Singleton<SceneLoader>
{
	[Header("Settings")]
	public float loadingDuration;
	public List<GameScene> scenes;

	bool isLoading;

	public override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);
	}

	public void LoadScene(SceneTag tag)
	{
		if (isLoading)
			return;

		isLoading = true;

		GameScene selected = GetSceneByTag(tag);

		if (selected != null)
			StartCoroutine(LoadSceneRoutine(selected));
		else
			isLoading = false;
	}

	IEnumerator LoadSceneRoutine(GameScene scene)
	{
		float timer = 0;

		while (timer != loadingDuration)
		{
			float percent = timer / loadingDuration;

			// animate shader
			timer += Time.deltaTime;
			yield return null;
		}

		scene.OnSceneLoaded();
		isLoading = false;
	}

	GameScene GetSceneByTag(SceneTag tag)
	{
		GameScene selected = scenes.Find(item => item.tag == tag);

		if (selected == null)
			Debug.LogError("Couldn't find scene for tag " + tag, gameObject);

		return selected;
	}

	public void SubscribeEvent(SceneTag tag, Action callback)
	{
		GameScene selected = GetSceneByTag(tag);

		if (selected != null)
			selected.ScheduleOnSceneLoaded(callback);
	}

	[Serializable]
	public class GameScene
	{
		public enum SceneTag
		{
			Menu,
			Hardware,
			Software_0
			// Software_
		}

		public int buildIndex;
		public SceneTag tag;

		Action onSceneLoaded;

		public void ScheduleOnSceneLoaded(Action callback)
		{
			if (callback != null)
				onSceneLoaded += callback;
		}

		public void OnSceneLoaded()
		{
			onSceneLoaded?.Invoke();
		}
	}
}
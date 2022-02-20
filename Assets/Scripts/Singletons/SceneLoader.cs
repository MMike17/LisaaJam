using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneLoader.GameScene;

public class SceneLoader : Singleton<SceneLoader>
{
	[Header("Settings")]
	public float fadeDuration;
	public float matrixAnimDuration;
	public List<GameScene> scenes;

	public bool isLoading { get; private set; }

	SceneTag currentSceneTag;

	protected override void Awake()
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
		float timer = LevelManager.IsSceneLevel ? 0 : fadeDuration;

		while (timer < fadeDuration)
		{
			timer += Time.deltaTime;
			float percent = timer / fadeDuration;

			LevelManager.Instance.SetShaderEdges(1 - percent);

			yield return new WaitForEndOfFrame();
		}

		timer = 0;
		
		SceneTransition.SetUpFade();
		while (timer < matrixAnimDuration)
		{
			timer += Time.deltaTime;
			float percent = timer / matrixAnimDuration;

			SceneTransition.SetFadePercent(percent);

			// animate matrix shader

			yield return new WaitForEndOfFrame();
		}
		SceneTransition.CleanUpFade();

		SceneManager.LoadScene(scene.buildIndex);
		yield return new WaitForEndOfFrame();

		timer = 0;
		
		SceneTransition.SetUpFade();
		while (timer < matrixAnimDuration)
		{
			timer += Time.deltaTime;
			float percent = 1 - (timer / matrixAnimDuration);

			SceneTransition.SetFadePercent(percent);

			if (LevelManager.IsSceneLevel)
				LevelManager.Instance.SetShaderEdges(0);

			yield return new WaitForEndOfFrame();
		}
		SceneTransition.CleanUpFade();
		
		timer = LevelManager.IsSceneLevel ? 0 : fadeDuration;

		while (timer < fadeDuration)
		{
			timer += Time.deltaTime;
			float percent = timer / fadeDuration;

			LevelManager.Instance.SetShaderEdges(percent);
			yield return new WaitForEndOfFrame();
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
				onSceneLoaded = callback;
		}

		public void OnSceneLoaded()
		{
			onSceneLoaded?.Invoke();
		}
	}
}
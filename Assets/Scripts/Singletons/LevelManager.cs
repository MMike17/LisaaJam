using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
	List<FadableObject> levelFadables;

	public static bool IsSceneLevel => Instance != null;

	protected override void Awake()
	{
		base.Awake();

		SceneLoader.Instance.SubscribeEvent(SceneLoader.GameScene.SceneTag.Software_0, () =>
		{
			StartLevel();
			Config.Instance.SetCanPause(true);
		});

		// SceneLoader.Instance.SubscribeEvent(SceneLoader.GameScene.SceneTag.Software_, StartLevel);

		Renderer[] renderers = FindObjectsOfType<Renderer>();
		levelFadables = new List<FadableObject>();

		foreach (Renderer renderer in renderers)
		{
			if (renderer.material.shader.name == "Shader Graphs/TronLightEdgeLit")
			{
				FadableObject fadable = new FadableObject(renderer);
				fadable.Init();

				levelFadables.Add(fadable);
			}
		}
	}

	public void SetShaderEdges(float percent)
	{
		foreach (FadableObject fadable in levelFadables)
			fadable.SetLineWidth(percent);
	}

	public void StartLevel()
	{
		// start virusses and player movement
	}

	class FadableObject
	{
		public Renderer renderer;

		public FadableObject(Renderer renderer)
		{
			this.renderer = renderer;
		}

		float initialWidth;

		public void Init()
		{
			initialWidth = renderer.material.GetFloat("_OutlineNormalStrength");
		}

		public void SetLineWidth(float percent)
		{
			renderer.material.SetFloat("_OutlineNormalStrength", Mathf.Lerp(0, initialWidth, percent));
		}
	}
}
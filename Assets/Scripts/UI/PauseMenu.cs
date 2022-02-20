using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[Header("Settings")]
	public float randomDelay;

	[Header("Scene references")]
	public GameObject panel;
	public SkinData[] datas;

	bool isPaused, scheduled;

	void Awake()
	{
		panel.SetActive(false);
		DontDestroyOnLoad(gameObject);

		Skinning.Init(datas[0]);

		isPaused = false;
	}

	void Update()
	{
		if (Input.GetKeyDown(Config.Instance.pauseKey) && Config.Instance.canPause)
			ChangePause();

		if (!scheduled)
		{
			scheduled = true;

			this.DelayAction(() =>
			{
				Skinning.ResetSkin(datas[Random.Range(0, datas.Length - 1)]);
				scheduled = false;
			}, randomDelay);
		}
	}

	public void MainMenu()
	{
		SceneLoader.Instance.LoadScene(SceneLoader.GameScene.SceneTag.Menu);
		ChangePause();
		Config.Instance.SetCanPause(false);
	}

	public void ChangePause()
	{
		isPaused = !isPaused;

		Time.timeScale = isPaused ? 0 : 1;
		panel.SetActive(isPaused);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
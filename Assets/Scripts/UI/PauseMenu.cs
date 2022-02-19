using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[Header("Scene references")]
	public GameObject panel;

	bool isPaused;

	void Awake()
	{
		panel.SetActive(false);
		DontDestroyOnLoad(gameObject);

		isPaused = false;
	}

	void Update()
	{
		if (Input.GetKeyDown(Config.Instance.pauseKey) && Config.Instance.canPause)
			ChangePause();
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
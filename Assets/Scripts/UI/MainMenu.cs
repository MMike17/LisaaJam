using UnityEngine;
using static SceneLoader.GameScene;

public class MainMenu : MonoBehaviour
{
	[Header("Scene references")]
	public GameObject mainPanel;
	public GameObject settingsPanel;

	public void Play()
	{
		SceneLoader.Instance.LoadScene(SceneTag.Hardware);
	}

	public void Settings(bool state)
	{
		mainPanel.SetActive(!state);
		settingsPanel.SetActive(state);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
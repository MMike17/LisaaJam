using UnityEngine;
using UnityEngine.UI;
using static SceneLoader.GameScene;

[RequireComponent(typeof(GraphicRaycaster))]
public class MainMenu : MonoBehaviour
{
	[Header("Scene references")]
	public GameObject mainPanel;
	public GameObject settingsPanel;

	GraphicRaycaster raycaster;

	void Awake()
	{
		raycaster = GetComponent<GraphicRaycaster>();

		SceneLoader.Instance.SubscribeEvent(SceneLoader.GameScene.SceneTag.Menu, () =>
		{
			FindObjectOfType<MainMenu>().SetUIInterractibility(true);
			Config.Instance.SetCanPause(false);
		});

		SceneLoader.Instance.SubscribeEvent(SceneLoader.GameScene.SceneTag.Hardware, () => Config.Instance.SetCanPause(true));

		// detect first launch
		if (!SceneLoader.Instance.isLoading)
		{
			SetUIInterractibility(true);
			Config.Instance.SetCanPause(false);
		}
	}

	void SetUIInterractibility(bool state)
	{
		raycaster.enabled = state;
	}

	public void Play()
	{
		SceneLoader.Instance.LoadScene(SceneTag.Hardware);
		SetUIInterractibility(false);
	}

	public void Settings(bool state)
	{
		mainPanel.SetActive(!state);
		settingsPanel.SetActive(state);
	}

	public void Quit()
	{
		Application.Quit();
		SetUIInterractibility(false);
	}
}
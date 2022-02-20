using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[Header("Settings")]
	public float randomDelay;

	[Header("Scene references")]
	public GameObject panel;
	public SkinData[] datas;
	public SkinData tempData;

	bool isPaused;
	SkinData nextData;

	void Awake()
	{
		panel.SetActive(false);
		DontDestroyOnLoad(gameObject);

		nextData = datas[Random.Range(0, datas.Length - 1)];
		tempData.FixSlots();
		StartCoroutine(RazerRGB());

		isPaused = false;
	}

	void Update()
	{
		if (Input.GetKeyDown(Config.Instance.pauseKey) && Config.Instance.canPause)
			ChangePause();
	}

	IEnumerator RazerRGB()
	{
		float timer = 0;
		SkinData current = nextData;
		nextData = datas[Random.Range(0, datas.Length - 1)];

		while (timer < randomDelay)
		{
			float percent = timer / randomDelay;

			int index = tempData.skin_slots.IndexOf(tempData.skin_slots.Find(item => item.tag == SkinTag.PRIMARY_WINDOW));
			tempData.skin_slots[index].color = Color.Lerp(current.skin_slots[index].color, nextData.skin_slots[index].color, percent);

			index = tempData.skin_slots.IndexOf(tempData.skin_slots.Find(item => item.tag == SkinTag.PRIMARY_ELEMENT));
			tempData.skin_slots[index].color = Color.Lerp(current.skin_slots[index].color, nextData.skin_slots[index].color, percent);

			Skinning.ResetSkin(tempData);

			yield return null;
		}

		StartCoroutine(RazerRGB());
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
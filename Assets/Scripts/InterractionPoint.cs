using UnityEngine;
using UnityEngine.UI;
using static SceneLoader.GameScene;

public class InterractionPoint : MonoBehaviour
{
	[Header("Settings")]
	public Color inColor;
	public Color outColor;

	[Header("Scene references")]
	public RectTransform indicator;
	public Image indicatorIn;
	public Canvas canvas;
	public new Renderer renderer;

	void Awake()
	{
		indicator.SetParent(canvas.transform);
		indicatorIn.color = outColor;
	}

	void Update()
	{
		indicator.gameObject.SetActive(renderer.isVisible);
		indicator.position = Camera.main.WorldToScreenPoint(transform.position);
	}

	public void Notify(bool state)
	{
		indicatorIn.color = state ? inColor : outColor;
	}

	public void OpenDigital()
	{
		SceneLoader.Instance.LoadScene(SceneTag.Software_0);
	}
}
using UnityEngine;

public class SkinManager : MonoBehaviour
{
	[Header("Settings")]
	public SkinData[] skinDatas;

	void Awake()
	{
		if (skinDatas == null || skinDatas.Length == 0)
			return;

		Skinning.Init(skinDatas[0]);
	}

	public void SetSkin(int index)
	{
		Skinning.ResetSkin(skinDatas[index]);
	}
}
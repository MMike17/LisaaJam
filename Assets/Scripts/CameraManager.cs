using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
	[Header("Settings")]
	public CameraPreset[] cameraPresets;

	[Header("Scene references")]
	public Camera mainCamera;

	[Header("Debug")]
	public int presetIndex;
	public bool enableTest;

	CameraPreset currentPreset => cameraPresets[presetIndex];
	int lastPresetIndex;

	void OnDrawGizmos()
	{
		if (cameraPresets != null)
		{
			presetIndex = Mathf.Clamp(presetIndex, 0, cameraPresets.Length - 1);

			foreach (CameraPreset preset in cameraPresets)
			{
				if (preset.needsEditorColor)
				{
					Color color = Color.HSVToRGB(Random.value, 1, 1);
					color.a = 0.5f;
					preset.SetEditorColor(color);
				}

				preset.ClampValues();

				if (preset.target != null)
				{
					Gizmos.color = preset.editorColor;
					Gizmos.DrawSphere(preset.target.position, 0.3f);
					Gizmos.DrawLine(preset.target.position, preset.target.position + preset.target.forward);
				}
			}

			if (enableTest)
				SetCameraSettings(currentPreset);
		}
	}

	void SetCameraSettings(CameraPreset preset)
	{
		mainCamera.transform.position = preset.target.position;
		mainCamera.transform.rotation = preset.target.rotation;
		mainCamera.transform.SetParent(preset.target);

		mainCamera.fieldOfView = preset.fov;
		mainCamera.orthographicSize = preset.orthographicFov;
		mainCamera.orthographic = preset.orthographic;
	}

	[Serializable]
	public class CameraPreset
	{
		public Transform target;
		public bool orthographic = false;
		public float fov;
		public float orthographicFov;

		public bool needsEditorColor => editorColor == default(Color);
		public Color editorColor { get; private set; }

		public void ClampValues()
		{
			if (orthographic)
				fov = 0;
			else
				orthographicFov = 0;
		}

		public void SetEditorColor(Color color)
		{
			editorColor = color;
		}
	}
}
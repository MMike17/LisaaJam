using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : Singleton<CameraManager>
{
	[Header("Settings")]
	public CameraPreset[] cameraPresets;
	public float minVerticalRot, maxVerticalRot, rotationSpeed;

	[Header("Scene references")]
	public Camera mainCamera;

	[Header("Debug")]
	public int presetIndex;
	public bool enableTest;

	CameraPreset currentPreset => cameraPresets[presetIndex];

	float verticalRotAmount;

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
				SetCameraSettings();
		}
	}

	public override void Awake()
	{
		base.Awake();

		verticalRotAmount = 0;
	}

	void SetCameraSettings()
	{
		mainCamera.transform.position = currentPreset.target.position;
		mainCamera.transform.rotation = currentPreset.target.rotation;
		mainCamera.transform.SetParent(currentPreset.target);

		mainCamera.fieldOfView = currentPreset.fov;
		mainCamera.orthographicSize = currentPreset.orthographicFov;
		mainCamera.orthographic = currentPreset.orthographic;
	}

	public void SetCameraPreset(int index)
	{
		presetIndex = Mathf.Clamp(index, 0, cameraPresets.Length - 1);
		SetCameraSettings();
	}

	public void AddVerticalRotation(float amount)
	{
		verticalRotAmount = Mathf.Clamp(verticalRotAmount + amount * rotationSpeed, minVerticalRot, maxVerticalRot);
		mainCamera.transform.rotation = currentPreset.target.rotation;
		mainCamera.transform.Rotate(Vector3.right * verticalRotAmount);
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
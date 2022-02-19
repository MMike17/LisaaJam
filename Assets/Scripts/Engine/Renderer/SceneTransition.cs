using System;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Material transitionMaterial;

    private MeshRenderer quad;

    private bool isFading;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public void SetUpFade()
    {
        if (isFading) return;
        isFading = true;
        var cam = Camera.main;
        if (cam == null) return;
        var screenCap = GetScreenCap(cam);
        if (screenCap == null) return;

        quad = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshRenderer>();
        SetQuadTransform(cam, quad.transform);

        quad.material = transitionMaterial;
        quad.material.SetTexture(MainTex, screenCap);
    }

    /// <param name="percent">Must be 0 - 1 range</param>
    public void SetFadePercent(float percent)
    {
        if (!isFading) SetUpFade();
        quad.material.SetFloat(DissolveAmount, percent);
    }

    public void CleanUpFade()
    {
        if (!isFading) return;
        Destroy(quad.gameObject);
        quad = null;
        isFading = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) SetUpFade();
    }

    private static void SetQuadTransform(Camera cam, Transform quad)
    {
        var aspectRatio = Screen.width / (float) Screen.height; // in respect to width

        var posOffset = cam.nearClipPlane + 0.01f;
        quad.transform.position = cam.transform.position + Vector3.forward * posOffset;
        var direction = quad.transform.position - cam.transform.position;
        quad.transform.rotation = Quaternion.LookRotation(direction);

        var height = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * posOffset * 2f;
        var width = height * aspectRatio;
        
        quad.SetParent(cam.transform);

        quad.localScale = new Vector3(width, height, 1f);
    }

    private static Texture2D GetScreenCap(Camera cam)
    {
        if (cam == null) return null;
        var width = cam.pixelWidth;
        var height = cam.pixelHeight;

        var result = new Texture2D(width, height, TextureFormat.RGB24, false);
        var renderTexture = new RenderTexture(width, height, 24);

        cam.targetTexture = renderTexture;
        cam.Render();
        RenderTexture.active = renderTexture;

        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;

        Destroy(renderTexture);

        return result;
    }
}
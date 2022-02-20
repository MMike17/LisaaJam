using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SceneTransition
{
    private static Material transitionMaterial, backdropMaterial;
    private static UniversalAdditionalCameraData uac;

    private static MeshRenderer quad, backdrop;

    private static bool isFading;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public static void SetUpFade()
    {
        if (isFading) CleanUpFade();

        var cam = GameObject.FindObjectOfType<Camera>();
        if (uac == null) uac = cam.GetComponent<UniversalAdditionalCameraData>();
        uac.renderPostProcessing = false;

        var screenCap = GetScreenCap(cam);
        if (screenCap == null) return;
        isFading = true;
        
        if (transitionMaterial == null) transitionMaterial = Resources.Load<Material>("SceneDissolveMaterial");
        if (backdropMaterial == null) backdropMaterial = Resources.Load<Material>("SceneBackdropMaterial");
        
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshRenderer>();
        quad.name = "Scene Transition - Dissolve Quad";
        SetQuadTransform(cam, quad.transform, 0.01f);
        quad.material = transitionMaterial;
        quad.material.SetTexture(MainTex, screenCap);
        
        backdrop = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshRenderer>();
        backdrop.name = "Scene Transition - Backdrop Quad";
        SetQuadTransform(cam, backdrop.transform, 0.02f);
        backdrop.material = backdropMaterial;
    }

    /// <param name="percent">Must be 0 - 1 range</param>
    public static void SetFadePercent(float percent)
    {
        if (!isFading) SetUpFade();
        quad.material.SetFloat(DissolveAmount, percent);
    }

    public static void CleanUpFade()
    {
        if (!isFading) return;
        GameObject.Destroy(quad.gameObject);
        GameObject.Destroy(backdrop.gameObject);
        quad = null;
        uac.renderPostProcessing = false;
        isFading = false;
    }
    
    private static void SetQuadTransform(Camera cam, Transform quad, float offset)
    {
        var aspectRatio = Screen.width / (float) Screen.height; // in respect to width
        quad.SetParent(cam.transform);
        var posOffset = cam.nearClipPlane + offset;
        quad.transform.position = cam.transform.position + cam.transform.forward * posOffset;
        var direction = quad.transform.position - cam.transform.position;
        quad.transform.rotation = Quaternion.LookRotation(direction);

        var height = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * posOffset * 2f;
        var width = height * aspectRatio;
        

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

        GameObject.Destroy(renderTexture);

        return result;
    }
}
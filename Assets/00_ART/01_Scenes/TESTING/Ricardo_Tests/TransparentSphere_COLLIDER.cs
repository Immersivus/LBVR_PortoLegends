using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(SphereCollider))]
public class InsideSphereVisibility : MonoBehaviour
{
    Renderer rend;
    SphereCollider col;

    [Header("Optional (assign if SDK uses custom camera)")]
    public Camera targetCamera;

    void OnEnable()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<SphereCollider>();
    }

    void Update()
    {
        Camera cam = GetActiveCamera();
        if (!cam || !rend || !col) return;

        Vector3 center = transform.TransformPoint(col.center);

        float radius = Mathf.Max(
            transform.lossyScale.x,
            transform.lossyScale.y,
            transform.lossyScale.z
        ) * col.radius;

        float distance = Vector3.Distance(cam.transform.position, center);

        bool inside = distance < radius;

        rend.forceRenderingOff = !inside;
    }

    Camera GetActiveCamera()
    {
        if (targetCamera != null)
            return targetCamera;

        // BEST FIX for SDKs: find the enabled rendering camera
        foreach (var cam in Camera.allCameras)
        {
            if (cam.isActiveAndEnabled && cam.CompareTag("MainCamera"))
                return cam;
        }

        // fallback: any enabled camera
        foreach (var cam in Camera.allCameras)
        {
            if (cam.isActiveAndEnabled)
                return cam;
        }

        return null;
    }
}
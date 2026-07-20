using UnityEngine;

public class OcclusionVisibilityController : MonoBehaviour
{
    public Renderer targetRenderer;
    public Renderer targetRendererPortal;
    public Camera targetCamera;

    public LayerMask occluderMask;

    void Update()
    {
        if (!targetRenderer) return;

        Camera cam = GetCamera();
        if (!cam) return;

        Vector3 origin = cam.transform.position;
        Vector3 targetPos = targetRenderer.bounds.center;

        Vector3 dir = targetPos - origin;
        float dist = dir.magnitude;

        Ray ray = new Ray(origin, dir.normalized);

        bool blocked = Physics.Raycast(ray, dist, occluderMask);

        // DEBUG (optional)
        // Debug.DrawRay(origin, dir.normalized * dist, blocked ? Color.red : Color.green);

        targetRenderer.enabled = !blocked;
        targetRendererPortal.enabled = !blocked;
    }

    Camera GetCamera()
    {
        if (targetCamera != null)
            return targetCamera;

        foreach (var cam in Camera.allCameras)
        {
            if (cam.isActiveAndEnabled)
                return cam;
        }

        return null;
    }
}
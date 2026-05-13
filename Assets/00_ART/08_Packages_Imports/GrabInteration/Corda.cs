using UnityEngine;

public class Corda : MonoBehaviour
{
    public Transform pontoTopo;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        lineRenderer.SetPosition(0, pontoTopo.position);
        lineRenderer.SetPosition(1, transform.position);
    }
}
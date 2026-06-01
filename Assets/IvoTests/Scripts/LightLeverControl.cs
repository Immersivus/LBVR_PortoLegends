using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LightLeverControl : MonoBehaviour
{

    [SerializeField] private HingeJoint verticalLever;
    [SerializeField] private HingeJoint horizontalLever;

    public List<Material> lightMaterials;

    private Renderer myRenderer;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (verticalLever.angle)
        {
            case >= 45:
                if (horizontalLever.angle < 45)
                {
                    myRenderer.material = lightMaterials[1];
                }
                else
                {
                    myRenderer.material = lightMaterials[2];
                }
                break;
            case < 45:
                if (horizontalLever.angle >= 45)
                {
                    myRenderer.material = lightMaterials[1];
                }
                else
                {
                    myRenderer.material = lightMaterials[0];
                }
                break;
        }
    }
}

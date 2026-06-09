using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using XRoam.Users;

public class DebugScript: MonoBehaviour
{
    public InputActionReference uiPress;
    public TextMeshProUGUI text;
    private void Update()
    {
        text.text = uiPress.action.ReadValue<float>().ToString();
    }
}

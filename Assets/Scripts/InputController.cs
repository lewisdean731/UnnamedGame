using UnityEngine;
using UnityEngine.EventSystems;
public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //Make sure mouse is NOT over a UI element
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            HandleMouseInput();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) { GameEvents.current.Mouse0Down(); }
        if (Input.GetMouseButtonDown(1)) { GameEvents.current.Mouse1Down(); }
    }
}
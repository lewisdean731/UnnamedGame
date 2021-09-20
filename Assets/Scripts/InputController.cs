using UnityEngine;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) { GameEvents.current.Mouse0Down(); }
        if (Input.GetMouseButtonDown(1)) { GameEvents.current.Mouse1Down(); }
    }
}
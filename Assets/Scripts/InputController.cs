using UnityEngine;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) { GameEvents.current.Mouse0Down(); }
        if (Input.GetMouseButtonDown(1)) { GameEvents.current.Mouse1Down(); }
    }


}

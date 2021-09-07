using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public float minHeight;
    public float maxHeight;
    public float zoomAmountScaler;

    public float movementSpeed;
    public float movementSpeedFast;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

    void HandleMouseInput()
    {
        // Scroll Wheel Zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            CalcZoom(Input.mouseScrollDelta.y);
        }

        // Right-click and drag movement
        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                Debug.Log(entry);
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        // Middle-mouse rotate
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }

    void HandleMovementInput()
    {
        // Movement

        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? movementSpeedFast : movementSpeed;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -moveSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -moveSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * moveSpeed);
        }

        // Rotation
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        // Zoom
        if (Input.GetKey(KeyCode.R))
        {
            CalcZoom(1);
        }
        if (Input.GetKey(KeyCode.F))
        {
            CalcZoom(-1);
        }
    }

    private void CalcZoom(float zoomValue = 1)
    {
        // Zoom slower when closer to zoom limits
        // See https://www.desmos.com/calculator and copy the below
        // \frac{\left(-n-\left(\frac{\left(a+n\right)}{\left(a+n\right)^{2}}\right)\cdot\ x^{2}\ +\ x\ \right)}{10}
        var maxmin = maxHeight + minHeight;
        var maxmin2 = maxmin * maxmin;
        zoomAmountScaler = (-minHeight - maxmin / maxmin2 * (newZoom.y * newZoom.y) + newZoom.y) / 10;

        if (zoomAmountScaler < 0.05f)
        {
            zoomAmountScaler = 0.05f;
        }

        // Allow scale out
        if (newZoom.y + 1 < maxHeight && zoomValue < 0)
        {
            newZoom += (zoomValue * zoomAmountScaler) * zoomAmount;
        }

        // Allow scale in
        else if (newZoom.y - 1 > minHeight && zoomValue > 0)
        {
            newZoom += (zoomValue * zoomAmountScaler) * zoomAmount;
        }
    }
}

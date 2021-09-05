using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public List<GameObject> selected;

    private void Awake()
    {
        selected = new List<GameObject>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        GameEvents.current.onMouse0Down += OnMouse0Down;
    }

    private void OnDestroy()
    {
        GameEvents.current.onMouse0Down -= OnMouse0Down;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnMouse0Down()
    {
        var raycastSelected = SelectionRaycast();
        if (raycastSelected != null)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                foreach (GameObject g in selected)
                {
                    RemoveOutline(g);
                }
                selected.Clear();
            }
            if (!selected.Contains(raycastSelected))
            {
                ApplyOutline(raycastSelected);
                selected.Add(raycastSelected);
            }
        }
    }

    private GameObject? SelectionRaycast()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != null)
            {
                Debug.Log(hit.transform.gameObject);
                return hit.transform.gameObject;
            }
        }
        return null;

    }

    private void ApplyOutline(GameObject g)
    {
        if (g.GetComponent<Outline>() == null)
        {
            var outline = g.AddComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 5f;
        }
    }

    private void RemoveOutline(GameObject g)
    {
        if (g.GetComponent<Outline>() != null)
        {
            Destroy(g.GetComponent<Outline>());
        }
    }
}

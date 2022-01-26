using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    int brushSize;

    private Color activeColor;
    private int activeElevation;

    bool applyColor = true;
    bool applyElevation = false;

    private void Awake()
    {
        activeColor = colors[0];
        activeElevation = 0;
    }

    private void Start()
    {
        GameEvents.current.onColorCell += OnColorCell;
        GameEvents.current.onCellSelected += OnCellSelected;
    }

    private void OnDestroy()
    {
        GameEvents.current.onColorCell -= OnColorCell;
        GameEvents.current.onCellSelected -= OnCellSelected;
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0; // bool
        if (applyColor)
        {
            activeColor = colors[index];
        }
    }

    public void SetApplyElevation()
    {
        applyColor = false;
        applyElevation = true;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void SetShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }



    public void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (applyColor)
            {
                cell.Color = activeColor;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
        }
    }

    public void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

    }

    private void OnColorCell(HexCell cell)
    {
        hexGrid.ColorCell(cell, activeColor);
    }

    private void OnCellSelected(HexCell cell)
    {
        EditCells(cell);
    }
}
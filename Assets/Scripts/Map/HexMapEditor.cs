using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

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

    public void EditCell(HexCell cell)
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

    private void OnColorCell(HexCell cell)
    {
        hexGrid.ColorCell(cell, activeColor);
    }

    private void OnCellSelected(HexCell cell)
    {
        EditCell(cell);
    }
}
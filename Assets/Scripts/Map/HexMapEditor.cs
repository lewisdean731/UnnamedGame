using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;
    private int activeElevation;

    private void Awake()
    {
        SelectColor(0);
        SetElevation(0);
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
        activeColor = colors[index];
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void EditCell(HexCell cell)
    {
        cell.color = activeColor;
        cell.Elevation = activeElevation;
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
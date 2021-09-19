using UnityEngine;

public class HexMapEditor : MonoBehaviour
{

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	void Awake()
	{
		SelectColor(0);
	}

	private void Start()
	{
		GameEvents.current.onColorCell += OnColorCell;
	}

	private void OnDestroy()
	{
		GameEvents.current.onColorCell -= OnColorCell;
	}

	void OnColorCell(Vector3 position)
    {
		hexGrid.ColorCell(position, activeColor);
	}

	public void SelectColor(int index)
	{
		activeColor = colors[index];
	}
}
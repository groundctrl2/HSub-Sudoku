using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    public SudokuGrid grid;
    public int index;
    public Vector3 position;
    private TextMeshProUGUI cellText;
    private bool isHovered = false;
    private bool isClicked = false;

    public void Initialize(SudokuGrid newGrid, int newIndex, Vector3 newPosition, TextMeshProUGUI textComponent)
    {
        grid = newGrid;
        index = newIndex;
        position = newPosition;
        transform.position = position;
        cellText = textComponent;
    }

    void OnMouseDown()
    {
        SetText($"0");
        isClicked = true;
    }

    void OnMouseOver()
    {
        if (!isHovered)
        {
            GetComponent<Renderer>().material = grid.materials[1]; // Hover material
            isHovered = true;
        }
    }

    void OnMouseExit()
    {
        if (isHovered)
        {
            GetComponent<Renderer>().material = grid.materials[0]; // Normal material
            isHovered = false;
        }
    }

    public void SetText(string text)
    {
        cellText.text = text;
    }
}
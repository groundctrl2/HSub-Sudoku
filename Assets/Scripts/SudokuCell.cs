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

    private static bool isSelecting = false;
    private static bool isDeselecting = false;
    private static bool isMouseDragging = false;
    private bool isSelected = false;

    Material normal;
    Material hovered;
    Material selected;

    public void Initialize(SudokuGrid newGrid, int newIndex, Vector3 newPosition, TextMeshProUGUI textComponent)
    {
        grid = newGrid;
        index = newIndex;
        position = newPosition;
        transform.position = position;
        cellText = textComponent;

        normal = grid.materials[0];
        hovered = grid.materials[1];
        selected = grid.materials[2];
    }

    // Set the cell's text
    public void SetText(string text)
    {
        cellText.text = text;
    }

    // Set the cell's material and bool value whether the cell is selected
    public void SetMaterial(Material material)
    {
        GetComponent<Renderer>().material = material;
        isSelected = (material == selected) ? true : false;
    }

    // Reset select bool values, set the material to normal, and update the grid's selected record
    public void Deselect()
    {
        isSelecting = false;
        isDeselecting = false;
        SetMaterial(normal);
        grid.SetSelected(index, false);
    }

    // Select/Deselect cell and start dragging
    void OnMouseDown()
    {
        if (!isMouseDragging)
        {
            if (!isSelected)
            {
                isSelecting = true;
                SetMaterial(selected);
                grid.SetSelected(index, true);
            }
            else
            {
                isDeselecting = true;
                SetMaterial(hovered);
                grid.SetSelected(index, false);
            }
        }
    }

    // Update cell depending on selecting states and whether mouse is dragging
    void OnMouseOver()
    {
        if (isMouseDragging && isSelecting)
        {
            SetMaterial(selected);
            grid.SetSelected(index, true);
        }
        else if (isMouseDragging && isDeselecting)
        {
            SetMaterial(hovered);
            grid.SetSelected(index, false);
        }
        else if (!isSelecting && !isDeselecting && !isSelected)
        {
            SetMaterial(hovered);
        }
    }

    // Set back to normal material if previously hovering
    void OnMouseExit()
    {
        if (!isSelecting && !isSelected)
        {
            SetMaterial(normal);
        }
    }

    // Set that mouse is dragging
    void OnMouseDrag()
    {
        isMouseDragging = true;
    }

    // Set selecting bool values back to false
    void OnMouseUp()
    {
        isMouseDragging = false;
        isSelecting = false;
        isDeselecting = false;
    }
}
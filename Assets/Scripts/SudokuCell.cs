using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuCell : MonoBehaviour
{
    public SudokuGrid grid;
    public int index;
    public Vector3 position;
    private bool isHovered = false;

    public void Initialize(SudokuGrid newGrid, int newIndex, Vector3 newPosition)
    {
        grid = newGrid;
        index = newIndex;
        position = newPosition;
        transform.position = position;
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
}

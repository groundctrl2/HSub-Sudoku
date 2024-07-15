using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using TMPro;

public class SudokuGrid : MonoBehaviour
{
    public GameObject cellsParent;
    public GameObject cellPrefab;
    public GameObject cellTextPrefab;
    public Material[] materials = new Material[3];

    private Vector3 gridCenter = new Vector3(-8.5f, 0, 0);
    private Vector3[] cellPositions = new Vector3[81];
    private SudokuCell[] sudokuCells = new SudokuCell[81];

    private bool[] cellsClicked = new bool[81];
    private int[] cellValues = new int[81];

    // Start is called before the first frame update
    void Start()
    {
        GetCellPositions();
        DrawGrid();

        for (int i = 0; i < 81; i++)
        {
            cellsClicked[i] = false;
            cellValues[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Calculates and stores (by index) all 81 cell positions based on cell prefab size
    private void GetCellPositions()
    {
        Renderer renderer = cellPrefab.GetComponent<Renderer>();
        Vector3 cellSize = renderer.bounds.size;
        float cellOffset = cellSize.x / 15;  // Assuming square cells; adjust if necessary

        // Calculate the total width and height of the grid
        float gridWidth = 9 * cellSize.x + 8 * cellOffset;
        float gridHeight = 9 * cellSize.y + 8 * cellOffset;

        // Calculate the top left corner based on the center
        Vector3 gridTopLeft = gridCenter + new Vector3(-gridWidth / 2, gridHeight / 2, 0);

        int index = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                // Calculate the position of each cell, adjusted by cell size
                float newX = (cellSize.x + cellOffset) * col + cellSize.x / 2;
                float newY = -(cellSize.y + cellOffset) * row - cellSize.y / 2;
                Vector3 position = gridTopLeft + new Vector3(newX, newY, 0);

                cellPositions[index++] = position; // Store cell position
            }
        }
    }

    // Iterates through each cell position and creates GameObject and corresponding SudokuCell
    private void DrawGrid()
    {
        for (int i = 0; i < cellPositions.Length; i++)
        {
            Vector3 position = cellPositions[i];

            // Create cell GameObject
            GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
            cell.transform.SetParent(cellsParent.transform, true); // Ensure cell is parented to the canvas's cells GameObject
            cell.AddComponent<BoxCollider>();

            // Add the text GameObject
            GameObject cellText = Instantiate(cellTextPrefab, cell.transform);
            cellText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform rectTransform = cellText.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;

            // Create SudokuCell
            SudokuCell sudokuCell = cell.AddComponent<SudokuCell>();
            sudokuCell.Initialize(this, i, position, cellText.GetComponent<TextMeshProUGUI>());
            sudokuCells[i] = sudokuCell; // Store SudokuCell
        }
    }

    // Set whether a cell at given index is selected
    public void SetSelected(int index, bool isSelected)
    {
        if (isSelected)
            cellsClicked[index] = true;
        else
            cellsClicked[index] = false;
    }

    // Add the given number value to the selected cells' text and recorded value
    public void NumberSelected(int buttonNumber)
    {
        for (int i = 0; i < 81; i++)
        {
            if (cellsClicked[i])
            {
                sudokuCells[i].SetText($"{buttonNumber}");
                cellValues[i] = buttonNumber;
            }
        }
    }

    // Deselect all cells
    public void DeselectAll()
    {
        for (int i = 0; i < 81; i++)
            sudokuCells[i].Deselect();
    }

    // Clear the given number value to the selected cells' text and recorded value and deselect 
    public void ClearSelected()
    {
        for (int i = 0; i < 81; i++)
        {
            if (cellsClicked[i])
            {
                sudokuCells[i].SetText("");
                cellValues[i] = 0;
                sudokuCells[i].Deselect();
                cellsClicked[i] = false;
            }
        }
    }
}
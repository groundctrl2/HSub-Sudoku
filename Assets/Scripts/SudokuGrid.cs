using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    public GameObject cellPrefab;
    public Material[] materials = new Material[2];

    private Vector3Int gridCenter = new Vector3Int(0, 0, 0);
    private Vector3[] cellPositions = new Vector3[81];
    private SudokuCell[] sudokuCells = new SudokuCell[81];

    // Start is called before the first frame update
    void Start()
    {
        GetCellPositions();
        DrawGrid();
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

            // Create GameObject
            GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
            cell.AddComponent<BoxCollider>();

            // Create SudokuCell
            SudokuCell sudokuCell = cell.AddComponent<SudokuCell>();
            sudokuCell.Initialize(this, i, position);

            sudokuCells[i] = sudokuCell; // Store SudokuCell
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms;

public class SudokuGrid : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject cellParent;
    public GameObject cellPrefab;
    public GameObject mainTextPrefab;
    public GameObject noteTextPrefab;
    public Material[] materials = new Material[3];

    private Vector3 gridCenter;
    private Vector3[] cellPositions = new Vector3[81];
    private SudokuCell[] sudokuCells = new SudokuCell[81];

    private bool[] cellsClicked = new bool[81];
    private int[] cellDigits = new int[81];
    private float cellSize; // Set in ResizeCellAndCenter()

    // Start is called before the first frame update
    void Start()
    {
        ResizeCellAndCenter();
        GetCellPositions();
        DrawGrid();

        for (int i = 0; i < 81; i++)
        {
            cellsClicked[i] = false;
            cellDigits[i] = 0;
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
        float cellOffset = cellSize.x / 15; // Assuming square cells; adjust if necessary

        // Calculate the total width and height of the grid
        float gridWidth = 9 * cellSize.x + 9 * cellOffset; // Adjusted for extra box/subgrid offsets (without: 8 * cellOffset)
        float gridHeight = 9 * cellSize.y + 9 * cellOffset; // Adjusted for extra box/subgrid offsets

        // Calculate the top left corner based on the center
        Vector3 gridTopLeft = gridCenter + new Vector3(-gridWidth / 2, gridHeight / 2, 0);

        int index = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                // Calculate the position of each cell, adjusted by cell size and offsets
                float newX = (cellSize.x + cellOffset) * col + cellSize.x / 2;
                float newY = -(cellSize.y + cellOffset) * row - cellSize.y / 2;

                // Add additional offset to separate 3x3 boxes/subgrids
                if (col >= 3) newX += cellOffset;
                if (col >= 6) newX += cellOffset;
                if (row >= 3) newY -= cellOffset;
                if (row >= 6) newY -= cellOffset;

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
            Vector2 noteSize = new Vector2(cellSize * 12, cellSize * 3);

            // Create cell GameObject
            GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
            cell.transform.SetParent(cellParent.transform, true); // Ensure cell is parented to the canvas's cells GameObject
            cell.transform.localPosition = new Vector3(position.x, position.y, 0); // Set the position explicitly
            cell.AddComponent<BoxCollider>();

            Debug.Log($"Cell {i} World Position: {cell.transform.position}, Local Position: {cell.transform.localPosition}");

            // Add the main text GameObject
            GameObject mainText = Instantiate(mainTextPrefab, cell.transform);
            mainText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform mainRectTransform = mainText.GetComponent<RectTransform>();
            mainRectTransform.anchoredPosition = Vector2.zero;

            // Add the upper note text GameObject
            GameObject upperText = Instantiate(noteTextPrefab, cell.transform);
            upperText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform upperRectTransform = upperText.GetComponent<RectTransform>();
            upperRectTransform.pivot = new Vector2(0.5f, 1f); // Anchor to top
            upperRectTransform.anchoredPosition = new Vector2(0, cellSize / 12); // Position at the top
            upperRectTransform.sizeDelta = noteSize; // Set the width to match the cell width

            // Add the middle note text GameObject
            GameObject middleText = Instantiate(noteTextPrefab, cell.transform);
            middleText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform middleRectTransform = middleText.GetComponent<RectTransform>();
            middleRectTransform.anchoredPosition = Vector2.zero;
            middleRectTransform.sizeDelta = noteSize; // Set the width to match the cell width

            // Add the lower note text GameObject
            GameObject lowerText = Instantiate(noteTextPrefab, cell.transform);
            lowerText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform lowerRectTransform = lowerText.GetComponent<RectTransform>();
            lowerRectTransform.anchoredPosition = new Vector2(0, -cellSize / 16); // Position at the bottom
            lowerRectTransform.sizeDelta = noteSize; // Set the width to match the cell width

            // Create SudokuCell
            SudokuCell sudokuCell = cell.AddComponent<SudokuCell>();
            sudokuCell.Initialize(this, i, position, mainText.GetComponent<TextMeshProUGUI>(), upperText.GetComponent<TextMeshProUGUI>(), middleText.GetComponent<TextMeshProUGUI>(), lowerText.GetComponent<TextMeshProUGUI>());
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

    // Add the given digit to the selected cells' text and recorded value
    public void AddDigitToSelected(int buttonNumber)
    {
        for (int i = 0; i < 81; i++)
        {
            if (cellsClicked[i])
            {
                sudokuCells[i].SetMainText($"{buttonNumber}");
                cellDigits[i] = buttonNumber;
                UpdateNotes();
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
                sudokuCells[i].SetMainText("");
                cellDigits[i] = 0;
                sudokuCells[i].Deselect();
                cellsClicked[i] = false;
            }
        }
    }

    private void ResizeCellAndCenter()
    {
        float orthographicSize = mainCamera.orthographicSize;
        float aspectRatio = mainCamera.aspect;
        float cameraWidth = orthographicSize * aspectRatio;
        float cellSizeRatio = 0.1f; // Ratio of the cell size to the canvas size
        cellSize = cameraWidth * cellSizeRatio;

        // Resize the cell prefab
        Vector3 cellScale = new Vector3(cellSize, cellSize, cellPrefab.transform.localScale.z);
        cellPrefab.transform.localScale = cellScale;

        // Set the grid center
        float gridCenterX = mainCamera.transform.position.x - cameraWidth / 2.3f;
        gridCenter = new Vector3(gridCenterX, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }

    private void UpdateNotes()
    {
        SudokuRules sudokuRules = new SudokuRules(cellDigits);
        for (int i = 0; i < 81; i++)
        {
            if (cellDigits[i] == 0)
                sudokuCells[i].SetNoteText(sudokuRules.notesGrid[i]);
        }
    }
}
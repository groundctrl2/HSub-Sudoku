using UnityEngine;
using TMPro;

// Manages the rendering and interaction of the Sudoku grid in the Unity game.
// Handles the creation of cells, updating their states, and processing user interactions.
public class SudokuGrid : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject cellParent; // Empty parent GameObject for cells
    public GameObject cellPrefab; // Type UI 2D Sprite
    public GameObject mainTextPrefab; // Type TMP Text
    public GameObject noteTextPrefab; // Type TMP Text
    public Material[] materials = new Material[4]; // 0:Normal, 1:Hovered, 2:Clicked/Selected, 3:Hidden Subset Error 
    public Color noteColor;
    public Color incorrectColor;

    // Positioning instantiating
    private Vector3 gridCenter; // Used to center grid when populating
    private Vector3[] cellPositions = new Vector3[81];
    private float cellSize; // Used when populating, set in ResizeCellAndCenter()

    // Sudoku cell
    private SudokuCell[] sudokuCells = new SudokuCell[81]; // 1D index corresponds to 2D position
    private bool[] cellsClicked = new bool[81]; // Bools represent which cells are clicked
    private int[] cellDigits = new int[81]; // Ints represent inputted digits in each cell

    // Sudoku rules
    private SudokuRules sudokuRules;
    private int[] incorrectDigits = new int[81]; // Cell indices contain 0 if correct/empty, or the incorrect value if incorrect

    // Hidden subset rules
    private HSubRules hSubRules;
    private bool isShowingHSubs = false;

    // Actions to take at program start
    void Start()
    {
        ResizeCellAndCenter();
        GetCellPositions();
        DrawGrid();
    }

    // Calculates and stores (by 1D index) all 81 cell positions based on cell prefab size
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

            // Add the main text GameObject
            GameObject mainText = Instantiate(mainTextPrefab, cell.transform);
            mainText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform mainRectTransform = mainText.GetComponent<RectTransform>();
            mainRectTransform.anchoredPosition = Vector2.zero;

            // Add the upper note text GameObject
            GameObject upperText = Instantiate(noteTextPrefab, cell.transform);
            upperText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform upperRectTransform = upperText.GetComponent<RectTransform>();
            upperRectTransform.anchoredPosition = new Vector2(0, cellSize / 9); // Position at the top
            upperRectTransform.sizeDelta = noteSize; // Set the width to match the cell width

            // Add the middle note text GameObject
            GameObject middleText = Instantiate(noteTextPrefab, cell.transform);
            middleText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform middleRectTransform = middleText.GetComponent<RectTransform>();
            middleRectTransform.anchoredPosition = Vector2.zero;
            middleRectTransform.sizeDelta = noteSize; // Set the width to match the cell width
            TextMeshProUGUI middleTextComponent = middleText.GetComponent<TextMeshProUGUI>();
            middleTextComponent.fontStyle = FontStyles.Bold; // Set to bold
            middleTextComponent.fontSize = middleTextComponent.fontSize + (middleTextComponent.fontSize / 5); // Set font size

            // Add the lower note text GameObject
            GameObject lowerText = Instantiate(noteTextPrefab, cell.transform);
            lowerText.transform.SetParent(cell.transform, false); // Ensure cell text is parented to the cell
            RectTransform lowerRectTransform = lowerText.GetComponent<RectTransform>();
            lowerRectTransform.anchoredPosition = new Vector2(0, -cellSize / 9); // Position at the bottom
            lowerRectTransform.sizeDelta = noteSize; // Set the width to match the cell width

            // Create SudokuCell
            SudokuCell sudokuCell = cell.AddComponent<SudokuCell>();
            sudokuCell.Initialize(this, i, position, mainText.GetComponent<TextMeshProUGUI>(), upperText.GetComponent<TextMeshProUGUI>(), middleText.GetComponent<TextMeshProUGUI>(), lowerText.GetComponent<TextMeshProUGUI>());
            sudokuCells[i] = sudokuCell; // Store SudokuCell
        }

        // Initial cell update
        UpdateCells();
    }

    // Resizes the cell prefab and updates the grid center Vector3
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

    // Set whether a cell at given index is selected
    public void SetSelected(int index, bool isSelected)
    {
        cellsClicked[index] = isSelected;
    }

    // Add the given digit/value to the clicked/selected cells' text and record value
    public void AddDigitToSelected(int buttonNumber)
    {
        for (int i = 0; i < 81; i++)
        {
            // Add the given digit if cell is selected
            if (cellsClicked[i])
            {
                // If input same digit as current (including if incorrect), clear cell
                if (buttonNumber == cellDigits[i] || buttonNumber.ToString() == sudokuCells[i].incorrectText)
                    ClearCell(i);
                // Else either add digit as valid or incorrect
                else
                {
                    // Check whether valid then add digit
                    SudokuRules sudokuRules = new SudokuRules(cellDigits);
                    bool isValid = sudokuRules.IsValidMove(i, buttonNumber);
                    sudokuCells[i].SetMainText($"{buttonNumber}", isValid, false);

                    // Store digit in corresponding (valid/incorrect) array record
                    if (isValid)
                        cellDigits[i] = buttonNumber;
                    else
                        incorrectDigits[i] = buttonNumber;
                }
            }
        }

        UpdateCells();
    }

    // Deselect all cells
    public void DeselectAll()
    {
        for (int i = 0; i < 81; i++)
            sudokuCells[i].Deselect();
    }

    // Clear all clicked/selected cells' text and recorded value then deselect 
    public void ClearSelected()
    {
        for (int i = 0; i < 81; i++)
        {
            // Reset all selected cells, as well as their array records
            if (cellsClicked[i])
            {
                sudokuCells[i].SetMainText("", true, false); // Clear text (technically a valid add)
                cellDigits[i] = 0;
                sudokuCells[i].Deselect();
                cellsClicked[i] = false;
            }
        }

        // Update cells after clearing
        UpdateCells();
    }

    // Clear individual cell at given index text and recorded value, doesn't deselect
    private void ClearCell(int index)
    {
        sudokuCells[index].SetMainText("", true, false); // Clear text (technically a valid add)
        cellDigits[index] = 0;
        incorrectDigits[index] = 0;

        // Update cells after clearing
        UpdateCells();
    }

    // Updates/Clears each cell's notes, incorrect digits, and hidden subsets based on current digits in cells
    private void UpdateCells()
    {
        // Get all cell notes
        sudokuRules = new SudokuRules(cellDigits);

        for (int i = 0; i < 81; i++)
        {
            // If cell at index i does not have an inputted value, set the note text
            if (cellDigits[i] == 0)
                sudokuCells[i].SetNoteText(sudokuRules.notesGrid[i]);
            // Else clear notes
            else
            {
                sudokuCells[i].ClearNoteText();
                sudokuRules.ClearCellNotes(i, cellDigits[i]);
            }

            // Reset formerly incorrect digits if now valid
            if (incorrectDigits[i] != 0)
            {
                if (sudokuRules.IsValidMove(i, incorrectDigits[i]))
                {
                    sudokuCells[i].SetMainText($"{incorrectDigits[i]}", true, false);
                    cellDigits[i] = incorrectDigits[i];
                }
            }
        }

        // Set/Reset hidden subsets
        SetHSubs(true);
    }

    // Calculate and set hidden subsets if true, clear hidden subsets if false
    private void SetHSubs(bool SetOrClear)
    {
        // If setting and showing hsubs, calculate and set text
        if (SetOrClear && isShowingHSubs)
        {
            hSubRules = new HSubRules(this, sudokuRules.notesGrid);
            ResetHSubs(hSubRules);
        }
        // Else if clearing and/or not showing hidden subsets, clear all hsub text
        else
            for (int i = 0; i < 81; i++)
                sudokuCells[i].SetHSubs(null, false);
    }

    // Reset hidden subsets based on provided hidden subset rule calculations
    public void ResetHSubs(HSubRules currentHSubRules)
    {
        for (int i = 0; i < 81; i++)
        {
            // If no digit in cell, set or clear hsubs
            if (cellDigits[i] == 0)
            {
                // If no more than 4 hsub values, set hsubs and record that cell contains hsubs
                if (currentHSubRules.hsubGrid[i].Count <= 4)
                {
                    sudokuCells[i].SetHSubs(currentHSubRules.hsubGrid[i], true);
                    sudokuCells[i].hasHsubValues = true;
                }
                // Else clear hsubs and record that cell contains no hsubs
                else
                {
                    sudokuCells[i].SetHSubs(null, true);
                    sudokuCells[i].hasHsubValues = false;
                }
            }
            // Else record that cell contains no hsubs
            else
                sudokuCells[i].hasHsubValues = false;
        }
    }

    // Toggle whether you can see notes and recalculate cells
    public void ToggleSeeNotes()
    {
        SudokuCell.ToggleSeeNotes();
        UpdateCells();
    }

    // Toggle whether you can select in multiple clicks without resetting
    public void ToggleMultiSelect()
    {
        SudokuCell.ToggleMultiSelect();
    }

    // Toggle whether you can see hidden subsets and then reset them
    public void ToggleHiddenSubsets(bool isOn)
    {
        isShowingHSubs = isOn;
        SetHSubs(isOn);
    }
}
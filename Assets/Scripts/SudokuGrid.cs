using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    public GameObject[] prefabs = new GameObject[2];
    private Vector3Int gridCenter = new Vector3Int(0, 0, 0);
    private SudokuCell[] cells;

    // Start is called before the first frame update
    void Start()
    {
        cells = GetCells();
        DrawGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private SudokuCell[] GetCells()
    {
        SudokuCell[] cells = new SudokuCell[81];
        Renderer renderer = prefabs[0].GetComponent<Renderer>();
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

                // Store empty cell
                cells[index++] = new SudokuCell(position, PrefabIndex.empty);
            }
        }

        return cells;
    }

    private void DrawGrid()
    {
        foreach (SudokuCell cell in cells)
        {
            GameObject cellPrefab = Instantiate(prefabs[(int)cell.prefabIndex], transform);
            cellPrefab.transform.localPosition = cell.position;
        }
    }
}

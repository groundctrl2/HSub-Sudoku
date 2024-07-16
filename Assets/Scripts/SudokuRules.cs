using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class SudokuRules
{
    public int[][] grid;
    public bool[][] notesGrid;

    // Constructor to initialize with the current 1D grid state
    public SudokuRules(int[] currentGrid)
    {
        grid = ConvertTo2D(currentGrid);
        notesGrid = GetNotes();
    }

    // Get a 2D grid (size 9x9) from given 1D grid (size 81)
    public int[][] ConvertTo2D(int[] oneDGrid)
    {
        int[][] twoDGrid = new int[9][];
        for (int i = 0; i < 9; i++)
        {
            twoDGrid[i] = new int[9];
            for (int j = 0; j < 9; j++)
            {
                // Calculate the 1D array index and assign to the 2D array
                twoDGrid[i][j] = oneDGrid[i * 9 + j];
            }
        }
        return twoDGrid;
    }

    // Get a 1D grid (size 81) from given 1D grid (size 9x9)
    public int[] ConvertTo1D(int[][] twoDGrid)
    {
        int[] oneDGrid = new int[81];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                // Calculate the 1D array index and assign from the 2D array
                oneDGrid[i * 9 + j] = twoDGrid[i][j];
            }
        }
        return oneDGrid;
    }

    // 2D input method to check if placing a digit is valid according to Sudoku rules
    public bool IsValidMove(int rowIndex, int colIndex, int digit)
    {
        return IsRowValid(rowIndex, digit) && IsColValid(colIndex, digit) && IsSubgridValid(rowIndex, colIndex, digit);
    }

    // 1D input method to check if placing a digit is valid according to Sudoku rules
    public bool IsValidMove(int index, int digit)
    {
        int rowIndex = index / 9;
        int colIndex = index % 9;

        return IsRowValid(rowIndex, digit) && IsColValid(colIndex, digit) && IsSubgridValid(rowIndex, colIndex, digit);
    }

    // Helper method to check if the digit is valid in the given row
    private bool IsRowValid(int rowIndex, int digit)
    {
        return !grid[rowIndex].Any(cell => cell == digit);
    }

    // Helper method to check if the digit is valid in the given column
    private bool IsColValid(int colIndex, int digit)
    {
        return !grid.Any(row => row[colIndex] == digit);
    }

    // Helper method to check if the digit is valid in the 3x3 subgrid
    private bool IsSubgridValid(int rowIndex, int colIndex, int digit)
    {
        // Determine the starting indices of the 3x3 subgrid
        int startRow = (rowIndex / 3) * 3; // integer division
        int startCol = (colIndex / 3) * 3; // integer division

        // Iterate over each cell in the 3x3 subgrid
        for (int i = startRow; i < startRow + 3; i++)
            for (int j = startCol; j < startCol + 3; j++)
                if (grid[i][j] == digit)
                    return false; // Digit is already present in the subgrid
        return true; // Digit is not present in the subgrid
    }

    // Get a (1D) bool[81][9] for each cell containing true/false if number (of corresponding index) is a valid move
    public bool[][] GetNotes()
    {
        bool[][] newNotesGrid = new bool[81][];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int index = i * 9 + j; // Calculate the index for the 1D representation
                newNotesGrid[index] = new bool[9];
                for (int num = 1; num <= 9; num++)
                {
                    newNotesGrid[index][num - 1] = IsValidMove(i, j, num) ? true : false;
                }
            }
        }
        return newNotesGrid;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class SudokuRules
{
    private int[][] grid;
    public bool[][] notesGrid;

    // Constructor to initialize with the current 1D grid state
    public SudokuRules(int[] currentGrid)
    {
        grid = ConvertTo2D(currentGrid);
        notesGrid = GetNotes();
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

    public void ClearCellNotes(int index, int digit)
    {
        for (int i = 0; i < 9; i++)
            notesGrid[index][i] = i + 1 == digit ? true : false;
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

    // Helper method to get a 2D grid (size 9x9) from given 1D grid (size 81)
    public static int[][] ConvertTo2D(int[] oneDGrid)
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

    // Helper method to get a 1D grid (size 81) from given 1D grid (size 9x9)
    public static int[] ConvertTo1D(int[][] twoDGrid)
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

    // Gets a given row from the array of 81 cell lists 'hsubGrid'
    public static int[] GetRow(int rowIndex)
    {
        int[] row = new int[9];
        for (int i = 0; i < 9; i++)
            row[i] = Get1DIndex(rowIndex, i);

        return row;
    }

    // Gets a given col from the array of 81 cell lists 'hsubGrid'
    public static int[] GetCol(int colIndex)
    {
        int[] col = new int[9];
        for (int i = 0; i < 9; i++)
            col[i] = Get1DIndex(i, colIndex);

        return col;
    }

    // Gets a given subgrid from the array of 81 cell lists 'hsubGrid' (subgridNumber's range 1-9 top-left to bottom-right)
    public static int[] GetSubGrid(int subgridNumber)
    {
        int[] subGrid = new int[9];
        int index = 0;

        // Calculate the starting row and column indices based on the box number
        int subGridRow = (subgridNumber - 1) / 3;
        int subGridCol = (subgridNumber - 1) % 3;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int row = subGridRow * 3 + i;
                int col = subGridCol * 3 + j;
                subGrid[index++] = Get1DIndex(row, col);
            }
        }

        return subGrid;
    }

    // Method to convert 2D array index to 1D array index
    public static int Get1DIndex(int rowIndex, int colIndex)
    {
        return rowIndex * 9 + colIndex;
    }

    // Method to convert 1D array index to 2D array/tuple index
    public static (int rowIndex, int colIndex) Get2DIndex(int index)
    {
        int rowIndex = index / 9;
        int colIndex = index % 9;
        return (rowIndex, colIndex);
    }
}

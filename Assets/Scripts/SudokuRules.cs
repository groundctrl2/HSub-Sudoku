using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class SudokuRules
{
    private int[] grid;
    public bool[][] notesGrid;

    // Constructor to initialize with the current 1D grid state
    public SudokuRules(int[] currentGrid)
    {
        grid = currentGrid;
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

    // Clear all notes in given cell except for the added digit
    public void ClearCellNotes(int index, int digit)
    {
        for (int i = 0; i < 9; i++)
            notesGrid[index][i] = i + 1 == digit ? true : false;
    }

    // 2D input method to check if placing a digit is valid according to Sudoku rules
    public bool IsValidMove(int rowIndex, int colIndex, int digit)
    {
        return IsRowValid(rowIndex, digit) && IsColValid(colIndex, digit) && IsSubGridValid(rowIndex, colIndex, digit);
    }

    // 1D input method to check if placing a digit is valid according to Sudoku rules
    public bool IsValidMove(int index, int digit)
    {
        int rowIndex = index / 9;
        int colIndex = index % 9;

        return IsRowValid(rowIndex, digit) && IsColValid(colIndex, digit) && IsSubGridValid(rowIndex, colIndex, digit);
    }

    // Helper method to check if the digit is valid in the given row
    private bool IsRowValid(int rowIndex, int digit)
    {
        int[] rowIndices = GetRow(rowIndex);

        foreach (int index in rowIndices)
            if (grid[index] == digit)
                return false;
        return true;
    }

    // Helper method to check if the digit is valid in the given column
    private bool IsColValid(int colIndex, int digit)
    {
        int[] colIndices = GetCol(colIndex);

        foreach (int index in colIndices)
            if (grid[index] == digit)
                return false;
        return true;
    }

    // Helper method to check if the digit is valid in the 3x3 subgrid
    private bool IsSubGridValid(int rowIndex, int colIndex, int digit)
    {
        int[] subGridIndices = GetSubGrid(rowIndex, colIndex);

        foreach (int index in subGridIndices)
            if (grid[index] == digit)
                return false;
        return true;
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
    public static int[] GetSubGrid(int subGridNumber)
    {
        int[] subGrid = new int[9];
        int index = 0;

        // Calculate the starting row and column indices based on the box number
        int subGridRow = (subGridNumber - 1) / 3;
        int subGridCol = (subGridNumber - 1) % 3;

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

    // Gets a given subgrid from the array of 81 cell lists 'hsubGrid' using rowIndex and colIndex
    public static int[] GetSubGrid(int rowIndex, int colIndex)
    {
        int[] subGrid = new int[9];
        int index = 0;

        // Calculate the starting row and column indices based on the given rowIndex and colIndex
        int subGridRow = (rowIndex / 3) * 3;
        int subGridCol = (colIndex / 3) * 3;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int row = subGridRow + i;
                int col = subGridCol + j;
                subGrid[index++] = Get1DIndex(row, col);
            }
        }

        return subGrid;
    }

    // Helper method to convert 2D array index to 1D array index
    public static int Get1DIndex(int rowIndex, int colIndex)
    {
        return rowIndex * 9 + colIndex;
    }

    // Helper method to convert 1D array index to 2D array/tuple index
    public static (int rowIndex, int colIndex) Get2DIndex(int index)
    {
        int rowIndex = index / 9;
        int colIndex = index % 9;
        return (rowIndex, colIndex);
    }
}

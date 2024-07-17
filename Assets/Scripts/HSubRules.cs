using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class HSubRules
{
    private bool[][] notesGrid; // 1D, inner array contains bool if specific digit (zero-indexed) is in it
    public List<int>[] hsubGrid = new List<int>[81]; // array of 81 cell note lists

    // Constructor to initialize with the current 1D notes grid state
    public HSubRules(bool[][] currentNotes)
    {
        // Initialize hsub grid (made of Lists)
        for (int i = 0; i < currentNotes.Length; i++)
        {
            hsubGrid[i] = new List<int>();
            for (int j = 0; j < currentNotes[i].Length; j++)
                if (notesGrid[i][j]) hsubGrid[i].Add(j + 1);
        }
    }

    // Returns list of tuples representing the single cell hidden subsets found
    private List<(int index, int digit)> GetSingleHSubs()
    {
        var singles = new List<(int index, int digit)>();

        //TODO

        return singles;
    }

    // Returns list of tuples representing the double cell hidden subsets found
    private List<(int index1, int index2, int digit1, int digit2)> GetDoubleHSubs()
    {
        var doubles = new List<(int index1, int index2, int digit1, int digit2)>();

        // TODO

        return doubles;
    }

    // Returns list of tuples representing the triple cell hidden subsets found
    private List<(int index1, int index2, int index3, int digit1, int digit2, int digit3)> GetTripleHSubs()
    {
        var triples = new List<(int index1, int index2, int index3, int digit1, int digit2, int digit3)>();

        // TODO

        return triples;
    }

    // Returns list of tuples representing the quadruple cell hidden subsets found
    private List<(int index1, int index2, int index3, int index4, int digit1, int digit2, int digit3, int digit4)> GetQuadrupleHSubs()
    {
        var quadruples = new List<(int index1, int index2, int index3, int index4, int digit1, int digit2, int digit3, int digit4)>();

        // TODO

        return quadruples;
    }

    // Takes a note group (row/col/subgrid) and returns a zero-indexed array containing how many times each digit occurs
    private int[] CountDigitOccurrences(List<int>[] noteGroup)
    {
        int[] digitOccurrences = new int[9];
        for (int i = 0; i < 9; i++)
        {
            digitOccurrences[i] = 0;
            foreach (int digit in noteGroup[i])
                digitOccurrences[digit - 1]++;
        }

        return digitOccurrences;
    }

    // Gets a given row from the array of 81 cell lists 'hsubGrid'
    private List<int>[] GetRow(int rowIndex)
    {
        List<int>[] row = new List<int>[9];
        for (int i = 0; i < 9; i++)
            row[i] = hsubGrid[Get1DIndex(rowIndex, i)];

        return row;
    }

    // Gets a given col from the array of 81 cell lists 'hsubGrid'
    private List<int>[] GetCol(int colIndex)
    {
        List<int>[] col = new List<int>[9];
        for (int i = 0; i < 9; i++)
            col[i] = hsubGrid[Get1DIndex(i, colIndex)];

        return col;
    }

    // Gets a given subgrid from the array of 81 cell lists 'hsubGrid' (subgridNumber's range 1-9 top-left to bottom-right)
    private List<int>[] GetSubGrid(int subgridNumber)
    {
        List<int>[] subGrid = new List<int>[9];
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
                subGrid[index++] = hsubGrid[Get1DIndex(row, col)];
            }
        }

        return subGrid;
    }

    // Method to convert 2D array index to 1D array index
    private int Get1DIndex(int rowIndex, int colIndex)
    {
        return rowIndex * 9 + colIndex;
    }

    // Method to convert 1D array index to 2D array/tuple index
    private (int rowIndex, int colIndex) Get2DIndex(int index)
    {
        int rowIndex = index / 9;
        int colIndex = index % 9;
        return (rowIndex, colIndex);
    }
}

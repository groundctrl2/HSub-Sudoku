using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class HSubRules
{
    public List<int>[] hsubGrid = new List<int>[81]; // array of 81 cell note lists

    // Constructor to initialize with the current 1D notes grid state
    public HSubRules(bool[][] currentNotes)
    {
        // Initialize hsub grid (made of Lists)
        for (int i = 0; i < currentNotes.Length; i++)
        {
            hsubGrid[i] = new List<int>();
            for (int j = 0; j < currentNotes[i].Length; j++)
                if (currentNotes[i][j]) hsubGrid[i].Add(j + 1);
        }
    }

    // Takes a note group (row/col/subgrid) and returns a zero-indexed array containing how many times each digit occurs
    private int[] CountDigitOccurrences(List<int>[] noteGroup)
    {
        int[] digitOccurrences = new int[9];
        for (int i = 0; i < 9; i++)
            digitOccurrences[i] = 0;

        // Count occurrences
        for (int i = 0; i < 9; i++)
            foreach (int digit in noteGroup[i])
                digitOccurrences[digit - 1]++;

        return digitOccurrences;
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
}

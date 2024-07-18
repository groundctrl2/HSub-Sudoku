using System;
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

    // Helper method. Given an array of indices, pulls the corresponding cell lists from the Hidden Subset Grid into its own list
    private List<int>[] GetNoteGroup(int[] indices)
    {
        var noteGroup = new List<int>[indices.Length];
        for (int i = 0; i < indices.Length; i++)
            noteGroup[i] = hsubGrid[indices[i]];

        return noteGroup;
    }

    // Returns an inverted version of given note group. As in each digit 1-9 has a list, and within each list is the indices that digit is found
    private List<int>[] InvertNoteGroup(List<int>[] noteGroup)
    {
        var invertedNoteGroup = new List<int>[9];
        for (int i = 0; i < 9; i++)
            invertedNoteGroup[i] = new List<int>();

        for (int index = 0; index < 9; index++)
            foreach (int note in noteGroup[index])
                invertedNoteGroup[note - 1].Add(index + 1); // + 1 to reflect position instead of index

        return invertedNoteGroup;
    }

    // Takes a note group (row/col/subgrid) and returns a zero-indexed array containing how many times each digit occurs
    private int[] CountDigitOccurrences(List<int>[] noteGroup)
    {
        int[] digitOccurrences = new int[9];
        for (int i = 0; i < 9; i++)
            foreach (int digit in noteGroup[i])
                if (digit >= 1 && digit <= 9)
                    digitOccurrences[digit - 1]++;

        return digitOccurrences;
    }

    // Helper method. Get the index/indices of a given digit in a given note group (row/col/subgrid) based on the amount of occurrences to find
    private int[] FindOccurrenceIndices(List<int>[] noteGroup, int digit, int occurrenceCount)
    {
        int[] occurrenceIndices = new int[occurrenceCount];

        int found = 0; // Amount of digits found and index for found occurrences
        int index = 0; // Current index
        while (found < occurrenceCount)
        {
            if (noteGroup[index].Contains(digit))
            {
                occurrenceIndices[found] = index;
                found++;
            }
            index++;
        }

        return occurrenceIndices;
    }

    // Returns list of tuples representing the single cell hidden subsets found
    private List<(int index, int digit)> GetSingleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverted)
    {
        var singles = new List<(int index, int digit)>();

        for (int i = 0; i < 9; i++)
        {
            if (occurrences[i] == 1)
            {
                int digit = i + 1;
                int occurrenceIndex = FindOccurrenceIndices(noteGroup, digit, 1)[0];

                if (isInverted)
                    singles.Add((digit, occurrenceIndex + 1)); // + 1 to reflect position instead of index
                else
                    singles.Add((occurrenceIndex + 1, digit)); // + 1 to reflect position instead of index
            }
        }

        return singles;
    }

    // Returns list of tuples representing the double cell hidden subsets found
    private List<(int index1, int index2, int digit1, int digit2)> GetDoubleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverted)
    {
        var doubles = new List<(int index1, int index2, int digit1, int digit2)>();

        // TODO

        return doubles;
    }

    // Returns list of tuples representing the triple cell hidden subsets found
    private List<(int index1, int index2, int index3, int digit1, int digit2, int digit3)> GetTripleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverted)
    {
        var triples = new List<(int index1, int index2, int index3, int digit1, int digit2, int digit3)>();

        // TODO

        return triples;
    }

    // Returns list of tuples representing the quadruple cell hidden subsets found
    private List<(int index1, int index2, int index3, int index4, int digit1, int digit2, int digit3, int digit4)> GetQuadrupleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverteds)
    {
        var quadruples = new List<(int index1, int index2, int index3, int index4, int digit1, int digit2, int digit3, int digit4)>();

        // TODO

        return quadruples;
    }
}

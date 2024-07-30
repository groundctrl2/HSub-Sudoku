using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using UnityEngine;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;

public class HSubRules
{
    public List<int>[] hsubGrid = new List<int>[81];

    // Found hidden subsets
    private List<(int index, int value)> singles;
    private List<(int index1, int index2, int value1, int value2)> doubles;
    private List<(int index1, int index2, int index3, int value1, int value2, int value3)> triples;
    private List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)> quadruples;

    // Constructor to initialize with the current 1D notes grid state
    public HSubRules(bool[][] currentNotes)
    {
        // Initialize hsub grid
        for (int i = 0; i < currentNotes.Length; i++)
        {
            hsubGrid[i] = new List<int>();
            for (int j = 0; j < currentNotes[i].Length; j++)
                if (currentNotes[i][j]) hsubGrid[i].Add(j + 1);
        }
    }

    // Given an array of indices, pulls the corresponding cell lists from the Hidden Subset Grid into its own list
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

    // Takes a note group (row/col/subgrid) and returns a zero-indexed array containing how many times each value occurs
    private static int[] CountDigitOccurrences(List<int>[] noteGroup)
    {
        int[] valueOccurrences = new int[9];
        for (int i = 0; i < 9; i++)
            foreach (int digit in noteGroup[i])
                if (digit >= 1 && digit <= 9)
                    valueOccurrences[digit - 1]++;

        return valueOccurrences;
    }

    // Get the index/indices of a given value in a given note group (row/col/subgrid) based on the amount of occurrences to find
    private static int[] FindOccurrenceIndices(List<int>[] noteGroup, int value, int occurrenceCount)
    {
        int[] occurrenceIndices = new int[occurrenceCount];

        int found = 0; // Amount of values found and index for found occurrences
        int index = 0; // Current index
        while (found < occurrenceCount)
        {
            if (noteGroup[index].Contains(value))
            {
                occurrenceIndices[found] = index;
                found++;
            }
            index++;
        }

        return occurrenceIndices;
    }

    // Get the occurrence indices of each value in a note group
    public static int[][] GetValueOccurrences(List<int>[] noteGroup)
    {
        var valueOccurrences = new int[9][]; // Jagged to allow arrays of different sizes
        var occurrenceCounts = CountDigitOccurrences(noteGroup);

        for (int i = 0; i < 9; i++)
            valueOccurrences[i] = FindOccurrenceIndices(noteGroup, i + 1, occurrenceCounts[i]);

        return valueOccurrences;
    }

    // Get hashset containing all unique values from given lists
    public static HashSet<int> GetUniqueValues(List<int> list1, List<int> list2)
    {
        var hashSet = new HashSet<int>();

        foreach (int value in list1)
            hashSet.Add(value);
        foreach (int value in list2)
            hashSet.Add(value);

        return hashSet;
    }
}

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

    // Takes a note group (row/col/subgrid) and returns a zero-indexed array containing how many times each value occurs
    private int[] CountDigitOccurrences(List<int>[] noteGroup)
    {
        int[] valueOccurrences = new int[9];
        for (int i = 0; i < 9; i++)
            foreach (int digit in noteGroup[i])
                if (digit >= 1 && digit <= 9)
                    valueOccurrences[digit - 1]++;

        return valueOccurrences;
    }

    // Helper method. Get the index/indices of a given value in a given note group (row/col/subgrid) based on the amount of occurrences to find
    private int[] FindOccurrenceIndices(List<int>[] noteGroup, int value, int occurrenceCount)
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

    // Returns list of tuples representing the single cell hidden subsets found
    private List<(int index, int value)> GetSingleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverted)
    {
        var singles = new List<(int index, int value)>();

        for (int i = 0; i < 9; i++)
        {
            if (occurrences[i] == 1)
            {
                int value = i + 1; // + 1 to reflect position instead of index
                int occurrence = FindOccurrenceIndices(noteGroup, value, 1)[0] + 1; // + 1 to reflect position instead of index

                if (isInverted)
                    singles.Add((value, occurrence));
                else
                    singles.Add((occurrence, value));
            }
        }

        return singles;
    }

    // Returns list of tuples representing the double cell hidden subsets found
    private List<(int index1, int index2, int value1, int value2)> GetDoubleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverted)
    {
        var doubles = new List<(int index1, int index2, int value1, int value2)>();
        var twoOccurrences = new List<(int value, int occurrence1, int occurrence2)>();

        // Get elements that occur twice and store their list indices
        for (int i = 0; i < 9; i++)
        {
            if (occurrences[i] == 2)
            {
                int value = i + 1; // + 1 to reflect position instead of index
                var valueOccurrences = FindOccurrenceIndices(noteGroup, value, 2);
                int occurrence1 = valueOccurrences[0] + 1; // + 1 to reflect position instead of index
                int occurrence2 = valueOccurrences[1] + 1;
                twoOccurrences.Add((value, occurrence1, occurrence2));
            }
        }

        // Find values with matching indices
        var indexDictionary = new Dictionary<(int occurrence1, int occurrence2), int>();
        foreach (var tuple in twoOccurrences)
        {
            var indices = (tuple.occurrence1, tuple.occurrence2);
            if (indexDictionary.ContainsKey(indices))
            {
                // Duplicate found, add a tuple with both elements and the indices to the list
                if (isInverted)
                    doubles.Add((indexDictionary[indices], tuple.value, tuple.occurrence1, tuple.occurrence2));
                else
                    doubles.Add((tuple.occurrence1, tuple.occurrence2, indexDictionary[indices], tuple.value));
            }
            else
                // Add the indices and value to the dictionary
                indexDictionary[indices] = tuple.value;
        }

        return doubles;
    }

    // Returns list of tuples representing the triple cell hidden subsets found
    private List<(int index1, int index2, int index3, int value1, int value2, int value3)> GetTripleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverted)
    {
        var triples = new List<(int index1, int index2, int index3, int value1, int value2, int value3)>();
        var threeOccurrences = new List<(int value, int occurrence1, int occurrence2, int occurrence3)>();

        // Get elements that occur three times and store their list indices
        for (int i = 0; i < 9; i++)
        {
            if (occurrences[i] == 3)
            {
                int value = i + 1; // + 1 to reflect position instead of index
                var valueOccurrences = FindOccurrenceIndices(noteGroup, value, 3);
                int occurrence1 = valueOccurrences[0] + 1; // + 1 to reflect position instead of index
                int occurrence2 = valueOccurrences[1] + 1;
                int occurrence3 = valueOccurrences[2] + 1;
                threeOccurrences.Add((value, occurrence1, occurrence2, occurrence3));
            }
        }

        // Find values with matching indices
        var indexDictionary = new Dictionary<(int occurrence1, int occurrence2, int occurrence3), List<int>>();
        foreach (var tuple in threeOccurrences)
        {
            var indices = (tuple.occurrence1, tuple.occurrence2, tuple.occurrence3);
            if (indexDictionary.ContainsKey(indices))
            {
                indexDictionary[indices].Add(tuple.value);
                if (indexDictionary[indices].Count == 3)
                {
                    // Triple found, add a tuple with all three elements and the indices to the list
                    var values = indexDictionary[indices];
                    if (isInverted)
                        triples.Add((values[0], values[1], values[2], indices.occurrence1, indices.occurrence2, indices.occurrence3));
                    else
                        triples.Add((indices.occurrence1, indices.occurrence2, indices.occurrence3, values[0], values[1], values[2]));
                }
            }
            else
            {
                // Add the indices and value to the dictionary
                indexDictionary[indices] = new List<int> { tuple.value };
            }
        }

        return triples;
    }

    // Returns list of tuples representing the quadruple cell hidden subsets found
    private List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)> GetQuadrupleHSubs(List<int>[] noteGroup, int[] occurrences, bool isInverted)
    {
        var quadruples = new List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)>();
        var fourOccurrences = new List<(int value, int occurrence1, int occurrence2, int occurrence3, int occurrence4)>();

        // Get elements that occur four times and store their list indices
        for (int i = 0; i < 9; i++)
        {
            if (occurrences[i] == 4)
            {
                int value = i + 1; // + 1 to reflect position instead of index
                var valueOccurrences = FindOccurrenceIndices(noteGroup, value, 4);
                int occurrence1 = valueOccurrences[0] + 1; // + 1 to reflect position instead of index
                int occurrence2 = valueOccurrences[1] + 1;
                int occurrence3 = valueOccurrences[2] + 1;
                int occurrence4 = valueOccurrences[3] + 1;
                fourOccurrences.Add((value, occurrence1, occurrence2, occurrence3, occurrence4));
            }
        }

        // Find values with matching indices
        var indexDictionary = new Dictionary<(int occurrence1, int occurrence2, int occurrence3, int occurrence4), List<int>>();
        foreach (var tuple in fourOccurrences)
        {
            var indices = (tuple.occurrence1, tuple.occurrence2, tuple.occurrence3, tuple.occurrence4);
            if (indexDictionary.ContainsKey(indices))
            {
                indexDictionary[indices].Add(tuple.value);
                if (indexDictionary[indices].Count == 4)
                {
                    // Quadruple found, add a tuple with all four elements and the indices to the list
                    var values = indexDictionary[indices];
                    if (isInverted)
                        quadruples.Add((values[0], values[1], values[2], values[3], indices.occurrence1, indices.occurrence2, indices.occurrence3, indices.occurrence4));
                    else
                        quadruples.Add((indices.occurrence1, indices.occurrence2, indices.occurrence3, indices.occurrence4, values[0], values[1], values[2], values[3]));
                }
            }
            else
            {
                // Add the indices and value to the dictionary
                indexDictionary[indices] = new List<int> { tuple.value };
            }
        }

        return quadruples;
    }
}

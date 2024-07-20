using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using UnityEngine;

public class HSubRules
{
    public List<int>[] hsubGrid = new List<int>[81]; // Used for computation
    private List<(int index, int value)> singles;
    private List<(int index1, int index2, int value1, int value2)> doubles;
    private List<(int index1, int index2, int index3, int value1, int value2, int value3)> triples;
    private List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)> quadruples;

    // Constructor to initialize with the current 1D notes grid state
    public HSubRules(bool[][] currentNotes)
    {
        // Initialize both hsub grids (both made of Lists)
        for (int i = 0; i < currentNotes.Length; i++)
        {
            hsubGrid[i] = new List<int>();
            for (int j = 0; j < currentNotes[i].Length; j++)
                if (currentNotes[i][j]) hsubGrid[i].Add(j + 1);
        }

        // Find HSubs
        var allSinglesFound = new List<(int index, int value)>();
        var allDoublesFound = new List<(int index1, int index2, int value1, int value2)>();
        var allTriplesFound = new List<(int index1, int index2, int index3, int value1, int value2, int value3)>();
        var allQuadruplesFound = new List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)>();

        // Search each row
        for (int row = 0; row < 9; row++)
        {
            int[] rowIndices = SudokuRules.GetRow(row);
            var noteGroup = GetNoteGroup(rowIndices);
            var invertedNoteGroup = InvertNoteGroup(noteGroup);
            int[] occurrences = CountDigitOccurrences(noteGroup);
            int[] invertedOccurrences = CountDigitOccurrences(invertedNoteGroup);

            // Get generic index singles (normal & inverted)
            var genericIndexSingles = GetSingleHSubs(noteGroup, occurrences, false);
            var genericIndexInvertedSingles = GetSingleHSubs(invertedNoteGroup, invertedOccurrences, true);

            // Convert indices back to original index and add to 'all list'
            foreach ((int index, int value) tuple in genericIndexSingles)
                allSinglesFound.Add((rowIndices[tuple.index - 1], tuple.value));
            foreach ((int index, int value) tuple in genericIndexInvertedSingles)
                allSinglesFound.Add((rowIndices[tuple.index - 1], tuple.value));

            GetHSubs(rowIndices, noteGroup, invertedNoteGroup, occurrences, invertedOccurrences, ref allDoublesFound, ref allTriplesFound, ref allQuadruplesFound);
        }

        // Search each column
        for (int col = 0; col < 9; col++)
        {
            int[] colIndices = SudokuRules.GetCol(col);
            var noteGroup = GetNoteGroup(colIndices);
            var invertedNoteGroup = InvertNoteGroup(noteGroup);
            int[] occurrences = CountDigitOccurrences(noteGroup);
            int[] invertedOccurrences = CountDigitOccurrences(invertedNoteGroup);

            GetHSubs(colIndices, noteGroup, invertedNoteGroup, occurrences, invertedOccurrences, ref allDoublesFound, ref allTriplesFound, ref allQuadruplesFound);
        }

        // Search each subgrid
        for (int subgrid = 1; subgrid < 10; subgrid++)
        {
            int[] subgridIndices = SudokuRules.GetSubGrid(subgrid);
            var noteGroup = GetNoteGroup(subgridIndices);
            var invertedNoteGroup = InvertNoteGroup(noteGroup);
            int[] occurrences = CountDigitOccurrences(noteGroup);
            int[] invertedOccurrences = CountDigitOccurrences(invertedNoteGroup);

            GetHSubs(subgridIndices, noteGroup, invertedNoteGroup, occurrences, invertedOccurrences, ref allDoublesFound, ref allTriplesFound, ref allQuadruplesFound);
        }

        // Store all unique hidden subsets found
        singles = allSinglesFound.Distinct().ToList();
        doubles = allDoublesFound.Distinct().ToList();
        triples = allTriplesFound.Distinct().ToList();
        quadruples = allQuadruplesFound.Distinct().ToList();
        UpdateHSubGrid();
    }

    // Update the hsub grid with the found hidden subsets
    public void UpdateHSubGrid()
    {
        bool[] smallestHSubAdded = new bool[81];

        // Clear the Hidden Subset Grid (computation already complete, ready for display)
        foreach (var list in hsubGrid)
            list.Clear();

        // Add singles
        foreach ((int index, int value) tuple in singles)
        {
            if (!smallestHSubAdded[tuple.index])
            {
                hsubGrid[tuple.index].Add(tuple.value);

                smallestHSubAdded[tuple.index] = true;
            }
        }

        // Add doubles
        foreach ((int index1, int index2, int value1, int value2) tuple in doubles)
        {
            if (!smallestHSubAdded[tuple.index1])
            {
                hsubGrid[tuple.index1].Add(tuple.value1);
                hsubGrid[tuple.index1].Add(tuple.value2);

                smallestHSubAdded[tuple.index1] = true;
            }
            if (!smallestHSubAdded[tuple.index2])
            {
                hsubGrid[tuple.index2].Add(tuple.value1);
                hsubGrid[tuple.index2].Add(tuple.value2);

                smallestHSubAdded[tuple.index2] = true;
            }
        }

        // Add triples
        foreach ((int index1, int index2, int index3, int value1, int value2, int value3) tuple in triples)
        {
            if (!smallestHSubAdded[tuple.index1])
            {
                hsubGrid[tuple.index1].Add(tuple.value1);
                hsubGrid[tuple.index1].Add(tuple.value2);
                hsubGrid[tuple.index1].Add(tuple.value3);

                smallestHSubAdded[tuple.index1] = true;
            }
            if (!smallestHSubAdded[tuple.index2])
            {
                hsubGrid[tuple.index2].Add(tuple.value1);
                hsubGrid[tuple.index2].Add(tuple.value2);
                hsubGrid[tuple.index2].Add(tuple.value3);

                smallestHSubAdded[tuple.index2] = true;
            }
            if (!smallestHSubAdded[tuple.index3])
            {
                hsubGrid[tuple.index3].Add(tuple.value1);
                hsubGrid[tuple.index3].Add(tuple.value2);
                hsubGrid[tuple.index3].Add(tuple.value3);

                smallestHSubAdded[tuple.index3] = true;
            }
        }

        // Add quadruples
        foreach ((int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4) tuple in quadruples)
        {
            if (!smallestHSubAdded[tuple.index1])
            {
                hsubGrid[tuple.index1].Add(tuple.value1);
                hsubGrid[tuple.index1].Add(tuple.value2);
                hsubGrid[tuple.index1].Add(tuple.value3);
                hsubGrid[tuple.index1].Add(tuple.value4);

                smallestHSubAdded[tuple.index1] = true;
            }
            if (!smallestHSubAdded[tuple.index2])
            {
                hsubGrid[tuple.index2].Add(tuple.value1);
                hsubGrid[tuple.index2].Add(tuple.value2);
                hsubGrid[tuple.index2].Add(tuple.value3);
                hsubGrid[tuple.index2].Add(tuple.value4);

                smallestHSubAdded[tuple.index2] = true;
            }
            if (!smallestHSubAdded[tuple.index3])
            {
                hsubGrid[tuple.index3].Add(tuple.value1);
                hsubGrid[tuple.index3].Add(tuple.value2);
                hsubGrid[tuple.index3].Add(tuple.value3);
                hsubGrid[tuple.index3].Add(tuple.value4);

                smallestHSubAdded[tuple.index3] = true;
            }
            if (!smallestHSubAdded[tuple.index4])
            {
                hsubGrid[tuple.index4].Add(tuple.value1);
                hsubGrid[tuple.index4].Add(tuple.value2);
                hsubGrid[tuple.index4].Add(tuple.value3);
                hsubGrid[tuple.index4].Add(tuple.value4);

                smallestHSubAdded[tuple.index4] = true;
            }
        }
    }

    // Helper method, Finds the Hidden Subsets for Doubles, Triples, and Quadruples (Singles only need to iterated through one group to find)
    private void GetHSubs(
    int[] indices,
    List<int>[] noteGroup,
    List<int>[] invertedNoteGroup,
    int[] occurrences,
    int[] invertedOccurrences,
    ref List<(int index1, int index2, int value1, int value2)> allDoublesFound,
    ref List<(int index1, int index2, int index3, int value1, int value2, int value3)> allTriplesFound,
    ref List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)> allQuadruplesFound)
    {
        // Get generic index doubles (normal & inverted)
        var genericIndexDoubles = GetDoubleHSubs(noteGroup, occurrences, false);
        var genericIndexInvertedDoubles = GetDoubleHSubs(invertedNoteGroup, invertedOccurrences, true);

        // Convert doubles indices back to original index and add to 'all list'
        foreach ((int index1, int index2, int value1, int value2) tuple in genericIndexDoubles)
            allDoublesFound.Add((indices[tuple.index1 - 1], indices[tuple.index2 - 1], tuple.value1, tuple.value2));
        foreach ((int index1, int index2, int value1, int value2) tuple in genericIndexInvertedDoubles)
            allDoublesFound.Add((indices[tuple.index1 - 1], indices[tuple.index2 - 1], tuple.value1, tuple.value2));

        // Get generic index triples (normal & inverted)
        var genericIndexTriples = GetTripleHSubs(noteGroup, occurrences, false);
        var genericIndexInvertedTriples = GetTripleHSubs(invertedNoteGroup, invertedOccurrences, true);

        // Convert triples indices back to original index and add to 'all list'
        foreach ((int index1, int index2, int index3, int value1, int value2, int value3) tuple in genericIndexTriples)
            allTriplesFound.Add((indices[tuple.index1 - 1], indices[tuple.index2 - 1], indices[tuple.index3 - 1], tuple.value1, tuple.value2, tuple.value3));
        foreach ((int index1, int index2, int index3, int value1, int value2, int value3) tuple in genericIndexInvertedTriples)
            allTriplesFound.Add((indices[tuple.index1 - 1], indices[tuple.index2 - 1], indices[tuple.index3 - 1], tuple.value1, tuple.value2, tuple.value3));

        // Get generic index quadruples (normal & inverted)
        var genericIndexQuadruples = GetQuadrupleHSubs(noteGroup, occurrences, false);
        var genericIndexInvertedQuadruples = GetQuadrupleHSubs(invertedNoteGroup, invertedOccurrences, true);

        // Convert quadruples indices back to original index and add to 'all list'
        foreach ((int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4) tuple in genericIndexQuadruples)
            allQuadruplesFound.Add((indices[tuple.index1 - 1], indices[tuple.index2 - 1], indices[tuple.index3 - 1], indices[tuple.index4 - 1], tuple.value1, tuple.value2, tuple.value3, tuple.value4));
        foreach ((int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4) tuple in genericIndexInvertedQuadruples)
            allQuadruplesFound.Add((indices[tuple.index1 - 1], indices[tuple.index2 - 1], indices[tuple.index3 - 1], indices[tuple.index4 - 1], tuple.value1, tuple.value2, tuple.value3, tuple.value4));
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

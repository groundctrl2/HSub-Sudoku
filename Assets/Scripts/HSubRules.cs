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
    private SudokuGrid grid;
    public List<int>[] hsubGrid = new List<int>[81];

    // Found hidden subsets
    private List<(int index, int value)> singles;
    private List<(int index1, int index2, int value1, int value2)> doubles;
    private List<(int index1, int index2, int index3, int value1, int value2, int value3)> triples;
    private List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)> quadruples;

    // Constructor to initialize with the current 1D notes grid state
    public HSubRules(SudokuGrid grid, bool[][] currentNotes)
    {
        this.grid = grid;

        // Initialize hsub grid
        for (int i = 0; i < currentNotes.Length; i++)
        {
            hsubGrid[i] = new List<int>();
            for (int j = 0; j < currentNotes[i].Length; j++)
                if (currentNotes[i][j]) hsubGrid[i].Add(j + 1);
        }

        // Store hsubGrid copy
        var previousHsubGrid = new List<int>[81];
        for (int i = 0; i < 81; i++)
            previousHsubGrid[i] = new List<int>(hsubGrid[i]);

        // Find hsubs and update hsub grid
        bool changed;
        do
        {
            changed = false;

            // Search and update each row
            for (int row = 0; row < 9; row++)
            {
                int[] indices = SudokuRules.GetRow(row);
                var noteGroup = GetNoteGroup(indices);
                var invertedNoteGroup = InvertNoteGroup(noteGroup);

                GetNoteGroupHSubs(noteGroup, indices, false);
                UpdateHSubGrid(indices);
                GetNoteGroupHSubs(invertedNoteGroup, indices, true);
                UpdateHSubGrid(indices);
            }

            // Search each column
            for (int col = 0; col < 9; col++)
            {
                int[] indices = SudokuRules.GetCol(col);
                var noteGroup = GetNoteGroup(indices);
                var invertedNoteGroup = InvertNoteGroup(noteGroup);

                GetNoteGroupHSubs(noteGroup, indices, false);
                UpdateHSubGrid(indices);
                GetNoteGroupHSubs(invertedNoteGroup, indices, true);
                UpdateHSubGrid(indices);
            }

            // Search each subgrid
            for (int subgrid = 1; subgrid < 10; subgrid++)
            {
                int[] indices = SudokuRules.GetSubGrid(subgrid);
                var noteGroup = GetNoteGroup(indices);
                var invertedNoteGroup = InvertNoteGroup(noteGroup);

                GetNoteGroupHSubs(noteGroup, indices, false);
                UpdateHSubGrid(indices);
                GetNoteGroupHSubs(invertedNoteGroup, indices, true);
                UpdateHSubGrid(indices);
            }

            // Check if hsubGrid and previous copy are different
            for (int i = 0; i < 81; i++)
            {
                if (!ListsAreEqual(hsubGrid[i], previousHsubGrid[i]))
                {
                    changed = true;
                    break;
                }
            }

            // Update previous copy with the current state of hsubGrid
            for (int i = 0; i < 81; i++)
                previousHsubGrid[i] = new List<int>(hsubGrid[i]);
        } while (changed);
    }

    // Takes the current singles, doubles, triples and quadruples lists and updates the hsub grid list
    private void UpdateHSubGrid(int[] indices)
    {
        // Update quadruples
        foreach ((int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4) in quadruples)
        {
            // Update quadruple index 1
            var newNote = new List<int>();
            if (hsubGrid[index1].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index1].Contains(value2))
                newNote.Add(value2);
            if (hsubGrid[index1].Contains(value3))
                newNote.Add(value3);
            if (hsubGrid[index1].Contains(value4))
                newNote.Add(value4);
            hsubGrid[index1] = newNote;

            // Update quadruple index 2
            newNote = new List<int>();
            if (hsubGrid[index2].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index2].Contains(value2))
                newNote.Add(value2);
            if (hsubGrid[index2].Contains(value3))
                newNote.Add(value3);
            if (hsubGrid[index2].Contains(value4))
                newNote.Add(value4);
            hsubGrid[index2] = newNote;

            // Update quadruple index 3
            newNote = new List<int>();
            if (hsubGrid[index3].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index3].Contains(value2))
                newNote.Add(value2);
            if (hsubGrid[index3].Contains(value3))
                newNote.Add(value3);
            if (hsubGrid[index3].Contains(value4))
                newNote.Add(value4);
            hsubGrid[index3] = newNote;

            // Update quadruple index 4
            newNote = new List<int>();
            if (hsubGrid[index4].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index4].Contains(value2))
                newNote.Add(value2);
            if (hsubGrid[index4].Contains(value3))
                newNote.Add(value3);
            if (hsubGrid[index4].Contains(value4))
                newNote.Add(value4);
            hsubGrid[index4] = newNote;

            // Update indices not part of quadruple
            foreach (int otherIndex in indices)
                if (otherIndex != index1 && otherIndex != index2 && otherIndex != index3 && otherIndex != index4)
                {
                    if (hsubGrid[otherIndex].Contains(value1))
                        hsubGrid[otherIndex].Remove(value1);
                    if (hsubGrid[otherIndex].Contains(value2))
                        hsubGrid[otherIndex].Remove(value2);
                    if (hsubGrid[otherIndex].Contains(value3))
                        hsubGrid[otherIndex].Remove(value3);
                    if (hsubGrid[otherIndex].Contains(value4))
                        hsubGrid[otherIndex].Remove(value4);
                }
        }

        // Update triples
        foreach ((int index1, int index2, int index3, int value1, int value2, int value3) in triples)
        {
            // Update triple index 1
            var newNote = new List<int>();
            if (hsubGrid[index1].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index1].Contains(value2))
                newNote.Add(value2);
            if (hsubGrid[index1].Contains(value3))
                newNote.Add(value3);
            hsubGrid[index1] = newNote;

            // Update triple index 2
            newNote = new List<int>();
            if (hsubGrid[index2].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index2].Contains(value2))
                newNote.Add(value2);
            if (hsubGrid[index2].Contains(value3))
                newNote.Add(value3);
            hsubGrid[index2] = newNote;

            // Update triple index 3
            newNote = new List<int>();
            if (hsubGrid[index3].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index3].Contains(value2))
                newNote.Add(value2);
            if (hsubGrid[index3].Contains(value3))
                newNote.Add(value3);
            hsubGrid[index3] = newNote;

            // Update indices not part of triple
            foreach (int otherIndex in indices)
                if (otherIndex != index1 && otherIndex != index2 && otherIndex != index3)
                {
                    if (hsubGrid[otherIndex].Contains(value1))
                        hsubGrid[otherIndex].Remove(value1);
                    if (hsubGrid[otherIndex].Contains(value2))
                        hsubGrid[otherIndex].Remove(value2);
                    if (hsubGrid[otherIndex].Contains(value3))
                        hsubGrid[otherIndex].Remove(value3);
                }
        }

        // Update doubles
        foreach ((int index1, int index2, int value1, int value2) in doubles)
        {
            // Update double index 1
            var newNote = new List<int>();
            if (hsubGrid[index1].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index1].Contains(value2))
                newNote.Add(value2);
            hsubGrid[index1] = newNote;

            // Update double index 2
            newNote = new List<int>();
            if (hsubGrid[index2].Contains(value1))
                newNote.Add(value1);
            if (hsubGrid[index2].Contains(value2))
                newNote.Add(value2);
            hsubGrid[index2] = newNote;

            // Update indices not part of double
            foreach (int otherIndex in indices)
                if (otherIndex != index1 && otherIndex != index2)
                {
                    if (hsubGrid[otherIndex].Contains(value1))
                        hsubGrid[otherIndex].Remove(value1);
                    if (hsubGrid[otherIndex].Contains(value2))
                        hsubGrid[otherIndex].Remove(value2);
                }
        }

        // Update singles
        foreach ((int index, int value) in singles)
        {
            // Update single index
            hsubGrid[index] = new List<int> { value };

            // Update indices not part of single
            foreach (int otherIndex in indices)
                if (otherIndex != index)
                    if (hsubGrid[otherIndex].Contains(value))
                        hsubGrid[otherIndex].Remove(value);
        }

        // Update sudoku grid cells
        grid.ResetHSubs(this);
    }

    // Given a note group , returns all singles, doubles, triples and quadruples with their non-generic indices
    // If given note group is inverted, set isInverted as true and values will be returned backwards
    private void GetNoteGroupHSubs(List<int>[] noteGroup, int[] indices, bool isInverted)
    {
        // Clear current hsub lists
        singles = new List<(int index, int value)>();
        doubles = new List<(int index1, int index2, int value1, int value2)>();
        triples = new List<(int index1, int index2, int index3, int value1, int value2, int value3)>();
        quadruples = new List<(int index1, int index2, int index3, int index4, int value1, int value2, int value3, int value4)>();

        // Sort value occurrences
        var oneValue = new List<int>();
        var twoValues = new List<int>();
        var threeValues = new List<int>();
        var fourValues = new List<int>();

        for (int i = 0; i < 9; i++)
        {
            if (noteGroup[i].Count == 1)
                oneValue.Add(i);
            else if (noteGroup[i].Count == 2)
            {
                twoValues.Add(i);
                threeValues.Add(i);
                fourValues.Add(i);
            }
            else if (noteGroup[i].Count == 3)
            {
                threeValues.Add(i);
                fourValues.Add(i);
            }
            else if (noteGroup[i].Count == 4)
                fourValues.Add(i);
        }

        // Get singles
        foreach (int singleIndex in oneValue)
        {
            if (isInverted)
                singles.Add((indices[noteGroup[singleIndex][0] - 1], singleIndex + 1));
            else
                singles.Add((indices[singleIndex], noteGroup[singleIndex][0]));
        }

        // Get doubles
        for (int i = 0; i < twoValues.Count; i++)
            for (int j = 0; j < twoValues.Count; j++)
                if (i < j)
                {
                    var uniqueValues = HSubRules.GetUniqueValues(noteGroup[twoValues[i]], noteGroup[twoValues[j]], null, null);
                    if (uniqueValues.Count == 2)
                    {
                        var values = uniqueValues.ToArray<int>();
                        int position1 = twoValues[i] + 1;
                        int position2 = twoValues[j] + 1;

                        if (isInverted)
                            doubles.Add((indices[values[0] - 1], indices[values[1] - 1], position1, position2));
                        else
                            doubles.Add((indices[position1 - 1], indices[position2 - 1], values[0], values[1]));
                    }
                }

        // Get triples
        for (int i = 0; i < threeValues.Count; i++)
            for (int j = 0; j < threeValues.Count; j++)
                if (i < j)
                    for (int k = 0; k < threeValues.Count; k++)
                        if (j < k)
                        {
                            var uniqueValues = HSubRules.GetUniqueValues(noteGroup[threeValues[i]], noteGroup[threeValues[j]], noteGroup[threeValues[k]], null);
                            if (uniqueValues.Count == 3)
                            {
                                var values = uniqueValues.ToArray<int>();
                                var positions = (threeValues[i] + 1, threeValues[j] + 1, threeValues[k] + 1);
                                var (pos1, pos2, pos3) = positions;

                                if (isInverted)
                                    triples.Add((indices[values[0] - 1], indices[values[1] - 1], indices[values[2] - 1], pos1, pos2, pos3));
                                else
                                    triples.Add((indices[pos1 - 1], indices[pos2 - 1], indices[pos3 - 1], values[0], values[1], values[2]));
                            }
                        }

        // Get quadruples
        for (int i = 0; i < fourValues.Count; i++)
            for (int j = 0; j < fourValues.Count; j++)
                if (i < j)
                    for (int k = 0; k < fourValues.Count; k++)
                        if (j < k)
                            for (int l = 0; l < fourValues.Count; l++)
                                if (k < l)
                                {
                                    var uniqueValues = HSubRules.GetUniqueValues(noteGroup[fourValues[i]], noteGroup[fourValues[j]], noteGroup[fourValues[k]], noteGroup[fourValues[l]]);
                                    if (uniqueValues.Count == 4)
                                    {
                                        var values = uniqueValues.ToArray<int>();
                                        var positions = (fourValues[i] + 1, fourValues[j] + 1, fourValues[k] + 1, fourValues[l] + 1);
                                        var (pos1, pos2, pos3, pos4) = positions;

                                        if (isInverted)
                                            quadruples.Add((indices[values[0] - 1], indices[values[1] - 1], indices[values[2] - 1], indices[values[3] - 1], pos1, pos2, pos3, pos4));
                                        else
                                            quadruples.Add((indices[pos1 - 1], indices[pos2 - 1], indices[pos3 - 1], indices[pos4 - 1], values[0], values[1], values[2], values[3]));
                                    }
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
    public List<int>[] InvertNoteGroup(List<int>[] noteGroup)
    {
        var invertedNoteGroup = new List<int>[9];
        for (int i = 0; i < 9; i++)
            invertedNoteGroup[i] = new List<int>();

        for (int index = 0; index < 9; index++)
            foreach (int note in noteGroup[index])
                // Only add if not 0 (which means the space is already filled)
                if (note != 0)
                    invertedNoteGroup[note - 1].Add(index + 1); // + 1 to reflect position instead of index

        return invertedNoteGroup;
    }

    // Get hashset containing all unique values from given lists
    public static HashSet<int> GetUniqueValues(List<int> list1, List<int> list2, List<int> list3, List<int> list4)
    {
        var hashSet = new HashSet<int>();

        if (list1 != null)
            foreach (int value in list1)
                hashSet.Add(value);
        if (list2 != null)
            foreach (int value in list2)
                hashSet.Add(value);
        if (list3 != null)
            foreach (int value in list3)
                hashSet.Add(value);
        if (list4 != null)
            foreach (int value in list4)
                hashSet.Add(value);

        return hashSet;
    }

    // Returns whether 2 given list<int> lists are equal
    private static bool ListsAreEqual(List<int> list1, List<int> list2)
    {
        if (list1 == list2)
            return true;
        if (list1 == null || list2 == null)
            return false;
        if (list1.Count != list2.Count)
            return false;
        for (int i = 0; i < list1.Count; i++)
            if (list1[i] != list2[i])
                return false;
        return true;
    }
}

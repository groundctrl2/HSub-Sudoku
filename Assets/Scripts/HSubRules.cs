using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using UnityEngine;

public class HSubRules
{
    public List<int>[] hsubGrid = new List<int>[81];

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

        // TESTING
        var testNG = GetNoteGroup(SudokuRules.GetRow(5));
        var testING = InvertNoteGroup(testNG);
        var clusters = GetIndexClusters(testNG);
        var invertedClusters = GetIndexClusters(testING);

        // print normal clusters
        string c = "clusters: \n";
        for (int i = 0; i < clusters.Count; i++)
        {
            string s = i.ToString() + ": index ";
            foreach (int index in clusters[i])
                s += index.ToString();
            c += s += ", ";
        }
        Debug.Log(c);

        // print inverted clusters
        string ic = "inverted clusters: \n";
        for (int i = 0; i < clusters.Count; i++)
        {
            string s = (i + 1).ToString() + ": value ";
            foreach (int index in invertedClusters[i])
                s += index.ToString();
            ic += s += ", ";
        }
        Debug.Log(ic);
    }

    // Returns 2D List of index clusters, index values remain generic. Each index appearing once, otherwise the clusters would join
    private List<List<int>> GetIndexClusters(List<int>[] noteGroup)
    {
        var clusters = new List<List<int>>();
        var clusterValues = new List<HashSet<int>>();

        // For each position in the note group, add to existing cluster or create a new one
        for (int position = 1; position <= 9; position++)
        {
            int positionIndex = position - 1;
            var clustersToJoin = new HashSet<int>();

            // Get clusters to join
            foreach (int value in noteGroup[positionIndex])
                for (int i = 0; i < clusterValues.Count; i++)
                    if (clusterValues[i].Contains(value))
                        clustersToJoin.Add(i);

            // Join old clusters and their values into new cluster
            var newClusters = new List<List<int>>();
            var newClusterValues = new List<HashSet<int>>();
            var newCluster = new List<int>();
            var newValues = new HashSet<int>();
            for (int i = 0; i < clusters.Count; i++)
            {
                // Join old clusters into new cluster
                if (clustersToJoin.Contains(i))
                {
                    newCluster.AddRange(clusters[i]);
                    foreach (int value in clusterValues[i])
                        newValues.Add(value);
                }
                // Add original clusters that are not being joined
                else
                {
                    newClusters.Add(clusters[i]);
                    newClusterValues.Add(clusterValues[i]);
                }
            }

            // Add current position and its values to new cluster
            newCluster.Add(position);
            foreach (int value in noteGroup[positionIndex])
                newValues.Add(value);

            // Add the new cluster and its values
            newClusters.Add(newCluster);
            newClusterValues.Add(newValues);

            // Update the original lists with the new ones
            clusters = newClusters;
            clusterValues = newClusterValues;
        }

        return clusters;
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
    private int[] CountDigitOccurrences(List<int>[] noteGroup)
    {
        int[] valueOccurrences = new int[9];
        for (int i = 0; i < 9; i++)
            foreach (int digit in noteGroup[i])
                if (digit >= 1 && digit <= 9)
                    valueOccurrences[digit - 1]++;

        return valueOccurrences;
    }

    // Get the index/indices of a given value in a given note group (row/col/subgrid) based on the amount of occurrences to find
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
}

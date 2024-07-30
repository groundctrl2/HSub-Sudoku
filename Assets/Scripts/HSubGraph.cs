using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class HSubGraph
{
    private List<int>[] noteGroup;
    private int[][] valueOccurrences; // Jagged to allow arrays of different sizes

    // Graph variables
    public List<int>[] adj; // Zero-indexed adjacency list representing each index
    public List<int>[,] adjValues; // adjacency list companion, stores the values shared between vertices

    // Connected Components (CC) variables
    private bool[] marked;
    private int[] id;
    private int[] size;
    private int count;

    // Constructor
    public HSubGraph(List<int>[] noteGroup)
    {
        this.noteGroup = noteGroup;
        valueOccurrences = HSubRules.GetValueOccurrences(noteGroup);

        // Graph construction
        adj = new List<int>[9];
        adjValues = new List<int>[9, 9];

        for (int i = 0; i < 9; i++)
        {
            adj[i] = new List<int>();
            for (int j = 0; j < 9; j++)
                adjValues[i, j] = new List<int>();
        }

        AddEdges();

        // Connected Components (CC) Construction
        marked = new bool[9];
        id = new int[9];
        size = new int[9];
        for (int v = 0; v < 9; v++)
        {
            if (!marked[v])
            {
                DepthFirstSearch(v);
                count++;
            }
        }
    }

    // For every value shared between 2+ index vertices, add an edge
    private void AddEdges()
    {
        for (int value = 1; value < 10; value++)
        {
            // Add each edge once
            var occurrences = valueOccurrences[value - 1];
            for (int i = 0; i < occurrences.Length; i++)
                for (int j = 0; j < occurrences.Length; j++)
                    if (i < j)
                    {
                        int v = occurrences[i];
                        int w = occurrences[j];
                        if (noteGroup[v].Count <= 4 && noteGroup[w].Count <= 4)
                        {
                            // Add w to v
                            if (!adj[v].Contains(w))
                                adj[v].Add(w);
                            adjValues[v, w].Add(value);

                            // Add v to w
                            if (!adj[w].Contains(v))
                                adj[w].Add(v);
                            adjValues[w, v].Add(value);
                        }
                    }
        }
    }

    // Depth first search for CC construction
    private void DepthFirstSearch(int v)
    {
        marked[v] = true;
        id[v] = count;
        size[count]++;

        foreach (int w in adj[v])
            if (!marked[w])
                DepthFirstSearch(w);
    }
}

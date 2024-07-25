using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class HSubGraph
{
    // Zero-indexed adjacency lists representing each value
    public List<(int value1, int value2, int value3, int value4)>[] adjValues;
    public List<int>[] adjIndices;

    // Connected Components variables
    private bool[] marked;
    private int[] id;
    private int[] size;
    private int count;

    // Constructor
    public HSubGraph(List<int>[] noteGroup)
    {
        // Graph construction
        adjValues = new List<(int value1, int value2, int value3, int value4)>[9];
        adjIndices = new List<int>[9];

        // Fill adj values and indices
        for (int v = 0; v < 9; v++)
        {
            adjValues[v] = new List<(int value1, int value2, int value3, int value4)>();
            adjIndices[v] = new List<int>();
        }

        // Add edges to adj
        for (int v = 0; v < 9; v++)
        {
            if (noteGroup[v].Count <= 4)
            {
                int[] values = new int[4];
                for (int i = 0; i < noteGroup[v].Count; i++)
                    values[i] = noteGroup[v][i];
                AddEdge(v, (values[0], values[1], values[2], values[3]));
            }
        }

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

    // Add the path to each element of the path as an edge in adj values and indices
    private void AddEdge(int index, (int value1, int value2, int value3, int value4) path)
    {
        int[] values = { path.value1, path.value2, path.value3, path.value4 };
        for (int i = 0; i < 4; i++)
        {
            if (values[i] != 0)
            {
                adjValues[values[i] - 1].Add(path);
                adjIndices[values[i] - 1].Add(index);
            }
        }
    }

    // Depth first search for Connected Component (CC) construction
    private void DepthFirstSearch(int valueIndex)
    {
        marked[valueIndex] = true;
        id[valueIndex] = count;
        size[count]++;

        foreach ((int value1, int value2, int value3, int value4) adj in adjValues[valueIndex])
        {
            int[] values = { adj.value1, adj.value2, adj.value3, adj.value4 };
            for (int i = 0; i < 4; i++)
                if (values[i] != 0 && !marked[values[i] - 1])
                    DepthFirstSearch(values[i] - 1);
        }
    }

    // Return the component id of the CC containing the given value/vertex
    public int GetID(int value)
    {
        return id[value - 1];
    }

    // Return the size of the CC containing the given value/vertex
    public int GetSize(int value)
    {
        return size[id[value - 1]];
    }

    // Return the amount of Connected Components in the graph
    public int GetCount()
    {
        return count;
    }

    // Returns whether the given 2 values/vertices are connected in the same CC
    public bool AreConnected(int value1, int value2)
    {
        return id[value1 - 1] == id[value2 - 1];
    }
}

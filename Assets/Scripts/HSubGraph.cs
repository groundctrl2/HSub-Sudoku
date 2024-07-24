using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class HSubGraph
{
    public List<(int value1, int value2, int value3, int value4)>[] adjValues;
    public List<int>[] adjIndices;

    // Constructor
    public HSubGraph(List<int>[] noteGroup)
    {
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
}

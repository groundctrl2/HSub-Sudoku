using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuCell
{
    public Vector3 position;
    public PrefabIndex prefabIndex;

    public SudokuCell(Vector3 newPosition, PrefabIndex newIndex)
    {
        position = newPosition;
        prefabIndex = newIndex;
    }
}

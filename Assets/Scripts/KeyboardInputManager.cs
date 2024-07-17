using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : MonoBehaviour
{
    public SudokuGrid grid;

    // Update is called once per frame
    void Update()
    {
        // Keys 1-9 operate same as number buttons
        for (int number = 1; number <= 9; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
            {
                grid.AddDigitToSelected(number);
            }
        }

        // Key 0, C, Delete, and Backspace operate same as clear button
        if (Input.GetKeyDown("0") || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
            grid.ClearSelected();

        // Key D operates same as deselect button
        if (Input.GetKeyDown(KeyCode.D))
            grid.DeselectAll();

        // Key N operates same as notes button
        if (Input.GetKeyDown(KeyCode.N))
            grid.ToggleSeeNotes();
    }
}

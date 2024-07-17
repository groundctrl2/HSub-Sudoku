using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : MonoBehaviour
{
    public SudokuGrid grid;
    private ButtonManager buttonManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get the ButtonManager component attached to the same GameObject
        buttonManager = GetComponent<ButtonManager>();
    }

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

        // Key N triggers the notes button
        if (Input.GetKeyDown(KeyCode.N))
            buttonManager.toggleButtons[0].onClick.Invoke(); // Note button index 

        // Key M triggers the multiselect button    
        if (Input.GetKeyDown(KeyCode.M))
            buttonManager.toggleButtons[1].onClick.Invoke(); // Multiselect button index 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public SudokuGrid grid;
    public Button[] buttons = new Button[11];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Create a local copy of the loop variable
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method to handle button click
    public void OnButtonClick(int buttonIndex)
    {
        StartCoroutine(ButtonClickRoutine(buttonIndex));
    }

    // Coroutine that sets all clicked cells text to set number
    IEnumerator ButtonClickRoutine(int buttonIndex)
    {
        // Number buttons
        if (buttonIndex < 9)
        {
            int buttonNumber = buttonIndex + 1; // zero-indexed originally
            grid.NumberSelected(buttonNumber);
        }
        // Clear button
        else if (buttonIndex == 9)
            grid.ClearSelected();
        // Deselect button
        else
        {
            grid.DeselectAll();
        }

        yield return new WaitForSeconds(0f);
    }
}

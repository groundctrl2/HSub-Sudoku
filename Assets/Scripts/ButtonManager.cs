using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public SudokuGrid grid;
    public Button[] clickButtons = new Button[11];
    public Button[] toggleButtons = new Button[2];
    public Color normalColor;
    public Color toggleColor;
    private bool[] toggleButtonValues;

    // Start is called before the first frame update
    void Start()
    {
        // Adjust colors
        if (normalColor.a == 0)
            normalColor.a = 1; // Set alpha to 1 if necessary (fully opaque)
        if (toggleColor.a == 0)
            toggleColor.a = 1; // Set alpha to 1 if necessary (fully opaque)


        // Click buttons
        for (int i = 0; i < clickButtons.Length; i++)
        {
            int index = i; // Create a local copy of the loop variable
            clickButtons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // Toggle buttons
        toggleButtonValues = new bool[toggleButtons.Length];
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            toggleButtonValues[i] = false;
            int index = i; // Create a local copy of the loop variable
            toggleButtons[i].onClick.AddListener(() => OnButtonToggle(index));
        }
    }

    // Method to handle click buttons
    public void OnButtonClick(int buttonIndex)
    {
        StartCoroutine(ButtonClickRoutine(buttonIndex));
    }

    // Coroutine for all click buttons
    IEnumerator ButtonClickRoutine(int buttonIndex)
    {
        // Number buttons
        if (buttonIndex < 9)
        {
            int buttonNumber = buttonIndex + 1; // zero-indexed originally
            grid.AddDigitToSelected(buttonNumber);
        }
        // Clear button
        else if (buttonIndex == 9)
            grid.ClearSelected();
        // Deselect button
        else // if (buttonIndex == 10)
            grid.DeselectAll();

        yield return new WaitForSeconds(0f);
    }

    // Method to handle toggle buttons
    public void OnButtonToggle(int buttonIndex)
    {
        StartCoroutine(ButtonToggleRoutine(buttonIndex));
    }

    // Coroutine for all toggle buttons
    IEnumerator ButtonToggleRoutine(int buttonIndex)
    {
        // Toggle color
        if (toggleButtonValues[buttonIndex])
        {
            toggleButtons[buttonIndex].GetComponent<Image>().color = normalColor;
        }
        else
        {
            toggleButtons[buttonIndex].GetComponent<Image>().color = toggleColor;
        }
        toggleButtonValues[buttonIndex] = !toggleButtonValues[buttonIndex];

        // Note toggle button
        if (buttonIndex == 0)
            grid.ToggleSeeNotes();
        else // (buttonIndex == 1)
            grid.ToggleMultiSelect();

        yield return new WaitForSeconds(0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;

public class SudokuCell : MonoBehaviour
{
    public SudokuGrid grid;
    public int index;
    public Vector3 position;
    private TextMeshProUGUI mainText;
    private TextMeshProUGUI upperText;
    private TextMeshProUGUI middleText;
    private TextMeshProUGUI lowerText;

    private Color mainColor = Color.black;
    private Color noteColor;
    private Color incorrectColor;

    private static bool isMultiSelecting = false;
    private static bool isShowingNotes = false;
    private static bool isSelecting = false;
    private static bool isDeselecting = false;
    private static bool isMouseDragging = false;
    private bool isSelected = false;

    Material normal;
    Material hovered;
    Material selected;

    public void Initialize(SudokuGrid newGrid, int newIndex, Vector3 newPosition, TextMeshProUGUI mainTextComponent, TextMeshProUGUI upperTextComponent, TextMeshProUGUI middleTextComponent, TextMeshProUGUI lowerTextComponent)
    {
        grid = newGrid;
        index = newIndex;
        position = newPosition;
        transform.position = position;
        mainText = mainTextComponent;
        upperText = upperTextComponent;
        middleText = middleTextComponent;
        lowerText = lowerTextComponent;

        // Set materials
        normal = grid.materials[0];
        hovered = grid.materials[1];
        selected = grid.materials[2];

        // Set colors
        noteColor = grid.noteColor;
        if (noteColor.a == 0)
            noteColor.a = 1; // Set alpha to 1 if necessary (fully opaque)
        incorrectColor = grid.incorrectColor;
        if (incorrectColor.a == 0)
            incorrectColor.a = 1; // Set alpha to 1 if necessary (fully opaque)

        SetColor(upperText, noteColor);
        SetColor(middleText, noteColor);
        SetColor(lowerText, noteColor);
    }

    // Set the cell's text
    public void SetMainText(string text, bool isValid)
    {
        mainText.text = text;

        // Set to incorrect color if conflicting digit
        if (isValid)
            SetColor(mainText, mainColor);
        else
            SetColor(mainText, incorrectColor);

        // Wipe out notes if adding a digit
        if (text != "")
        {
            upperText.text = "";
            middleText.text = "";
            lowerText.text = "";
        }
    }

    // Toggle isShowingNotes bool and returns value
    public static void ToggleSeeNotes()
    {
        isShowingNotes = !isShowingNotes;
    }

    // Set the cell's note text
    public void SetNoteText(bool[] notes)
    {
        string upperString = "";
        string middleString = "";
        string lowerString = "";

        if (isShowingNotes)
        {
            for (int i = 0; i < 9; i++)
            {
                int number = i + 1; // "Calculate" the number from the index
                if (number <= 3)
                    upperString += notes[i] ? number + "  " : "   ";
                else if (number <= 6)
                    middleString += notes[i] ? number + "  " : "   ";
                else
                    lowerString += notes[i] ? number + "  " : "   ";
            }

            // Trim any trailing spaces
            upperString = upperString.Trim();
            middleString = middleString.Trim();
            lowerString = lowerString.Trim();
        }

        // Set the text properties
        upperText.text = upperString;
        middleText.text = middleString;
        lowerText.text = lowerString;
    }

    // Clear the cell's note text
    public void ClearNoteText()
    {
        upperText.text = "";
        middleText.text = "";
        lowerText.text = "";
    }

    // Set the cell's material and bool value whether the cell is selected
    public void SetMaterial(Material material)
    {
        GetComponent<Renderer>().material = material;
        isSelected = (material == selected) ? true : false;
    }

    // Set a TMP text's color
    private void SetColor(TextMeshProUGUI text, Color color)
    {
        text.color = color;
        text.fontMaterial.SetColor("_OutlineColor", color);
    }

    // Reset select bool values, set the material to normal, and update the grid's selected record
    public void Deselect()
    {
        isSelecting = false;
        isDeselecting = false;
        SetMaterial(normal);
        grid.SetSelected(index, false);
    }

    // Toggle whether you can select in multiple clicks without resetting
    public static void ToggleMultiSelect()
    {
        isMultiSelecting = !isMultiSelecting;
    }

    // Select/Deselect cell and start dragging
    void OnMouseDown()
    {
        if (!isMouseDragging)
        {
            // If multiselecting, can continue to select with multiple mouse downs
            if (isMultiSelecting)
            {
                if (!isSelected)
                {
                    isSelecting = true;
                    SetMaterial(selected);
                    grid.SetSelected(index, true);
                }
                else
                {
                    isDeselecting = true;
                    SetMaterial(hovered);
                    grid.SetSelected(index, false);
                }
            }
            // If not multiselecting, only allow one selection per mouse down
            else
            {
                if (!isSelected)
                {
                    grid.DeselectAll();
                    isSelecting = true;
                    SetMaterial(selected);
                    grid.SetSelected(index, true);
                }
                else
                {
                    isDeselecting = true;
                    SetMaterial(normal);
                    grid.SetSelected(index, false);
                }
            }
        }
    }

    // Update cell depending on selecting states and whether mouse is dragging
    void OnMouseOver()
    {
        if (isMouseDragging && isSelecting)
        {
            SetMaterial(selected);
            grid.SetSelected(index, true);
        }
        else if (isMouseDragging && isDeselecting)
        {
            SetMaterial(hovered);
            grid.SetSelected(index, false);
        }
        else if (!isSelecting && !isDeselecting && !isSelected)
        {
            SetMaterial(hovered);
        }
    }

    // Set back to normal material if previously hovering
    void OnMouseExit()
    {
        if (!isSelecting && !isSelected)
        {
            SetMaterial(normal);
        }
    }

    // Set that mouse is dragging
    void OnMouseDrag()
    {
        isMouseDragging = true;
    }

    // Set selecting bool values back to false
    void OnMouseUp()
    {
        isMouseDragging = false;
        isSelecting = false;
        isDeselecting = false;
    }
}
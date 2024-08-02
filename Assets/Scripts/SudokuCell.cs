using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System;

public class SudokuCell : MonoBehaviour
{
    public SudokuGrid grid;
    public int index;
    public Vector3 position;
    public MainTextState state;
    public string incorrectText = "";

    private TextMeshProUGUI upperText;
    private TextMeshProUGUI lowerText;
    private TextMeshProUGUI middleText;
    private TextMeshProUGUI mainText;

    private Color mainColor = Color.black;
    private Color noteColor;
    private Color incorrectColor;

    private static bool isMultiSelecting = false;
    private static bool isShowingNotes = false;
    private static bool isSelecting = false;
    private static bool isDeselecting = false;
    private static bool isMouseDragging = false;
    public bool hasHsubValues = false;
    private bool isSelected = false;
    private bool hasHsubError = false;

    Material normal;
    Material hovered;
    Material selected;
    Material hsubError;

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
        hsubError = grid.materials[3];

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

    public enum MainTextState
    {
        empty, valid, incorrect, hsub
    }

    // Set the cell's text
    public void SetMainText(string text, bool isValid, bool isHsub)
    {
        mainText.text = text;
        incorrectText = "";

        // Set to normal value
        if (isValid)
        {
            SetColor(mainText, mainColor);
            if (text == "")
                state = MainTextState.empty;
            else
                state = MainTextState.valid;
        }
        // Set to hidden subset value
        else if (isHsub)
        {
            SetColor(mainText, noteColor);
            state = MainTextState.hsub;
        }
        // Set to incorrect value
        else
        {
            SetColor(mainText, incorrectColor);
            state = MainTextState.incorrect;
            incorrectText = text;
        }

        // Wipe out notes if adding a digit
        if (text != "")
        {
            upperText.text = "";
            middleText.text = "";
            lowerText.text = "";
        }
    }

    // Toggle isShowingNotes bool
    public static void ToggleSeeNotes()
    {
        isShowingNotes = !isShowingNotes;
    }

    // Set isShowingHsubs bool
    public void SetHSubs(List<int> hsubNotes, bool isShowing)
    {
        string middleString = "";

        // Process hsub notes if currently showing and notes provided (notes often not provided to reset cells that go from 1 hsub note to over 4)
        if (hsubNotes != null)
        {
            if (isShowing)
            {
                if (state == MainTextState.empty || state == MainTextState.hsub)
                {
                    // Set text
                    if (hsubNotes.Count == 1)
                        SetMainText(hsubNotes[0].ToString(), false, true);
                    else
                    {
                        if (state == MainTextState.hsub)
                            SetMainText("", true, false);
                        foreach (int hsub in hsubNotes)
                            middleString += hsub.ToString();
                    }

                    // Set material to hsub error (and note text elements to hsub color) if no available digits in hsub grid
                    if (hsubNotes.Count == 0 && hasHsubValues)
                    {
                        hasHsubError = true;
                        SetMaterial(hsubError);
                        SetColor(upperText, mainColor);
                        SetColor(lowerText, mainColor);
                        mainText.text = "";
                    }
                    // Set back to previous state if previously had hsubError that's fixed now
                    else if (hasHsubError)
                    {
                        hasHsubError = false;
                        SetMaterial(normal);
                        SetColor(upperText, noteColor);
                        SetColor(lowerText, noteColor);
                        if (hsubNotes.Count == 1)
                            SetMainText(hsubNotes[0].ToString(), false, true);
                    }
                }
            }
            else
            {
                // Reset cell's main text if not showing and hsub currently in main text
                if (state == MainTextState.hsub)
                    SetMainText("", true, false);
                if (hasHsubError)
                {
                    SetMaterial(normal);
                    hasHsubError = false;
                }
            }
        }
        else if (state == MainTextState.hsub)
            SetMainText("", true, false);
        else
        {
            hasHsubError = false;
            SetMaterial(normal);
            SetColor(upperText, noteColor);
            SetColor(lowerText, noteColor);
        }

        // Set the text property
        middleText.text = middleString;
    }

    // Set the cell's note and hidden subset text
    public void SetNoteText(bool[] notes)
    {
        string upperString = "";
        string lowerString = "";

        if (isShowingNotes && state == MainTextState.empty)
        {
            List<int> noteDigits = new List<int>();
            for (int i = 0; i < 9; i++)
                if (notes[i]) noteDigits.Add(i + 1); // Store each digit (+1 because zero-indexed)

            foreach (int digit in noteDigits)
            {
                if (digit <= 5)
                    upperString = upperString + digit.ToString();
                else
                    lowerString = lowerString + digit.ToString();
            }
        }

        // Set the text properties
        upperText.text = upperString;
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
    private void SetMaterial(Material material)
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
        if (!hasHsubError)
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
        if (!isMouseDragging && !hasHsubError)
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
            if (!hasHsubError)
                SetMaterial(selected);
            grid.SetSelected(index, true);
        }
        else if (isMouseDragging && isDeselecting)
        {
            if (!hasHsubError)
                SetMaterial(hovered);
            grid.SetSelected(index, false);
        }
        else if (!isSelecting && !isDeselecting && !isSelected && !hasHsubError)
            SetMaterial(hovered);
    }

    // Set back to normal material if previously hovering
    void OnMouseExit()
    {
        if (!isSelecting && !isSelected && !hasHsubError)
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
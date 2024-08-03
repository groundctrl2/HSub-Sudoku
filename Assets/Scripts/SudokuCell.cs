using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Represents a single sudoku cell. 
// This class handles the display and interaction logic for the cell, including setting text, selecting/deselecting, and managing state.
public class SudokuCell : MonoBehaviour
{
    public SudokuGrid grid;
    public int index; // Cell's grid index (1D)
    public Vector3 position; // Cell's position in canvas
    public MainTextState state; // Stores current (or lack of) value of the main text
    public string incorrectText = ""; // Stores current text if value/digit is incorrect

    // Cell text elements
    private TextMeshProUGUI upperText;
    private TextMeshProUGUI lowerText;
    private TextMeshProUGUI middleText;
    private TextMeshProUGUI mainText;

    // Materials
    Material normal;
    Material hovered;
    Material selected;
    Material hsubError;

    // Colors
    private Color mainColor = Color.black;
    private Color noteColor;
    private Color incorrectColor;

    // Selection mouse states
    private static bool isMultiSelecting = false;
    private static bool isShowingNotes = false;
    private static bool isSelecting = false;
    private static bool isDeselecting = false;
    private static bool isMouseDragging = false;

    // Cell's selection and hsub states
    private bool isSelected = false;
    public bool hasHsubValues = false;
    private bool hasHsubError = false;

    // Initializer
    public void Initialize(SudokuGrid newGrid, int newIndex, Vector3 newPosition, TextMeshProUGUI mainTextComponent, TextMeshProUGUI upperTextComponent, TextMeshProUGUI middleTextComponent, TextMeshProUGUI lowerTextComponent)
    {
        // Set instance variables
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

        // Set text elements' colors
        SetColor(upperText, noteColor);
        SetColor(middleText, noteColor);
        SetColor(lowerText, noteColor);
    }

    // Enum to keep track of main text's state
    public enum MainTextState
    {
        empty, valid, incorrect, hsub
    }

    // Set the cell's main text (empty, valid digit, incorrect digit, or hsub digit)
    public void SetMainText(string text, bool isValid, bool isHsub)
    {
        mainText.text = text; // Set text
        incorrectText = ""; // Reset inccorect text value

        // Set as normal value
        if (isValid)
        {
            SetColor(mainText, mainColor);
            if (text == "")
                state = MainTextState.empty;
            else
                state = MainTextState.valid;
        }
        // Set as hidden subset value
        else if (isHsub)
        {
            SetColor(mainText, noteColor);
            state = MainTextState.hsub;
        }
        // Set as incorrect value
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

    // Set hidden subset text (and main text if needed) based on provided hsubNotes (Null if just resetting) and whether hsubs are showing
    // Parameter hsubNotes indices represent each position in the given note group, each value with the lists represents an hsub digit
    public void SetHSubs(List<int> hsubNotes, bool isShowing)
    {
        string middleString = ""; // Beginning value

        // Process hsub notes if currently showing and notes are provided
        // Otherwise reset cells that go from 1 hsub note to over 4
        if (hsubNotes != null)
        {
            // If is showing (and in turn calculating) hidden subsets, option to set hsub text (possibly in main text element) and hsub error cells
            if (isShowing)
            {
                // Set hsub text (possibly in main text element) and hsub error cells if current main text state is empty or hsub
                if (state == MainTextState.empty || state == MainTextState.hsub)
                {
                    // Set text
                    // If only one hsub value, set value to main text element
                    if (hsubNotes.Count == 1)
                        SetMainText(hsubNotes[0].ToString(), false, true);
                    // Else set hsub values to hsub text element
                    else
                    {
                        // Reset main text element if previously only had one hsub value
                        if (state == MainTextState.hsub)
                            SetMainText("", true, false);

                        // Add each hsub value to middle text element's string
                        foreach (int hsub in hsubNotes)
                            middleString += hsub.ToString();
                    }

                    // If no possible/available values in hsub grid, set material to hsub error (and note text elements to hsub color for readability)
                    if (hsubNotes.Count == 0 && hasHsubValues)
                    {
                        hasHsubError = true;
                        SetMaterial(hsubError);
                        SetColor(upperText, mainColor);
                        SetColor(lowerText, mainColor);
                        mainText.text = "";
                    }
                    // Else if previously had hsubError that's now fixed, set back to previous (normal) state 
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
            // Else if not showing, clear hsub text if in main text element and set hsub error cell's to normal material
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
        // (Hsub list null) Else if main text state is hsub, clear main text element
        else if (state == MainTextState.hsub)
            SetMainText("", true, false);
        // (Hsub list null) Else set cell back to normal state
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

    // Set the cell's note text
    // Parameter indices represent values 1-9, if true at index then the corresponding value is valid and can be added
    public void SetNoteText(bool[] notes)
    {
        string upperString = ""; // If valid, contains digits 1-5
        string lowerString = ""; // If valid, contains digits 6-9

        // If showing notes and (to not overlap) the main text does not contain an hsub, add given notes
        if (isShowingNotes && state == MainTextState.empty)
        {
            // Get notes
            List<int> noteDigits = new List<int>();
            for (int i = 0; i < 9; i++)
                if (notes[i]) noteDigits.Add(i + 1); // Store each digit (+1 because zero-indexed)

            // Add notes to upper and lower text strings
            foreach (int digit in noteDigits)
            {
                if (digit <= 5)
                    upperString = upperString + digit.ToString();
                else
                    lowerString = lowerString + digit.ToString();
            }
        }

        // Set the note texts with the built strings
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

    // Set a TMP text element's color
    private void SetColor(TextMeshProUGUI text, Color color)
    {
        text.color = color;
        text.fontMaterial.SetColor("_OutlineColor", color);
    }

    // Reset select bool values, set the material to normal, and update the grid's selected record for the cell
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

    // Select/Deselect cell and start dragging when appropriate
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
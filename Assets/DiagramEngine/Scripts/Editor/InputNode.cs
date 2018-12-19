using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputNode : BaseInputNode {

    private InputType inputType;

    public enum InputType{
    
        Number,
        Randomization
    }

    private string randomFrom = "";
    private string randomTo = "";

    private string inputValue = "";

    public InputNode() {

        windowTitle = "Input Node";
         
    }

    public override void DrawWindow() {
        base.DrawWindow();

        inputType = (InputType)EditorGUILayout.EnumPopup("Input type: ", inputType);

        if (inputType == InputType.Number) {

            inputValue = EditorGUILayout.TextField("Value", inputValue); 
        }
        else if(inputType == InputType.Randomization) {
            randomFrom = EditorGUILayout.TextField("From", randomFrom);
            randomTo = EditorGUILayout.TextField("To", randomTo);

            if(GUILayout.Button("Calculate Random")) {
                CalculateRandom();
            }
        }
    }

    public override void DrawCurves() {
        base.DrawCurves();


    }

    private void CalculateRandom() {

        float rFrom = 0f;
        float rTo = 0f;

        float.TryParse(randomFrom, out rFrom);
        float.TryParse(randomTo, out rTo);

        int randFrom = (int)(rFrom * 10);
        int randTo = (int)(rTo * 10);

        int selected = Random.Range(randFrom, randTo + 1);

        float selectedValue = selected / 10f;

        inputValue = selectedValue.ToString();
    }

    public override string GetResult() {
        return inputValue;

    }
}

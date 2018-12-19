using UnityEngine;
using System.Collections;
using UnityEditor;

//inherits from BaseInputNode
public class InputNode: BaseInputNode {

	//We want to have two types of input, a user inputed number or a random number
	private InputType inputType;
	public enum InputType {
		Number,
		Randomization
	}

	//variables for our random type
	private string randomFrom = "";
	private string randomTo = "";
	//variable for the user inputed value
	private string inputValue = "";

	//We add a default value on the input node
	public InputNode() {
		windowTitle = "Input Node";
	}

	//we pass the abstract DrawWindow
	public override void DrawWindow() {

		base.DrawWindow();

		//We make a enum popup so the user can choose the type
		inputType = (InputType) EditorGUILayout.EnumPopup("Input type: ", inputType);


		if(inputType == InputType.Number) 
		{
			//we add a textfield for the user to insert a value
			inputValue = EditorGUILayout.TextField("Value", inputValue);
		}
		else if(inputType == InputType.Randomization) 
		{
			//we add 2 textfields, the min and max range the random number can have
			randomFrom = EditorGUILayout.TextField("From", randomFrom);
			randomTo = EditorGUILayout.TextField("To", randomTo);

			//We don't want to calculate it every frame so we add a button to generate a new random value when the user presses it
			if(GUILayout.Button("Calculate Random")) 
			{
				calculateRandom();
			}
		}

		nodeResult = inputValue.ToString();

	}

	//Pass the abstract class
	public override void DrawCurves ()
	{

	}

	//function to calculate the random number
	private void calculateRandom() {
		float rFrom = 0;
		float rTo = 0;
		
		float.TryParse(randomFrom, out rFrom);
		float.TryParse(randomTo, out rTo);

		//we cast it as an int so that we don't have decimal values
		int randFrom = (int)(rFrom * 10);
		int randTo = (int)(rTo * 10);

		//we calculate the random
		int selected = UnityEngine.Random.Range(randFrom, randTo + 1);

		//and make it into a float once again
		float selectedValue = selected / 10;

		//we pass the final value as a string to the input value
		inputValue = selectedValue.ToString();
	}

	public override void Tick(float deltaTime)
	{
		
		
	}


}

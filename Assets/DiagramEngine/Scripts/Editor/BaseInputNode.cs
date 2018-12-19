using UnityEngine;
using System.Collections;

//inherits from base node
public abstract class BaseInputNode: BaseNode {

	//We use this class in every node that can be used as input

	protected string nodeResult = "None";

	//retunrs the result of the node
	public virtual string getResult() {
		return nodeResult;
	}

	//we pass DrawCurves since it's abstract
	public override void DrawCurves ()
	{

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInputNode : BaseNode {

    /// <summary>
    /// Get result of the node
    /// </summary>
    /// <returns>The result.</returns>
	public virtual string GetResult() {
        return "None";
    }

    public override void DrawCurves() {
        throw new System.NotImplementedException();
    }
}

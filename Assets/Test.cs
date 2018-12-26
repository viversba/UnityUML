using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {

        DiagramEngine.getInstance().Run();
	}
}

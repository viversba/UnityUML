using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    //Lexer lexer = new Lexer()

	// Use this for initialization
	void Start () {
        
        //UMLEngine.getInstance.runInEditMode();
        Program program = new Program();

        program.Start();
	}


}

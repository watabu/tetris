using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlInput))]
public class Game : MonoBehaviour {
    PlInput pInput;
	// Use this for initialization
	void Start () {
        pInput = GetComponent<PlInput>();
        Debug.Log("Debug start\n");
       //pInput.ChangePlConkind(0, PlInput.ConKind.JOYCON);
	}
    // Update is called once per frame
    void Update()
    {


    }
}

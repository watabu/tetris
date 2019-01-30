using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//もっぱらデバッグ用
public class Game : MonoBehaviour {
   
    public PlInput PI; 
	// Use this for initialization
	void Start () {
        PI = GetComponent<PlInput>();
        Debug.Log("Game's Start");
        PI.ChangePlConkind(0, PlInput.ConKind.KEYBOARD1);
        PI.ChangePlConkind(1, PlInput.ConKind.KEYBOARD2);
        Debug.Log(PlInput.Player[0].ConKind);
        Debug.Log(PlInput.Player[1].ConKind);
    }
    // Update is called once per frame
    void Update()
    {

        if (PI.GetInput(0, PlInput.Key.KEY_SUBMIT) == 1)
        {
            Debug.Log("0 Submit");

        }
        if (PI.GetInput(1, PlInput.Key.KEY_SUBMIT) == 1)
        {
            Debug.Log("1 Submit");
        }
    }
}

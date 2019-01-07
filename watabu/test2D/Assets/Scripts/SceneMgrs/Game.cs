using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//もっぱらデバッグ用
public class Game : MonoBehaviour {
    PlayerInput PlInput = GetComponent<PlayerInput>(); //できない！！！！！！なんで！！！！
	// Use this for initialization
	void Start () {
        Debug.Log("Debug start\n");
        PlInput.ChangePlConkind(0, PlayerInfo.eConKind.JOYCON);
	}
    // Update is called once per frame
    void Update()
    {

        if (PlInput.GetInput(0, PlayerInput.KEY_INPUT.KEY_SUBMIT) == 1)
        {
            Debug.Log("0 Submit");

        }
    }
}

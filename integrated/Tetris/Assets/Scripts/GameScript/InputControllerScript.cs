using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControllerScript : MonoBehaviour {
    public PlInput input;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3Int GetInputDirection(int playerNum)
    {
        Vector3Int direction = new Vector3Int();
        direction.x=input.GetInput2(playerNum,PlInput.Key.KEY_HORIZON);
        direction.y=input.GetInput2(playerNum,PlInput.Key.KEY_VERTICAL);
        direction.z=0;
        return direction;
    }

}

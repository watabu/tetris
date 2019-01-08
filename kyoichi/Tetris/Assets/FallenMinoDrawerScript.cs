using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenMinoDrawerScript : MonoBehaviour {

    GameObject[] cells;
    public GameObject minoController;

	// Use this for initialization
	void Start () {
        Vector2Int[,] controllCells = minoController.GetComponent<MinoControllerScript>().GetControllCoods();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

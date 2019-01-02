using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour {

    public GameObject gameBoard;
    public GameObject nextMino;
    public GameObject minoController;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        nextMino.GetComponent<NextMinoContainer>().FillMinoList();
        gameBoard.GetComponent<GameBoardScript>().Resume();
        gameBoard.GetComponent<GameBoardScript>().GetNextMino();
    }

    public void EndGame(){
        /*nextMino.SetActive(false);
        gameBoard.SetActive(false);
        minoController.SetActive(false);*/
    }


}

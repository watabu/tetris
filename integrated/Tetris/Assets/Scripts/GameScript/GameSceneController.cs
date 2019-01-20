using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Buttonクラスのスクリプトの参照用

//ゲームの進行を管理するクラス
//ゲームの開始時や終了時にUIを出させたり
//
public class GameSceneController : MonoBehaviour {
    //各クラスの参照
    public GameObject gameBoard;
    public GameObject nextMino;
    public GameObject minoController;
    //UIの参照
    public GameObject readyCanvasPrefab;
    public GameObject endDialogPrefab;
    
    //シーンの状態を表す変数
    public enum GameState
    {
        Null,
        Ready,
        Playing,
        End,
    }
    [SerializeField]
    private GameState state=GameState.Null;

    // Use this for initialization
    void Start () {
        //ゲーム開始時のUIをPrefabから生成
        GameObject readyCanvas=UsefulFunctions.CloneObject(readyCanvasPrefab);

        //UIの中のボタンが押されたときゲームを開始させるように関数を登録
        GameObject button = readyCanvas.transform.Find("Button").gameObject;
        button.GetComponent<Button>().onClick.AddListener(StartGame);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //ゲームを開始させる関数
    public void StartGame()
    {
        state=GameState.Ready;
        nextMino.GetComponent<NextMinoContainer>().FillMinoList();
        gameBoard.GetComponent<GameBoardScript>().Resume();
        gameBoard.GetComponent<GameBoardScript>().GetNextMino();
    }

    //ゲームを終了させる関数
    public void EndGame(){
        state=GameState.End;
        /*nextMino.SetActive(false);
        gameBoard.SetActive(false);
        minoController.SetActive(false);*/
    }


}

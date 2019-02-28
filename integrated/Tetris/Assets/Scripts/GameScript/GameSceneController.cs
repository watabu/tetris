using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Buttonクラスのスクリプトの参照用

//ゲームの進行を管理するクラス
//ゲームの開始時や終了時にUIを出させたり
//ミノのコントローラに新しいミノを登録させる
//
public class GameSceneController : MonoBehaviour {
    //各クラスの参照
    [Header("Scene Object References")]
    public GameBoardScript gameBoard;
    public NextMinoContainer nextMino;
    public GameObject minoController;
    public BoardMinoRegister minoRegister;
    //UIの参照
    [Header("UI References")]
    public GameObject readyCanvasPrefab;
    public GameObject endDialogPrefab;

    //シーンの状態を表す変数
    private enum GameState
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
        minoController.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //ゲームを開始させる関数
    public void StartGame()
    {
        state=GameState.Ready;
        nextMino.FillMinoList();
        gameBoard.Resume();
        minoController.SetActive(true);
        MinoUpdate();
    }

    //ゲームを終了させる関数
    public void EndGame(){
        state=GameState.End;
        /*nextMino.SetActive(false);
        gameBoard.SetActive(false);
        minoController.SetActive(false);*/
    }

    //新しいミノを生成させ、コントローラーに登録する関数
    public void MinoUpdate()
    {
        MinoUpdate(nextMino.GetNextMino());
    }
    public void MinoUpdate(GameObject mino_)
    {
        minoRegister.RegisterMino(mino_);
    }


}

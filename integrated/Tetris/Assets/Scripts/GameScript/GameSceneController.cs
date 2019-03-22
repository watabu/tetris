using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Buttonクラスのスクリプトの参照用
using UnityEngine.SceneManagement;

//ゲームの進行を管理するクラス
//ゲームの開始時や終了時にUIを出させたり
//ミノのコントローラに新しいミノを登録させる
//
//3/21 １プレイヤー分のオブジェクトをまとめて管理するクラスと分離
//
public class GameSceneController : MonoBehaviour {
    //各クラスの参照
    [Header("1P Object References")]
    public PlayerControllManager playerControll1P;
    [Header("2P Object References")]
    public PlayerControllManager playerControll2P;

    //UIの参照
    [Header("UI Prefab")]
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
        GenerateStartGUI();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateStartGUI()
    {
        //ゲーム開始時のUIをPrefabから生成
        GameObject readyCanvas = UsefulFunctions.CloneObject(readyCanvasPrefab);

        //UIの中のボタンが押されたときゲームを開始させるように関数を登録
        GameObject button = readyCanvas.transform.Find("Button").gameObject;
        button.GetComponent<Button>().onClick.AddListener(StartGame);
    }
    void GenerateEndGUI()
    {
        GameObject endCanvas = UsefulFunctions.CloneObject(endDialogPrefab);

        //UIの中のボタンが押されたときゲームを開始させるように関数を登録
        GameObject button = endCanvas.transform.Find("Restart").gameObject;
        button.GetComponent<Button>().onClick.AddListener(()=> { SceneManager.LoadScene("Game"); });
        /*button.GetComponent<Button>().onClick.AddListener(GenerateStartGUI);
        button.GetComponent<Button>().onClick.AddListener(() => { Destroy(endCanvas); });*/
        GameObject end = endCanvas.transform.Find("End").gameObject;
        end.GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("Title"); });
    }

    //ゲームを開始させる関数
    public void StartGame()
    {
        state=GameState.Ready;
        playerControll1P.StartGame();
        playerControll2P.StartGame();
    }

    //ゲームを終了させる関数
    public void EndGame()
    {
        state = GameState.End;
        //ゲーム終了時のUIをPrefabから生成
        GenerateEndGUI();
        playerControll1P.EndGame();
        playerControll2P.EndGame();
    }
    

}

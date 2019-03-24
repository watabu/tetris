using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーの入力やボードなどの全体を１プレイヤー分管理するクラス
//PlayerControllManager manager1P;
//PlayerControllManager manager2P;
//のように管理
public class PlayerControllManager : MonoBehaviour
{
    //各クラスの参照
    [Header("Object References")]
    public GameBoardScript gameBoard;
    public NextMinoContainer nextMino;
    public GameObject minoController;
    public BoardMinoRegister minoRegister;

    // Start is called before the first frame update
    void Start()
    {
        minoController.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    //ゲーム関連のオブジェクトを開始する
    public void StartGame()
    {
        nextMino.FillMinoList();
        gameBoard.Restart();
        minoController.SetActive(true);
        minoController.GetComponent<MinoControllerScript>().Restart();
        MinoUpdate();
    }

    //ゲーム関連のオブジェクトを終了させる
    public void EndGame()
    {
        gameBoard.Stop();
        minoController.SetActive(false);
    }

    public void MinoUpdate()
    {
        MinoUpdate(nextMino.GetNextMino());
    }
    public void MinoUpdate(GameObject mino_)
    {
        minoRegister.RegisterMino(mino_);
    }
}

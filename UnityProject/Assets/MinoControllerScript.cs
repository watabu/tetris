using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//ミノを入力で動かすクラス
//ミノクラスを登録ではなく、複数(5x5の配列)のセルを登録
//
public class MinoControllerScript : MonoBehaviour {

    GameObject[,] cells;//[5,5]サイズを前提(回転の中心は[2,2])
    public GameObject input;//入力クラスの参照
    public int fallSpeed;//ミノが落ちる速さ (何フレーム(60フレーム→１秒)に１回１マス落ちるか)
    public GameObject gameBoard;
    int count;//
    bool minoStuckFlag;//ミノが止まったか

    [SerializeField] UnityEvent OnMinoStuck;//ミノを動かせなくなったとき実行する関数を格納する変数

    // Use this for initialization
    void Start () {
        minoStuckFlag = false;
        count = 0;
        cells = new GameObject[5, 5];
        if (OnMinoStuck == null) OnMinoStuck = new UnityEvent(); //イベント・インスタンスの作成
    }
    // Update is called once per frame
	void Update () {
        if (count % fallSpeed == 0)//一定間隔でミノを下に落とす
        {
            MoveDown();
        }
        count++;
        if (IsStuck())//ミノを動かせなくなったとき関数を実行する
        {
            OnMinoStuck.Invoke();
        }
	}

    //複数のセルを動かせるようにする
    //5x5サイズの配列を渡されることを前提としているため、
    //回転の中心はcells[2,2]
    //とする
    public void RegisterCells(GameObject[,] cells_) { cells = cells_; }
    public void RemoveCells() { cells = null; } //セルの操作をやめる

    bool IsStuck(){return minoStuckFlag; } //ミノが動かせなくなったかどうか(内部処理用)

    public bool RotateLeft() { return true; } //左回り(反時計回り)にまわす
    public bool RotateRight() { return true; }//右回り(時計回り)にまわす

    public bool MoveDown()//
    {
        for (int y = 0; y < 5; y++)//下の段から順番にまわす
        {
            for (int x = 0; x < 5; x++)
            {
                if (!MoveCell(cells[y, x], 0, -1))
                {
                    return false;//下にミノがあった場合
                }
            }
        }
        return true;
    }
    public bool MoveLeft()//
    {
        for (int x = 0; x < 5; x++)//左の段から順番にまわす
        {
            for (int y = 0; y < 5; y++)
            {
                if (!MoveCell(cells[y, x], -1, 0))
                {
                    return false;//下にミノがあった場合
                }
            }
        }
        return true;
    }
    public bool MoveRight()//
    {
        for (int x = 4; x >= 0; x--)//右の段から順番にまわす
        {
            for (int y = 0; y < 5; y++)
            {
                if (!MoveCell(cells[y, x], 1, 0))
                {
                    return false;//下にミノがあった場合
                }
            }
        }
        return true;

    }

    //格納しているセルを(offsetX,offsetY)だけ動かす関数
    //移動に失敗した場合falseを返す
    //内部処理用
    private bool MoveCell(GameObject cell,int offsetX,int offsetY)
    {
        if (cell != null)
        {
            Vector2Int cood = cell.GetComponent<CellScript>().boardCood;
            Vector2Int destination = gameBoard.GetComponent<GameBoardScript>().MoveCell(cood, offsetX, offsetY);
            if (cood == destination)//下にミノがあった場合
            {
                return false;
            }
        }
        return true;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//ミノを入力で動かすクラス
//ミノクラスを登録ではなく、複数(5x5の配列)のセルを登録
//
public class MinoControllerScript : MonoBehaviour
{

    Vector2Int[,] cells;//[4,4]サイズを前提(回転の中心は[1.5,1.5])
    public GameObject input;//入力クラスの参照
    public int fallSpeed;//ミノが落ちる速さ (何フレーム(60フレーム→１秒)に１回１マス落ちるか)
    public GameObject gameBoard;
    private GameBoardScript gameBoardS;
    int count;//
    bool minoStuckFlag;//ミノが止まったか
    private int minoSize;

    [SerializeField] UnityEvent OnMinoStuck;//ミノを動かせなくなったとき実行する関数を格納する変数

    private void Awake()
    {
        minoStuckFlag = false;
        count = 0;
        minoSize = 4;
        cells = new Vector2Int[minoSize, minoSize];
        if (OnMinoStuck == null) OnMinoStuck = new UnityEvent(); //イベント・インスタンスの作成
        gameBoardS = gameBoard.GetComponent<GameBoardScript>();
    }

    // Use this for initialization
    void Start()
    {
        minoStuckFlag = false;
        count = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsStuck())//ミノを動かせなくなったとき関数を実行する
        {
            OnMinoStuck.Invoke();
        }
        else
        {
            if (count % fallSpeed == 0)//一定間隔でミノを下に落とす
            {
                MoveDown();
            }
            count++;
        }
    }

    //複数のセルを動かせるようにする
    //5x5サイズの配列を渡されることを前提としているため、
    //回転の中心はcells[(minoSize-1)/2,(minoSize-1)/2]
    //とする
    public void RegisterCells(Vector2Int[,] cells_) {
        cells = cells_;
        minoStuckFlag = false;
        count = 0;
    }
    public void RemoveCells()//セルの操作をやめる
    {
        cells = new Vector2Int[minoSize, minoSize];
        minoStuckFlag = false;
        count = 0;
    }
    
    bool IsStuck() { return minoStuckFlag; } //ミノが動かせなくなったかどうか(内部処理用)

    public bool RotateLeft() { return true; } //左回り(反時計回り)にまわす
    public bool RotateRight() { return true; }//右回り(時計回り)にまわす

    public bool MoveDown()//
    {
        //Debug.Log("move down");

        for (int y = 0; y < minoSize; y++)//下の段から順番にまわす
            for (int x = 0; x < minoSize; x++)
                if (!MoveCell(ref cells[y, x], 0, -1))
                    return false;//下にミノがあった場合

        return true;
    }
    public bool MoveLeft()//
    {
        for (int x = 0; x < minoSize; x++)//左の段から順番にまわす
            for (int y = 0; y < minoSize; y++)
                if (!MoveCell(ref cells[y, x], -1, 0))
                    return false;//下にミノがあった場合
        return true;
    }
    public bool MoveRight()//
    {
        for (int x = minoSize; x >= 0; x--)//右の段から順番にまわす
            for (int y = 0; y < minoSize; y++)
                if (!MoveCell(ref cells[y, x], 1, 0))
                    return false;//下にミノがあった場合
        return true;
    }

    //格納しているセルを(offsetX,offsetY)だけ動かす関数
    //移動に失敗した場合falseを返す
    //内部処理用
    private bool MoveCell(ref Vector2Int cell, int offsetX, int offsetY)
    {
        if (!IsNull(cell))
        {
            Vector2Int destination = gameBoardS.MoveCell(cell, offsetX, offsetY);
            if (cell == destination)//下にミノがあった場合
            {
                minoStuckFlag = true;
                return false;
            }
            cell = destination;
        }
        return true;
    }

    //格納している配列のマス[cell.y,cell.x]の座標に
    //オブジェクトがないときtrueを返す
    private bool IsNull(Vector2Int cell) { return cell.x == -1 || cell.y == -1; }

}

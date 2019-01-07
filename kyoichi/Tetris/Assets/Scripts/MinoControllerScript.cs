using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//ミノを入力で動かすクラス
//ミノクラスを登録ではなく、複数(5x5の配列)のセルを登録
//
public class MinoControllerScript : MonoBehaviour
{
    //１次元配列にすると移動で下から順番にやる処理とか面倒になる
    Vector2Int[,] cells;
    public GameObject input;//入力クラスの参照
    public int fallSpeed;//ミノが落ちる速さ (何フレーム(60フレーム→１秒)に１回１マス落ちるか)
    public GameObject gameBoard;
    private GameBoardScript gameBoardS;
    int count;//
    bool minoStuckFlag;//ミノが止まったか
     int minoSizeY;
     int minoSizeX;

    [SerializeField] UnityEvent OnMinoStuck;//ミノを動かせなくなったとき実行する関数を格納する変数

    private void Awake()
    {
        minoStuckFlag = false;
        count = 0;
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
            return;
        }
        Vector3Int moveOffset = Vector3Int.zero;
        if (count % fallSpeed == 0)//一定間隔でミノを下に落とす
        {
            moveOffset = Vector3Int.down;
        }
        SwitchCellToControl(moveOffset);//セルをコントロールレイヤーに切り替え、moveOffset分移動させる
        if (MoveAble())
        {
            SwitchCellToBoard();//コントロールレイヤーに置いたセルを元に戻す
        }
        else
        {
            SwitchCellToBoard(moveOffset*-1);//コントロールレイヤーに置いたセルを元に戻す
            minoStuckFlag = true;
            return;
        }
        count++;
    }

    //複数のセルを動かせるようにする
    //5x5サイズの配列を渡されることを前提としているため、
    //回転の中心はcells[(minoSize-1)/2,(minoSize-1)/2]
    //とする
    public void RegisterCells(Vector2Int[,] cells_) {
        minoSizeY = cells_.GetLength(0);
        minoSizeX = cells_.GetLength(1);
        cells = cells_;
        minoStuckFlag = false;
        count = 0;
    }
    public void RemoveCells()//セルの操作をやめる
    {
        cells = null;
        minoStuckFlag = false;
        count = 0;
    }
    
    bool IsStuck() { return minoStuckFlag; } //ミノが動かせなくなったかどうか(内部処理用)

    public bool RotateLeft() { return true; } //左回り(反時計回り)にまわす
    public bool RotateRight() { return true; }//右回り(時計回り)にまわす

    void SwitchCellToControl()
    {
        for (int y = 0; y < minoSizeY; y++)
            for (int x = 0; x < minoSizeX; x++)
                gameBoardS.SwitchCellLayer(0,cells[y, x]);
    }
    void SwitchCellToBoard()
    {
        for (int y = 0; y < minoSizeY; y++)
            for (int x = 0; x < minoSizeX; x++)
                gameBoardS.SwitchCellLayer(1,cells[y, x]);
    }

    void SwitchCellToControl(Vector3Int moveOffset)
    {
        for (int y = 0; y < minoSizeY; y++)
            for (int x = 0; x < minoSizeX; x++)
            {
                cells[y, x]=gameBoardS.SwitchCellLayerTo(0,cells[y, x], moveOffset);
            }
    }
    void SwitchCellToBoard(Vector3Int moveOffset)
    {
        for (int y = 0; y < minoSizeY; y++)
            for (int x = 0; x < minoSizeX; x++)
            {
                cells[y, x]=gameBoardS.SwitchCellLayerTo(1,cells[y, x], moveOffset);
            }
    }
    
    //コントロールレイヤーに置いたセルが表示レイヤーのセルとかぶらないか
    bool MoveAble()
    {
        for (int y = 0; y < minoSizeY; y++)
            for (int x = 0; x < minoSizeX; x++)
                if (!gameBoardS.IsEmpty(0,cells[y,x]))
                {
                    return false;
                }
        return true;
    }

    //格納している配列のマス[cell.y,cell.x]の座標に
    //オブジェクトがないときtrueを返す
    private bool IsNull(Vector2Int cell) { return cell.x == -1 || cell.y == -1; }

}

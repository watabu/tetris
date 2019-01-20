using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//ミノを入力で動かすクラス
//ミノクラスを登録ではなく、複数(2次元の配列)のセルを登録
//
public class MinoControllerScript : MonoBehaviour
{
    //入力で移動させるセル配列
    [SerializeField]
    Vector2Int[] cells=null;
    public InputControllerScript input;//入力クラスの参照
    public GameObject gameBoard;//ボードの参照
    [Range(0, 3)]
    public int playerNum;//どのプレイヤーのミノを操作しているか
    [Range(5,120)]
    public int fallSpeed;//ミノが落ちる速さ (何フレーム(60フレーム→１秒)に１回１マス落ちるか)
    private GameBoardScript gameBoardS;

    [SerializeField]
    float time;//
    bool minoStuckFlag;//ミノが止まったか

    public bool moveFlag_D=false;

    [SerializeField]
    UnityEvent OnMinoStuck;//ミノを動かせなくなったとき実行する関数を格納する変数

    private void Awake()
    {
        if (OnMinoStuck == null) OnMinoStuck = new UnityEvent(); //イベント・インスタンスの作成
        gameBoardS = gameBoard.GetComponent<GameBoardScript>();
    }

    // Use this for initialization
    void Start()
    {
        minoStuckFlag = false;
        time = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (moveFlag_D)
        {
            if (IsStuck())//ミノを動かせなくなったとき関数を実行する
            {
                OnMinoStuck.Invoke();
                return;
            }
            Vector3Int moveOffset = GetOffset();
            SwitchCellTo(GameBoardScript.BoardLayer.Default, GameBoardScript.BoardLayer.Controll, moveOffset);//セルをコントロールレイヤーに切り替え、moveOffset分移動させる
            if (IsSwitchAble())
            {
                SwitchCellTo(GameBoardScript.BoardLayer.Controll, GameBoardScript.BoardLayer.Default);//コントロールレイヤーに置いたセルを元に戻す
            }
            else
            {
                SwitchCellTo(GameBoardScript.BoardLayer.Controll, GameBoardScript.BoardLayer.Default, moveOffset * -1);//コントロールレイヤーに置いたセルを元に戻す
                minoStuckFlag = true;
                return;
            }
            time += Time.deltaTime;
            moveFlag_D = false;
        }
    }

    //複数のセルを動かせるようにする
    //5x5サイズの配列を渡されることを前提としているため、
    //回転の中心はcells[(minoSize-1)/2,(minoSize-1)/2]
    //とする
    public void RegisterCells(Vector2Int[] cells_) {
        Debug.Log("mino registered");
        cells = cells_;
        minoStuckFlag = false;
        time = 0;
    }
    public void RemoveCells()//セルの操作をやめる
    {
        cells = null;
        minoStuckFlag = false;
        time = 0;
    }

    Vector3Int GetOffset()
    {
        if ((int)time % fallSpeed == 0)//一定間隔でミノを下に落とす
        {
            return Vector3Int.down;
        }
        return input.GetInputDirection(playerNum);
    }
    
    bool IsStuck() { return minoStuckFlag; } //ミノが動かせなくなったかどうか(内部処理用)

    public bool RotateLeft() { return true; } //左回り(反時計回り)にまわす
    public bool RotateRight() { return true; }//右回り(時計回り)にまわす

    void SwitchCellTo(GameBoardScript.BoardLayer baselayer, GameBoardScript.BoardLayer destlayer)
    {
        for (int i = 0; i < cells.Length; i++)
                gameBoardS.SwitchCellLayer(baselayer, destlayer, cells[i]);
    }

    void SwitchCellTo(GameBoardScript.BoardLayer baselayer, GameBoardScript.BoardLayer destlayer, Vector3Int moveOffset)
    {
        for (int i = 0; i < cells.Length; i++)
            cells[i]=gameBoardS.SwitchCellLayerTo(baselayer, destlayer, cells[i], moveOffset);
    }
    
    //コントロールレイヤーに置いたセルが表示レイヤーのセルとかぶらないか
    bool IsSwitchAble()
    {
        for (int i = 0; i < cells.Length; i++)
            if (!gameBoardS.IsEmpty(0,cells[i]))
                {
                    return false;
                }
        return true;
    }

    //格納している配列のマス[cell.y,cell.x]の座標に
    //オブジェクトがないときtrueを返す
    private bool IsNull(Vector2Int cell) { return cell.x == -1 || cell.y == -1; }

    public Vector2Int[] GetControllCoods()
    {
        return cells;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

//ミノを入力で動かすクラス
//ミノクラスを登録ではなく、複数(1次元の配列)のセルを登録
//
public class MinoControllerScript : MonoBehaviour
{
    [Header("Object References")]
    public InputControllerScript input;//入力クラスの参照
    public GameObject gameBoard;//ボードの参照

    [Header("Control Status")]
    [Range(0, 3)]
    public int playerID;//どのプレイヤーのミノを操作しているか
    [Range(5, 120)]
    public int fallSpeed;//ミノが落ちる速さ (何フレーム(60フレーム→１秒)に１回１マス落ちるか)

    [Header("Call Back Function")]
    [SerializeField]
    UnityEvent OnMinoStuck;//ミノを動かせなくなったとき実行する関数を格納する変数

    //入力で移動させるセル配列
    [Header("Private Property")]
    public bool canMoveUp;
    [SerializeField]
    Vector3Int[,] cells = null;//動かすマスの座標の配列(正方形)
    [SerializeField]
    int cellSize;//配列の一辺の長さ
    [SerializeField]
    int count = 0;
    int stuckCount = 0;//他のミノに何フレームぶつかっているか

    private GameBoardScript gameBoardS;//ゲームボードのスクリプトの参照

    bool minoStuckFlag;//ミノが止まったか

    //Start()の前に実行する関数
    //他のクラスでも参照するオブジェクトはこの関数の中で生成させるべき
    private void Awake()
    {
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
        Vector3Int moveOffset = GetOffset();//今のフレームで動かすオフセット値を取得
        bool turnflag = moveOffset.z != 0;
        if (!turnflag)
        {
            SwitchCellTo(BoardLayer.Default, BoardLayer.Controll, moveOffset);//セルをコントロールレイヤーに切り替え、moveOffset分移動させる

            if (IsSwitchAble())//動かしたセルがボードのほかのセルに重ならないとき
            {
                SwitchCellTo(BoardLayer.Controll, BoardLayer.Default);//コントロールレイヤーに置いたセルを元に戻す
            }
            else
            {
                SwitchCellTo(BoardLayer.Controll, BoardLayer.Default, moveOffset*-1);//コントロールレイヤーに置いたセルを元に戻す
                if (moveOffset.x == 0) minoStuckFlag = true;
                return;
            }
        }
        else
        {
            SwitchCellTo(BoardLayer.Default, BoardLayer.Controll, moveOffset.z == 1);//セルをコントロールレイヤーに切り替え、moveOffset.zが1なら時計回りに回す
            if (IsSwitchAble())//動かしたセルがボードのほかのセルに重ならないとき
            {
                SwitchCellTo(BoardLayer.Controll, BoardLayer.Default);//コントロールレイヤーに置いたセルを元に戻す
            }
            else
            {
                SwitchCellTo(BoardLayer.Controll, BoardLayer.Default, GetInsideOffset());//コントロールレイヤーに置いたセルを元に戻す
                if (moveOffset.x == 0) minoStuckFlag = true;
                return;
            }
        }

        count++;
        //Debug.Break();
    }

    //複数のセルを動かせるようにする
    public void RegisterCells(Vector3Int[,] cells_)
    {
        Debug.Log("mino registered");
        int height = cells_.GetLength(0), width = cells_.GetLength(1);
        cellSize = Mathf.Max(height, width);//セルの配列のサイズを引数の配列の大きさに合わせる
        cells = new Vector3Int[cellSize, cellSize];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                cells[i, j] = cells_[i, j];
        minoStuckFlag = false;
        count = 0;
    }
    public void RemoveCells()//セルの操作をやめる
    {
        cells = null;
        minoStuckFlag = false;
        count = 0;
    }

    //そのフレーム時での移動させる相対座標を返す
    //回転させる場合はz座標を1か-1にする
    //  時計回り 1 反時計回り -1
    Vector3Int GetOffset()
    {
        if (count % fallSpeed ==0)//一定間隔でミノを下に落とす
        {
            return Vector3Int.down;
        }
        Vector3Int ans = input.GetInputDirection(playerID);
        int buttonSubmit = input.GetInputDown(playerID, PlInput.Key.KEY_SUBMIT);
        int buttonCancel = input.GetInputDown(playerID, PlInput.Key.KEY_CANCEL)*-1;
        if (ans.y > 0&&!canMoveUp) ans.y = 0;//上にはいけないようにする
        ans.z = buttonSubmit + buttonCancel;
        Debug.Log(ans);
        return ans;
    }
    
    bool IsStuck() { return minoStuckFlag; } //ミノが動かせなくなったかどうか(内部処理用)

    public bool RotateLeft() { return true; } //左回り(反時計回り)にまわす
    public bool RotateRight() { return true; }//右回り(時計回り)にまわす

    void SwitchCellTo(BoardLayer baselayer,BoardLayer destlayer)
    {
        SwitchCellTo(baselayer, destlayer, Vector3Int.zero);
    }

    void SwitchCellTo(BoardLayer baselayer, BoardLayer destlayer, Vector3Int moveOffset)
    {
        for (int i = 0; i < cellSize; i++)
            for (int j = 0; j < cellSize; j++)
                cells[i, j] = gameBoardS.SwitchCellLayerTo(baselayer, destlayer, cells[i, j], moveOffset);
    }

    void SwitchCellTo(BoardLayer baselayer, BoardLayer destlayer, bool rotatesClockwise)
    {
        Vector3Int center = GetCenter();
        if (rotatesClockwise)
        {
            for (int i = 0; i < cellSize; i++)
                for (int j = 0; j < cellSize; j++)
                {
                    if (cells[i,j] != new Vector3Int(-100, -100, -100))
                    {
                        Vector3Int cood = cells[i,j] - center;
                        cells[i,j] = gameBoardS.SwitchCellLayerTo(baselayer, destlayer, new Vector3Int(cells[i,j].x, cells[i,j].y, 0), new Vector3Int(-cood.x - cood.y, cood.x - cood.y, 0));
                    }
                }
        }
        else
        {
            for (int i = 0; i < cells.GetLength(0); i++)
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                if (cells[i,j] != new Vector3Int(-100, -100, -100))
                {
                    Vector3Int cood = cells[i,j] - center;
                    cells[i,j] = gameBoardS.SwitchCellLayerTo(baselayer, destlayer, new Vector3Int(cells[i,j].x, cells[i,j].y, 0), new Vector3Int(cood.y - cood.x, -cood.y - cood.x, 0));
                }
            }
        }
    }

    //コントロールレイヤーに置いたセルが表示レイヤーのセルとかぶらないか
    bool IsSwitchAble()
    {
        foreach(var cell in cells)
            if (!gameBoardS.IsEmpty(BoardLayer.Default, cell))
                {
                    return false;
                }
        return true;
    }
    //壁にめり込んでいるとき、ボードにもどるための移動量を返す
    //壁にめり込んでいないとき(0,0)を返す
    Vector3Int GetInsideOffset()
    {
        bool stuckRightFlag = false,stuckLeftFlag=false;
        Vector3Int ans = new Vector3Int();
        for (int y = 0; y < cellSize; y++) {
            for (int x = 0; x < cellSize / 2; x++)
            {
                if (!gameBoardS.IsEmpty(BoardLayer.Default, cells[y, x]))
                {
                    ans = new Vector3Int((x + 1), 0, 0);
                    stuckLeftFlag = true;
                    break;
                }
            }
            for (int x = cellSize / 2; x < cellSize; x++)
            {
                if (!gameBoardS.IsEmpty(BoardLayer.Default, cells[y, x]))
                {
                    ans= new Vector3Int(-(cellSize - x), 0, 0);
                    stuckRightFlag = true;
                    break;
                }
            }
        }
        if (stuckLeftFlag ^ stuckRightFlag) return ans;

        for (int x = 0; x < cellSize; x++)
        {
            for (int y = 0; y < cellSize; y++)
            {
                if (!gameBoardS.IsEmpty(BoardLayer.Default, cells[y, x]))
                {
                    return new Vector3Int(0, 1, 0);
                }
            }
        }
        /*        foreach (var cell in cells)
            if (gameBoardS.IsLeftWall(cell)) return new Vector3Int(gameBoardS.edgeCellCood[0].x-cell.x,0,0);
        foreach (var cell in cells)
            if (gameBoardS.IsRightWall(cell)) return new Vector3Int(gameBoardS.edgeCellCood[1].x - cell.x, 0,0);
        foreach (var cell in cells)
            if (gameBoardS.IsBottomWall(cell)) return new Vector3Int(0, gameBoardS.edgeCellCood[0].y - cell.y, 0);*/
        return Vector3Int.zero;
    }

    //ミノの中心となるマスの座標を返す
    Vector3Int GetCenter()
    {
        Vector3Int ans = Vector3Int.zero;
        foreach (var cell in cells)
        {
            if (cell != new Vector3Int(-100, -100, -100))//不正な値でなければ
                ans += cell;
        }
        ans.x = ans.x / 4 ; ans.y = ans.y / 4 ;
        Debug.Log(ans);
        return ans;
    }

    //格納している配列のマス[cell.y,cell.x]の座標に
    //オブジェクトがないときtrueを返す
    private bool IsNull(Vector3Int cell) { return cell.x == -1 || cell.y == -1; }

    private bool HitsLeftWall()
    {
        foreach(var cell in cells)
            if (gameBoardS.IsLeftWall(cell)) return true;
        return false;
    }
    private bool HitsRIghtWall()
    {
        foreach (var cell in cells)
            if (gameBoardS.IsRightWall(cell)) return true;
        return false;
    }
    private bool HitsBottomWall()
    {
        foreach (var cell in cells)
            if (gameBoardS.IsBottomWall(cell)) return true;
        return false;
    }

    public Vector3Int[,] GetControllCoods()
    {
        return cells;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

//ミノを入力で動かすクラス
//ミノクラスを登録ではなく、複数(2次元の配列)のセルを登録
//
//2/19   ミノの回転の挙動を改善　変に移動しないように
//2/20?  OnMinoRegisteredを追加
//2/22   OnMinoStuckに GameBoardModifier.CheckLine()を追加
//      GameBoardScript.GetNextMino()の前に実行するようにしてください
//       上ボタンを押したとき、下まで一気に移動するように
//       ミノが回転したときどの方向にうごかしても空白がないとき動かせないようにする予定
//3/24   ハードドロップ時ミノが止まるよう修正
//       ミノが止まるまでの猶予時間を実装
public class MinoControllerScript : MonoBehaviour
{
    [Header("Object References")]
    public InputControllerScript input;//入力クラスの参照
    public GameBoardScript gameBoard;
    public PlayerControllManager playerController;
    public HoldContainer holdContainer;
    public FallenMinoDrawer fallenMinoDrawer;//落ちる場所に半透明のミノを表示するクラスの参照

    [Header("Control Status")]
    [Range(0, 3)]
    public int playerID;//どのプレイヤーのミノを操作しているか
    [Range(5, 120)]
    public int fallSpeed;//ミノが落ちる速さ (何フレーム(60フレーム→１秒)に１回１マス落ちるか)
    public int minoFixFlame;//落下時ミノが固定されるまでの猶予フレーム
    public bool minoRevisedFlag;//ミノが回転したあとはまる場所に補正させられたか

    [Header("Call Back Function"), SerializeField]
    UnityEvent OnMinoStuck;//ミノを動かせなくなったとき実行する関数を格納する変数
    [SerializeField]
    UnityEvent OnMinoRegistered;//ミノが登録されたとき実行する関数を格納する変数

    [Header("Private Property"), SerializeField]
    bool canMoveUp;//上にも動かせるかのフラグ(デバッグ用)
    [SerializeField]
    Vector3Int[,] cells = null;//動かすマスの座標の配列(正方形)
    [SerializeField]
    int cellSize;//配列の一辺の長さ
    [SerializeField]
    int count = 0;
    [SerializeField]
    Vector3Int originCood;//ミノを格納する2次元配列の左下の座標
    [SerializeField]
    int stuckCount = 0;//他のミノに何フレームぶつかっているか
    GameObject mino ;

    private AudioSource moveSE;//動かしたとき鳴らすSE

    bool minoStuckFlag;//ミノが止まったか

    public Vector3Int GetOriginCood() { return originCood; }

    //Start()の前に実行する関数
    //他のクラスでも参照するオブジェクトはこの関数の中で生成させるべき
    private void Awake()
    {
        //イベント・インスタンスの作成
        if (OnMinoStuck == null) OnMinoStuck = new UnityEvent();
        if (OnMinoRegistered == null) OnMinoRegistered = new UnityEvent();
    }

    // Use this for initialization
    void Start()
    {
        minoStuckFlag = false;
        count = 0;
        moveSE = GetComponent<AudioSource>();
        if (moveSE == null)
        {
            Debug.LogWarning("Controller SE is Null!");
        }
    }

    public void Restart()
    {
        RemoveCells();
    }
    // Update is called once per frame
    void Update()
    {
        if (IsStuck())//ミノを動かせなくなったとき関数を実行する
        {
            Debug.Log("mino stuck");
            OnMinoStuck.Invoke();
            return;
        }
        int buttonHold = input.GetInputDown(playerID, PlInput.Key.KEY_HOLD);
        if (buttonHold != 0)
        {
            if (!holdContainer.HasMino())//もしホールドにミノが格納されてないとき
            {
                holdContainer.Register(mino);//
                EraceControllCells();//今操作しているミノを消す
                playerController.MinoUpdate();//新しくミノを登録する
            }
            else
            {
                GameObject mino_ = holdContainer.GetMino();
                holdContainer.Register(mino);
                EraceControllCells();//今操作しているミノを消す
                playerController.MinoUpdate(mino_);
            }
            return;
        }
        MoveMino();
        count++;
        //Debug.Break();
    }


    void MoveMino()
    {
        Vector3Int moveOffset = GetOffset();//今のフレームで動かすオフセット値を取得
        if (moveOffset.sqrMagnitude == 0)//動かさないとき
        {
            if (IsOnGround())
            {
                stuckCount++;//もし地面についているならカウントを開始
                if (stuckCount >= minoFixFlame) minoStuckFlag = true;//もし地面についたフレームがリミットを上回ればミノの操作をやめる
            }
            return;
        }
        stuckCount = 0;//ミノを動かしたので、地面についてるかのカウントをリセット

        if (count % fallSpeed != 0) moveSE.PlayOneShot(moveSE.clip);//自然落下でなければ動かす効果音をならす

        bool turnflag = moveOffset.z != 0;//z成分が0以外のとき回転させるようにする
        if (!turnflag)
        {
            SwitchCellTo(BoardLayer.Default, BoardLayer.Controll, moveOffset);//セルをコントロールレイヤーに切り替え、moveOffset分移動させる
        }
        else
        {
            RotateCellTo(BoardLayer.Default, BoardLayer.Controll, moveOffset.z == 1);//セルをコントロールレイヤーに切り替え、moveOffset.zが1なら時計回りに回す
        }

        if (IsSwitchAble())//動かしたセルがボードのほかのセルに重ならないとき
        {
            SwitchCellTo(BoardLayer.Controll, BoardLayer.Default);//コントロールレイヤーに置いたセルを元に戻す
            if (!turnflag) originCood += moveOffset;
        }
        else
        {
            if (!turnflag)
            {
                SwitchCellTo(BoardLayer.Controll, BoardLayer.Default, moveOffset * -1);//コントロールレイヤーに置いたセルを元に戻す
                //if (moveOffset.x == 0) minoStuckFlag = true;//床に置いて下ボタンを押したときミノが止まったことにしてる
            }
            else
            {
                Vector3Int insideOffset = GetInsideOffset(moveOffset);
                if (insideOffset.z != 0)//もし回転してもうまくおけなかった場合
                {
                    //逆回転して元に戻す(未実装)
                    return;
                }
                SwitchCellTo(BoardLayer.Controll, BoardLayer.Default, insideOffset);//コントロールレイヤーに置いたセルを元に戻す
                originCood += insideOffset;
            }
        }
    }

    //複数のセルを動かせるように登録する
    //mino_ ミノのクラス(Tスピンの判定に使う予定)
    //originCood_ 動かすミノの左下の座標
    public void RegisterCells(GameObject mino_, Vector3Int[,] cells_, Vector3Int originCood_)
    {
        Debug.Log("mino registered");
        int height = cells_.GetLength(0), width = cells_.GetLength(1);
        cellSize = Mathf.Max(height, width);//セルの配列のサイズを引数の配列より大きい正方形にする
        cells = new Vector3Int[cellSize, cellSize];
        for (int i = 0; i < cellSize; i++)
            for (int j = 0; j < cellSize; j++)
                cells[i, j] = (i < height && j < width) ? cells_[i, j] : GameBoardScript.nullCood;

        minoStuckFlag = false;
        minoRevisedFlag = false;
        originCood = originCood_;
        count = 0;
        mino = UsefulFunctions.CloneObject(mino_);
        OnMinoRegistered.Invoke();
    }
    public void RemoveCells()//セルの操作をやめる
    {
        Debug.Log("mino removed");
        cells = null;
        mino = null;
        minoStuckFlag = false;
        count = 0;
        originCood = Vector3Int.zero;
    }

    //そのフレーム時での移動させる相対座標を返す
    //回転させる場合はz座標を1か-1にする
    //時計回り 1 反時計回り -1
    Vector3Int GetOffset()
    {
        if (count % fallSpeed ==0&&!IsOnGround())//一定間隔でミノを下に落とす
        {
            return Vector3Int.down;
        }

        Vector3Int ans = input.GetInputDirection(playerID);
        int buttonSubmit = input.GetInputDown(playerID, PlInput.Key.KEY_SUBMIT);
        int buttonCancel = input.GetInputDown(playerID, PlInput.Key.KEY_CANCEL)*-1;
        if (ans.y > 0)
        {
            if (canMoveUp)
            {
                ans.y = 1;
            }
            else//canMoveUpがfalseのとき、上にはいけないようにする
            {
                ans.y = -fallenMinoDrawer.GetMinoStuckBelowCount();
                minoStuckFlag = true;//ハードドロップ時ミノ即時固定
            }
        }
        ans.z = buttonSubmit + buttonCancel;
        return ans;
    }
    
    bool IsStuck() { return minoStuckFlag; } //ミノが動かせなくなったかどうか(内部処理用)
    bool IsOnGround() { return fallenMinoDrawer.GetMinoStuckBelowCount() == 0; }//ミノが地面やほかのミノに乗っかっているとき

    //baseLayerにあるミノをdestLayerにmoveOffset分移動させる
    void SwitchCellTo(BoardLayer baselayer, BoardLayer destlayer) { SwitchCellTo(baselayer, destlayer, Vector3Int.zero); }
    void SwitchCellTo(BoardLayer baselayer, BoardLayer destlayer, Vector3Int moveOffset)
    {
        for (int i = 0; i < cellSize; i++)
            for (int j = 0; j < cellSize; j++)
                cells[i, j] = gameBoard.SwitchCellLayerTo(baselayer, destlayer, cells[i, j], moveOffset);
    }

    void RotateCellTo(BoardLayer baselayer, BoardLayer destlayer, bool rotatesClockwise)
    {
        if (rotatesClockwise)
            RotateCellClockwiseTo(baselayer, destlayer);
        else
            RotateCellAnticlockwiseTo(baselayer, destlayer);
    }

    //ミノを時計回りに動かす
    void RotateCellClockwiseTo(BoardLayer baselayer, BoardLayer destlayer)
    {
        float yCenter = (float)originCood.y + ((float)cellSize) / 2f - 0.5f;
        for (int i = 0; i < cells.GetLength(0); i++)
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (!IsNull(cells[i, j]))
                {
                    Vector3Int dest = cells[i, j] + new Vector3Int(0, (int)((yCenter - (float)cells[i, j].y) * 2f), 0);
                    dest -= originCood;
                    int temp = dest.x;
                    dest.x = dest.y;
                    dest.y = temp;
                    dest += originCood;
                    cells[i, j] = gameBoard.SwitchCellLayerTo(baselayer, destlayer, cells[i, j], dest - cells[i, j]);
                }
            }
    }
    //ミノを反時計回りに動かす
    void RotateCellAnticlockwiseTo(BoardLayer baselayer, BoardLayer destlayer)
    {
        float yCenter = (float)originCood.y + ((float)cellSize) / 2f - 0.5f;
        for (int i = 0; i < cells.GetLength(0); i++)
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (!IsNull(cells[i, j]))
                {
                    Vector3Int dest = cells[i, j] - originCood;
                    int temp = dest.x;
                    dest.x = dest.y;
                    dest.y = temp;
                    dest += originCood;
                    dest += new Vector3Int(0, (int)((yCenter - (float)dest.y) * 2f), 0);
                    cells[i, j] = gameBoard.SwitchCellLayerTo(baselayer, destlayer, cells[i, j], dest - cells[i, j]);
                }
            }
    }

    //コントロールレイヤーに置いたセルが表示レイヤーのセルとかぶらないか
    bool IsSwitchAble()
    {
        if (cells == null)
        {
            Debug.LogAssertion("controll cells is null");
            return false;
        }
        foreach(var cell in cells)
            if (!gameBoard.IsEmpty(BoardLayer.Default, cell)|| !gameBoard.IsEmpty(BoardLayer.Wall, cell))
                    return false;
        return true;
    }
    //壁にめり込んでいるとき、ボードにもどるための移動量を返す
    //壁にめり込んでいないとき(0,0,0)を返す
    //y=-1,x=-1から順番にチェックしていくため、左下に移動することが一番優先的
    //もしどの方向にも動けないようなら回転しないようにする->まだ
    Vector3Int GetInsideOffset(Vector3Int moveOffset)
    {
        minoRevisedFlag = false;
        for (int y = -1; y <= 1; y++)
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue;
                int count = 0;
                foreach (var cell in cells)
                {

                    if (IsNull(cell)) continue;
                    if (gameBoard.IsEmpty(BoardLayer.Default, cell + new Vector3Int(x, y, 0)) &&
                         gameBoard.IsEmpty(BoardLayer.Wall, cell + new Vector3Int(x, y, 0))) count++;
                }
                if (count == 4)
                {
                    minoRevisedFlag = true;//空きスペースに移動したので修正されたフラグを立てる
                    return new Vector3Int(x, y, 0);
                }
            }
        return new Vector3Int(0, 0, moveOffset.z * -1);
    }

    void EraceControllCells()
    {
        foreach (var cell in cells)
            if (!IsNull(cell))
                gameBoard.SetCell(BoardLayer.Default, null, cell.x, cell.y);
    }

    //格納しているセルの座標が不正な値の時trueを返す
    private bool IsNull(Vector3Int cell) { return cell == GameBoardScript.nullCood; }

    //動かすマスの座標のリストを返す
    public Vector3Int[,] GetControllCoods() { return cells; }
    //動かすミノを返す
    public GameObject GetMino() { return mino; }
}


/*
//ミノをx軸に平行な軸でひっくり返す
 void FlipCellXTo(BoardLayer baselayer, BoardLayer destlayer)
{
    float yCenter = (float)originCood.y+ ((float)cellSize) / 2f - 0.5f;

    for (int i = 0; i < cells.GetLength(0); i++)
        for (int j = 0; j < cells.GetLength(1); j++)
        {
            if (!IsNull(cells[i, j]))
            {
                cells[i, j] = gameBoardS.SwitchCellLayerTo(baselayer, destlayer, cells[i, j], new Vector3Int(0, (int)((yCenter - (float)cells[i, j].y) * 2f), 0));
            }
        }
}
//ミノを右斜め下の対角線にそってひっくり返す
void FlipCellDiagTo(BoardLayer baselayer, BoardLayer destlayer)
{
    for (int i = 0; i < cells.GetLength(0); i++)
        for (int j = 0; j < cells.GetLength(1); j++)
        {
            if (!IsNull(cells[i,j]))
            {
                Vector3Int dest = cells[i, j] - originCood;
                int temp = dest.x;
                dest.x = dest.y;
                dest.y = temp;
                dest += originCood;
                cells[i, j] = gameBoardS.SwitchCellLayerTo(baselayer, destlayer, cells[i, j], dest - cells[i, j]);
            }
        }
}*/

/* //ミノの中心となるマスの座標を返す
 Vector3Int GetCenter()
 {
     Vector3Int ans = Vector3Int.zero;
     foreach (var cell in cells)
     {
         if (!IsNull(cell))//不正な値でなければ
             ans += cell;
     }
     ans.x = ans.x / 4 ; ans.y = ans.y / 4 ;
     Debug.Log(ans);
     return ans;
 }*/

/* 
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

 //もし左右どちらも埋まっていれば
 foreach(var cell in cells)
     if (!gameBoardS.IsEmpty(BoardLayer.Default, cell))
     {
         return new Vector3Int(0, 1, 0);
     }
     */

/* private bool HitsLeftWall()
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
 }*/

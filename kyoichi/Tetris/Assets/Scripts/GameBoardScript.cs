using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GameBoardScript : MonoBehaviour
{
    GameBoardUpdateScript gameBoard;

    public GameObject cellController;//ミノを動かすクラスの参照
    public Vector2Int generateCood;//ミノをどこのマスに出現させるか
    public Vector2Int[] gameoverCells;//どこのマスが埋まったらゲームオーバーになるか

    public GameObject nextMinoContainer;//次のミノを保持しているクラス

    bool activeFlag;//プレイヤーがミノを動かせるかどうかのフラグ
    float cellSize;//セル１つの大きさ

    int minoNum;//ミノの

    Tilemap tilemap;

    [SerializeField] UnityEvent OnMinoFilled;//ミノが上まで積まれたとき実行する関数を格納する変数
    [SerializeField] bool minoFilledFlag;

    void Awake()
    {
        minoNum = 4;
        cellController.SetActive(false);//コントローラーを無効化
        if (OnMinoFilled == null) OnMinoFilled = new UnityEvent(); //イベント・インスタンスの作成
        tilemap=transform.Find("Grid/BoardCell").GetComponent<Tilemap>();
    }

    // Use this for initialization
    void Start()
    {
        gameBoard = GetComponent<GameBoardUpdateScript>();
        activeFlag = false;
        //cellSize = gameBoard.cellSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeFlag)
        {
            //CheckLine();
        }
        if (IsFilled())
        {
            OnMinoFilled.Invoke();
        }
    }
    public void Stop()//ゲーム盤を動かせないようにする
    {
        cellController.SetActive(false);//コントローラーを無効化
        activeFlag = false;
    }
    public void Resume()//ゲーム盤を動かせるようにする
    {
        cellController.SetActive(true);
        activeFlag = true;
    }

    public void ClearCell()//ゲーム盤のミノを全消去する
    {
    }
    public void ClearCell(Vector2Int cood) { ClearCell(cood.y, cood.x); }
    public void ClearCell(int x,int y) {  }
    //次のミノをNextを保持するクラスから取得する
    //ミノは取得したあと関数内でコントロールクラスに渡される
    public void GetNextMino()
    {
        if (minoFilledFlag) return;//もしゲーム盤が上まで埋まってたら渡さない
        GameObject mino = nextMinoContainer.GetComponent<NextMinoContainer>().GetNextMino();//次のミノのコンテナからミノを取得
        bool[,] cells = mino.GetComponent<MinoScript>().GetShape();//ミノのデータからセルを生成する
        Tile tile = mino.GetComponent<MinoScript>().GetCell();
        Vector2Int[,] cellscood = new Vector2Int[minoNum, minoNum];//コントローラに渡すセルの座標
        for (int y = 0; y < minoNum; y++)
            for (int x = 0; x < minoNum; x++)
                if (cells[y, x])//ミノから生成したセルが(x,y)のマスで存在するとき
                {
                    if (IsEmpty(generateCood.x + x, generateCood.y + y))//生成するマスでミノが存在しないとき
                    {
                        ReDefineCell(tile, generateCood.x + x, generateCood.y + y);
                        cellscood[y, x] = new Vector2Int(generateCood.x + x, generateCood.y + y);
                    }
                    else
                    {
                        minoFilledFlag = true;
                        return;
                    }
                }
                else
                {
                    cellscood[y, x] = new Vector2Int(-1, -1);//そのマスにオブジェクトがない
                }
        cellController.GetComponent<MinoControllerScript>().RegisterCells(cellscood);//どの座標のセルを移動させるかの情報を渡す
    }

    void CheckLine()//ミノが一列すべてうまったかどうか(内部処理用)
    {
        int cellYCount = 0;
        for (int y = 0; y < gameBoard.height; y++)
        {
            for (int x = 0; x < gameBoard.width; x++)
                if (IsEmpty(x, y))//もし空白があったら
                {
                    if (cellYCount == 0) return;
                    for (int y2 = 0; y2 < cellYCount; y++)
                        for (int x2 = 0; x2 < gameBoard.width; x++)
                            ClearCell(x2, y2);//それまでの列のセルを消去
                    for (int y2 = cellYCount; y2 < gameBoard.height; y++)
                        for (int x2 = 0; x2 < gameBoard.width; x++)
                            MoveCell(x2, y2, 0, -cellYCount);//上のセルを下に移動
                    return;
                }
            cellYCount++;
        }
    }

    public Vector2Int MoveCell(Vector2Int cell, Vector2Int offset)             { return MoveCell  (cell.x, cell.y, offset.x, offset.y); }
    public Vector2Int MoveCell(Vector2Int cell, int offsetX, int offsetY)      { return MoveCell  (cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCell(int cellX, int cellY, int offsetX, int offsetY) { return MoveCellTo(cellX, cellY, cellX + offsetX, cellY + offsetY); }

    public Vector2Int MoveCellTo(Vector2Int cell, Vector2Int destination)      { return MoveCellTo(cell.x, cell.y, destination.x, destination.y); }
    public Vector2Int MoveCellTo(Vector2Int cell, int offsetX, int offsetY)    { return MoveCellTo(cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCellTo(int cellX, int cellY, int destinationX, int destinationY)
    {
        if (IsEmpty(cellX, cellY) || !IsEmpty(destinationX, destinationY))
            return new Vector2Int(cellX, cellY);
        if (tilemap != null)
        {
            ReDefineCell(tilemap.GetTile(new Vector3Int(cellX, cellY,0)), destinationX, destinationY);
            tilemap.SetTile(new Vector3Int(cellX, cellY, 0), null);//移動元のセルを削除
            return new Vector2Int(destinationX, destinationY);
        }
        return new Vector2Int(cellX, cellY);
    }
    
    private void ReDefineCell(TileBase cell, int destinationX, int destinationY)
    {
        if (tilemap != null)
        {
            tilemap.SetTile(new Vector3Int(destinationX, destinationY, 0), cell);
        }
    }

    //マスにミノがあるときtrue、ないときfalseを返す
    public bool IsEmpty(int cellX, int cellY) {
        return !tilemap.HasTile(new Vector3Int(cellX, cellY, 0));
    }
    public bool IsFilled() { return minoFilledFlag; }

}

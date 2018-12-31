using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameBoardScript : MonoBehaviour
{
    GameBoardUpdateScript gameBoard;
    GameObject[,] cellList; //座標は cellList[y,x]　のように参照(y,xの順にするメリットなし？要検討)

    public GameObject cellController;//ミノを動かすクラスの参照
    public Vector2Int generateCood;//ミノをどこのマスに出現させるか
    public Vector2Int[] gameoverCells;//どこのマスが埋まったらゲームオーバーになるか

    public GameObject nextMinoContainer;//次のミノを保持しているクラス

    bool activeFlag;//プレイヤーがミノを動かせるかどうかのフラグ
    float cellSize;//セル１つの大きさ

    int minoNum;//ミノの

    [SerializeField] UnityEvent OnMinoFilled;//ミノが上まで積まれたとき実行する関数を格納する変数
    bool minoFilledFlag;

    void Awake()
    {
        minoNum = 4;
        cellController.SetActive(false);//コントローラーを無効化
        if (OnMinoFilled == null) OnMinoFilled = new UnityEvent(); //イベント・インスタンスの作成
    }

    // Use this for initialization
    void Start()
    {
        gameBoard = GetComponent<GameBoardUpdateScript>();
        activeFlag = false;
        cellList = new GameObject[gameBoard.height, gameBoard.width];
        cellSize = gameBoard.cellSize ;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeFlag)
        {
            CheckLine();
        }
        if (isFilled())
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
        for (int y = 0; y < gameBoard.height; y++)
            for (int x = 0; x < gameBoard.width; x++)
                Destroy(cellList[y, x]);
    }

    //次のミノをNextを保持するクラスから取得する
    //ミノは取得したあと関数内でコントロールクラスに渡される
    public void GetNextMino()
    {
        if (minoFilledFlag) return;//もしゲーム盤が上まで埋まってたら渡さない
        GameObject mino = nextMinoContainer.GetComponent<NextMinoContainer>().GetNextMino();
        GameObject[,] cells = mino.GetComponent<MinoScript>().GetCells();//ミノのデータからセルを生成する
        Vector2Int[,] cellscood = new Vector2Int[minoNum, minoNum];//コントローラに渡すセルの座標
        for (int y = 0; y < minoNum; y++)
            for (int x = 0; x < minoNum; x++)
                if (cells[y, x] != null)//ミノから生成したセルが(x,y)のマスで存在するとき
                {
                    if (isEmpty(generateCood.x + x, generateCood.y + y))//生成するマスでミノが存在しないとき
                    {
                        ReDefineCell(cells[y, x], generateCood.x + x, generateCood.y + y);
                        cellscood[y,x] = new Vector2Int(generateCood.x + x, generateCood.y + y);
                    }
                    else
                    {
                        foreach(GameObject cls in cells)
                            Destroy(cls);
                        minoFilledFlag = true;
                        return;
                    }
                }
                else
                {
                    cellscood[y,x] = new Vector2Int(-1, -1);//そのマスにオブジェクトがない
                }
        cellController.GetComponent<MinoControllerScript>().RegisterCells(cellscood);
    }

    void CheckLine()//ミノが一列すべてうまったかどうか(内部処理用)
    {
        int cellYCount = 0;
        for (int y = 0; y < gameBoard.height; y++)
        {
            int cellXCount = 0;
            for (int x = 0; x < gameBoard.width; x++)
                if (!isEmpty(x,y)) cellXCount++;

            if (cellXCount == gameBoard.width)//一列すべてうまっているとき
            {
                for (int x = 0; x < gameBoard.width; x++)
                    Destroy(cellList[y, x]);
                cellYCount++;
            }
        }
        if (cellYCount > 0)
        {
            for (int y = cellYCount; y < gameBoard.height; y++)
                for (int x = 0; x < gameBoard.width; x++)
                    MoveCell(x, y, 0, -cellYCount);
        }
    }

    public Vector2Int MoveCell(Vector2Int cell, Vector2Int offset)             { return MoveCell  (cell.x, cell.y, offset.x, offset.y); }
    public Vector2Int MoveCell(Vector2Int cell, int offsetX, int offsetY)      { return MoveCell  (cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCell(int cellX, int cellY, int offsetX, int offsetY) { return MoveCellTo(cellX, cellY, cellX + offsetX, cellY + offsetY); }

    public Vector2Int MoveCellTo(Vector2Int cell, Vector2Int destination)      { return MoveCellTo(cell.x, cell.y, destination.x, destination.y); }
    public Vector2Int MoveCellTo(Vector2Int cell, int offsetX, int offsetY)    { return MoveCellTo(cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCellTo(int cellX, int cellY, int destinationX, int destinationY)
    {
        if (isOutLange(cellX, cellY) || isOutLange(destinationX, destinationY)||
            isEmpty(cellX, cellY) || !isEmpty(destinationX, destinationY))
            return new Vector2Int(cellX, cellY);
        ReDefineCell(cellList[cellY, cellX], destinationX, destinationY);
        cellList[cellY, cellX] = null;
        return new Vector2Int(destinationX, destinationY);
    }

    //[destinationY,destinationX]のマスにセルオブジェクトcellを設定する
    private void ReDefineCell(GameObject cell, int destinationX, int destinationY)
    {
        cellList[destinationY, destinationX] = cell;
        cellList[destinationY, destinationX].transform.parent = transform;
        cellList[destinationY, destinationX].transform.position = new Vector2(cellSize * destinationX, cellSize * destinationY);
        cellList[destinationY, destinationX].transform.position += transform.position;
    }

    //マスにミノがあるときtrue、ないときfalseを返す
    public bool isEmpty(int cellX, int cellY) { return cellList[cellY, cellX] == null; }
    //マスの範囲外の座標のとき、falseを返す
    public bool isOutLange(int X, int Y) { return X < 0 || X >= gameBoard.width || Y < 0 || Y >= gameBoard.height; }

    public bool isFilled() { return minoFilledFlag; }

}

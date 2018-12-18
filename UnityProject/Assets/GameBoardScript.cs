using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameBoardScript : MonoBehaviour
{

    GameObject[,] cellList; //座標は cellList[y,x]　のように参照(y,xの順にするメリットなし？要検討)

    public GameObject cellController;//ミノを動かすクラスの参照
    public int boardWidth;
    public int boardHeight;
    public float cellPadding;//セルごとの余白
    public GameObject cellPrefab;//ゲーム盤のマスのPrefab
    public Vector2Int generateCood;//ミノをどこのマスに出現させるか
    public Vector2Int[] gameoverCells;//どこのマスが埋まったらゲームオーバーになるか

    public GameObject nextMinoContainer;//次のミノを保持しているクラス

    bool activeFlag;//プレイヤーがミノを動かせるかどうかのフラグ
    float cellSize;//セル１つの大きさ

    [SerializeField] UnityEvent OnMinoFilled;//ミノが上まで積まれたとき実行する関数を格納する変数
    bool minoFilledFlag;

    void Awake()
    {
        cellController.SetActive(false);//コントローラーを無効化
        if (OnMinoFilled == null) OnMinoFilled = new UnityEvent(); //イベント・インスタンスの作成
    }

    // Use this for initialization
    void Start()
    {
        activeFlag = false;
        cellList = new GameObject[boardHeight, boardWidth];
        cellSize = cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        //マスのスプライトを生成
        if (boardHeight == 0 || boardWidth == 0)
            Debug.Log("マスの高さまたは幅が0のため、マスが生成されません！");
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                GameObject cell = Instantiate(
                    cellPrefab,
                    gameObject.transform.position + new Vector3((cellPadding + cellSize) * x, (cellPadding + cellSize) * y, 0.0f),
                    Quaternion.identity
                    );
                cell.gameObject.transform.parent = gameObject.transform;//セルの座標を親(ゲーム盤)とリンクさせる
            }
        }
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
        for (int y = 0; y < boardHeight; y++)
            for (int x = 0; x < boardWidth; x++)
                Destroy(cellList[y, x]);
    }

    //次のミノをNextを保持するクラスから取得する
    //ミノは取得したあと関数内でコントロールクラスに渡される
    public void GetNextMino()
    {
        if (minoFilledFlag) return;
        GameObject mino = nextMinoContainer.GetComponent<NextMinoContainer>().GetNextMino();
        GameObject[,] cells = mino.GetComponent<MinoScript>().GetCells();//ミノのデータからセルを生成する
        Vector2Int[,] cellscood = new Vector2Int[5, 5];//コントローラに渡すセルの座標
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                if (cells[y, x] != null)//ミノから生成したセルが(x,y)のマスで存在するとき
                {
                    if (cellList[generateCood.y + y, generateCood.x + x] == null)//生成するマスでミノが存在しないとき
                    {
                        cells[y, x].transform.parent = transform;
                        cells[y, x].transform.position = new Vector2(cellSize * (generateCood.x + x), cellSize * (generateCood.y + y));
                        cells[y, x].transform.position += transform.position;
                        cellList[generateCood.y + y, generateCood.x + x] = cells[y, x];
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
                    cellscood[y, x] = new Vector2Int(-1, -1);// null を表現する値
                }
        cellController.GetComponent<MinoControllerScript>().RegisterCells(cellscood);
    }

    void CheckLine()//ミノが一列すべてうまったかどうか(内部処理用)
    {
        int cellYCount = 0;
        for (int y = 0; y < boardHeight; y++)
        {
            int cellXCount = 0;
            for (int x = 0; x < boardWidth; x++)
                if (cellList[y, x] != null) cellXCount++;

            if (cellXCount == boardWidth)//一列すべてうまっているとき
            {
                for (int x = 0; x < boardWidth; x++)
                    Destroy(cellList[y, x]);
                cellYCount++;
            }
        }
        if (cellYCount > 0)
        {
            for (int y = cellYCount; y < boardHeight; y++)
                for (int x = 0; x < boardWidth; x++)
                    MoveCell(x, y, 0, -cellYCount);
        }
    }

    public Vector2Int MoveCell(Vector2Int cell, Vector2Int offset) { return MoveCell(cell.x, cell.y, offset.x, offset.y); }
    public Vector2Int MoveCell(Vector2Int cell, int offsetX, int offsetY) { return MoveCell(cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCell(int cellX, int cellY, int offsetX, int offsetY) { return MoveCellTo(cellX, cellY, cellX + offsetX, cellY + offsetY); }

    public Vector2Int MoveCellTo(Vector2Int cell, Vector2Int destination) { return MoveCellTo(cell.x, cell.y, destination.x, destination.y); }
    public Vector2Int MoveCellTo(Vector2Int cell, int offsetX, int offsetY) { return MoveCellTo(cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCellTo(int cellX, int cellY, int destinationX, int destinationY)
    {
        if (isOutLange(cellX, cellY) || isOutLange(destinationX, destinationY)||
            isEmpty(cellX, cellY) || !isEmpty(destinationX, destinationY))
            return new Vector2Int(cellX, cellY);
        cellList[destinationY, destinationX] = cellList[cellY, cellX];
        cellList[destinationY, destinationX].transform.position = new Vector2(cellSize * destinationX, cellSize * destinationY);
        cellList[destinationY, destinationX].transform.position += transform.position;
        cellList[cellY, cellX] = null;
        return new Vector2Int(destinationX, destinationY);
    }
    //マスにミノがあるときtrue、ないときfalseを返す
    public bool isEmpty(int cellX, int cellY) { return cellList[cellY, cellX] == null; }
    //マスの範囲外の座標のとき、falseを返す
    private bool isOutLange(int X, int Y) { return X < 0 || X >= boardWidth || Y < 0 || Y >= boardHeight; }

    public bool isFilled() { return minoFilledFlag; }

}

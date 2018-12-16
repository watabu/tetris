using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardScript : MonoBehaviour {

    GameObject[,] cellList; //座標は cellList[y,x]　のように参照(y,xの順にするメリットなし？要検討)

    public GameObject cellController;//ミノを動かすクラスの参照
    public int boardWidth;
    public int boardHeight;
    public GameObject cellPrefab;//ゲーム盤のマスのPrefab
    public float cellPadding;//セルごとの余白
    public Vector2Int generateCood;//ミノをどこのマスに出現させるか
    public Vector2Int[] gameoverCells;//どこのマスが埋まったらゲームオーバーになるか

    bool activeFlag;//プレイヤーがミノを動かせるかどうかのフラグ

    // Use this for initialization
    void Start () {
        activeFlag = true;
        cellList = new GameObject[boardHeight, boardWidth];
        float cellSize = cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        //マスのスプライトを生成
        if (boardHeight == 0 || boardWidth == 0)
            Debug.Log("マスの高さまたは幅が0のため、マスが生成されません！");
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                GameObject cell = Instantiate(
                    cellPrefab,
                    new Vector3((cellPadding + cellSize) * x, (cellPadding + cellSize) * y, 0.0f),
                    Quaternion.identity
                    );
                cell.gameObject.transform.parent = gameObject.transform;//セルの座標を親(ゲーム盤)とリンクさせる
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (activeFlag)
        {
            CheckLine();
        }
	}

    public GameObject RegisterMino(GameObject mino)//ミノをゲーム盤に登録する
    {
        cellList[generateCood.y,generateCood.x]=mino;
        //cellController.GetComponent<MinoControllerScript>().RegisterMino(mino);
        return mino;
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
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Destroy(cellList[y, x]);
            }
        }

    }

    void CheckLine()//ミノが一列すべてうまったかどうか(内部処理用)
    {
        int cellYCount=0;
        for (int y = 0; y < boardHeight; y++)
        {
            int cellXCount = 0;
            for (int x = 0; x < boardWidth; x++)
            {
                if (cellList[y, x] != null) cellXCount++;
            }

            if (cellXCount == boardWidth)//一列すべてうまっているとき
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    Destroy(cellList[y,x]);
                }
                cellYCount++;
            }
        }
        if (cellYCount > 0)
        {
            for (int y = cellYCount; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    MoveCell(x, y, 0, -cellYCount);
                }
            }
        }
    }

    public Vector2Int MoveCell(Vector2Int cell, Vector2Int offset)
    {
        return MoveCell(cell.x ,cell.y,offset.x,offset.y);
    }
    public Vector2Int MoveCell(Vector2Int cell, int offsetX, int offsetY)
    {
        return MoveCell(cell.x, cell.y, offsetX, offsetY);
    }
    public Vector2Int MoveCell(int cellX, int cellY, int offsetX, int offsetY)
    {
        if (isEmpty(cellX, cellY) || isEmpty(cellX + offsetX, cellY + offsetY))
            return new Vector2Int(cellX, cellY);
        cellList[cellY + offsetY, cellX + offsetX] = cellList[cellY, cellX];
        return new Vector2Int(cellX + offsetX, cellY + offsetY);
    }

    public Vector2Int MoveCellTo(Vector2Int cell, Vector2Int destination)
    {
        return MoveCellTo(cell.x, cell.y, destination.x, destination.y);
    }
    public Vector2Int MoveCellTo(Vector2Int cell, int offsetX, int offsetY)
    {
        return MoveCellTo(cell.x, cell.y, offsetX, offsetY);
    }
    public Vector2Int MoveCellTo(int cellX, int cellY, int destinationX, int destinationY)
    {
        if (isEmpty(cellX, cellY) || isEmpty(destinationX, destinationY))
            return new Vector2Int(cellX, cellY);
        cellList[destinationY, destinationX] = cellList[cellY, cellX];
        return new Vector2Int(destinationX, destinationY);
    }

    public bool isEmpty(int cellX, int cellY)//マスにミノがあるときtrue、ないときfalseを返す
    {
        return cellList[cellY, cellX] == null;
    }

}

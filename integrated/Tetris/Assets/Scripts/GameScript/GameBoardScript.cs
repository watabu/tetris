using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

//ゲームボードを制御するクラス
//セルの表示レイヤーとプログラムの制御用レイヤーがある
//例えばセルを移動させるとき、いったん制御用レイヤーに全部移動させて
//制御用レイヤーにあるセルが表示レイヤーにあるセルと重ならなかったら表示レイヤーに戻す処理を行う
public class GameBoardScript : MonoBehaviour
{

    public enum BoardLayer
    {
        Default,
        Controll,
        Back
    }

    public GameObject cellController;//ミノを動かすクラスの参照
    [SerializeField] Vector3Int generateCood;//ミノをどこのマスに出現させるか

    public GameObject nextMinoContainer;//次のミノを保持しているクラス

    bool activeFlag;//プレイヤーがミノを動かせるかどうかのフラグ
    
    public Tilemap tilemap;
    public Tilemap controlTile;
    [SerializeField] UnityEvent OnMinoFilled;//ミノが上まで積まれたとき実行する関数を格納する変数
    [SerializeField] bool minoFilledFlag;

    

    void Awake()
    {
        cellController.SetActive(false);//コントローラーを無効化
        if (OnMinoFilled == null) OnMinoFilled = new UnityEvent(); //イベント・インスタンスの作成
    }

    // Use this for initialization
    void Start()
    {
        //生成させる座標を計算
        var generateCell = transform.Find("Grid/MinoGenerateCell");
        if (generateCell != null)
            generateCood = tilemap.WorldToCell(generateCell.position);
        activeFlag = false;
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
    public void ClearCell(int x,int y) { tilemap.SetTile(new Vector3Int(x, y, 0), null);  }
    //次のミノをNextを保持するクラスから取得する
    //ミノは取得したあと関数内でコントロールクラスに渡される
    public void GetNextMino()
    {
        if (minoFilledFlag) return;//もしゲーム盤が上まで埋まってたら渡さない
        GameObject mino = nextMinoContainer.GetComponent<NextMinoContainer>().GetNextMino();//次のミノのコンテナからミノを取得
        bool[,] cells = mino.GetComponent<MinoScript>().GetShape();//ミノのデータからセルを生成する
        int minoLengthY = cells.GetLength(0);
        int minoLengthX = cells.GetLength(1);
        Tile tile = mino.GetComponent<MinoScript>().GetCell();
        Vector2Int[] cellscood = new Vector2Int[minoLengthY * minoLengthX];//コントローラに渡すセルの座標
        for (int y = 0; y < minoLengthY; y++)
            for (int x = 0; x < minoLengthX; x++)
                if (cells[y, x])//ミノから生成したセルが(x,y)のマスで存在するとき
                {
                    if (IsEmpty(0, generateCood.x + x, generateCood.y + y))//生成するマスでミノが存在しないとき
                    {
                        ReDefineCell(tilemap, tile, generateCood.x + x, generateCood.y + y);
                        cellscood[x + y * minoLengthX] = new Vector2Int(generateCood.x + x, generateCood.y + y);
                    }
                    else
                    {
                        minoFilledFlag = true;
                        return;
                    }
                }
                else
                {
                    cellscood[x + y * minoLengthY] = new Vector2Int(-100, -100);//そのマスにオブジェクトがない
                }
        cellController.GetComponent<MinoControllerScript>().RegisterCells(cellscood);//どの座標のセルを移動させるかの情報を渡す
    }

    void CheckLine()//ミノが一列すべてうまったかどうか(内部処理用)
    {
        /*int cellYCount = 0;
        for (int y = 0; y < gameBoard.height; y++)
        {
            for (int x = 0; x < gameBoard.width; x++)
                if (IsEmpty(0,x, y))//もし空白があったら
                {
                    if (cellYCount == 0) return;
                    for (int y2 = 0; y2 < cellYCount; y++)
                        for (int x2 = 0; x2 < gameBoard.width; x++)
                            ClearCell(x2, y2);//それまでの列のセルを消去
                    for (int y2 = cellYCount; y2 < gameBoard.height; y++)
                        for (int x2 = 0; x2 < gameBoard.width; x++)
                            MoveCell(0,x2, y2, 0, -cellYCount);//上のセルを下に移動
                    return;
                }
            cellYCount++;
        }*/
    }

    public void SwitchCellLayer(BoardLayer baselayer, BoardLayer destlayer, Vector2Int cell)
    {
        Tilemap basetile = GetLayer(baselayer), destTile = GetLayer(destlayer);
        Vector3Int cood = new Vector3Int(cell.x, cell.y, 0);
        TileBase prevCell = basetile.GetTile(cood);
        if (prevCell != null)
        {
            destTile.SetTile(cood, prevCell);//移動元のセルを削除
            basetile.SetTile(cood, null);//移動元のセルを削除
        }
    }
    //layerの座標cellにあるセルのレイヤーを変える
    public Vector2Int SwitchCellLayerTo(BoardLayer baselayer, BoardLayer destlayer, Vector2Int cell, Vector3Int offset)
    {
        Tilemap basetile = GetLayer(baselayer), destTile = GetLayer(destlayer);
        Vector3Int cood = new Vector3Int(cell.x, cell.y, 0);
        TileBase prevCell = basetile.GetTile(cood);
        if (prevCell != null)
        {
            destTile.SetTile(cood + offset, prevCell);//移動元のセルを削除
            basetile.SetTile(cood, null);//移動元のセルを削除
            return cell + new Vector2Int(offset.x,offset.y);
        }
        return cell;
    }

    public Vector2Int MoveCell(BoardLayer layer,Vector2Int cell, Vector2Int offset)             { return MoveCell  (layer,cell.x, cell.y, offset.x, offset.y); }
    public Vector2Int MoveCell(BoardLayer layer, Vector2Int cell, int offsetX, int offsetY)      { return MoveCell  (layer, cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCell(BoardLayer layer, int cellX, int cellY, int offsetX, int offsetY) { return MoveCellTo(layer, cellX, cellY, cellX + offsetX, cellY + offsetY); }
                                                                                                                     
    public Vector2Int MoveCellTo(BoardLayer layer, Vector2Int cell, Vector2Int destination)      { return MoveCellTo(layer, cell.x, cell.y, destination.x, destination.y); }
    public Vector2Int MoveCellTo(BoardLayer layer, Vector2Int cell, int offsetX, int offsetY)    { return MoveCellTo(layer, cell.x, cell.y, offsetX, offsetY); }
    public Vector2Int MoveCellTo(BoardLayer layer, int cellX, int cellY, int destinationX, int destinationY)
    {
        if (IsEmpty(layer, cellX, cellY) || !IsEmpty(layer, destinationX, destinationY))
            return new Vector2Int(cellX, cellY);
        Tilemap tile = GetLayer(layer);
        if (tile != null)
        {
            ReDefineCell(tile, tile.GetTile(new Vector3Int(cellX, cellY, 0)), destinationX, destinationY);
            tile.SetTile(new Vector3Int(cellX, cellY, 0), null);//移動元のセルを削除
            return new Vector2Int(destinationX, destinationY);
        }
        return new Vector2Int(cellX, cellY);
    }
    
    private void ReDefineCell(Tilemap map,TileBase cell, int destinationX, int destinationY)
    {
        if (map != null)
        {
            map.SetTile(new Vector3Int(destinationX, destinationY, 0), cell);
        }
    }

    //マスにミノがあるときtrue、ないときfalseを返す
    public bool IsEmpty(BoardLayer layer, Vector2Int cood) { return IsEmpty(layer, cood.x, cood.y); }
    public bool IsEmpty(BoardLayer layer, int cellX, int cellY)
    {
        Tilemap tile = GetLayer(layer);
        bool ans=!tile.HasTile(new Vector3Int(cellX, cellY, 0));
        TileBase obj = tile.GetTile(new Vector3Int(cellX, cellY, 0));
        Debug.Log("layer" + layer + ":" + cellX + "," + cellY + " is " + ans);
        Debug.Log("tile" + ":" +  " is " + obj??"null");
        return ans;
    }
    public bool IsFilled() { return minoFilledFlag; }

    Tilemap GetLayer(BoardLayer layer)
    {
        switch (layer)
        {
            case BoardLayer.Default:
                return tilemap;
            case BoardLayer.Controll:
                return controlTile;
            default:
                return tilemap;
        }
    }
    Tilemap GetOtherLayer(BoardLayer layer)
    {
        switch (layer)
        {
            case BoardLayer.Default:
                return controlTile;
            case BoardLayer.Controll:
                return tilemap;
            default:
                return controlTile;
        }
    }

}

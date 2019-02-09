using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public enum BoardLayer
{
    Default,
    Controll,
    Back
}

//ゲームボードを制御するクラス
//セルの表示レイヤーとプログラムの制御用レイヤーがある
//例えばセルを移動させるとき、いったん制御用レイヤーに全部移動させて
//制御用レイヤーにあるセルが表示レイヤーにあるセルと重ならなかったら表示レイヤーに戻す処理を行う
public class GameBoardScript : MonoBehaviour
{
    [Header("Object References")]
    public GameObject cellController;//ミノを動かすクラスの参照
    public GameObject nextMinoContainer;//次のミノを保持しているクラス
    public GameObject generateCell;
    public GameObject edgeCell;

    [Header("Tilemap References")]
    public Tilemap tilemap;
    public Tilemap controlTile;

    [Header("Board Filled")]
    [SerializeField] UnityEvent OnMinoFilled;//ミノが上まで積まれたとき実行する関数を格納する変数

    [Header("Private Property")]
    [SerializeField] bool minoFilledFlag;
    [SerializeField] Vector3Int generateCood;//ミノをどこのマスに出現させるか
    bool activeFlag;//プレイヤーがミノを動かせるかどうかのフラグ

    public Vector3Int[] edgeCellCood;
    public int height//ボードの高さを取得するためのプロパティ
    {
        get { return edgeCellCood[1].y - edgeCellCood[0].y; }
    }
    public int width//ボードの幅を取得するためのプロパティ
    {
        get { return edgeCellCood[1].x - edgeCellCood[0].x; }
    }

    void Awake()
    {
        cellController.SetActive(false);//コントローラーを無効化
        if (OnMinoFilled == null) OnMinoFilled = new UnityEvent(); //イベント・インスタンスの作成
    }

    // Use this for initialization
    void Start()
    {
        //生成させる座標を計算
            generateCood = tilemap.WorldToCell(generateCell.transform.position);
        activeFlag = false;

        GenerateEdgeCood();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeFlag)
        {
            if (CheckLine())
            {
                GetNextMino();
            }
        }
        if (IsFilled())
            OnMinoFilled.Invoke();
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
        Vector3Int[,] cellscood = new Vector3Int[minoLengthY , minoLengthX];//コントローラに渡すセルの座標
        for (int y = 0; y < minoLengthY; y++)
            for (int x = 0; x < minoLengthX; x++)
                if (cells[y, x])//ミノから生成したセルが(x,y)のマスで存在するとき
                {
                    if (IsEmpty(0, generateCood.x + x, generateCood.y + y))//生成するマスでミノが存在しないとき
                    {
                        ReDefineCell(tilemap, tile, generateCood.x + x, generateCood.y + y);
                        cellscood[y , x ] = new Vector3Int(generateCood.x + x, generateCood.y + y,0);
                    }
                    else
                    {
                        minoFilledFlag = true;
                        return;
                    }
                }
                else
                {
                    cellscood[y , x ] = new Vector3Int(-100, -100,-100);//そのマスにオブジェクトがない
                }
        cellController.GetComponent<MinoControllerScript>().RegisterCells(cellscood);//どの座標のセルを移動させるかの情報を渡す
    }

    bool CheckLine()//ミノが一列すべてうまったかどうか(内部処理用)
    {
        int cellYCount = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                if (IsEmpty(BoardLayer.Default, edgeCellCood[0].x + x, edgeCellCood[0].y + y))//もし空白があったら
                {
                    if (cellYCount == 0) return false;
                    cellController.GetComponent<MinoControllerScript>().RemoveCells();
                    for (int y2 = 0; y2 < cellYCount; y2++)
                        for (int x2 = 0; x2 < width; x2++)
                            ClearCell(edgeCellCood[0].x + x2, edgeCellCood[0].y + y2);//それまでの列のセルを消去
                    for (int y2 = cellYCount; y2 < height; y2++)
                        for (int x2 = 0; x2 < width; x2++)
                            MoveCell(BoardLayer.Default, edgeCellCood[0].x + x2, edgeCellCood[0].y + y2, 0, -cellYCount);//上のセルを下に移動
                    return true;
                }
            cellYCount++;
        }
        return false;
    }

    //layerの座標cellにあるセルのレイヤーを変える
    public void SwitchCellLayer(BoardLayer baselayer, BoardLayer destlayer, Vector3Int cell)
    {
        SwitchCellLayerTo(baselayer, destlayer, cell, Vector3Int.zero);
    }
    //layerの座標cellにあるセルのレイヤーを変え、offset分だけ座標を移動させる
    public Vector3Int SwitchCellLayerTo(BoardLayer baselayer, BoardLayer destlayer, Vector3Int cell, Vector3Int offset)
    {
        Tilemap basetile = GetLayer(baselayer), destTile = GetLayer(destlayer);
        Vector3Int cood = new Vector3Int(cell.x, cell.y, 0);
        TileBase prevCell = basetile.GetTile(cood);
        if (prevCell != null)
        {
            destTile.SetTile(cood + offset, prevCell);//移動元のセルを削除
            basetile.SetTile(cood, null);//移動元のセルを削除
            return cell + offset;
        }
        return cell;
    }

    public Vector3Int MoveCell(BoardLayer layer, Vector3Int cell, Vector3Int offset)             { return MoveCell  (layer,cell.x, cell.y, offset.x, offset.y); }
    public Vector3Int MoveCell(BoardLayer layer, Vector3Int cell, int offsetX, int offsetY)      { return MoveCell  (layer, cell.x, cell.y, offsetX, offsetY); }
    public Vector3Int MoveCell(BoardLayer layer, int cellX, int cellY, int offsetX, int offsetY) { return MoveCellTo(layer, cellX, cellY, cellX + offsetX, cellY + offsetY); }
                                                                                                                   
    public Vector3Int MoveCellTo(BoardLayer layer, Vector3Int cell, Vector3Int destination)      { return MoveCellTo(layer, cell.x, cell.y, destination.x, destination.y); }
    public Vector3Int MoveCellTo(BoardLayer layer, Vector3Int cell, int offsetX, int offsetY)    { return MoveCellTo(layer, cell.x, cell.y, offsetX, offsetY); }
    public Vector3Int MoveCellTo(BoardLayer layer, int cellX, int cellY, int destinationX, int destinationY)
    {
        if (IsEmpty(layer, cellX, cellY) || !IsEmpty(layer, destinationX, destinationY))
            return new Vector3Int(cellX, cellY,0);
        Tilemap tile = GetLayer(layer);
        if (tile != null)
        {
            ReDefineCell(tile, tile.GetTile(new Vector3Int(cellX, cellY, 0)), destinationX, destinationY);
            tile.SetTile(new Vector3Int(cellX, cellY, 0), null);//移動元のセルを削除
            return new Vector3Int(destinationX, destinationY,0);
        }
        return new Vector3Int(cellX, cellY,0);
    }
    
    private void ReDefineCell(Tilemap map,TileBase cell, int destinationX, int destinationY)
    {
        if (map != null)
        {
            map.SetTile(new Vector3Int(destinationX, destinationY, 0), cell);
        }
    }

    //マスにミノがあるときtrue、ないときfalseを返す
    public bool IsEmpty(BoardLayer layer, Vector3Int cood) { return IsEmpty(layer, cood.x, cood.y); }
    public bool IsEmpty(BoardLayer layer, int cellX, int cellY)
    {
        Tilemap tile = GetLayer(layer);
        bool ans=!tile.HasTile(new Vector3Int(cellX, cellY, 0));
        TileBase obj = tile.GetTile(new Vector3Int(cellX, cellY, 0));
        /*Debug.Log("layer" + layer + ":" + cellX + "," + cellY + " is " + ans);
        Debug.Log("tile" + ":" +  " is " + obj??"null");*/
        return ans;
    }
    public bool IsFilled() { return minoFilledFlag; }
    public bool IsLeftWall(Vector3Int cell) { return cell.x == (edgeCellCood[0].x - 1); }//座標cellがボードの左の壁だったらtrueを返す
    public bool IsRightWall(Vector3Int cell) { return cell.x == (edgeCellCood[1].x + 1); }//座標cellがボードの右の壁だったらtrueを返す
    public bool IsBottomWall(Vector3Int cell) { return cell.y == (edgeCellCood[0].y - 1); }//座標cellがボードの下の壁だったらtrueを返す

    Tilemap GetLayer(BoardLayer layer)
    {
        switch (layer)
        {
            case BoardLayer.Default:    return tilemap;
            case BoardLayer.Controll:   return controlTile;
            default:                    return tilemap;
        }
    }
    Tilemap GetOtherLayer(BoardLayer layer)
    {
        switch (layer)
        {
            case BoardLayer.Default:    return controlTile;
            case BoardLayer.Controll:   return tilemap;
            default:                    return controlTile;
        }
    }

    public Vector3 TiletoWorld(Vector3Int cell)
    {
        return  tilemap.CellToWorld(cell);
    }

    void GenerateEdgeCood()
    {
            Transform[] edgeTransform = new Transform[2];
        edgeTransform[0]= edgeCell.transform.Find("Cell00");
        edgeTransform[1]= edgeCell.transform.Find("Cell01");
        if (edgeTransform[0] != null|| edgeTransform[1] != null )
        {
            edgeCellCood = new Vector3Int[2];
            for (int i = 0; i < 2; i++)
            {
                edgeCellCood[i] = tilemap.WorldToCell(edgeTransform[i].position);
            }
            if (edgeCellCood[0].x > edgeCellCood[1].x)
            {
                Vector3Int temp = edgeCellCood[0];
                edgeCellCood[0] = edgeCellCood[1];
                edgeCellCood[0] = temp;
            }
        }
        else
            Debug.Log("Not found Cell00 or Cell01");

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

//ボードのレイヤー情報の列挙体
public enum BoardLayer
{
    Default,//デフォルトレイヤー　ミノが置かれるレイヤー
    Controll,//コントロールレイヤー　移動させたミノを一時保管させるレイヤー
    Wall,//壁レイヤー　ボードの壁をおく
    WallAndDefault,//壁＋デフォルトレイヤー　(未実装)
    Back//背景レイヤー　空のセルが置かれているレイヤー
}

//ゲームボードを制御するクラス
//セルの表示レイヤーとプログラムの制御用レイヤーがある
//例えばセルを移動させるとき、いったん制御用レイヤーに全部移動させて
//制御用レイヤーにあるセルが表示レイヤーにあるセルと重ならなかったら表示レイヤーに戻す処理を行う
//
//2/19 GameBoardModifier.csと分離 列消去は向こう側へ
//2/25 壁とミノのレイヤーを分離
//
[RequireComponent(typeof(GameBoardModifier))]
public class GameBoardScript : MonoBehaviour
{
    [Header("Object References")]
    public GameObject nextMinoContainer;//次のミノを保持しているクラス
    public GameObject edgeCell;

    [Header("Tilemap References")]
    public Tilemap wallmap;//壁のタイルマップ
    public Tilemap tilemap;//表示させるタイルマップ
    public Tilemap controlmap;//内部プログラム用タイルマップ(カメラには映らない)

    [Header("Board Filled")]
    [SerializeField] UnityEvent OnMinoFilled;//ミノが上まで積まれたとき実行する関数を格納する変数

    [Header("Private Property")]
    [SerializeField] bool minoFilledFlag;
    bool activeFlag;//プレイヤーがミノを動かせるかどうかのフラグ

    public Vector3Int[] edgeCellCood;//[0]が左下 [1] が右上
    public int height//ボードの高さを取得するためのプロパティ
    {
        get { return edgeCellCood[1].y - edgeCellCood[0].y; }
    }
    public int width//ボードの幅を取得するためのプロパティ
    {
        get { return edgeCellCood[1].x - edgeCellCood[0].x; }
    }

    public static readonly Vector3Int nullCood;//不正な座標を表す定数(Vector3Intにnullを代入できないため)

    //静的コンストラクタについてはこちらを参照
    //https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/classes-and-structs/static-constructors
    static GameBoardScript()
    {
        nullCood = new Vector3Int(-100, -100, -100);
    }

    void Awake()
    {
        if (OnMinoFilled == null) OnMinoFilled = new UnityEvent(); //イベント・インスタンスの作成
    }

    // Use this for initialization
    void Start()
    {
        activeFlag = false;

        GenerateEdgeCood();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeFlag) return;
        if (IsFilled())
            OnMinoFilled.Invoke();
    }
    public void Stop()//ゲーム盤を動かせないようにする
    {
        //cellController.SetActive(false);//コントローラーを無効化
        activeFlag = false;
    }
    public void Resume()//ゲーム盤を動かせるようにする
    {
        //cellController.SetActive(true);
        activeFlag = true;
    }
    public void Restart()//ゲーム盤を最初からに
    {
        //cellController.SetActive(true);
        activeFlag = true;
        for(int y = 0; y < height; y++)
            for(int x = 0; x < width; x++)
                ReDefineCell(tilemap, null, edgeCellCood[0].x + x, edgeCellCood[0].y + y);
    }


    //layerの座標cellにあるセルのレイヤーを変える
    public void SwitchCellLayer(BoardLayer baselayer, BoardLayer destlayer, Vector3Int cell)
    {
        SwitchCellLayerTo(baselayer, destlayer, cell, Vector3Int.zero);
    }
    //layerの座標cellにあるセルのレイヤーを変え、offset分だけ座標を移動させる
    public Vector3Int SwitchCellLayerTo(BoardLayer baselayer, BoardLayer destlayer, Vector3Int cell, Vector3Int offset)
    {
        if (!activeFlag) return GameBoardScript.nullCood;
        Tilemap basetile = GetLayer(baselayer), destTile = GetLayer(destlayer);
        Vector3Int cood = new Vector3Int(cell.x, cell.y, 0);
        TileBase prevCell = basetile.GetTile(cood);
        if (prevCell != null)
        {
            destTile.SetTile(cood + offset, prevCell);//
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
        if (!activeFlag) return GameBoardScript.nullCood;

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

    //mapにtileをセットする
    //もしmapがnullだったりしたらfalseを返す
    bool ReDefineCell(Tilemap map, TileBase cell, int destinationX, int destinationY)
    {
        if (!activeFlag) return false;
        if (map == null)
        {
            Debug.LogWarning("map is null!");
            return false;
        }
        map.SetTile(new Vector3Int(destinationX, destinationY, 0), cell);
        return true;
    }
    //layerのmapにtileをセットする
    public bool SetCell(BoardLayer layer, TileBase cell, int destinationX, int destinationY)
    {
        if (!activeFlag) return false;
        if (!IsEmpty(layer, destinationX, destinationY))return false;
        return ReDefineCell(GetLayer(layer), cell, destinationX, destinationY);
    }

    //マスにミノがあるときtrue、ないときfalseを返す
    public bool IsEmpty(BoardLayer layer, Vector3Int cood) { return IsEmpty(layer, cood.x, cood.y); }
    public bool IsEmpty(BoardLayer layer, int cellX, int cellY) { return !GetLayer(layer).HasTile(new Vector3Int(cellX, cellY, 0)); }

    public bool IsFilled(){ return minoFilledFlag; }
    public void SetFilledFlag(bool filledFlag) { minoFilledFlag= filledFlag; }

    public bool IsWall(Vector3Int cood) { return IsEmpty(BoardLayer.Wall, cood.x, cood.y); }

    public Tilemap GetLayer(BoardLayer layer)
    {
        switch (layer)
        {
            case BoardLayer.Default: return tilemap;
            case BoardLayer.Controll: return controlmap;
            case BoardLayer.Wall: return wallmap;
            default: return tilemap;
        }
    }
    public Tilemap GetOtherLayer(BoardLayer layer)
    {
        switch (layer)
        {
            case BoardLayer.Default: return controlmap;
            case BoardLayer.Controll: return tilemap;
            case BoardLayer.Wall: return wallmap;
            default: return controlmap;
        }
    }

    public Vector3 CellToWorld(Vector3Int cell) { return  tilemap.CellToWorld(cell); }
    public Vector3 CellToLocal(Vector3Int cell) { return  tilemap.CellToLocal(cell); }
    public Vector3 WorldToCell(Vector3 worldCood) { return  tilemap.WorldToCell(worldCood); }

    void GenerateEdgeCood()
    {
        Transform[] edgeTransform = new Transform[2];
        edgeTransform[0] = edgeCell.transform.Find("Cell00");
        edgeTransform[1] = edgeCell.transform.Find("Cell01");
        if (edgeTransform[0] != null || edgeTransform[1] != null)
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

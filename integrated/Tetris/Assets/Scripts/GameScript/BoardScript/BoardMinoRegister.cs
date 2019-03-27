using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

//ボードにミノを登録するクラス
//GameObject Mino　からタイルマップに変換し、ボードに生成する
//
//3/25 generateCell
public class BoardMinoRegister : MonoBehaviour
{
    [Header("Object reference")]
    public MinoControllerScript minoController;//ミノを動かすクラスの参照
    public GameObject generateCell;
    public GameBoardScript gameBoard;

    [Header("Tilemap reference")]
    public Tilemap tilemap;//表示させるタイルマップ

    [Header("private property"),SerializeField]
    Vector3Int generateCood;//ミノをどこのマスに出現させるか

    // Use this for initialization
    void Start()
    {
        //cellController.SetActive(false);//コントローラーを無効化
        //生成させる座標を計算
        generateCood = tilemap.WorldToCell(generateCell.transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RegisterMino(GameObject mino_)
    {
        if (gameBoard.IsFilled()) return;//もしゲーム盤が上まで埋まってたら渡さない

        GameObject mino = mino_;
        bool[,] cells = mino.GetComponent<MinoScript>().GetShape();//ミノのデータからセルを生成する
        Tile tile = mino.GetComponent<MinoScript>().GetCell();

        Vector3Int[,] cellscood = ConvertMinoData(cells, tile);

        minoController.RegisterCells(mino, cellscood, generateCood);//どの座標のセルを移動させるかの情報を渡す

    }

    //ミノのbool配列のデータとtileから
    //GameBoardにミノを生成する
    Vector3Int[,] ConvertMinoData(bool[,] minoShapeData, Tile tile)
    {
        int minoLength = minoShapeData.GetLength(0);
        Vector3Int[,] cellscood = new Vector3Int[minoLength, minoLength];//コントローラに渡すセルの座標
        for (int y = 0; y < minoLength; y++)
            for (int x = 0; x < minoLength; x++)
                if (minoShapeData[y, x])//ミノから生成したセルが(x,y)のマスで存在するとき
                {
                    if (gameBoard.IsEmpty(BoardLayer.Default, generateCood.x + x, generateCood.y + y + (1 - minoLength)))//生成するマスでミノが存在しないとき
                    {
                        gameBoard.SetCell(BoardLayer.Default, tile, generateCood.x + x, generateCood.y + y+(1- minoLength));
                        cellscood[y, x] = new Vector3Int(generateCood.x + x, generateCood.y + y + (1 - minoLength), 0);
                    }
                    else
                    {
                        gameBoard.SetFilledFlag(true);
                    }
                }
                else
                {
                    cellscood[y, x] = GameBoardScript.nullCood;//そのマスにオブジェクトがない
                }
        return cellscood;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//どの地点に落ちるかのミノを半透明で描画するやつ
//
public class FallenMinoDrawer : MonoBehaviour
{
    [Header("Object Reference")]
    public MinoControllerScript minoController;
    public GameBoardScript gameBoard;
    [Header("Mino draw option"),Range(0.0f,1.0f)]
    public float minoAlpha;

    Tile tile;

    int minoHitBelowCount;//ミノが何マス下でぶつかるか
    public int GetMinoStuckBelowCount() { return minoHitBelowCount; }

    Vector3Int[,] cellCoods;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cellCoods = minoController.GetControllCoods();
        minoHitBelowCount =CalculateMinoStuckBelowCount();
        GenerateFallenMino();
    }

    public void SetMinoTile()
    {
        if (minoController == null)
        {
            Debug.LogWarning("mino contoller is null");
            return;
        }
        MinoScript mino=minoController.GetMino().GetComponent<MinoScript>();
        tile = mino.GetCell();
    }

    //マスの座標の配列をもとに半透明のミノを生成
    void GenerateMino(Vector3Int[] fallenCellCoods)
    {
        //以前生成していたスプライトを削除
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        foreach (var cell in fallenCellCoods)
            if (cell != GameBoardScript.nullCood)
            {
                GameObject obj = new GameObject();
                obj.AddComponent<SpriteRenderer>();
                obj.GetComponent<SpriteRenderer>().sprite = tile.sprite;
                obj.GetComponent<SpriteRenderer>().color = new Color(tile.color.r, tile.color.g, tile.color.b, minoAlpha);
                float spriteSize = tile.sprite.bounds.size.x;
                obj.transform.position = gameBoard.CellToLocal(cell) + new Vector3(spriteSize / 2f, spriteSize / 2f, 0);
                obj.transform.SetParent(transform, false);//obj.transformはローカルのものなので、第２引数はfalseにしておく
            }
    }

    void GenerateFallenMino()
    {
        if (cellCoods == null)
        {
            Debug.LogWarning("controller tileCood is null");
            return;
        }
        Vector3Int[] ansCoods = new Vector3Int[4];

        if (minoHitBelowCount != -1)
        {
            int count = 0;
            foreach (var cell in cellCoods)
            {
                if (cell == GameBoardScript.nullCood) continue;
                ansCoods[count] = cell + new Vector3Int(0, -minoHitBelowCount, 0);
                count++;
            }
            GenerateMino(ansCoods);
        }
    }

    //何マス下に来たとき壁やほかのミノに接地するかを計算する
    //ぶつからない場合、-1を返す
    int CalculateMinoStuckBelowCount()
    {
        if (cellCoods == null) return -1;
        int loopCount = 0;
        foreach (var cell in cellCoods)
            if (cell != GameBoardScript.nullCood)
            {
                loopCount = cell.y - gameBoard.edgeCellCood[0].y;//ボードの下まで何マスあるか
                break;
            }

        for (int h = 0; h < loopCount + 2; h++)//ボードの下(念のため+2マス)までにミノが置いてあるかチェック
        {
            foreach (var cell in cellCoods)
            {
                if (cell == GameBoardScript.nullCood) continue;
                Vector3Int checkCood = cell + new Vector3Int(0, -h, 0);
                if (!IsInArray(cellCoods, checkCood) &&
                    !gameBoard.IsEmpty(BoardLayer.Default, checkCood) || !gameBoard.IsEmpty(BoardLayer.Wall, checkCood))
                {
                    return h - 1;
                }
            }
        }
        return -1;
    }
    bool IsInArray(Vector3Int[] array, Vector3Int cood)
    {
        foreach (var i in array)
            if (cood == i) return true;
        return false;
    }
    bool IsInArray(Vector3Int[,] array, Vector3Int cood)
    {
        foreach (var i in array)
            if (cood == i) return true;
        return false;
    }

}

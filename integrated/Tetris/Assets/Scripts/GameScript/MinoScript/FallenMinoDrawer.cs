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
    [Range(0.0f,1.0f)]
    public float minoAlpha;

    Tile tile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GenerateFallenMino();
    }

    public void SetMinoTile()
    {
        if (minoController == null)
        {
            Debug.Log("mino contoller is null");
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
        Vector3Int[,] cellCoods = minoController.GetControllCoods();

        if (cellCoods == null)
        {
            Debug.Log("controller tileCood is null");
            return;
        }
        Vector3Int[] ansCoods = new Vector3Int[4];

        int loopCount = 0;
        foreach (var cell in cellCoods)
            if (cell != GameBoardScript.nullCood)
            {
                loopCount = cell.y - gameBoard.edgeCellCood[0].y;//ボードの下まで何マスあるか
                break;
            }

        bool stuckFlag = false;
        for (int h = 0; h < loopCount+2; h++)//ボードの下(念のため+2マス)までにミノが置いてあるかチェック
        {
            foreach (var cell in cellCoods)
            {
                if (cell == GameBoardScript.nullCood) continue;
                Vector3Int checkCood = cell + new Vector3Int(0, -h, 0);
                if (!InArray(cellCoods, checkCood) &&!gameBoard.IsEmpty(BoardLayer.Default, checkCood))
                {
                    stuckFlag = true;
                    break;
                }
            }

            if (stuckFlag)
            {
                int count = 0;
                foreach (var cell in cellCoods)
                {
                    if (cell == GameBoardScript.nullCood) continue;
                    ansCoods[count] = cell + new Vector3Int(0, -h+1, 0);
                    count++;
                }
                GenerateMino(ansCoods);
                break;//for(h~~)を抜ける
            }
        }
    }

    bool InArray(Vector3Int[] array,Vector3Int cood)
    {
        foreach (var i in array)
            if (cood == i) return true;
        return false;
    }
    bool InArray(Vector3Int[,] array, Vector3Int cood)
    {
        foreach (var i in array)
            if (cood == i) return true;
        return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

//ミノの基本クラス
//ミノを生成するクラスにどのようなセルでどのような配置のオブジェクトを生成させるかというもの
//ミノを生成するための設計図みたいなもの
public class MinoScript : MonoBehaviour {

    public Tile cellPrefab;//ミノ１つ１つを構成するセルのクラスの参照
    public String[] cellState; //ミノの形を指定するデータ   

    bool[,] cellFlag;//cellFlag[y,x]のように参照 (true : セルが存在 false : セルがない )
    int minoLengthY;
    int minoLengthX;

    void Awake()
    {
        minoLengthY = cellState.Length;
        minoLengthX = cellState[0].Length;

        cellFlag = new bool[minoLengthY, minoLengthX];
        for (int y = 0; y < minoLengthY; y++)
        {
            for (int x = 0; x < minoLengthX; x++)
            {
                char c = cellState[y][x];
                cellFlag[y, x] = (c == 'x');
            }
        }
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

	}

    //ミノの１つ１つのマスのスプライトデータを返す
    public Tile GetCell() { return cellPrefab; }
    //ミノの形を表すブーリアンデータを返す
    public bool[,] GetShape() { return cellFlag; }

    //ミノクラスからセルを１つのゲームオブジェクトとして返す
    //返り値のゲームオブジェクトはセル１つ１つを子要素として持つ
    public GameObject AsSprite()
    {
        GameObject output =new GameObject();

        float cellSize = GetCellSize();
        for (int y = 0; y < minoLengthY; y++)
            for (int x = 0; x < minoLengthX; x++)
                if (cellFlag[y, x])
                {
                    GameObject obj = new GameObject();
                    obj.AddComponent<SpriteRenderer>();
                    obj.GetComponent<SpriteRenderer>().sprite = cellPrefab.sprite;
                    obj.GetComponent<SpriteRenderer>().color = cellPrefab.color;
                    obj.transform.position = new Vector2(cellSize * (x - 1.5f), cellSize * (y - 1.5f));
                    obj.transform.position += transform.position;
                    obj.transform.SetParent(output.transform);
                }
        return output;
    }

    //ミノのマスのスプライトの１辺の長さを返す
    public float GetCellSize() { return cellPrefab.sprite.bounds.size.x; }

}

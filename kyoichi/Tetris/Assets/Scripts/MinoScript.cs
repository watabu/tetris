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

    int minoSize;

    void Awake()
    {
        minoSize = 4;
        cellFlag = new bool[minoSize, minoSize];
        for (int y = 0; y < minoSize; y++)
        {
            for (int x = 0; x < minoSize; x++)
            {
                char c = cellState[y][x];
                if (c == '_')
                {
                    cellFlag[y, x] = false;
                }
                else if (c == 'x')
                {
                    cellFlag[y, x] = true;
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

	}

    //ミノからセルのオブジェクトを２次元配列として返す
    //GameObject[minoSize, minoSize]
    public Tile GetCell()
    {
        return cellPrefab;
    }
    public bool[,] GetShape()
    {
        return cellFlag;
    }

    //ミノクラスからセルを１つのゲームオブジェクトとして返す
    //
    public GameObject AsSprite()
    {
        GameObject output =new GameObject();

        float cellSize = GetCellSize();
        for (int y = 0; y < minoSize; y++)
            for (int x = 0; x < minoSize; x++)
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

    public float GetCellSize()
    {
        return cellPrefab.sprite.bounds.size.x;
    }

}

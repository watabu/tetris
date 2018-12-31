using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//ミノの基本クラス
//ミノを生成するクラスにどのようなセルでどのような配置のオブジェクトを生成させるかというもの
//ミノを生成するための設計図みたいなもの
public class MinoScript : MonoBehaviour {

    public GameObject cellPrefab;//ミノ１つ１つを構成するセルのクラスの参照
    public String[] cellState; //ミノの形を指定するデータ   
    bool[,] cellFlag;//cellFlag[y,x]のように参照 (true : セルが存在 false : セルがない )
    Vector2Int minoOriginCood;//ミノのゲーム盤での基準座標

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

    public GameObject[,] GetCells()
    {
        GameObject[,] cells = new GameObject[minoSize, minoSize];
        for(int y = 0; y < minoSize; y++)
        {
            for(int x = 0; x < minoSize; x++)
            {
                if (cellFlag[y, x])
                {
                    cells[y, x] = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                }
            }
        }
        return cells;
    }

    public GameObject AsSprite()
    {
        GameObject[,] obj = GetCells();
        GameObject output =new GameObject();
        float cellSize = GetCellSize();
        for (int y = 0; y < minoSize; y++)
            for (int x = 0; x < minoSize; x++)
                if (obj[y, x] != null)
                {
                    obj[y, x].transform.position = new Vector2(cellSize * (x - 1.5f), cellSize * (y - 1.5f));
                    obj[y, x].transform.position += transform.position;
                    obj[y, x].transform.SetParent(output.transform);
                    obj[y, x].transform.position += new Vector3(0, 0, -1);
                }
        return output;
    }

    public float GetCellSize()
    {
        return cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
    }

}

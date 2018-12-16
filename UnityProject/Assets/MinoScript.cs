using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//ミノの基本クラス
//ミノを生成するクラスにどのようなセルでどのような配置のオブジェクトを生成させるかというもの
//ミノを生成するための設計図みたいなもの
public class MinoScript : MonoBehaviour {

    public GameObject cellPrefab;//ミノ１つ１つを構成するセルのクラスの参照
    public bool[,] cellFlag;    //mino[y,x]のように参照 (true : セルが存在 false : セルがない )
    Vector2Int minoOriginCood;//ミノのゲーム盤での基準座標

	// Use this for initialization
	void Start () {
        cellFlag = new bool[5, 5];
	}
	
	// Update is called once per frame
	void Update () {

	}

    public GameObject[,] GetCells()
    {
        GameObject[,] cells = new GameObject[5, 5];
        for(int y = 0; y < 5; y++)
        {
            for(int x = 0; x < 5; x++)
            {
                if (cellFlag[y, x])
                {
                    cells[y, x] = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                }
            }
        }
        return cells;
    }

}

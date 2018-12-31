using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
public class GameBoardUpdateScript : MonoBehaviour
{
    public int width;
    public int height;
    public float cellPadding;//セルごとの余白
    public GameObject cellPrefab;//ゲーム盤のマスのPrefab
    [HideInInspector]
    public float cellSize;//セル１つの大きさ

    void Awake()
    {
        cellSize = cellPrefab != null ?
            cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x :
            0;
    }

    // Use this for initialization
    void Start()
    {
        CreateCells();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void CreateCells()
    {
        Debug.Log(transform.childCount);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //マスのスプライトを生成
        if (height == 0 || width == 0)
            Debug.Log("マスの高さまたは幅が0のため、マスが生成されません！");
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject cell = Instantiate(
                    cellPrefab,
                    gameObject.transform.position + new Vector3((cellPadding + cellSize) * x, (cellPadding + cellSize) * y, 10.0f),
                    Quaternion.identity
                    );
                cell.gameObject.transform.SetParent(gameObject.transform);//セルの座標を親(ゲーム盤)とリンクさせる
            }
        }

    }

}

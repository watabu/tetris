using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;//シャッフル用
using System;//シャッフル用

//ミノを生成させる関数
public class MinoGeneratorScript : MonoBehaviour
{

    //ミノのプレハブオブジェクト
    //このリストからランダムでミノを生成
    public GameObject[] minoPrefab;

    int minoCount;

    int[] minoID;

    // Use this for initialization
    void Start()
    {
        minoCount = 0;
        minoID = new int[minoPrefab.Length];
        for (int i = 0; i < minoPrefab.Length; i++)
        {
            minoID[i] = i;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    //ミノをリストの中のデータからランダムに生成する
    public GameObject GetMino()
    {
        minoCount++;
        if (minoCount % minoID.Length == 0)
        {
            SetMinoID();
            minoCount = 0;
        }
        return UsefulFunctions.CloneObject(minoPrefab[minoID[minoCount]]);
    }

    void SetMinoID()
    {
        minoID = minoID.OrderBy(i => Guid.NewGuid()).ToArray();//配列をシャッフルする
    }

}

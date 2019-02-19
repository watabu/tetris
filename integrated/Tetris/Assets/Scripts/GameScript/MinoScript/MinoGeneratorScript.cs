using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ミノを生成させる関数
public class MinoGeneratorScript : MonoBehaviour {

    //ミノのプレハブオブジェクト
    //このリストからランダムでミノを生成
    public GameObject[] minoPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //ミノをリストの中のデータからランダムに生成する
    public GameObject GetMino()
    {
        return UsefulFunctions.CloneObject(minoPrefab[Random.Range(0, minoPrefab.Length)]);
    }

}

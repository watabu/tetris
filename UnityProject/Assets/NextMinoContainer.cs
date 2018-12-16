using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nextを制御するためのクラス
//
public class NextMinoContainer : MonoBehaviour {
    //nextを格納するリスト
    //mino[0]が一番古いミノ(次に取得するミノ)
    //mino[Length-1]が一番新しく登録したミノ
    List<GameObject> mino;

	// Use this for initialization
	void Start () {
        mino = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    GameObject Register(GameObject mino_)//ミノを登録する
    {
        mino.Add(mino_);
        return mino_;
    }

    GameObject GetNextMino()//次のミノを返す(返した後はこのクラス内から削除されるので注意)
    {
        GameObject output = mino[0];
        mino.RemoveAt(1);
        return output;
    }

}

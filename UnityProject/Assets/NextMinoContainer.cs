using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nextを制御するためのクラス
//
public class NextMinoContainer : MonoBehaviour {
    //nextを格納するリスト
    //mino[0]が一番古いミノ(次に取得するミノ)
    //mino[Length-1]が一番新しく登録したミノ
        [SerializeField] List<GameObject> mino;
    public GameObject minoGenerator;

    // Use this for initialization
    void Start()
    {
        mino = new List<GameObject>();
    }
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject Register(GameObject mino_)//ミノを登録する
    {
        mino.Add(mino_);
        mino_.transform.parent = transform;
        return mino_;
    }

    public void FillMinoList()
    {
        for (int i = 0; i < 5; i++)
        {
            Register(minoGenerator.GetComponent<MinoGeneratorScript>().GetMino());
        }
    }

    public GameObject GetNextMino()//次のミノを返す(返した後はこのクラス内から削除されるので注意)
    {
        GameObject output = mino[0];
        mino.RemoveAt(0);
        Register(minoGenerator.GetComponent<MinoGeneratorScript>().GetMino());
        return output;
    }

}

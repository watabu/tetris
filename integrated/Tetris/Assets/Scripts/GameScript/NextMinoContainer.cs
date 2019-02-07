using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Nextコンテナリストを制御するためのクラス
//
public class NextMinoContainer : MonoBehaviour {
    public GameObject minoGenerator;

    [Range(1,10)]
    public int nextContainerSize;

    [Header("Next Container References")]
    public GameObject nextMinoBegin;//コンテナの先頭
    public GameObject nextMinoEnd;//コンテナの最後尾

    [SerializeField] UnityEvent OnMinoGenerated;//

    // Use this for initialization
    void Start()
    {
    }
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject Register(GameObject mino_)//ミノを登録する
    {
        nextMinoBegin.GetComponent<NextContainerScript>().Register(mino_);
        return mino_;
    }

    public void FillMinoList()
    {
        for (int i = 0; i <= nextContainerSize; i++)
        {
            Register(minoGenerator.GetComponent<MinoGeneratorScript>().GetMino());
        }
    }

    public GameObject GetNextMino()
    {
        GameObject obj= nextMinoEnd.GetComponent<NextContainerScript>().GetMino();
        Register(minoGenerator.GetComponent<MinoGeneratorScript>().GetMino());
        return obj;
    }

}

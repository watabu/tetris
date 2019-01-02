using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Nextを制御するためのクラス
//
public class NextMinoContainer : MonoBehaviour {
    public GameObject nextContainerPrefab;
    private GameObject nextMinoBegin;
    private GameObject nextMinoEnd;
    public GameObject minoGenerator;
    public int nextContainerSize;

    [SerializeField] UnityEvent OnMinoGenerated;//

    // Use this for initialization
    void Start()
    {
        float sizeY=nextContainerPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        GameObject prevContainer=null;
        GameObject container = Instantiate(
            nextContainerPrefab,
            gameObject.transform.position + new Vector3(0.0f, 0.0f, 10.0f),
            Quaternion.identity
            );
        nextMinoBegin = container;
        prevContainer = container;
        container.transform.SetParent(transform);
        for (int i = 1; i < nextContainerSize; i++)//最初の１つ以外を生成
        {
            container = Instantiate(
                nextContainerPrefab,
                gameObject.transform.position + new Vector3(0.0f, -sizeY * (nextContainerSize-i), 10.0f),
                Quaternion.identity
                );
            container.GetComponent<NextContainerScript>().prevContainer = prevContainer;
            prevContainer.GetComponent<NextContainerScript>().nextContainer = container;
            prevContainer = container;
            container.transform.SetParent(transform);
        }
        nextMinoEnd = container;
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

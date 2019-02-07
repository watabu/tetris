using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nextコンテナ一つ一つのクラス
//
public class NextContainerScript : MonoBehaviour {
    //コンテナが保持するミノクラス
    GameObject mino;
    [Header("Next Container References")]
    public GameObject nextContainer;
    public GameObject prevContainer;
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //ミノを登録する
    public void Register(GameObject mino_)
    {
        if (mino_ == null) return;
        if (nextContainer != null)//もし次のコンテナのリンクがあるなら
            nextContainer.GetComponent<NextContainerScript>().Register(mino);//自分の持っていたミノを次のコンテナに渡す
        mino = mino_;//ミノの上書き
        GenerateSprite();
    }

    public GameObject GetMino()
    {
        return mino;
    }

    private void GenerateSprite()
    {
        foreach (Transform child in transform)//ミノによって生成していたセルを削除
            Destroy(child.gameObject);

        MinoScript minoS = mino.GetComponent<MinoScript>();
        GameObject sprite = minoS.AsSprite();
        sprite.transform.position += transform.position;
        sprite.transform.SetParent(transform);
        mino.transform.SetParent(transform);
    }

}

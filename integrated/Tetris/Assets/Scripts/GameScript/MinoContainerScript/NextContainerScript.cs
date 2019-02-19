using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nextコンテナ一つ一つのクラス
//
public class NextContainerScript : MonoBehaviour {
    //コンテナが保持するミノクラス
    GameObject mino;
    [Header("Next Container References")]
    public GameObject nextContainer;//次のコンテナの参照
    public GameObject prevContainer;//前のコンテナの参照
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

    public GameObject GetMino() { return mino; }

    //ミノからスプライトを自身の子として生成する
    private void GenerateSprite()
    {
        //以前生成していたスプライトを削除
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        MinoScript minoS = mino.GetComponent<MinoScript>();
        GameObject sprite = minoS.AsSprite();
        sprite.transform.position += transform.position;
        sprite.transform.SetParent(transform);
        mino.transform.SetParent(transform);
    }

}

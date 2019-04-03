using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//オジャマミノがどれだけたまってるかを表示するメーターのスクリプト
public class OjamaStuckScript : MonoBehaviour
{
    public GameObject pileSprite;
    public Vector2 pileTopCood;//オジャマミノの山の１番上の座標
    public int OjamaStuck;
    List<GameObject> SpriteList;
    public int ListCount;
    private void Awake()
    {
        SpriteList = new List<GameObject>();

    }
    // Start is called before the first frame update
    void Start()
    {
        //おじゃまメーターのテスト
        //int i = 0;
        //GameObject pilesprite = Instantiate(pileSprite, new Vector3(0, -2.1f+0.3f * i, 0), Quaternion.identity) as GameObject;
       // pilesprite.transform.SetParent(this.transform, false);
       
    }

    // Update is called once per frame
    void Update()
    {
        ListCount = SpriteList.Count;
    }

    public void ChangeStuck(int num)
    {
        OjamaStuck = num;
       // Debug.Log(OjamaStuck);
        //バーへの表示の更新
        if (OjamaStuck != SpriteList.Count)//変更されたなら
        {
       
            for (int i = 0; i < SpriteList.Count; i++)//一回全部消す
            {
                //Debug.Log("<color=blue>pliesprite deleted</color>");
                // DestroyImmediate(SpriteList[i],true);
                Destroy(SpriteList[i]);
            }
            SpriteList.Clear();
            for (int i =0 ; i < OjamaStuck; i++)
            {               
                GameObject pilesprite = Instantiate(pileSprite, new Vector3(0, 0.3f * i-2.1f, 0), Quaternion.identity) as GameObject;
                pilesprite.transform.SetParent(this.transform, false);
                SpriteList.Add(pilesprite);
               // Debug.Log("<color=blue>pliesprite placed</color>");
            }

        }


    }

    
}

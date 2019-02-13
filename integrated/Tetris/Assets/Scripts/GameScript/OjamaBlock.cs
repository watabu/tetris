using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//消されたときとかRENの処理
public class OjamaBlock : MonoBehaviour
{
    public const int MaxPlayerNum = PlInput.MaxPlayerNum;
    int[] OjamaStock;//送られたおじゃまブロックの列の数


    public void SendOjama(int playerNum, int lineNum)//何列消したかを渡す
    {


    }
    public void GetOjama(int playerNum)//おじゃまブロックをintでもらう？セルをいじってもらう？
    {

    }

    private void Awake()
    {
        OjamaStock = new int[MaxPlayerNum];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

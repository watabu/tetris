using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//おじゃまブロックの送受信とかRENの処理 AwakeとStartが違うだけ
//見本用
public class デモ用 : MonoBehaviour
{
    public const int MaxPlayerNum = PlInput.MaxPlayerNum;
    public const int MaxRen = 30;
    int[][] OjamaStock;//送られたおじゃまブロックの列の数
    int[] RenNum;//プレイヤーが何RENか


    public void SendOjama(int playerNum, int lineNum)//何列消したかを送って処理してもらう　０でも送ること
    {//毎フレーム呼ぶとRENが途切れてしまう仕様なのでブロックが固定されたら呼ぶ感じ？

        int i;
        int victim = playerNum;//送られる人
        if (lineNum <= 0)
        {//Renが途切れたら
            RenNum[playerNum] = -1;
        }
        else
        {
            RenNum[playerNum] += 1;//Renを足す
            while (victim == playerNum)//自分以外になるまで
            {
                victim = Random.Range(0, MaxPlayerNum);//0からMaxPlayerNum-1まで
            }
            i = 0;
            while (i < MaxRen)
            {
                if (OjamaStock[victim][i] == 0)//i=0から順番に入れてく
                {
                    if (ClearLineToOjamaNum(lineNum) == 0)//
                    {
                        OjamaStock[victim][i] = RenToOjamaNum(RenNum[playerNum]);//RENの分
                    }
                    else
                    {
                        OjamaStock[victim][i] = ClearLineToOjamaNum(lineNum);  //消した数の分
                        OjamaStock[victim][i + 1] = RenToOjamaNum(RenNum[playerNum]);//RENの分
                    }
                    break;

                }
                i++;
            }
            
        }
    }
    public void GetOjama(int playerNum)//おじゃまブロックをintでもらう？セルをいじってもらう？
    {

    }
    public int ClearLineToOjamaNum(int num)//消した列の数からおじゃまブロックの列数を返す
    {
        if (num < 2) return 0;
        else if (num < 4) return num - 1;
        else if (num == 4) return 4;
        else
        {
            Debug.Log("error in OjamaBlock clearLineToOjamaNum, num > 4");
            return 0;
        }
    }

    public int RenToOjamaNum(int Ren)//Renの数からおじゃまブロックの列数を返す
    {
        if (Ren < 2) return 0;
        else if (Ren < 4) return 1;
        else if (Ren < 6) return 2;
        else if (Ren < 8) return 3;
        else if (Ren < 11) return 4;
        else
        {
            return 5;
        }
    }


    private void Awake()
    {

        OjamaStock = new int[MaxPlayerNum][];
        RenNum = new int[MaxRen];

        for (int i = 0; i < MaxPlayerNum; i++)
        {
            OjamaStock[i] = new int[MaxRen];
            RenNum[i] = -1;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < MaxPlayerNum; i++)
        {
            RenNum[i] = -1;
        }

        for (int i = 0; i < 5; i++)
        {
            SendOjama(0, i);
            Debug.Log(RenNum[0]);
            //0,1,2,3,4列送る
        }
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(OjamaStock[1][i]);

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}


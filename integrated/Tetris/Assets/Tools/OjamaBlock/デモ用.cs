﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//使い方見本
//おじゃまブロックの送受信とかRENの処理
public class デモ用 : MonoBehaviour
{
    public const int MaxPlayerNum = PlInput.MaxPlayerNum;
    public const int MaxRen = 30;
    int[][] OjamaStock;//送られたおじゃまブロックの列の数
    int[] RenNum;//プレイヤーが何RENか
    bool backtoback;//未実装

    public void SendOjama(int playerNum, int lineNum)//何列消したかを送って処理してもらう　０でも送ること（RENの処理）
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
                    if (ClearLineToOjamaNum(lineNum) > 0)//送る列が０でない
                    {
                        OjamaStock[playerNum][0] -= ClearLineToOjamaNum(lineNum);//とりあえず相殺

                        while (OjamaStock[playerNum][0] <= 0 && OjamaStock[playerNum][1] > 0)//先頭のおじゃまが０以下なら配列をずらす
                        {
                            OjamaStock[playerNum][0] += OjamaStock[playerNum][1];
                            for (int j = 1; j < MaxRen - 1; j++)
                            {
                                OjamaStock[playerNum][j] = OjamaStock[playerNum][j + 1];
                            }
                            OjamaStock[playerNum][MaxRen - 1] = 0;
                        }
                        //相殺しきってあまったらOjamaStock[][0]が負のままになる
                        if (OjamaStock[playerNum][0] < 0)
                        {
                            OjamaStock[victim][i] = -OjamaStock[playerNum][0];  //消した数の分
                            OjamaStock[playerNum][0] = 0;
                        }
                    }
                    if (RenToOjamaNum(RenNum[playerNum]) > 0)//RENによる送る列が０でない
                    {
                        OjamaStock[playerNum][0] -= RenToOjamaNum(RenNum[playerNum]);//とりあえず相殺

                        while (OjamaStock[playerNum][0] <= 0 && OjamaStock[playerNum][1] > 0)//先頭のおじゃまが０以下なら配列をずらす
                        {
                            OjamaStock[playerNum][0] += OjamaStock[playerNum][1];
                            for (int j = 1; j < MaxRen - 1; j++)
                            {
                                OjamaStock[playerNum][j] = OjamaStock[playerNum][j + 1];
                            }
                            OjamaStock[playerNum][MaxRen - 1] = 0;
                        }
                        //相殺しきってあまったらOjamaStock[][0]が負のままになる
                        if (OjamaStock[playerNum][0] < 0)
                        {
                            if (OjamaStock[victim][i] == 0)
                            {
                                OjamaStock[victim][i] = -OjamaStock[playerNum][0];  //消した数の分                                                       
                            }
                            else if (i + 1 < MaxRen)
                            {
                                OjamaStock[victim][i + 1] = -OjamaStock[playerNum][0];  //消した数の分
                            }
                            OjamaStock[playerNum][0] = 0;
                        }
                    }
                    break;
                  
                }
                i++;
            }
            
        }
    }

    public void GetOjama(int playerNum)//おじゃまブロックをintでもらう？セルを直接いじって生成してもらう？
    {

    }



    public int ClearLineToOjamaNum(int num)//消した列の数からおじゃまブロックの列数を返す
    {
        if (num < 2) return 0;
        else if (num < 4) return num-1;
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

    public int Ren(int playerNum)//現在のREN数を返す
    {
        return RenNum[playerNum];
    }
    public void ResetRen(int playerNum)//Renを０にする
    {
        RenNum[playerNum] = 0;
    }
    public void ResetOjamaStock(int playerNum)//おじゃまストックを０にする
    {
        for (int i = 0; i < MaxRen; i++)
        {
            OjamaStock[playerNum][i] = 0;
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

    // デバッグ用
    void Start()
    {
        int mode = 0;
        for (int i = 0; i < MaxPlayerNum; i++)
        {
            RenNum[i] = -1;
        }

        if (mode == 0)//基本的な動作のテスト
        {
            for (int i = 0; i < 5; i++)
            {
                SendOjama(0, i);//0,0,1,2,4
                Debug.Log(RenNum[0]);
            }
            for (int i = 0; i < 6; i++)
            {
                Debug.Log(OjamaStock[1][i]);
            }
        }
        else if (mode == 2)//相殺のテスト
        {
            SendOjama(0, 4);
            SendOjama(0, 0);
            SendOjama(0, 3);
            SendOjama(0, 3);
            SendOjama(0, 3);
            Debug.Log(RenNum[0]);
            //0,1,2,3,4列送る

            for (int i = 0; i < 6; i++)
            {
                Debug.Log(OjamaStock[1][i]);

            }
            SendOjama(1, 3);//2
            SendOjama(1, 3);//2
            SendOjama(1, 3);//2+1
            Debug.Log("player2");
            for (int i = 0; i < 6; i++)
            {
                Debug.Log(OjamaStock[1][i]);

            }
            Debug.Log("player1");
            for (int i = 0; i < 6; i++)
            {
                Debug.Log(OjamaStock[0][i]);

            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

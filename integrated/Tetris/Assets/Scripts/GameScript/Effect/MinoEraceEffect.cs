using UnityEngine;
using System.Collections;

//ミノが消えたとき発生するエフェクトを制御するスクリプト
public class MinoEraceEffect : MonoBehaviour
{
    GameObject effect;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateEffect()
    {

    }

    public void TspinEffect(int playerNum, int yCount)//TSDとかTSMとか表示する
    {
        if (yCount == 1)
        {

        }
        else if (yCount == 2)
        {

        }
        else if (yCount == 3)
        {

        }


        else
        {

            Debug.Log("error in MinoEraceEffect:yCount of TspinEffect");
        }

    }

}

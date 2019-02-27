using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//今何RENか(これはこのクラスで制御するかな？)とか
//Tスピン,TETRISはしたかどうかを記録するクラス
public class EraceManager : MonoBehaviour
{
    [Header("Mino reference")]
    public GameObject TMino;
    public MinoControllerScript minoController;

    [Header("Call Back Function"), SerializeField]
    UnityEvent OnEraceByTSpin;//Tスピンされたとき実行する関数を格納する変数

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //セルが消されたとき、何によって消えたのかを確認する
    //yCount 消された列の総数
    public void CheckEraceMino(int yCount, GameObject mino)
    {
        //もし消したミノがT型のとき、
        if (mino.GetComponent<MinoScript>().GetShape() == TMino.GetComponent<MinoScript>().GetShape())
        {
            if (minoController.minoRevisedFlag)
                OnEraceByTSpin.Invoke();
        }
    }
}

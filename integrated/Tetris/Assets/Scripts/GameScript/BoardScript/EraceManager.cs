using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//今何RENか(これはこのクラスで制御するかな？)とか
//Tスピン,TETRISはしたかどうかを記録するクラス
public class EraceManager : MonoBehaviour
{
    [System.Serializable]
    public class RenCallBack : UnityEngine.Events.UnityEvent<int> { }

    [Header("Mino reference")]
    public GameObject TMino;
    public MinoControllerScript minoController;

    [Header("Call Back Function"), SerializeField]
    UnityEvent OnEraceByTSpin;//Tスピンされたとき実行する関数を格納する変数
    [SerializeField]
    RenCallBack OnRenChanged;//Renを更新する

    public OjamaBlock Ojama;

    private void Awake()
    {
        Ojama = GetComponent<OjamaBlock>();
    }
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
        if (yCount <= 0 || mino == null)
        {

            Ojama.SendOjama(0, 0);
            OnRenChanged.Invoke(-1);
            GenarateOjama(Ojama.GetOjama(0));//とりあえずplayer１だけ
        }
        //もし消したミノがT型のとき、
        else if (mino.GetComponent<MinoScript>().GetShape() == TMino.GetComponent<MinoScript>().GetShape())
        {
            if (minoController.minoRevisedFlag)//本当はTの４隅が埋まってるかどうかも判定する
                OnEraceByTSpin.Invoke();
           // OnRenChanged.Invoke(-1);
        }
        else//普通に消したとき
        {
            Ojama.SendOjama(0, yCount);
            Debug.Log("Ren is");
            Debug.Log(Ojama.Ren(0));
            Debug.Log("Send Ojama is");
            Debug.Log(yCount);
            OnRenChanged.Invoke(Ojama.Ren(0));
        }

    }

    public void GenarateOjama(List<int> Ojamalist)
    {
        if (Ojamalist == null) return;//送られるものがない





    }

}

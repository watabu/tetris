using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//今何RENか(これはこのクラスで制御するかな？)とか
//Tスピン,TETRISはしたかどうかを記録するクラス
//GameBoardが保持する
//
//3/30 TSpin実装完了
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
    [SerializeField]
    RenCallBack OnStuckChanged;//Renを更新する

<<<<<<< HEAD
=======

>>>>>>> eb57a186a3074f471e0cb00db3e9a7486287b9bc
    GameBoardScript gameBoard;
    public OjamaBlock Ojama;
    [Range(0, 3)]
    public int playerID;
    private void Awake()
    {
        Ojama = GetComponent<OjamaBlock>();
        gameBoard = GetComponent<GameBoardScript>();
<<<<<<< HEAD
=======
        

>>>>>>> eb57a186a3074f471e0cb00db3e9a7486287b9bc
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OnStuckChanged.Invoke(Ojama.TotalOjama(playerID));
    }
    //セルが消されたとき、何によって消えたのかを確認する
    //yCount 消された列の総数
    public void CheckEraceMino(int yCount, MinoControllerScript mino)
    {
<<<<<<< HEAD
        int player = playerID;
        if (yCount <= 0 || mino == null)//消されていない
        {

            Ojama.SendOjama(player, 0);
            OnRenChanged.Invoke(-1);
            GenarateOjama(Ojama.GetOjama(player));//とりあえずplayer１だけ
=======
        if (yCount <= 0)
        {
            Ojama.SendOjama(0, 0);
            OnRenChanged.Invoke(-1);
            GenarateOjama(Ojama.GetOjama(0));//とりあえずplayer１だけ
>>>>>>> eb57a186a3074f471e0cb00db3e9a7486287b9bc
            return;
        }
        //もし消したミノがT型のとき、
        MinoScript minoShape = mino.GetMino().GetComponent<MinoScript>();
        if (minoShape.GetShape() == TMino.GetComponent<MinoScript>().GetShape())
        {
            Vector3Int minoLeftTop = mino.GetOriginCood();

            //Tの４隅が埋まってるとき
            if (gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop) && gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(2, 0, 0)) &&
                gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(0, -2, 0)) && gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(2, -2, 0)))
<<<<<<< HEAD
                OnEraceByTSpin.Invoke();//Tspinエフェクトなど
            Ojama.SendOjama(player, yCount);
            OnRenChanged.Invoke(Ojama.Ren(0));//Ren表記
=======
                OnEraceByTSpin.Invoke();
>>>>>>> eb57a186a3074f471e0cb00db3e9a7486287b9bc
            // OnRenChanged.Invoke(-1);
        }
        else//普通に消したとき
        {
            Ojama.SendOjama(player, yCount);
            /*Debug.Log("Ren is");
            Debug.Log(Ojama.Ren(0));
            Debug.Log("Send Ojama is");
            Debug.Log(yCount);*/
            OnRenChanged.Invoke(Ojama.Ren(0));//Ren表記
        }

    }
<<<<<<< HEAD

    //盤面に生成する関数はGameBoardModifier.csにある
=======
>>>>>>> eb57a186a3074f471e0cb00db3e9a7486287b9bc
    public void GenarateOjama(List<int> Ojamalist)
    {
        if (Ojamalist == null) return;//送られるものがない





    }

}

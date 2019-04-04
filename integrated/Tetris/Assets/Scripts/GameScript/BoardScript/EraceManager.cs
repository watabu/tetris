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
    [System.Serializable]
    public class OjamaCallBack: UnityEngine.Events.UnityEvent<int,int> { }
    [Header("Mino reference")]
    public GameObject TMino;
    public MinoControllerScript minoController;

    [Header("Call Back Function"), SerializeField]
    RenCallBack OnEraceByTSpin;//Tスピンされたとき実行する関数を格納する変数
    [SerializeField]
    RenCallBack OnRenChanged;//Renを更新する
    [SerializeField]
    RenCallBack OnStuckChanged;//Renを更新する
    [SerializeField]
    OjamaCallBack OnOjamaGenerated;//おじゃまを生成する
    GameBoardScript gameBoard;
    public OjamaBlock Ojama;
    [Range(0, 3)]
    public int playerID;

    AudioSource audioSources;
    private void Awake()
    {
        Ojama = GetComponent<OjamaBlock>();
        gameBoard = GetComponent<GameBoardScript>();
        audioSources = GetComponent<AudioSource>();
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
        int player = playerID;
        if (yCount <= 0 || mino == null)//消されていない
        {
            OnEraceByTSpin.Invoke(0);
            Ojama.SendOjama(player, 0);
            OnRenChanged.Invoke(-1);
            GenarateOjama(Ojama.GetOjama(player));//消していないのでストックにあるならおじゃまを生成する

            return;
        }
        //もし消したミノがT型のとき、
        MinoScript minoShape = mino.GetMino().GetComponent<MinoScript>();
        if (CompareBool( minoShape.GetShape() , TMino.GetComponent<MinoScript>().GetShape()))
        {
            Vector3Int minoLeftTop = mino.GetOriginCood();

            //Tの3隅が埋まってるとき
            bool A = gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop),
                B = gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(2, 0, 0)),
                C = gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(0, -2, 0)),
                D = gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(2, -2, 0));
            //if (gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop) && gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(2, 0, 0)) &&
            //    gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(0, -2, 0)) && gameBoard.IsEmpty(BoardLayer.Default, minoLeftTop + new Vector3Int(2, -2, 0)))
            if (A && B && (C || D) || (A || B) && C && D)
            {
                OnEraceByTSpin.Invoke(yCount);//Tspinエフェクトなど
                Debug.Log("<color=0F0>Tspin!</color>");
                audioSources.pitch = 1;
            }
            else
            {
                
                OnEraceByTSpin.Invoke(0);

            }
            Ojama.SendOjama(player, yCount);
            OnRenChanged.Invoke(Ojama.Ren(player));//Ren表記

            audioSources.PlayOneShot(audioSources.clip);
        }
        else//普通に消したとき
        {
            OnEraceByTSpin.Invoke(0);
            Ojama.SendOjama(player, yCount);
            /*Debug.Log("Ren is");
            Debug.Log(Ojama.Ren(0));
            Debug.Log("Send Ojama is");
            Debug.Log(yCount);*/
            OnRenChanged.Invoke(Ojama.Ren(player));//Ren表記
            if (audioSources.pitch < 2.5 && yCount != 4)
            {
                audioSources.pitch = 1;
                //audioSources.pitch = 1 + 0.1f * Ojama.Ren(player);
                //だんだん音が高くなってくようにしたかったけどまた今度
            }
            if (yCount == 4)//４列消したとき音
            {
                audioSources.pitch = 1;

            }
            audioSources.PlayOneShot(audioSources.clip);
        }

    }

    //盤面に生成する関数はGameBoardModifier.csにある

    public void GenarateOjama(List<int> Ojamalist)
    {
        if (Ojamalist == null)
        {
            Debug.LogWarning("Ojamalist == null");
            return;
        }
        if (Ojamalist.Count <= 0)
        {
            return;
        }//なんもなかった
        //おじゃまブロックの生成　穴の位置を確立でばらけさすのは今度  
       
        foreach(var height in Ojamalist)
        {
            OnOjamaGenerated.Invoke(playerID,height);
        }

    }

    void Showbool(bool[,] cellflag)
    {
        Debug.Log(cellflag[0, 0] +" "+ cellflag[0, 1] +" "+ cellflag[0, 2]);
        Debug.Log(cellflag[1, 0] + " " + cellflag[1, 1] + " " + cellflag[1, 2]);
        Debug.Log(cellflag[2, 0] + " " + cellflag[2, 1] + " " + cellflag[2, 2]);
    }
    bool CompareBool(bool[,] cellflag1, bool[,] cellflag2)
    {
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if (cellflag1[i, j] != cellflag2[i, j]) return false;
            }
        }
        return true;
    }
}

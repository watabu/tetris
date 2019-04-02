using UnityEngine;
using System.Collections;
using GamepadInput;

/* メモ
 * ボタンを押された状態が関数が呼び出されたタイミングで左右されないようにしたものは
 * return Keystatus[][][]みたいになってる。
 * 関数が呼ばれた瞬間のボタンの状態を返すのはGetInput2 
 * ってふうにやってみたけど意味ないよって感じだったら教えてください。
 * 2/9に、キーボード１の回転のキーが矢印じゃなくてK,Lにしたのを忘れてたのでコメント文だけ変更
 *2/13 GetInputdeltaを追加　移動速度をやんわりできるように
 * 3/1 GetInputdelta2を追加　おしっぱだと早く移動する、みたいな感じで使う
 */
public class PlInput : MonoBehaviour
{
    public const int MaxPlayerNum = 2;//最大プレイ人数
    public const int MaxKey = 5;//キーの数
    public const double fixtime = 0.25;
    public enum ConKind
    {
        NOTHING,//初期状態
        KEYBOARD1,//キーボード１移動がFPS使用のwasd,回転がK、L、ホールドがスペース
        KEYBOARD2,//キーボード２移動が矢印、ｚｘで回転、ホールドがスペース
        JOYCON//joycon
    }
    public enum Key
    {
        KEY_HORIZON,//右が１、左がー１
        KEY_VERTICAL,//上が１、下がー１
        KEY_SUBMIT,//Aボタン　右回転
        KEY_CANCEL,//Bボタン　左回転
        KEY_HOLD//Lボタン or R
    }//
    public class Playerinfo//プレイヤーの情報を持っておく 他に追加するかもしれないので一応クラスにしておく
    {

        public ConKind ConKind;//コントローラーの種類
        public int JoyConNum;//ゲームパッド番号
        //     int playerNum;//通常配列の添え字と同じプレイヤー番号になるからいらない？
        public Playerinfo()
        {
            ConKind = ConKind.NOTHING;
            JoyConNum = -1;//未登録
        }
    }//


    static public Playerinfo[] Player;
    static public ConKind conkind1 = ConKind.NOTHING;
    static public ConKind conkind2 = ConKind.NOTHING;
    static int[][][] Keystatus;
    static double[][] KeyPushcount; //キーが押され続けている時間をカウント

   static  public ConKind GetConKind(int playerNum)
    {
        if(playerNum==0) return conkind1;
        return conkind2;
    }
    public int GetInputdelta2(int playerNum, Key key, double deltatime)//deltatime秒に一回押されている状態（１，－１）を返す
    {//旧GetInputdelta()
        /*横に移動するときに一秒ボタンが押しっぱなしのときに２個ぶん移動するようにしたい、ってときに
         * GetInputdelta(playerNum, key, 0.5) とするとおしっぱの時は一秒に２回だけ、１、－１を返し他のときには０を返す
         * 他はGetInputDown()と同じように使える
         */
        if (GetInput(playerNum, key) == 0)//押されていない
        {
            KeyPushcount[playerNum][(int)key] = 0;//初期化
            return 0;
        }
        else if (GetInputDown(playerNum, key) != 0)//押された瞬間
        {
            KeyPushcount[playerNum][(int)key] = 0;//初期化
            return GetInputDown(playerNum, key);
        }
        else//押され続けている
        {
            KeyPushcount[playerNum][(int)key] += 1 * Time.deltaTime;

            if (KeyPushcount[playerNum][(int)key] > deltatime)
            {
                if (key == Key.KEY_VERTICAL && GetInput(playerNum,key)>0 && KeyPushcount[playerNum][(int)key] < deltatime*3)
                {//上入力だけdeltaTimeを大きく
                    return 0;   
                }
                    KeyPushcount[playerNum][(int)key] = 0;//初期化
                return GetInput(playerNum, key);//押されているやつ　１かー１
            }
            return 0;
        }
    }
    public int GetInputdelta(int playerNum, Key key, double deltatime)//GetInputdelta　[(int)key + MaxKey]を管理している
    {
        //中身は大体GetInputdelta2と同じ  一定時間押され続けると高速移動みたいな
        if (GetInput(playerNum, key) == 0)//押されていない
        {
            KeyPushcount[playerNum][(int)key + MaxKey] = 0;//初期化
            return 0;
        }
        else if (GetInputDown(playerNum, key) != 0)//押された瞬間
        {
            KeyPushcount[playerNum][(int)key + MaxKey] = deltatime - fixtime;//初期化 一回目だけfixtimeたったら返すように

            return GetInputDown(playerNum, key);
        }
        else//押され続けている
        {
            KeyPushcount[playerNum][(int)key + MaxKey] += 1 * Time.deltaTime;

            if (KeyPushcount[playerNum][(int)key + MaxKey] > deltatime)
            {
                KeyPushcount[playerNum][(int)key + MaxKey] = 0;//初期化
                return GetInput(playerNum, key);//押されているやつ　１かー１
            }

            return 0;
        }

    }
    public int GetInput(int playerNum, Key key)//Update毎の入力を返す　押されていれば1 or -1 なければ0
    {
        return Keystatus[playerNum][(int)key][0];
    }

    public int GetInput1(int playerNum, int key)//keyをintで渡せる
    {
        if (key < 0 || key >= MaxKey)
        {
            Debug.Log("error in GetInputDown1: keyが範囲外");
            return -2;
        }
        return Keystatus[playerNum][(int)key][0];


    }

    public int GetInputDown(int playerNum, Key key)//押された瞬間だけ Update毎の入力を返す
    {
        if (Keystatus[playerNum][(int)key][0] != Keystatus[playerNum][(int)key][1])//前と違ければ
        {
            return Keystatus[playerNum][(int)key][0];//0-1 なら１を返し1-0なら０を返す、つまり押された時だけ１とか-1を返す
        }
        return 0;
    }

    public int GetInputDown1(int playerNum, int key)//key をint で渡せる関数一応範囲外ならDebug.Logを出します。
    {
        if (key < 0 || key >= MaxKey)
        {
            Debug.Log("error in GetInputDown1: keyが範囲外");
            return -2;
        }
        if (Keystatus[playerNum][key][0] != Keystatus[playerNum][key][1])//前と違ければ
        {
            return Keystatus[playerNum][key][0];//0-1 なら１を返し1-0なら０を返す、つまり押された時だけ１とか-1を返す
        }
        return 0;
    }

    public int ChangePlConkind(int playerNum, ConKind ConKind)//ConKindを変更する　変更できれば0、失敗すれば-1
    {
        Debug.Log("changeConKind");
        if (ConKind != ConKind.NOTHING)
        {
           // if (Player[playerNum].ConKind == ConKind.NOTHING)//既に登録されてなければ
          //  {
                Player[playerNum].ConKind = ConKind;
                Debug.Log(GetConKind(playerNum));
                conkind1 = Player[0].ConKind;
                conkind2 = Player[1].ConKind;
                return 0;
           // }
        }
        else //NOTHINGを代入するとき
        {
            Player[playerNum].ConKind= ConKind.NOTHING;//選びなおすときとか
            conkind1 = Player[0].ConKind;
            conkind2 = Player[1].ConKind;
            return 0;
        }

       // return -1;
    }
    public int GetInput2(int playerNum, Key key)//GetInput1した時点での入力状態を返す
    {

        switch (Player[playerNum].ConKind)//コントローラーの種類によって
        {
            case ConKind.KEYBOARD1://wasd形式　FPSみたいな左手が移動 Submit がL CancelがK
                switch (key)
                {
                    case Key.KEY_HORIZON:
                        if (Input.GetKey(KeyCode.D)) return 1;//右
                        if (Input.GetKey(KeyCode.A)) return -1;//左
                        break;
                    case Key.KEY_VERTICAL:
                        if (Input.GetKey(KeyCode.W)) return 1;
                        if (Input.GetKey(KeyCode.S)) return -1;
                        break;
                    case Key.KEY_SUBMIT:
                        if (Input.GetKey(KeyCode.L)) return 1;
                        break;

                    case Key.KEY_CANCEL:
                        if (Input.GetKey(KeyCode.K)) return 1;
                        break;
                    case Key.KEY_HOLD:
                        if (Input.GetKey(KeyCode.Space)) return 1;

                        break;
                }
                return 0;
            //break;   
            case ConKind.KEYBOARD2://矢印形式　右手が移動 SubmitがX,CancelがZ
                switch (key)
                {
                    case Key.KEY_HORIZON:
                        if (Input.GetKey(KeyCode.RightArrow)) return 1;//右
                        if (Input.GetKey(KeyCode.LeftArrow)) return -1;//左
                        break;
                    case Key.KEY_VERTICAL:
                        if (Input.GetKey(KeyCode.UpArrow)) return 1;
                        if (Input.GetKey(KeyCode.DownArrow)) return -1;
                        break;
                    case Key.KEY_SUBMIT:
                        if (Input.GetKey(KeyCode.X)) return 1;
                        break;

                    case Key.KEY_CANCEL:
                        if (Input.GetKey(KeyCode.Z)) return 1;
                        break;
                    case Key.KEY_HOLD:
                        if (Input.GetKey(KeyCode.Space)) return 1;

                        break;
                }
                return 0;
            //break;  

            case ConKind.JOYCON://joycon
                {
                    if (Player[playerNum].JoyConNum != -1)
                    {
                        GamepadState state = GamePad.GetState((GamePad.Index)Player[playerNum].JoyConNum);//入力状態

                        switch (key)
                        {
                            case Key.KEY_HORIZON:
                                if (state.LeftTrigger > 0) return 1;//右
                                if (state.LeftTrigger < 0) return -1;//左
                                break;
                            case Key.KEY_VERTICAL:
                                if (state.RightTrigger > 0) return 1;
                                if (state.RightTrigger < 0) return -1;
                                break;
                            case Key.KEY_SUBMIT:
                                if (state.B) return 1;
                                break;

                            case Key.KEY_CANCEL:
                                if (state.A) return 1;
                                break;
                            case Key.KEY_HOLD:
                                if (state.LeftShoulder) return 1;
                                if (state.RightShoulder) return 1;
                                break;
                        }

                        return 0;
                    }
                    Debug.Log("error in PlInput: ジョイコン番号が未登録");
                    break;
                }
        }
        return -2;//エラー ConKindがNOTHINGのときとか
    }

    public void Awake()//Startから内容をうつした
    {     
        Keystatus = new int[MaxPlayerNum][][];
        KeyPushcount = new double[MaxPlayerNum][];
        for (int i = 0; i < MaxPlayerNum; i++)
        {
            Keystatus[i] = new int[MaxKey][];
            KeyPushcount[i] = new double[MaxKey * 2];
            for (int j = 0; j < MaxKey; j++)
            {
                Keystatus[i][j] = new int[2];
                Keystatus[i][j][0] = new int();
                Keystatus[i][j][1] = new int();
            }
        }
        Player = new Playerinfo[MaxPlayerNum];
        Player[0] = new Playerinfo();
        Player[1] = new Playerinfo();
        //conkind1 = ConKind.KEYBOARD1;
       // conkind2 = ConKind.KEYBOARD2;
        ShowConKind();

    }
    // Use this for initialization
    void Start()
    {
        ShowConKind();
        Player[0].ConKind = conkind1;
        Player[1].ConKind = conkind2;
        Debug.Log("PlInput start");
        ShowConKind();
    }

    // Update is called once per frame
    void Update()//毎秒ボタンを監視する 
    {
        for (int i = 0; i < MaxPlayerNum; i++)
        {

            for (int j = 0; j < MaxKey; j++)
            {

                Keystatus[i][j][1] = Keystatus[i][j][0];//前の情報
                Keystatus[i][j][0] = GetInput2(i, (Key)j);//最新
            }
        }

    }
    public void ShowConKind()
    {
        Debug.Log("Player[0].ConKind is " + PlInput.GetConKind(0));
        Debug.Log("Player[1].ConKind is " + PlInput.GetConKind(1));
    }
}
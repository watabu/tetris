using UnityEngine;
using System.Collections;
using GamepadInput;
//たぶんこのままだと押してる間ずっと値を返すので面倒。
//あとでおされたときだけに変更したい。
public class PlayerInput : MonoBehaviour
{
    public enum KEY_INPUT
    {
        KEY_HORIZON,//右が１、左がー１
        KEY_VERTICAL,//上が１、下がー１
        KEY_SUBMIT,//Aボタン　右回転
        KEY_CANCEL,//Bボタン　左回転
        KEY_HOLD//Lボタン or R
    }
    public PlayerInfo.Player[] Player;

   public int GetInput(int playerNum, KEY_INPUT key)//入力があれば1or-1、なければ0を返す　もしコントローラーが登録されてなければー２
    {
        GamepadState state = GamePad.GetState((GamePad.Index)playerNum);//入力状態
        //if (Player[playerNum].ConKind == PlayerInfo.eConKind.KEYBOARD1) {
        switch (Player[playerNum].ConKind)//コントローラーの種類によって
        {
            case PlayerInfo.eConKind.KEYBOARD1://wasd形式　FPSみたいな左手が移動
                switch (key)
                {
                    case KEY_INPUT.KEY_HORIZON:
                        if (Input.GetKey(KeyCode.D)) return 1;//右
                        if (Input.GetKey(KeyCode.A)) return -1;//左
                        break;
                    case KEY_INPUT.KEY_VERTICAL:
                        if (Input.GetKey(KeyCode.W)) return 1;
                        if (Input.GetKey(KeyCode.S)) return -1;
                        break;
                    case KEY_INPUT.KEY_SUBMIT:
                        if (Input.GetKey(KeyCode.RightArrow)) return 1;
                        break;

                    case KEY_INPUT.KEY_CANCEL:
                        if (Input.GetKey(KeyCode.LeftArrow)) return 1;
                        break;
                    case KEY_INPUT.KEY_HOLD:
                        if (Input.GetKey(KeyCode.Space)) return 1;

                        break;
                }
                return 0;
            //break;   
            case PlayerInfo.eConKind.KEYBOARD2://矢印形式　右手が移動
                switch (key)
                {
                    case KEY_INPUT.KEY_HORIZON:
                        if (Input.GetKey(KeyCode.RightArrow)) return 1;//右
                        if (Input.GetKey(KeyCode.LeftArrow)) return -1;//左
                        break;
                    case KEY_INPUT.KEY_VERTICAL:
                        if (Input.GetKey(KeyCode.UpArrow)) return 1;
                        if (Input.GetKey(KeyCode.DownArrow)) return -1;
                        break;
                    case KEY_INPUT.KEY_SUBMIT:
                        if (Input.GetKey(KeyCode.X)) return 1;
                        break;

                    case KEY_INPUT.KEY_CANCEL:
                        if (Input.GetKey(KeyCode.Z)) return 1;
                        break;
                    case KEY_INPUT.KEY_HOLD:
                        if (Input.GetKey(KeyCode.Space)) return 1;

                        break;
                }
                return 0;
            //break;  
           
            case PlayerInfo.eConKind.JOYCON://joycon
                {
                    switch (key)
                    {
                        case KEY_INPUT.KEY_HORIZON:
                            if (state.LeftTrigger > 0) return 1;//右
                            if (state.LeftTrigger < 0) return -1;//左
                            break;
                        case KEY_INPUT.KEY_VERTICAL:
                            if (state.RightTrigger > 0) return 1;
                            if (state.RightTrigger < 0) return -1;
                            break;
                        case KEY_INPUT.KEY_SUBMIT:
                            if (state.B) return 1;                       
                            break;

                        case KEY_INPUT.KEY_CANCEL:
                            if (state.A) return 1;
                            break;
                        case KEY_INPUT.KEY_HOLD:
                            if (state.LeftShoulder) return 1;
                            if (state.RightShoulder) return 1;
                            break;
                    }

                    return 0;
                }
                //break;

        }
        return -2;//エラー ConKindがNOTHINGのときとか
    }

    public int ChangePlConkind(int playerNum,PlayerInfo.eConKind eConkind)//ConKindを変更する　変更できれば0、失敗すれば-1
    {
        if (eConkind != PlayerInfo.eConKind.NOTHING)
        {
            if (Player[playerNum].ConKind == 0)//既に登録されてなければ
            {
                Player[playerNum].ConKind = eConkind;
                return 0;
            }
        }
        else //NOTHINGを代入するとき
        {
            Player[playerNum].ConKind = PlayerInfo.eConKind.NOTHING;//選びなおすときとか
            return 0;
        }

        return -1;
    }
    // Use this for initialization
    void Start()
    {
        Player = new PlayerInfo.Player[PlayerInfo.MaxPlayerNum];
        //入力状態を一気に取得したほうがいいかと思ったけどまだ未実装
        /*private GamepadState[] state = new GamepadState[4];
        public void GetInputAll(GamepadState[] mState)
        {
            for(int i=0; i < 4; i++)
            {
                state[i] = GamePad.GetState((GamePad.Index)i);
            }
        }*/
    }

// Update is called once per frame
void Update()
    {

    }
}

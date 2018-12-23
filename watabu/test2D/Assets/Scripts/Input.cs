using UnityEngine;
using System.Collections;
using GamepadInput;

public class PlayerCon : MonoBehaviour
{
    public enum KEY_INPUT
    {
        KEY_HORIZON,//右が１、左がー１
        KEY_VERTICAL,//上が１、下がー１
        KEY_SUBMIT,//Aボタン
        KEY_CANCEL,//Bボタン
        KEY_HOLD//Lボタン or R
    }

    public int GetInput(int playerNum, KEY_INPUT key)//
    {
        GamepadState state = GamePad.GetState((GamePad.Index)playerNum);//入力状態
        switch (key)
        {
            case KEY_INPUT.KEY_HORIZON:
                if (state.LeftTrigger > 0) return 1;//右
                if(state.LeftTrigger <0)return -1;//左
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
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

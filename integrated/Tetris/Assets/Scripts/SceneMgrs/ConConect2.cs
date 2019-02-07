using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UIコンポーネントの使用
using GamepadInput;

public class ConConect2 : MonoBehaviour
{
    public PlInput In;
    Button back;
    Button reconect1;
    Button reconect2;
    Button enter;
    // Use this for initialization
    void Start()
    {
        In = GetComponent<PlInput>();
        back = GameObject.Find("/Canvas/Button back").GetComponent<Button>();
        reconect1 = GameObject.Find("/Canvas/Button reconect").GetComponent<Button>();
        reconect2 = GameObject.Find("/Canvas/Button reconect (1)").GetComponent<Button>();
        enter = GameObject.Find("/Canvas/Button enter").GetComponent<Button>();
        reconect1.Select();
    }
    //意図的に同じコントローラーを登録できるようにしてる（面白そうだから）
    public void Reconect1()
    {

        if (PlInput.Player[0].ConKind == PlInput.ConKind.NOTHING)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                In.ChangePlConkind(0, PlInput.ConKind.KEYBOARD1);

            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                In.ChangePlConkind(0, PlInput.ConKind.KEYBOARD2);

            }
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)i))
                {
                    In.ChangePlConkind(0, PlInput.ConKind.JOYCON);
                    //PlInput.Player[0].JoyConNum = i;
                }
            }
        }
        else if (PlInput.Player[0].ConKind != PlInput.ConKind.NOTHING)
        {
            In.ChangePlConkind(0, PlInput.ConKind.NOTHING);
            //PlInput.Player[0].JoyConNum = -1;
        }
         Debug.Log("Player[0].ConKind is " + PlInput.Player[0].ConKind);
    }

    public void Reconect2()
    {

        if (PlInput.Player[1].ConKind == PlInput.ConKind.NOTHING)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                In.ChangePlConkind(1, PlInput.ConKind.KEYBOARD1);

            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                In.ChangePlConkind(1, PlInput.ConKind.KEYBOARD2);

            }
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)i))
                {
                    In.ChangePlConkind(1, PlInput.ConKind.JOYCON);
                    //PlInput.Player[1].JoyConNum = i;
                }
            }
        }
        else if (PlInput.Player[1].ConKind != PlInput.ConKind.NOTHING)
        {
            In.ChangePlConkind(1, PlInput.ConKind.NOTHING);
           // PlInput.Player[1].JoyConNum = -1;
        }
         Debug.Log("Player[1].ConKind is " + PlInput.Player[1].ConKind);
    }
    // Update is called once per frame
    void Update()
    {

    }
}

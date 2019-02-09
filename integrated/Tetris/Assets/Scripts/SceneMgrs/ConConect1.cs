using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UIコンポーネントの使用
using GamepadInput;
public class ConConect1 : MonoBehaviour
{
    public PlInput In;
    public Button back;
    public Button reconect;
    public Button enter;
    // Use this for initialization
    void Start()
    {
        In = GetComponent<PlInput>();
        back = GameObject.Find("/Canvas/Button back").GetComponent<Button>();
        reconect = GameObject.Find("/Canvas/Button reconect").GetComponent<Button>();
        enter = GameObject.Find("/Canvas/Button enter").GetComponent<Button>();
        reconect.Select();
    }
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
        else if(PlInput.Player[0].ConKind!=PlInput.ConKind.NOTHING)
        {
            In.ChangePlConkind(0, PlInput.ConKind.NOTHING);
            //PlInput.Player[0].JoyConNum = -1;
        }
       // Debug.Log("Player[0].ConKind is " + PlInput.Player[0].ConKind);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

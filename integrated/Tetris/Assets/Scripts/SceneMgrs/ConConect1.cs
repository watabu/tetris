using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UIコンポーネントの使用
using GamepadInput;
public class ConConect1 : MonoBehaviour
{
    public PlInput In;
    Button back;
    Button reconect;
    Button enter;
    public Sprite JoyCon, Nothing, KeyBoard1, KeyBoard2;
    private AudioSource SE_Submit;
    private AudioSource SE_Cancel;
    // Use this for initialization
    void Awake()
    {
        In = GetComponent<PlInput>();
        back = GameObject.Find("/Canvas/Button back").GetComponent<Button>();
        reconect = GameObject.Find("/Canvas/Button reconect").GetComponent<Button>();
        enter = GameObject.Find("/Canvas/Button enter").GetComponent<Button>();
        reconect.Select();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        SE_Submit = audioSources[1];
        SE_Cancel = audioSources[0];

    }

    public void Reconect1()
    {
        var ConImg = GameObject.Find("/Canvas/Panel/ConImage").GetComponent<Image>();
        if (PlInput.Player[0].ConKind == PlInput.ConKind.NOTHING)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                In.ChangePlConkind(0, PlInput.ConKind.KEYBOARD1);
                SE_Submit.PlayOneShot(SE_Submit.clip);
                ConImg.sprite = KeyBoard1;

            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                In.ChangePlConkind(0, PlInput.ConKind.KEYBOARD2);
                SE_Submit.PlayOneShot(SE_Submit.clip);
                ConImg.sprite = KeyBoard2;
            }
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)i))
                {
                    In.ChangePlConkind(0, PlInput.ConKind.JOYCON);
                    PlInput.Player[0].JoyConNum = i;
                    SE_Submit.PlayOneShot(SE_Submit.clip);
                    ConImg.sprite = JoyCon;
                }
            }
        }
        else if (PlInput.Player[0].ConKind != PlInput.ConKind.NOTHING)
        {
            In.ChangePlConkind(0, PlInput.ConKind.NOTHING);
            PlInput.Player[0].JoyConNum = -1;
            SE_Cancel.PlayOneShot(SE_Cancel.clip);
            ConImg.sprite = Nothing;
        }
        // Debug.Log("Player[0].ConKind is " + PlInput.Player[0].ConKind);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

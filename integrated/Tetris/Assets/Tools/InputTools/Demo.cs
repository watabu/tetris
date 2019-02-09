using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour
{
    public PlInput Input;
    // Use this for initialization
    void Start()
    {
        Input = GetComponent<PlInput>();//InputだとunityのInputと混じってめんどそうなので適当に名前いじってください。
        //ConKindは初期状態ではNOTHINGにしてあるので変更する
        Input.ChangePlConkind(0, PlInput.ConKind.KEYBOARD1);//移動がwasd,回転が←、→、ホールドがスペース
        Input.ChangePlConkind(1, PlInput.ConKind.JOYCON);//移動が矢印、ｚｘで回転、ホールドがスペース
        PlInput.Player[1].JoyConNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetInputDown(0, PlInput.Key.KEY_SUBMIT) == 1)
        {
            Debug.Log("Player0 Submitted");
        }
        if (Input.GetInputDown1(1, 2) == 1)
        {
            Debug.Log("Player1 Submitted");
        }

    }
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 20, Screen.width, Screen.height));

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        DrawLabels();

        for (int i = 0; i < PlInput.MaxPlayerNum; i++)
            DrawState(i);
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.EndArea();

    }
    void DrawState(int controller)//コントローラ番号
    {
        GUILayout.Space(45);

        GUILayout.BeginVertical();

        // buttons
        GUILayout.Label("Player " + (controller+1));
        GUILayout.Label("" + Input.GetInput(controller,PlInput.Key.KEY_SUBMIT));
        GUILayout.Label("" + Input.GetInput(controller, PlInput.Key.KEY_CANCEL));
        GUILayout.Label("" + Input.GetInput1(controller, 4));

        GUILayout.Label("");

        // Axes
        GUILayout.Label("" + Input.GetInput(controller, PlInput.Key.KEY_HORIZON));
        GUILayout.Label("" + Input.GetInput1(controller, 1));
        
        //GUILayout.EndArea();
        GUILayout.EndVertical();

    }

    void DrawLabels()
    {
        //GUILayout.BeginArea(new Rect(30, 0, width - 30, Screen.height));

        GUILayout.BeginVertical();
        // buttons
        GUILayout.Label(" ", GUILayout.Width(80));
        GUILayout.Label("A(Submit)");
        GUILayout.Label("B(Cancel)");
        GUILayout.Label("HOLD");
 
        GUILayout.Label("");

        GUILayout.Label("XAxis");
        GUILayout.Label("YAxis");

        GUILayout.EndVertical();

    }
}


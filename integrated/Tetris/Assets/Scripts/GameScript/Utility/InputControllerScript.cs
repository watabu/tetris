using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//3/1 Inputの変更、操作性の向上をしました
public class InputControllerScript : MonoBehaviour {
    [Header("Input Script Reference")]
    public PlInput input;
    private void Awake()
    {
    }
    // Use this for initialization
    void Start ()
    {
        //入力方法の初期化
        //今はとりあえずConKind.KEYBOARD1で
        input.ChangePlConkind(0, PlInput.ConKind.KEYBOARD1);//移動がwasd,回転がK、L、ホールドがスペース
        input.ChangePlConkind(1, PlInput.ConKind.KEYBOARD1);//移動がwasd,回転がK、L、ホールドがスペース
        PlInput.Player[1].JoyConNum = 1;
        PlInput.Player[1].JoyConNum = 1;
    }

    // Update is called once per frame
    void Update () {

    }
    //playernumのIDの入力の方向を取得する
    public Vector3Int GetInputDirection(int playerNum)
    {
        Vector3Int direction = new Vector3Int();
        /*     direction.x=input.GetInput2(playerNum,PlInput.Key.KEY_HORIZON);
             direction.y=input.GetInput2(playerNum,PlInput.Key.KEY_VERTICAL);
             */
        direction.x = input.GetInputdelta(playerNum, PlInput.Key.KEY_HORIZON,0.0625);
        direction.y = input.GetInputdelta2(playerNum, PlInput.Key.KEY_VERTICAL,0.125);
        direction.z=0;
        return direction;
    }
    //ボタンが押されていたら1を返す
    public int GetInput(int playerNum,PlInput.Key key)
    {
        return input.GetInput2(playerNum, key);
    }
    //ボタンが押された瞬間だったら1or-1を返す
    public int GetInputDown(int playerNum, PlInput.Key key)
    {    
        return input.GetInputDown(playerNum, key);
    }

}

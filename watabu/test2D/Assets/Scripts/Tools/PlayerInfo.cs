using UnityEngine;
using System.Collections;

public class PlayerInfo: MonoBehaviour
{
    public const int MaxPlayerNum = 2;//最大プレイ人数
    public enum eConKind
    {
           NOTHING,//初期状態
           KEYBOARD1,//キーボード１移動がFPS使用のwasd,回転が←、→、ホールドがスペース
           KEYBOARD2,//キーボード２移動が矢印、ｚｘで回転、ホールドがスペース
           JOYCON//joycon
    }

    public class Player//プレイヤーの情報を持っておく 他に追加するかもしれないので一応クラスにしておく
    {
      
        public eConKind ConKind ;//コントローラーの種類
   //     int playerNum;//通常配列の添え字と同じプレイヤー番号になるからいらない？
        Player()
        {
            ConKind = eConKind.NOTHING;
        }
    }//

}

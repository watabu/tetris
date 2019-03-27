using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用
using UnityEngine.Events;

//あらかじめ決められた長方形の範囲のゲームボードの
//セルの列消去、ペナルティの生成を行う
//
//2/19 GameBoardScript.csと分離
//2/22  OnMinoEracedを追加 ミノを消したとき実行するイベントを管理
//     実行される関数はvoid Func(int)の形
//     引数には消したミノの列が渡される
//      ミノを消すタイミングをミノが止まった時に修正
//     ミノを動かしている間に勝手に消されなくなった
//2/23  OnMinoFilledを追加 ミノが埋まった瞬間に実行するイベント
//     これでミノが消える前にエフェクトを生成する処理を管理できるように
public class GameBoardModifier : MonoBehaviour
{
    //たぶんこのクラス内でのみ使うコールバック用クラス
    [System.Serializable]
    public class ModifierCallBack : UnityEngine.Events.UnityEvent<int,GameObject>{}

    [Header("Object refelence")]
    public MinoControllerScript minoController;

    [Header("Call Back Function"), SerializeField]
    UnityEvent OnMinoFilled;//ミノが１列埋まった瞬間実行する関数を格納する変数
    [SerializeField]
    ModifierCallBack OnMinoEraced;//ミノを消したあと実行する関数を格納する変数

    //public MinoControllerScript minoController;
    GameBoardScript gameBoardScript;

    Vector3Int leftBottomCood;//ボードの左下の座標
    int height;//ボードの高さ
    int width;//ボードの幅

    private void Awake()
    {
        if (OnMinoEraced == null) OnMinoEraced = new ModifierCallBack(); //イベント・インスタンスの作成
    }

    // Use this for initialization
    void Start()
    {
        gameBoardScript = GetComponent<GameBoardScript>();
        SetBoardRange(gameBoardScript.edgeCellCood[0],gameBoardScript.height, gameBoardScript.width);
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    void SetBoardRange(Vector3Int leftBottomCood_, int height_, int width_)//ボードの範囲を指定する
    {
        leftBottomCood = leftBottomCood_;
        height = height_;
        width = width_;
    }
    //ミノが一列すべてうまったかどうかを確認する
    //ミノの操作が止まった時実行する
    public void CheckLine()
    {
        var yList = new List<int>();//どのy座標が埋まったかのリスト
        for (int y = 0; y < height; y++)
        {
            int xCount = 0;
            for (int x = 0; x < width; x++)
                if (!gameBoardScript.IsEmpty(BoardLayer.Default, leftBottomCood.x + x, leftBottomCood.y + y))//もし空白がなかったら
                    xCount++;
            if (xCount == width)//１行すべて埋まってたらリストに追加
                yList.Add(y);
        }
        if (yList.Count == 0)
        {
            return;//もし埋まっている列がなかったら終了
        }
        OnMinoFilled.Invoke();

        foreach (var yLaw in yList)
        {
            //minoController.RemoveCells();
            for (int x = 0; x < width; x++)
                ClearCell(leftBottomCood.x + x, leftBottomCood.y + yLaw);//yLaw列のセルを全消去
            for (int y2 = yLaw; y2 < height; y2++)
                for (int x2 = 0; x2 < width; x2++)
                    gameBoardScript.MoveCell(BoardLayer.Default, leftBottomCood.x + x2, leftBottomCood.y + y2, 0, -1);//上のセルを下に移動
        }
        OnMinoEraced.Invoke(yList.Count,minoController.GetMino());
        //return true;
    }

    public void GenerateOjama(int playerNum)
    {


    }
    

    
    public void ClearCell()//ゲーム盤のミノを全消去する
    {
    }
    public void ClearCell(Vector2Int cood) { ClearCell(cood.y, cood.x); }
    public void ClearCell(int x, int y) { gameBoardScript.GetLayer(BoardLayer.Default).SetTile(new Vector3Int(x, y, 0), null); }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List用
using UnityEngine.Events;
using UnityEngine.Tilemaps;

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
//3/30  ModifierCallBackの第２引数をGameObjectからMinoControllerScriptに変更
//     どのミノで消したかの情報を渡すのではなく、消したときのコントローラーの状態を渡すように
public class GameBoardModifier : MonoBehaviour
{
    //たぶんこのクラス内でのみ使うコールバック用クラス
    [System.Serializable]
    public class ModifierCallBack : UnityEngine.Events.UnityEvent<int,MinoControllerScript>{}

    [Header("Object refelence")]
    public Tile Ojama;//
    public MinoControllerScript minoController;

    [Header("Call Back Function"), SerializeField]
    ModifierCallBack OnMinoFilled;//ミノが１列埋まった瞬間実行する関数を格納する変数
    [SerializeField]
    ModifierCallBack OnMinoEraced;//ミノを消したあと実行する関数を格納する変数

    //public MinoControllerScript minoController;
    GameBoardScript gameBoardScript;

    Vector3Int leftBottomCood;//ボードの左下の座標
    Vector3Int rightTopCood;//ボードの右上の座標

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
        SetBoardRange(gameBoardScript.edgeCellCood[0], gameBoardScript.edgeCellCood[1], gameBoardScript.height, gameBoardScript.width);
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    void SetBoardRange(Vector3Int leftBottomCood_, Vector3Int rightTopCood_, int height_, int width_)//ボードの範囲を指定する
    {
        leftBottomCood = leftBottomCood_;
        rightTopCood = rightTopCood_;
        height = height_;
        width = width_ +1;
    }
    //ミノが一列すべてうまったかどうかを確認する
    //ミノの操作が止まった時実行する
    public void CheckLine()
    {
        var yList = new List<int>();//どのy座標が埋まったかのリスト
        for (int y = 0; y < height; y++)
        {
            //Debug.Log("<color=blue>CheckLine</color>");
            int xCount = 0;
            for (int x = 0; x < width; x++)
                if (!gameBoardScript.IsEmpty(BoardLayer.Default, leftBottomCood.x + x, leftBottomCood.y + y))//もし空白がなかったら
                    xCount++;
            if (xCount == width)//１行すべて埋まってたらリストに追加
                yList.Add(y);
        }
        if (yList.Count == 0)
        {
            OnMinoEraced.Invoke(yList.Count, minoController);//Renが途切れることを知らせる
            return;//もし埋まっている列がなかったら終了
        }
        int n = -1;

        OnMinoFilled.Invoke(yList.Count, minoController);
        foreach (var yLaw in yList)
        {
            n++;//下げられた消された列のyと下げられてないyLawを合わせるための補正
            //minoController.RemoveCells();
            for (int x = 0; x < width; x++)
                ClearCell(leftBottomCood.x + x, leftBottomCood.y + yLaw - n );//yLaw列のセルを全消去
            for (int y2 = yLaw-n; y2 < height; y2++)
                for (int x2 = 0; x2 < width; x2++)
                    gameBoardScript.MoveCell(BoardLayer.Default, leftBottomCood.x + x2, leftBottomCood.y + y2, 0, -1);//上のセルを下に移動
        }
        OnMinoEraced.Invoke(yList.Count,minoController);

        //return true;
    }
    //オジャマミノを自分のボードに生成する
    //ojamaSize どれだけの高さか
    //holeX いずれ盤面の情報を取得するかもしれないのでここでholeXを決める
    public void GenerateOjama(int ojamaSize)
    {
        int holeX;
        if (ojamaSize > height)

        {
            //そのうち限界まで積み上げて負け処理に
            Debug.LogError("Ojama size is bigger than board size");
            return;
        }
        holeX = Random.Range(0, width + 1);
        Debug.Log("<color=2f2>holeX is"+ holeX+"</color>");
        for (int y = rightTopCood.y; y >= leftBottomCood.y; y--)
            for (int x = 0; x < width; x++)
                gameBoardScript.MoveCell(BoardLayer.Default, leftBottomCood.x + x, y, 0, ojamaSize);//上のセルを下に移動

        for (int y = 0; y < ojamaSize; y++)
        {
            for (int x = 0; x < width; x++)
                //Sleepとか使って下からだんだんとせりあがってくるようにしたいが非同期の遅延はわからなかった
                if (x != holeX)
                {
                    gameBoardScript.SetCell(BoardLayer.Default, Ojama, leftBottomCood.x + x, leftBottomCood.y + y);
                }
        }
    }



    public void ClearCell()//ゲーム盤のミノを全消去する
    {
    }
    public void ClearCell(Vector2Int cood) { ClearCell(cood.y, cood.x); }
    public void ClearCell(int x, int y) { gameBoardScript.GetLayer(BoardLayer.Default).SetTile(new Vector3Int(x, y, 0), null); }

}

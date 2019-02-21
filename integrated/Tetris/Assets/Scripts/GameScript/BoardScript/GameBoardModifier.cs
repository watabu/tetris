using UnityEngine;
using System.Collections;
using System.Collections.Generic;//Lisy用

//あらかじめ決められた長方形の範囲のゲームボードの
//セルの列消去、ペナルティの生成を行う
//2/19 GameBoardScript.csと分離
public class GameBoardModifier : MonoBehaviour
{
    //public MinoControllerScript minoController;
    GameBoardScript gameBoardScript;

    Vector3Int leftBottomCood;
    int height;
    int width;

    // Use this for initialization
    void Start()
    {
        gameBoardScript = GetComponent<GameBoardScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLine();
    }
    public void SetBoardRange(Vector3Int leftBottomCood_, int height_, int width_)
    {
        leftBottomCood = leftBottomCood_;
        height = height_;
        width = width_;
    }
    bool CheckLine()//ミノが一列すべてうまったかどうか(内部処理用)
    {
        var list = new List<int>();
        for (int y = 0; y < height; y++)
        {
            int xCount = 0;
            for (int x = 0; x < width; x++)
                if (!gameBoardScript.IsEmpty(BoardLayer.Default, leftBottomCood.x + x, leftBottomCood.y + y))//もし空白がなかったら
                    xCount++;
            if (xCount == width)//１行すべて埋まってたら
                list.Add(y);
        }
        if (list.Count == 0)//もし埋まっている列がなかったらfalseを返して終了
            return false;

        foreach (var yLaw in list)
        {
            //minoController.RemoveCells();
            for (int x = 0; x < width; x++)
                ClearCell(leftBottomCood.x + x, yLaw);//それまでの列のセルを消去
            for (int y2 = yLaw; y2 < height; y2++)
                for (int x2 = 0; x2 < width; x2++)
                    gameBoardScript.MoveCell(BoardLayer.Default, leftBottomCood.x + x2, leftBottomCood.y + y2, 0, -1);//上のセルを下に移動
        }
        gameBoardScript.GetNextMino();
        return true;
    }

    public void ClearCell()//ゲーム盤のミノを全消去する
    {
    }
    public void ClearCell(Vector2Int cood) { ClearCell(cood.y, cood.x); }
    public void ClearCell(int x, int y) { gameBoardScript.GetLayer(BoardLayer.Default).SetTile(new Vector3Int(x, y, 0), null); }

}

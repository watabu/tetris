using UnityEngine;
using System.Collections;

//あらかじめ決められた長方形の範囲のゲームボードの
//セルの列消去、ペナルティの生成を行う
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
        int cellYCount = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                if (gameBoardScript.IsEmpty(BoardLayer.Default, leftBottomCood.x + x, leftBottomCood.y + y))//もし空白があったら
                {
                    if (cellYCount == 0) return false;
                    //minoController.RemoveCells();
                    for (int y2 = 0; y2 < cellYCount; y2++)
                        for (int x2 = 0; x2 < width; x2++)
                            ClearCell(leftBottomCood.x + x2, leftBottomCood.y + y2);//それまでの列のセルを消去
                    for (int y2 = cellYCount; y2 < height; y2++)
                        for (int x2 = 0; x2 < width; x2++)
                            gameBoardScript.MoveCell(BoardLayer.Default, leftBottomCood.x + x2, leftBottomCood.y + y2, 0, -cellYCount);//上のセルを下に移動
                    gameBoardScript.GetNextMino();
                    return true;
                }
            cellYCount++;
        }
        return false;
    }

    public void ClearCell()//ゲーム盤のミノを全消去する
    {
    }
    public void ClearCell(Vector2Int cood) { ClearCell(cood.y, cood.x); }
    public void ClearCell(int x, int y) { gameBoardScript.GetLayer(BoardLayer.Default).SetTile(new Vector3Int(x, y, 0), null); }

}

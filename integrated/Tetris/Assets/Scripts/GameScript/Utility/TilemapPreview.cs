using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

//https://qiita.com/keidroid/items/5d0e1bafc2c1d9a467e9
[CustomPreview(typeof(Tilemap))]
public class TilemapPreview : ObjectPreview
{
    //プレビューの1マスのサイズ
    private static readonly float PREVIEW_CELL_SIZE = 24.0f;
    //プレビューの1マスのマージン
    private static readonly int PREVIEW_MARGIN = 1;
    //タイルがない場合のSpriteのResources下パス
    private static readonly string NO_TILE_SPRITE_PATH = "TilemapPreview/red";
    //基準点の場合のSpriteのResources下パス
    private static readonly string BASE_POSITION_SPRITE_PATH = "TilemapPreview/green";
    //プレビューのタイトル
    private static readonly GUIContent previewTitle = new GUIContent("Tilemap");

    private Tilemap tilemap = null;

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override GUIContent GetPreviewTitle()
    {
        return previewTitle;
    }

    public override void Initialize(Object[] targets)
    {
        base.Initialize(targets);

        foreach (Tilemap target in targets)
        {
            tilemap = target;
            break;
        }
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        Vector3Int origin = tilemap.origin;
        Vector3Int size = tilemap.size;
        List<GUIContent> contents = new List<GUIContent>();

        for (int y = size.y - 1; y >= 0; y--)
        { //3D座標からUI座標にするためyは逆
            for (int x = 0; x < size.x; x++)
            {
                Vector3Int gridPos = new Vector3Int(origin.x + x, origin.y + y, 0);
                Sprite sprite = tilemap.GetSprite(gridPos);

                //タイルが設定されていない場合
                if (sprite == null)
                {
                    sprite = Resources.Load<Sprite>(NO_TILE_SPRITE_PATH);
                }
                GUIContent content = new GUIContent(string.Format("{0},{1}", gridPos.x, gridPos.y),
                                                    AssetPreview.GetAssetPreview(sprite));
                contents.Add(content);
            }
        }

        GUIStyle style = new GUIStyle();
        style.fixedWidth = PREVIEW_CELL_SIZE;
        style.fixedHeight = PREVIEW_CELL_SIZE;
        style.margin = new RectOffset(PREVIEW_MARGIN, PREVIEW_MARGIN, PREVIEW_MARGIN, PREVIEW_MARGIN);
        style.imagePosition = ImagePosition.ImageOnly;

        GUI.SelectionGrid(r, -1, contents.ToArray(), size.x, style);

        Sprite basePositionSprite = Resources.Load<Sprite>(BASE_POSITION_SPRITE_PATH);
        Rect center = new Rect(r.x - origin.x * PREVIEW_CELL_SIZE - origin.x * PREVIEW_MARGIN,
                               r.y - origin.y * PREVIEW_CELL_SIZE - origin.y * PREVIEW_MARGIN,
                               PREVIEW_CELL_SIZE, PREVIEW_CELL_SIZE);
        GUI.DrawTexture(center, basePositionSprite.texture);
    }
}
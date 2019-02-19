using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UsefulFunctions : MonoBehaviour
{

    //originaiを複製する関数
    //座標や向きは設定されていない(0のまま)
    public static GameObject CloneObject(GameObject original)
    {
        return Instantiate(original, Vector3.zero, Quaternion.identity);
    }

    //親のScaleに依存せず、グローバルScaleのもとでオブジェクトを移動させる
    public static void TransformNonScale(ref Transform transform, Vector3 offset)
    {
        Vector3 lossScale = transform.lossyScale;
        transform.position += new Vector3(offset.x / lossScale.x,
            offset.y / lossScale.y,
            offset.z / lossScale.z);
    }

    public static void SceneChange(String sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void RotateArrayClockwise<Type>(Type[,] array)
    {
        // 引数の2次元配列 array を時計回りに回転させたものを返す
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        var t = new Type[cols, rows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                t[j, rows - i - 1] = array[i, j];
            }
        }
    }
    public static void RotateArrayAnticlockwise<Type>(Type[,] array)
    {
        // 引数の2次元配列 array を反時計回りに回転させたものを返す
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        var t = new Type[cols, rows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                t[cols - j - 1, i] = array[i, j];
            }
        }
    }

}
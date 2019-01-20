using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UsefulFunctions : MonoBehaviour {

    //originaiを複製する関数
    //座標や向きは設定されていない(0のまま)
    public static GameObject CloneObject(GameObject originai)
    {
        return Instantiate(originai, Vector3.zero,Quaternion.identity);
    }

    //親のScaleに依存せず、グローバルScaleのもとでオブジェクトを移動させる
    public static void TransformNonScale(ref Transform transform,Vector3 offset)
    {
        Vector3 lossScale = transform.lossyScale;
        transform.position += new Vector3(offset.x/ lossScale.x,
            offset.y / lossScale.y, 
            offset.z / lossScale.z);
    }

    public static void SceneChange(String sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}

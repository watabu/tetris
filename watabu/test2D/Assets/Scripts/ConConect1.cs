using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UIコンポーネントの使用

public class ConConect1 : MonoBehaviour
{
    Button back;
    Button reconect;
    Button enter;
    // Use this for initialization
    void Start()
    {
        back = GameObject.Find("/Canvas/Button back").GetComponent<Button>();
        reconect = GameObject.Find("/Canvas/Button reconect").GetComponent<Button>();
        enter = GameObject.Find("/Canvas/Button enter").GetComponent<Button>();
        reconect.Select();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

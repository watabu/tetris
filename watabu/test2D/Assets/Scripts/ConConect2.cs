using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UIコンポーネントの使用


public class ConConect2 : MonoBehaviour
{
    Button back;
    Button reconect1;
    Button reconect2;
    Button enter;
    // Use this for initialization
    void Start()
    {
        back = GameObject.Find("/Canvas/Button back").GetComponent<Button>();
        reconect1 = GameObject.Find("/Canvas/Button reconect").GetComponent<Button>();
        reconect2 = GameObject.Find("/Canvas/Button reconect (1)").GetComponent<Button>();
        enter = GameObject.Find("/Canvas/Button enter").GetComponent<Button>();
        reconect1.Select();
    }
    // Update is called once per frame
    void Update()
    {

    }
}

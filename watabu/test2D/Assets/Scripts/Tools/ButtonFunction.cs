using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//色々なシーンで使うボタンの機能
public class ButtonFunction : MonoBehaviour
{
    public void MoveToConConect1()//一人用画面へ
    {
        SceneManager.LoadScene("ConConect1");
    }
    public void MoveToConConect2()//二人用画面へ
    {
        SceneManager.LoadScene("ConConect2");
    }
    public void MoveToTitle()
    {
        SceneManager.LoadScene("Title");
    }
    public void MoveToSetting()
    {
        SceneManager.LoadScene("Setting");
    }


}

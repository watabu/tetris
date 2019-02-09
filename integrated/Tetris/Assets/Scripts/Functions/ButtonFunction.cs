using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//色々なシーンで使うボタンの機能
public class ButtonFunction : MonoBehaviour
{
    AudioSource SE_Submit;
    AudioSource SE_Cancel;

    private void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        SE_Submit = audioSources[1];
        SE_Cancel = audioSources[0];
    }

    public void MoveToConConect1()//一人用画面へ
    {
        SceneManager.LoadScene("ConConect1");
        SE_Submit.PlayOneShot(SE_Submit.clip);
    }
    public void MoveToConConect2()//二人用画面へ
    {
        SceneManager.LoadScene("ConConect2");
        SE_Submit.PlayOneShot(SE_Submit.clip);
    }
    public void MoveToTitle()
    {
        SceneManager.LoadScene("Title");
        SE_Cancel.PlayOneShot(SE_Cancel.clip);
    }
    public void MoveToSetting()
    {
        SceneManager.LoadScene("Setting");
        SE_Submit.PlayOneShot(SE_Submit.clip);
    }


}

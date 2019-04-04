using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//色々なシーンで使うボタンの機能
public class ButtonFunction : MonoBehaviour
{
     AudioSource SE_Submit;
     AudioSource SE_Cancel;
    public Button button;
    private void Awake()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        Debug.Log(audioSources[0]);
        Debug.Log(audioSources[1]);
        SE_Submit = audioSources[1];
       SE_Cancel = audioSources[0];
    }
    
    public void SetSelected()//ボタン用
    {
        button.Select();
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
    public void MoveToGame()
    {
        SceneManager.LoadScene("Game"); 
        SE_Submit.PlayOneShot(SE_Submit.clip);
    }
    public void _SE_Submit()
    {
        SE_Submit.PlayOneShot(SE_Submit.clip);
    }
    public void _SE_Cancel()
    {
        SE_Submit.PlayOneShot(SE_Cancel.clip);
    }
}

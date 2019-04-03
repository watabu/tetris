using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Buttonクラスのスクリプトの参照用
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        GameObject exit = transform.Find("Exit").gameObject;
        exit.GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("Title"); });
        GameObject continuee = transform.Find("Continue").gameObject;
        continuee.GetComponent<Button>().onClick.AddListener(() => { FadeOut(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }
    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
}

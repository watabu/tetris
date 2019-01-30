using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UIコンポーネントの使用

public class Title : MonoBehaviour
{

    Button Player1;
    Button Player2;
    public AudioClip audioClip;
    AudioSource audioSource;


    // Use this for initialization
    void Start()
    {
        Player1 = GameObject.Find("/Canvas/1 Player").GetComponent<Button>();
        Player2 = GameObject.Find("/Canvas/2 Player").GetComponent<Button>();
        Player1.Select();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.Play();
    }
}


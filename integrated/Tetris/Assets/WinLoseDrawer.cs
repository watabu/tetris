using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultState
{
    Null,
    Win1P,
    Win2P,
    Draw,
}

public class WinLoseDrawer : MonoBehaviour
{

    public GameObject winLogoPrefab;
    public GameObject loseLogoPrefab;

    public Transform originCood1P;
    public Transform originCood2P;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowResult(ResultState result)
    {
        switch (result)
        {
            case ResultState.Win1P:
                Instantiate(winLogoPrefab, originCood1P.position, Quaternion.identity).transform.SetParent(transform);
                Instantiate(loseLogoPrefab, originCood2P.position, Quaternion.identity).transform.SetParent(transform);
                break;
            case ResultState.Win2P:
                Instantiate(loseLogoPrefab, originCood1P.position, Quaternion.identity).transform.SetParent(transform);
                Instantiate(winLogoPrefab, originCood2P.position, Quaternion.identity).transform.SetParent(transform);
                break;
            case ResultState.Draw:
                break;
        }
    }

    public void HideResult()
    {
        foreach(Transform child in transform)//var にするとエラー
        {
            Destroy(child.gameObject);
        }
    }

}

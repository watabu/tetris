using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
//RENを画面に表示する
public class RenScript : MonoBehaviour
{
    public TextMeshProUGUI RenNum;
    
    public void ChangeRen(int ren)
    {
        RenNum.text = ren.ToString() + "Ren";
        if (ren <=0)
        {
            RenNum.enabled = false;
        }
        else
        {
            RenNum.enabled = true;
        }
        Debug.Log(RenNum.text);
    }

    private void Awake()
    {
        RenNum.enabled = false;        
    }
    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

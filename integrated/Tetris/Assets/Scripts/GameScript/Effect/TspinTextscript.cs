using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class TspinTextscript : MonoBehaviour
{
    public TextMeshProUGUI TspinText;

    public void ChangeTspintext(int n)
    {
        if (n == 0)
        {
            TspinText.enabled = false;
            return;
        }
        else if (n == 1)
            TspinText.text = "Tspin\nSingle";
        else if (n == 2)
            TspinText.text = "Tspin\nDouble";
        else if (n == 3)
            TspinText.text = "Tspin\nTriple";
        TspinText.enabled = true;
    }
    private void Awake()
    {
        TspinText.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapBoot : MonoBehaviour
{
    public Text amtText;
    public Image amtImg;
    float progress = 0;

    public void StartBooting()
    {
        InvokeRepeating("IncreaseAmt", 0, 0.01f);
    }

    void IncreaseAmt()
    {
        if (progress < 1)
        {
            progress += 0.01f;
        }
        else
        {
            progress = 1;

            CancelInvoke("IncreaseAmt");
        }
        amtImg.fillAmount = progress;
        amtText.text = (progress * 100).ToString("0") + "%";
    }
}

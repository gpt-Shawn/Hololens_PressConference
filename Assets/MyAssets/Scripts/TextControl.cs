using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextControl : MonoBehaviour
{
    public GameObject ScanFrameImage;
    public GameObject text;
    // Start is called before the first frame update
    
    void Start()
    {
        
        Invoke("ShowText", 1.5f);
    }

    public void ShowText()
    {
        text.SetActive(true);
    }

    public void HideText()
    {
        text.SetActive(false);
    }
}

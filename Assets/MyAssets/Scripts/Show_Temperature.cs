using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Show_Temperature : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float TempValue;
    public Image TempImage;
    private Text TemperText;


    // Start is called before the first frame update
    void Start()
    {
        TemperText = gameObject.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        //string to float
        if (TemperText.text.Length > 2) 
        {
            try
            {
                TempValue = float.Parse(TemperText.text.Substring(0, TemperText.text.Length - 2));
            }
            catch {
                TempValue = 20;
            }

            if (0 <= TempValue && TempValue < 20)
            {
                var temp_color = new Color32(0, 255,
                                                (byte)TempValue.Remap(0, 20, 255, 0),
                                                255);
                TemperText.color = temp_color;
                TempImage.color  = temp_color;
            }
            else if (20 <= TempValue && TempValue < 30)
            {
                var temp_color = new Color32((byte)TempValue.Remap(20, 30, 0, 255),
                                                255,
                                                0,
                                                255);
                TemperText.color = temp_color;
                TempImage.color = temp_color;
            }
            else if (30 <= TempValue && TempValue < 50)
            {
                var temp_color = new Color32(255,
                                                (byte)TempValue.Remap(30, 50, 255, 0),
                                                0,
                                                255);
                TemperText.color = temp_color;
                TempImage.color = temp_color;
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    public Text fpsText;
    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private bool toggle = true;

    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!toggle)
            {
                fpsText.enabled = true;
                toggle = true;
            }
            else
            {
                fpsText.enabled = false;
                toggle = false;
            }
        }

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:0} FPS", fps);
            fpsText.text = format;

            if (fps < 30)
                fpsText.color = Color.yellow;
            else
                if (fps < 10)
                fpsText.color = Color.red;
            else
                fpsText.color = Color.green;
            //	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}

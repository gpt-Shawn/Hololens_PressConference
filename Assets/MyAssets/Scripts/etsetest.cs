using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class etsetest : MonoBehaviour
{
    public Text text;

    bool nextDisable;
    bool nextEnable;
    bool countdown;
    float timer = 5;

    // Start is called before the first frame update
    void Start()
    {
        countdown = true;
        nextDisable = true;
        Invoke("DisableCam", 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
            }
        }

        if (nextDisable)
        {
            text.text = "關閉攝影機：" + timer.ToString("0.0") + " 秒";
        }

        if (nextEnable)
        {
            text.text = "開啟攝影機：" + timer.ToString("0.0") + " 秒";
        }
    }

    void DisableCam()
    {
        CameraDevice.Instance.Deinit();
        CameraDevice.Instance.Stop();
        timer = 5;
        nextDisable = false;
        nextEnable = true;
        text.color = Color.green;
        Invoke("ReEnableCam", 5);
    }

    void ReEnableCam()
    {
        CameraDevice.Instance.Init();
        CameraDevice.Instance.Start();
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        timer = 5;
        nextDisable = true;
        nextEnable = false;
        text.color = Color.red;
        Invoke("DisableCam", 5);
    }
}

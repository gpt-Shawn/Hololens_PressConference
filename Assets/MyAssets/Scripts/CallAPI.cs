using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Vuforia;
using UnityEngine.XR.WSA.Input;
using HoloToolkit.Unity.InputModule;

public class CallAPI : MonoBehaviour
{
    public GameObject[] offices;
    public GameObject retreat;
    public Text qrText;
    public Text VuforiaText;
    public Text resultText;
    public QRCodeDecodeControllerForWSA qrcodeController;
    //public SonarFxSwitcher switcher;
    public ScannerEffectDemo scanner;


    bool countdown; //撤退指令倒數
    float timer; //撤退指令倒數
    float nextUpdate;
    public static int floorValue = 0;
    public GameObject ScanFrame;
    public GameObject ScanBar;
    public static string PK;
    public GameObject PathManager;
    [TextArea]
    public string PersonPK;
    public static bool EnableQRScan;
    

    //接收 QRCode 資訊
    string url_QR = "http://192.168.99.240:8080/stsp/ar/qrCode";
    string posName = "position";

    //回傳撤退狀態
    string url_retreat = "http://192.168.99.240:8080/stsp/ar/status";

    //更新撤退狀態為 0
    string url_update = "http://192.168.99.240:8080/stsp/ar/update";

    //更新撤退狀態為 1
    string url_msg = "http://192.168.99.240:8080/stsp/ar/msg";

    //上傳角度資訊
    string upload_angle = "http://192.168.99.240:8080/stsp/ar/orientation";


    void Start()
    {
        Debug.developerConsoleVisible = false;
        if (this.qrcodeController != null)
        {
            this.qrcodeController.onQRScanFinished += new QRCodeDecodeControllerForWSA.QRScanFinished(this.qrScanFinished);
        }
        //switcher = GetComponent<SonarFxSwitcher>();
        InvokeRepeating("GetRetreatStatus", 0, 1); //每 1 秒偵測撤退指令
        InvokeRepeating("UpAngle", 0, 1);//每1秒上傳使用者的角度
        ChangeFloor();//先關閉所有樓層
        PersonPK = PersonPK.Replace("\n","");
        PK = PersonPK;
        EnableQRScan = false;
        StartCoroutine(toggleSonar());
    }


    private void Update()
    {
        #region Mysql（不使用）
        //if (Mysql_model.isConnected)
        //{
        //    if (Time.time >= nextUpdate)
        //    {
        //        nextUpdate = Mathf.FloorToInt(Time.time) + 1;
        //        Mysql_model.GetRetreatStatus("1");
        //        print(Mysql_model.value);
        //        if (Mysql_model.value == 1)
        //        {
        //            RetreatText.enabled = true;
        //            countdown = true;
        //            if(timer >= 3)
        //            {
        //                value = 0;
        //                RetreatText.enabled = false;
        //                Mysql_model.SetRetreatStatus(value);
        //                countdown = false;
        //            }
        //        }
        //    }
        //}
        #endregion

        #region 鍵盤按下動作（Debug 用）
#if UNITY_EDITOR_WIN
        if (Input.GetKeyDown(KeyCode.Keypad1)) //按下改變撤退數值
        {
            StartCoroutine(UploadMsg());
        }

        if (Input.GetKeyDown(KeyCode.Keypad2)) //按下開始 QRCode 掃描
        {
            StartCoroutine(UploadPosition(posName, "1,1,1,1"));
        }

        if (Input.GetKeyDown(KeyCode.Keypad3)) //按下開始掃描特效
        {
            StartCoroutine(toggleSonar());
            Debug.Log("toggleSonar");
        }
        //foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) //偵測按了什麼鍵
        //{
        //    if (Input.GetKey(vKey))
        //    {
        //        print(vKey.ToString());
        //    }
        //}
#endif
        #endregion

        #region 關閉 Vuforia 鏡頭倒數（註解）
        //if (countdown) //20 秒倒數完關閉 Vuforia 鏡頭
        //{
        //    if (timer >= 20)
        //    {
        //        countdown = false;
        //        ResetQRCamera(); //重設鏡頭
        //    }
        //    else
        //    {
        //        if (!DefaultTrackableEventHandler.tCountdown) //Vuforia 鏡頭未掃描時繼續倒數
        //        {
        //            timer += Time.deltaTime;
        //        }
        //    }
        //}
        //else
        //{
        //    timer = 0;
        //}
        #endregion

        if (countdown) //關閉撤退指令倒數
        {
            timer += Time.deltaTime;

            if (timer >= 4)
            {
                countdown = false;
                timer = 0;
                retreat.SetActive(false);
            }
        }

        /*
        if (DefaultTrackableEventHandler.usingSonar) //如果為 true，就開啟掃描特效
        {
            StartCoroutine(toggleSonar()); //掃描特效
        }
        */

        //如果為true，表示要開啟 QR Code scan 功能
        if (EnableQRScan == true) {
            EnableQRScan = false;
            StartQRcodeWork();//開啟QRcode鏡頭
            QRcodeReset();//QR Code 鏡頭重設
        }

        if (DefaultTrackableEventHandler.ChangePositionFinished) //如果為true，表示Vuforia已校正至正確位置
        {
            VuforiaBehaviour.Instance.enabled = false; //校正完後關閉 Vuforia 鏡頭
            DefaultTrackableEventHandler.ResetVariable();//將變數還原至預設值
            StartCoroutine(DisableText(VuforiaText));//關閉掃描校正點的文字訊息

            InputManager.ClickEnable = true;//恢復 Clicker 功能
            ChangeFloor();//顯示當下樓層

            //關閉眼前的掃描視窗
            Animator frameAnimat = ScanFrame.GetComponent<Animator>();
            frameAnimat.SetBool("FrameOut",true);
            ScanBar.SetActive(false);
            Invoke("ColseScanFrame", 2f);
            
        }

    }

    private void qrScanFinished(string dataText) //QR Code 鏡頭掃描完資訊
    {
        double lon=0f;
        double lat=0f;
        int floor=0;
        //resultText.text = dataText;
        if (dataText != null)
        {
            string[] data = dataText.Split(new char[] { ',' }); //資料格式
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == "")
                    return;
            }
            lon = double.Parse(data[0]);
            lat = double.Parse(data[1]);
            floor = int.Parse (data[2]);
            DefaultTrackableEventHandler.Pillar = data[3];
            //resultText.text = "ＰＫ：" + PersonPK + '\n' + "經度：" + lon.ToString("0.000") + '\n' + "緯度：" + lat.ToString("0.000") + '\n' + "樓層：" + floor.ToString() + "F" + '\n';
            resultText.text = "樓層：" + floor.ToString() + "F";
            floorValue = floor;
        }
        //開始等待上傳後端經緯度位置資訊，上傳完後開啟vuforia 功能
        StartCoroutine(UploadPosition(posName, PersonPK +","+ lon.ToString() +","+ lat.ToString() +","+ floor.ToString() )); 
        
        StopWork(); //關閉 QR Code 鏡頭
        StartCoroutine(DisableText(qrText));

        //開啟vuforia 功能
        VuforiaBehaviour.Instance.enabled = true;
        VuforiaText.enabled = true;
        VuforiaText.text = "請掃描校正圖";
        VuforiaText.color = new Color32(0, 255, 255, 255);
    }

    int loop = 0;
    public void StartQRcodeWork() //開啟 QR Code 鏡頭
    {
        qrText.enabled = true;
        qrText.text = "請掃描 QR Code";
        qrText.color = new Color32(0, 255, 255, 255);
        //print("重啟 QRCode 掃描，loop 為：" + loop++);
        qrcodeController.StartWork();
    }

    public void StopWork() //關閉 QR Code 鏡頭
    {
        print("關閉 QRCode 掃描");
        qrcodeController.StopWork();
    }

    void OpenVuforiaCam() //開啟 Vuforia 鏡頭
    {
        StopWork(); //關閉 QR Code 鏡頭
        VuforiaBehaviour.Instance.enabled = true; //開啟 Vuforia 鏡頭
        VuforiaText.enabled = true;
        VuforiaText.text = "請按下按鈕掃描";
        VuforiaText.color = new Color32(0, 255, 255, 255);
    }

    public void QRcodeReset() 
    {
        //print("重啟 QRCode 掃描");
        if (this.qrcodeController != null)
        {
            this.qrcodeController.Reset();
        }
    }

    private void GetRetreatStatus()
    {
        StartCoroutine(UploadRetreat()); //開始準備接收是否有撤退指令
    }
    private void UpAngle()
    {
        StartCoroutine(UploadAngle());//上傳角度資訊
    }

    IEnumerator UploadRetreat() //連接後端檢查是否有撤退指令
    {
        UnityWebRequest www = UnityWebRequest.Post(url_retreat, "0");
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            print(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string result = www.downloadHandler.text;
                print(result);
                if (result == "1")
                {
                    retreat.SetActive(true);
                    StartCoroutine(UploadUpdate()); //若有撤退指令（值為 1），就開始準備更新撤退指令資料
                    PathManager.SetActive(true);
                }
            }
        }
    }

    IEnumerator UploadMsg() //連接後端改變撤退數值（僅測試用）
    {
        UnityWebRequest www = UnityWebRequest.Post(url_msg, "0");
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            print(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;
            print(result);
        }
    }

    IEnumerator UploadUpdate() //連接後端更新撤退指令資料
    {
        yield return new WaitForSeconds(1f);
        UnityWebRequest www = UnityWebRequest.Post(url_update, "0");
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            print(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string result = www.downloadHandler.text;
                print(result);
                if (!countdown)
                {
                    countdown = true; //開始倒數 5 秒關閉撤退 Text
                }
                else //重複收到撤退指令不會在中途就關閉字樣
                {
                    timer -= 4;
                    if (timer <= 0)
                    {
                        timer = 0;
                    }
                }
            }
        }
    }

    IEnumerator UploadPosition(string name, string pos) //連接後端上傳後端經緯度位置資訊
    {
        WWWForm form = new WWWForm();
        form.AddField(name, pos);
        UnityWebRequest www = UnityWebRequest.Post(url_QR, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            print(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string result = www.downloadHandler.text;
                if (result == "success")
                {
                    qrText.color = Color.green;
                    qrText.text = "完成掃描 QR Code";
                    
                }
                else
                {
                    qrText.color = Color.red;
                    qrText.text = "完成掃描，但內容無法上傳後台";
                }
            }
            
        }
        

    }
    void ColseScanFrame()
    {
        Animator frameAnimat = ScanFrame.GetComponent<Animator>();
        frameAnimat.SetBool("FrameOut", false);
        ScanFrame.SetActive(false);
        ScanBar.SetActive(true);
    }

    IEnumerator ResetCanvasSetting() //讓 UI 回到 ARCamera 的位置
    {
        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator DisableText(Text text)
    {
        yield return new WaitForSeconds(5f);
        text.enabled = false;
    }

    IEnumerator UploadAngle()
    {
        string angle="";
        for (int i = 0; i < offices.Length; i++)
        {
            if (offices[i].activeSelf)
            {
                Vector3 office_direct = Vector3.ProjectOnPlane(offices[i].transform.forward, new Vector3(0f, 1f, 0f));//算office投影在xz平面上的向量
                Vector3 my_direct = Vector3.ProjectOnPlane(transform.forward, new Vector3(0f, 1f, 0f));//算自身向量投影在xz平面上的向量
                angle = Vector3.SignedAngle( office_direct, my_direct, new Vector3(0f, 1f, 0f)).ToString("0");//兩者在空間上的向量角度差
                break;
            }
        }
        WWWForm form = new WWWForm();
        form.AddField("orientation", PK + "," + angle);
        //form.AddField("orientation", angle);
        UnityWebRequest www = UnityWebRequest.Post(upload_angle, form);
        yield return www.SendWebRequest();
        
        if (www.isNetworkError)
        {
            print(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                string result = www.downloadHandler.text;
                Debug.Log(result);
            }

        }
    }


    void ChangeFloor()
    {
        for (int i = 0; i < offices.Length; i++)
        {
            offices[i].SetActive(false);
        }
        switch (floorValue)
        {
            case 1:
                offices[0].SetActive(true);
                break;
            case 2:
                offices[1].SetActive(true);
                break;
            default:
                break;
        }
    }

    IEnumerator toggleSonar() //開啟聲納特效
    {
        //DefaultTrackableEventHandler.usingSonar = false;
        //switcher.Toggle();
        while (true)
        {
            ScannerEffectDemo._scanning = true;
            yield return new WaitForSeconds(3f);
            //switcher.Toggle();
            ScannerEffectDemo._scanning = false;
            yield return null;
        }
    }
}

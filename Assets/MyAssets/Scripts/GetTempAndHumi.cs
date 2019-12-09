using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GraphQL;
using System.Text;
using UnityEngine.UI;

public class GetTempAndHumi : MonoBehaviour
{

    public string url = "http://210.241.120.82/stsp/ar/td";
    

    public Text[] Temperatures;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(GetJsonData());
        InvokeRepeating("CallGetData", 0, 60);
    }
    
    private void CallGetData() 
    {
        StartCoroutine(GetJsonData());

    }

    
    IEnumerator GetJsonData() //接收json data
    {
        
        UnityWebRequest www = UnityWebRequest.Post(url, "0");

        //yield return www.Send();
        yield return www.SendWebRequest();
        
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            for (int i = 0; i < 26; i++)
            {
                Temperatures[i].text = www.responseCode.ToString();
            }

        }
        else
        {
            if (www.responseCode == 200)//如果回傳成功，開始解析接收的資料
            {
                string result = www.downloadHandler.text;
                result = "{\"Items\":" + result + "}";
                JsonDataTempAndHumi[] Data = JsonHelper.FromJson<JsonDataTempAndHumi>(result);
                Debug.Log(result);
                for (int i = 0; i < Data.Length; i++) 
                {
                    /*
                    Debug.Log(  Data[i].id          + "\n" +
                                Data[i].name        + "\n" +
                                Data[i].temperature + "\n" +
                                Data[i].humidity    + "\n" );
                    */
                    Temperatures[i].text = Data[i].temperature + "°C";
                }

            }
            else 
            {
                string result = www.downloadHandler.text;
                Debug.Log(result);
            }
        }
    }

    [Serializable]
    public class JsonDataTempAndHumi
    {
        public string id;
        public string name;
        public string temperature;
        public string humidity;

    }
    


}

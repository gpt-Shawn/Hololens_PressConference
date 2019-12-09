using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecvJoinHand : MonoBehaviour
{
    
    public string url = "http://210.241.120.80/map-position";
    public GameObject FireManPrefab;
    public GameObject Buildings;
    float TimeCounter;
    Vector2 ancher1;
    Vector2 ancher2;
    Vector2 ancher3;
    public Text AncherObj1;
    public Text AncherObj2;
    public Text AncherObj3;

    string testJson = @"{
	""Items"": [{
		""xaxis"": ""120.279352685357"",
		""yaxis"": ""23.0922357581469"",
		""zaxis"": ""0"",
		""rid"": ""B8:27:EB:1C:D7:31"",
		""floor"": 1,
		""time"": ""2019-11-11 09:04:10""
	}, {
		""xaxis"": ""120.2791566"",
		""yaxis"": ""23.0923831"",
		""zaxis"": ""0"",
		""rid"": ""b8:27:eb:5f:1a:8d"",
		""floor"": 1,
		""time"": ""2019-11-11 10:31:36""

    }, {
		""xaxis"": ""120.279352685357"",
		""yaxis"": ""23.0922357581469"",
		""zaxis"": ""0"",
		""rid"": ""b8:27:eb:f8:7e:fc"",
		""floor"": 1,
		""time"": ""2019-11-11 09:04:08""

    }]
}";

    void Start()
    {
        //StartCoroutine(GetJsonData());
        ancher1 = new Vector2(float.Parse(AncherObj1.text.Split(',')[0]), float.Parse(AncherObj1.text.Split(',')[1]));
        ancher2 = new Vector2(float.Parse(AncherObj2.text.Split(',')[0]), float.Parse(AncherObj2.text.Split(',')[1]));
        ancher3 = new Vector2(float.Parse(AncherObj3.text.Split(',')[0]), float.Parse(AncherObj3.text.Split(',')[1]));
        TimeCounter = 0;
        StartCoroutine(GetJsonData());

    }
    private void Update()
    {
        //每隔一段時間呼叫一次Function
        TimeCounter += Time.deltaTime;
        if (TimeCounter >= 2) 
        {
            StartCoroutine(GetJsonData());
            TimeCounter = 0;
        }
    }

    IEnumerator GetJsonData() //接收json data
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        //先刪除所有的消防員標籤
        for (int i = Buildings.transform.childCount - 1; i >= 0; i--)
        {
            if (Buildings.transform.GetChild(i).transform.name == "fireman")
            {
                Destroy(Buildings.transform.GetChild(i).gameObject);
            }
        }

        if (www.isNetworkError)
        {
            print(www.error);
        }
        else
        {
            if (www.responseCode == 200)//如果回傳成功，開始解析接收的資料
            {

                string result = www.downloadHandler.text;
                result = FixJson(result);
                Debug.Log(result);
                JsonDataJoinHand[] Data = JsonHelper.FromJson<JsonDataJoinHand>(result);
                //依照資料新增人員標示
                for (int i = 0; i < Data.Length; i++)
                {
                    //如果在同一樓層且MAC跟自己不一樣
                    if (CallAPI.floorValue.ToString() == Data[i].floor && CallAPI.PK != Data[i].rid)
                    {
                        var fireman_icon = Instantiate(FireManPrefab, new Vector3(0, 0, 0), Quaternion.identity).gameObject;
                        fireman_icon.transform.name = "fireman";
                        fireman_icon.transform.parent = Buildings.transform;
                        fireman_icon.transform.localPosition = new Vector3(float.Parse(Data[i].xaxis).Remap(ancher1.x, ancher3.x, AncherObj1.transform.localPosition.x, AncherObj3.transform.localPosition.x),
                                                                            fireman_icon.transform.localPosition.y,
                                                                            float.Parse(Data[i].yaxis).Remap(ancher1.y, ancher2.y, AncherObj1.transform.localPosition.z, AncherObj2.transform.localPosition.z));
                        fireman_icon.transform.position = new Vector3(fireman_icon.transform.position.x,
                                                                      Camera.main.gameObject.transform.position.y,
                                                                      fireman_icon.transform.position.z);
                    }
                }
            }
        }
    }
    private string FixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

}

[Serializable]
public class JsonDataJoinHand {
    public string xaxis;
    public string yaxis;
    public string zaxis;
    public string rid;
    public string floor;
    public string time;
    public string name;
    
}
//Unity Json 解析參考網站
//https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity

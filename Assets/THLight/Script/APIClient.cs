using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using GraphQL;
using System;

public class APIClient : MonoBehaviour {

    //not a real api url
    public string apiUrl = "https://59.125.177.138/api/";
    public GameObject Buildings;
    public JsonDataTHLight[] Data;
    public GameObject RecvJson_JoinHand;

    [TextArea]
    public string query = @"{
allBeaconEntities {
results(limit: 50,offset:0) {
id
data 
mac 
lastSignal{
  position{
    map{ 
      id
      name 
    }
  }
  lat 
  long 
}
}
}
}
";
    


    public IEnumerator QueryCall(System.Action<bool> callback) {
        GraphQLClient client = new GraphQLClient(apiUrl);

        using (UnityWebRequest www = client.Query(query, "", "")) {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.certificateHandler = new BypassCertificate();
            yield return www.Send();

            if (www.isNetworkError) {//如果網路出問題抓不到資料
                Debug.Log(www.error);
                if(RecvJson_JoinHand.activeSelf == false)//改連線至晶翰API抓人員資料
                    RecvJson_JoinHand.SetActive(true);
                callback(false);
            } else {//如果網路無問題可以抓到資料
                if (RecvJson_JoinHand.activeSelf == true)//關閉晶翰API的連線
                    RecvJson_JoinHand.SetActive(false);
                string responseString = www.downloadHandler.text;
                Debug.Log(responseString);
                JSONObject response = new JSONObject(responseString);
                JSONObject data = response.GetField("data");
                data = data.GetField("allBeaconEntities");
                data = data.GetField("results");
                
                string result = FixJson(data.ToString());
                Debug.Log(result);
                Data = JsonHelper.FromJson<JsonDataTHLight>(result);
                /*
                for (int i = 0; i < Data.Length; i++)
                {
                    Debug.Log(Data[i].id + "\n" +
                              Data[i].data + "\n" +
                              Data[i].mac + "\n"+
                              Data[i].lastSignal.position.map.id + "\n"+
                              Data[i].lastSignal.position.map.name + "\n"+
                              Data[i].lastSignal.lat + "\n"+
                              Data[i].lastSignal.@long + "\n");

                }
                */
                callback(true);
            }
        }

    }


    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
    private string FixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }
    [Serializable]
    public class JsonDataTHLight
    {
        public string id;
        public string data;
        public string mac;
        public lastSignals lastSignal;
    }
    [Serializable]
    public class lastSignals
    {
        public positions position;
        public string lat;
        public string @long;
    }
    [Serializable]
    public class positions 
    {
        public maps map;
    }
    [Serializable]
    public class maps
    {
        public string id;
        public string name;
    }


    void accessData(JSONObject obj){
        switch(obj.type){
            case JSONObject.Type.OBJECT:
                  for(int i = 0; i < obj.list.Count; i++){
                        string key = (string)obj.keys[i];
                        JSONObject j = (JSONObject)obj.list[i];
                        Debug.Log(key);
                        accessData(j);
                  }
                  break;
            case JSONObject.Type.ARRAY:
                  foreach(JSONObject j in obj.list){
                        accessData(j);
                  }
                  break;
            case JSONObject.Type.STRING:
                  Debug.Log(obj.str);
                  break;
            case JSONObject.Type.NUMBER:
                  Debug.Log(obj.n);
                  break;
            case JSONObject.Type.BOOL:
                  Debug.Log(obj.b);
                  break;
            case JSONObject.Type.NULL:
                  Debug.Log("NULL");
                  break;

        }
    }

}

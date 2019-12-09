using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecvTHLight : MonoBehaviour
{
    public APIClient api;//此api負責抓取太和光Json資料
    public GameObject FireManPrefab;
    public GameObject Buildings;
    float TimeCounter;
    Vector2 ancher1;
    Vector2 ancher2;
    Vector2 ancher3;
    public Text AncherObj1;
    public Text AncherObj2;
    public Text AncherObj3;

    void Start()
    {
        StartCoroutine(api.QueryCall((bool success) =>
        {
            if (success)
                Debug.Log("success!");
            else
                Debug.Log("fail!");
        }));
        GetJsonData();
        ancher1 = new Vector2(float.Parse(AncherObj1.text.Split(',')[0]), float.Parse(AncherObj1.text.Split(',')[1]));
        ancher2 = new Vector2(float.Parse(AncherObj2.text.Split(',')[0]), float.Parse(AncherObj2.text.Split(',')[1]));
        ancher3 = new Vector2(float.Parse(AncherObj3.text.Split(',')[0]), float.Parse(AncherObj3.text.Split(',')[1]));
        TimeCounter = 0;
    }

    private void Update()
    {
        //每隔一段時間呼叫一次Function
        TimeCounter += Time.deltaTime;
        if (TimeCounter >= 60)
        {
            StartCoroutine(api.QueryCall((bool success) => { }));
            GetJsonData();
            TimeCounter = 0;
        }
    }

    private void GetJsonData()
    {
        //先刪除所有的消防員標籤
        for (int i = Buildings.transform.childCount - 1; i >= 0; i--)
        {
            if (Buildings.transform.GetChild(i).transform.name == "fireman")
            {
                Destroy(Buildings.transform.GetChild(i).gameObject);
            }
        }
        //依照資料新增人員標示
        for (int i=0;i<api.Data.Length; i++) 
        {
            //如果在同一樓層且MAC跟自己不一樣
            if (CallAPI.floorValue.ToString() == api.Data[i].lastSignal.position.map.id && CallAPI.PK != api.Data[i].mac)
            {
                var fireman_icon = Instantiate(FireManPrefab, new Vector3(0, 0, 0), Quaternion.identity).gameObject;
                fireman_icon.transform.name = "fireman";
                fireman_icon.transform.parent = Buildings.transform;
                fireman_icon.transform.localPosition = new Vector3(float.Parse(api.Data[i].lastSignal.@long).Remap(ancher1.x, ancher3.x, AncherObj1.transform.localPosition.x, AncherObj3.transform.localPosition.x),
                                                                    fireman_icon.transform.localPosition.y,
                                                                    float.Parse(api.Data[i].lastSignal.lat).Remap(ancher1.y, ancher2.y, AncherObj1.transform.localPosition.z, AncherObj2.transform.localPosition.z));
            }
        }


    }


}

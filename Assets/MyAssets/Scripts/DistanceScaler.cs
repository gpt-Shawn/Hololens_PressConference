using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceScaler : MonoBehaviour {

    public Transform cam;
    public float dis;
    public Transform textCanvas;
    public Text distanceText;
    public bool DeActiveCanvasByDis;
    public float DeActiveDistance = 20f;
    public bool ActiveCanvasByDis;
    public float ActiveDistance = 5f;
    GameObject CanvasObj;
	// Use this for initialization
	void Start () {
        cam = Camera.main.transform;
        CanvasObj = gameObject.transform.Find("Canvas").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        
		if (cam)
        {
            dis = Vector3.Distance(cam.position, transform.position);
            transform.localScale = new Vector3(dis, dis, dis)/4;
        }
        if (textCanvas)
        {
            textCanvas.LookAt(cam);
        }
        if (distanceText)
        {
            distanceText.text = dis.ToString("F1") + "m";
        }

        if (DeActiveCanvasByDis) //如果為true，大於距離後隱藏Canvas
        {
            if(dis< DeActiveDistance)
                CanvasObj.SetActive(true);
            else
                CanvasObj.SetActive(false);
        }
        if (ActiveCanvasByDis) //如果為true，大於距離後顯示Canvas
        {
            if (dis < ActiveDistance)
                CanvasObj.SetActive(false);
            else
                CanvasObj.SetActive(true);
        }



    }
}

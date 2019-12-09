using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculatPosi : MonoBehaviour
{

    public GameObject cam;
    public Text dist_text;
    public Text angle_text;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float dist = Vector3.Distance(cam.transform.position, transform.position);//距離計算
        Vector3 my_direct = Vector3.ProjectOnPlane(cam.transform.forward, new Vector3(0f, 1f, 0f));//算camera投影在xz平面上的向量
        Vector3 qr_direct = Vector3.ProjectOnPlane(-transform.forward, new Vector3(0f, 1f, 0f));//算自身向量投影在xz平面上的向量
        float angle = Vector3.Angle(my_direct, qr_direct);//兩者在空間上的向量角度差
        dist_text.text = dist.ToString()+" m";
        angle_text.text = angle.ToString()+" degree";


    }
}

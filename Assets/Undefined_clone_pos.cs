using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Undefined_QRcode_clone_pos : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // text.text = GameObject.Find("office_obj_QR").GetComponentInChildren<Renderer>()
        if (GameObject.Find("office_obj_QR").GetComponentInChildren<Renderer>().enabled == true)
        {
            
            this.transform.position = GameObject.Find("office_obj_QR").transform.position;
            this.transform.rotation = GameObject.Find("office_obj_QR").transform.rotation;
        }
       

    }
}

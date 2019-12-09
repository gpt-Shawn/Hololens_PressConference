using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Undefined_QRcode : MonoBehaviour
{

    public bool clonebool = true;



    public GameObject objInImageTarget;

    public GameObject cloneofficeObj;

    public Text text;
    // public GameObject Obj;

    // Start is called before the first frame update
    void Start()
    {
        clonebool = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if (DefaultTrackableEventHandler.cloneOffice == true && clonebool == true)
        // {
        //     GameObject cloneObj = Instantiate(cloneofficeObj, objInImageTarget.transform.position, objInImageTarget.transform.rotation);
        //     clonebool = false;
        // }

        if (GameObject.Find("office_obj_QR").GetComponentInChildren<Renderer>().enabled == true)
        {
            text.text = "mesh : true";
        }
        else
        {

            text.text = "mesh : false";
        }


    }
}

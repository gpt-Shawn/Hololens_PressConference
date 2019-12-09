using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class AwakeScanImage : MonoBehaviour
{

    public GameObject ScanImage;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetObjActive", 4f);
    }

    void SetObjActive()
    {
        ScanImage.SetActive(true);
    }
    
}

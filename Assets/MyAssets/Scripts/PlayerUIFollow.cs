using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIFollow : MonoBehaviour
{
    public GameObject arCam;
    public bool camCanvas;
    public bool minimap;


    private void Update()
    {
        
        if (camCanvas)
        {
            transform.position = Vector3.Lerp( transform.position, new Vector3(arCam.transform.position.x, -4f, arCam.transform.position.z), 5f*Time.deltaTime );
            transform.rotation = Quaternion.Euler(90, arCam.transform.eulerAngles.y, arCam.transform.eulerAngles.z);
        }
        if (minimap)
        {
            transform.rotation = Quaternion.Euler(0, arCam.transform.eulerAngles.y, 0);
        }
        
    }
}

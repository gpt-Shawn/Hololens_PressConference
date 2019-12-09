using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;

public class CameraSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XRSettings.eyeTextureResolutionScale = 0.5f;
        XRSettings.renderViewportScale = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

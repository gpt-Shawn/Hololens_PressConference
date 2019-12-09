using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIcon : MonoBehaviour
{

    public GameObject ExitIcon;
    public Camera cam;


    // Start is called before the first frame update
    void OnEnable()
    {
        cam = Camera.main;
        ExitIcon.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.name == "ARCamera")
        {
            ExitIcon.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.transform.name == "ARCamera")
        {
            ExitIcon.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.name == "ARCamera")
        {
            ExitIcon.SetActive(false);
        }
    }

}

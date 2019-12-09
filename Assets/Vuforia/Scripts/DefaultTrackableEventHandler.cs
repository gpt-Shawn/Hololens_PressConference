/*==============================================================================
Copyright (c) 2019 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{

    public static bool ChangePositionFinished = false; //�O�_�i�H�}�l�ե���m
    public static bool usingSonar = false; //�O�_�i�H�X�{���y�S��
    
    public static float tTimer = 0;
    //public static bool FirstSee = true;
    public static bool hasFound = false;
    
    Text VuforiaText;
    public GameObject QR_Position;
    public GameObject Office;
    public GameObject[] Pillars;
    public static string Pillar;

    #region PROTECTED_MEMBER_VARIABLES
    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status m_PreviousStatus;
    protected TrackableBehaviour.Status m_NewStatus;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        VuforiaText = Camera.main.transform.Find("PlayerCanvas/Vuforia").GetComponent<Text>();
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName +
                  " " + mTrackableBehaviour.CurrentStatus +
                  " -- " + mTrackableBehaviour.CurrentStatusInfo);

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        if (mTrackableBehaviour)
        {
            hasFound = true;//�ݨ�ؼЬ�true
            var rendererComponents = mTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mTrackableBehaviour.GetComponentsInChildren<Collider>(true);
            var canvasComponents = mTrackableBehaviour.GetComponentsInChildren<Canvas>(true);

            // Enable rendering:
            foreach (var component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (var component in colliderComponents)
                component.enabled = true;

            // Enable canvas':
            foreach (var component in canvasComponents)
                component.enabled = true;
            
        }
    }


    protected virtual void OnTrackingLost()
    {
        if (mTrackableBehaviour)
        {
            hasFound = false;//�S�ݨ�ؼЬ�false
            var rendererComponents = mTrackableBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mTrackableBehaviour.GetComponentsInChildren<Collider>(true);
            var canvasComponents = mTrackableBehaviour.GetComponentsInChildren<Canvas>(true);
            
            // Disable rendering:
            foreach (var component in rendererComponents)
                component.enabled = false;

            // Disable colliders:
            foreach (var component in colliderComponents)
                component.enabled = false;

            // Disable canvas':
            foreach (var component in canvasComponents)
                component.enabled = false;

        }

    }

    #endregion // PROTECTED_METHODS

    void Update()
    {
        if (hasFound)//�p�G����w�쪺QRcode
        {
            tTimer += Time.deltaTime;

            if (tTimer < 0.5f) //���y��Ϲ���}�l�˼� 0.5 ��
            {
                VuforiaText.text = "���y��";
                VuforiaText.color = new Color32(255, 216, 0, 255);
            }
            else//0.5��᧹�����y
            {
                hasFound = false;
                VuforiaText.text = "���y����";
                VuforiaText.color = Color.green;
                ChangePositionFinished = true;
                usingSonar = true;
                tTimer = 0;
            }

            Positioning();//�wmodel�쥿�T��m
        }
        

    }
    public static void ResetVariable() 
    {
        ChangePositionFinished = false;
        hasFound = false;
        tTimer = 0;
    }

    void Positioning()
    {
        
        for (int i = 0; i < Pillars.Length; i++) //�̬W��W�٧��ӬW��
        {
            if (Pillars[i].transform.name == Pillar) {
                QR_Position = Pillars[i];
                break;
            }
        }
        
        //�H�U�O�w��\��(���)
        Vector3 office_forward = Vector3.ProjectOnPlane(QR_Position.transform.forward, new Vector3(0f, 1f, 0f));
        //Debug.Log(QR_Position.transform.name);
        Vector3 targetDir = Vector3.ProjectOnPlane(-transform.up, new Vector3(0f, 1f, 0f));
        float step = 200f * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(office_forward, targetDir, step, 0.0f);
        Office.transform.rotation = Quaternion.LookRotation(newDir);
        Office.transform.eulerAngles -= QR_Position.transform.localEulerAngles;

        //�H�U�O�w��\��(����)
        Office.transform.position += Vector3.Lerp(QR_Position.transform.position, transform.position, 0.99f) - QR_Position.transform.position;
    }
}






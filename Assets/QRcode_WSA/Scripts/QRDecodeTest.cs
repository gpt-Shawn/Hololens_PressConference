using System;
using UnityEngine;
using UnityEngine.UI;

public class QRDecodeTest : MonoBehaviour
{
	public QRCodeDecodeControllerForWSA e_qrController;

	public Text UiText;

	public GameObject resetBtn;
	public GameObject StartBtn;
	public GameObject StopBtn;

	public GameObject scanLineObj;

	/// <summary>
	/// when you set the var is true,if the result of the decode is web url,it will open with browser.
	/// </summary>
	public bool isOpenBrowserIfUrl;

	private void Start()
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.onQRScanFinished += new QRCodeDecodeControllerForWSA.QRScanFinished(this.qrScanFinished);
		}
        Invoke("StartWork", 5);
	//	StartWork ();
	}

	private void Update()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			StartWork ();
		}
	}

	private void qrScanFinished(string dataText)
	{
		if (isOpenBrowserIfUrl) {
			if (Utility.CheckIsUrlFormat(dataText))
			{
				if (!dataText.Contains("http://") && !dataText.Contains("https://"))
				{
					dataText = "http://" + dataText;
				}
				Application.OpenURL(dataText);
			}
		}
		this.UiText.text = dataText;
		if (this.resetBtn != null)
		{
			this.resetBtn.SetActive(true);
		}
		if (this.scanLineObj != null)
		{
			this.scanLineObj.SetActive(false);
		}
	}
	int loop =0;

	public void StartWork()
	{
		Debug.Log ("curren loop is " + loop++);
		StartBtn.SetActive (false);
		StopBtn.SetActive (true);
		e_qrController.StartWork ();
	}

	public void StopWork()
	{
		StartBtn.SetActive (true);
		StopBtn.SetActive (false);
		e_qrController.StopWork ();
	}

	public void Reset()
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.Reset();
		}
		if (this.UiText != null)
		{
			this.UiText.text = string.Empty;
		}
		if (this.resetBtn != null)
		{
			this.resetBtn.SetActive(false);
		}
		if (this.scanLineObj != null)
		{
			this.scanLineObj.SetActive(true);
		}
	}

	public void GotoNextScene(string scenename)
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.StopWork();
		}
		Application.LoadLevel(scenename);
	}


}

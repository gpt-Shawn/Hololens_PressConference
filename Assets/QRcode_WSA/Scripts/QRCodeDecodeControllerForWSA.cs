/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using ZXing;

public class QRCodeDecodeControllerForWSA : MonoBehaviour
{
	public delegate void QRScanFinished(string str);  //declare a delegate to deal with the QRcode decode complete
	public event QRScanFinished onQRScanFinished;  		//declare a event with the delegate to trigger the complete event

	bool decoding = false;		
	bool tempDecodeing = false;
	string dataText = null;
	public DeviceCameraController e_DeviceController = null; 
	private Color[] orginalc;   	//the colors of the camera data.
	private byte[] targetbyte;		//the pixels of the camera image.
	private int W, H;			//width/height of the camera image
	int byteIndex = 0;				
	int framerate = 0; 		

	#if UNITY_IOS
	int blockWidth = 450;
	#elif UNITY_ANDROID
	int blockWidth = 350;
	#else
	int blockWidth = 350;
	#endif
	bool isInit = false;
	BarcodeReader barReader;

	bool isRunning = false;
	void Start()
	{
		barReader = new BarcodeReader ();
		
		if (!e_DeviceController) {
			e_DeviceController = GameObject.FindObjectOfType<DeviceCameraController>();
			if(e_DeviceController)
			{
				Debug.Log("the Device Controller is not exsit,Please Drag DeviceCamera from project to Hierarchy");
			}
		}
	}


	void Update()
	{
		if (isRunning) {
			#if UNITY_EDITOR
			if (framerate++ % 10== 0) {
			#else
			if (framerate++ % 8== 0) {
			#endif
				if (e_DeviceController == null || !e_DeviceController.isPlaying  ) {
					return;
				}

				if (e_DeviceController.isPlaying && !decoding && e_DeviceController.cameraTexture.isPlaying)
				{

					W = e_DeviceController.cameraTexture.width;					// get the image width
					H = e_DeviceController.cameraTexture.height;				// get the image height 

					if (W < 100 || H < 100) {
						return;
					}
					if(!isInit && W>100 && H>100)
					{
						blockWidth = (int)((Math.Min(W,H)/3f) *2);

						isInit = true;
						if(targetbyte == null)
						{
							targetbyte = new byte[ blockWidth * blockWidth ];
						}
					}

					int posx = ((W-blockWidth)>>1);//
					int posy = ((H-blockWidth)>>1);

					orginalc = e_DeviceController.cameraTexture.GetPixels(posx,posy,blockWidth,blockWidth);// get the webcam image colors

					int z = 0;

					// convert the image color data
					for(int y = blockWidth - 1; y >= 0; y--) {
						for(int x = 0; x < blockWidth; x++) {
							targetbyte[z++]  = (byte)(((int)(orginalc[y * blockWidth+ x].r *255))<<16 | ((int)(orginalc[y * blockWidth + x].g *255))<<8 | ((int)(orginalc[y * blockWidth + x].b*255) ));
						}
					}
				}
			}
		}

	}

	IEnumerator ProcessScanCode()
	{
		while (true && !decoding)
		{
			if (targetbyte == null) {
				
				yield return new  WaitForSeconds(1f);
				continue;
			}

			LuminanceSource luminanceSource = null;
			luminanceSource = new RGBLuminanceSource(targetbyte,blockWidth,blockWidth,true);
			var data = barReader.Decode(luminanceSource);
			if (data != null) // if get the result success
			{
				decoding = true; 	// set the variable is true
				dataText = data.Text;	// use the variable to save the code result
				//clear the targetbyte arr is zero
				for (int i = 0;i!= targetbyte.Length;i++) {
					targetbyte[i] = 0;
				}
			}
			if(decoding)
			{
				// if the status variable is change
				if(tempDecodeing != decoding)
				{
					onQRScanFinished(dataText);//triger the scan finished event;
				}
				tempDecodeing = decoding;
				break;
			}
			yield return new  WaitForSeconds(1f);
		}
		yield return new WaitForEndOfFrame ();
	}

	/// <summary>
	/// Reset this scan param
	/// </summary>
	public void Reset()
	{
		decoding = false;
		tempDecodeing = decoding;
		StartCoroutine (ProcessScanCode ());
	}

	public void StopWork()
	{
		decoding = true;
		if (e_DeviceController != null) {
			e_DeviceController.StopWork();
		}
		isRunning = false;
	}

	/// <summary>
	/// Starts the work.
	/// </summary>
	public void StartWork()
	{
		if (e_DeviceController != null) {
			e_DeviceController.StartWork();
			isRunning = true;
		}
		StartCoroutine (ProcessScanCode ());
	}
	/// <summary>
	/// Decodes the by static picture.
	/// </summary>
	/// <returns> return the decode result string </returns>
	/// <param name="tex">target texture.</param>
	public static string DecodeByStaticPic(Texture2D tex)
	{
		BarcodeReader codeReader = new BarcodeReader ();
		codeReader.AutoRotate = true;
		codeReader.TryInverted = true;

		Result data = codeReader.Decode (tex.GetPixels32 (), tex.width, tex.height);
		if (data != null) {
			return data.Text;
		} else {
			return "decode failed!";
		}
	}
	
}
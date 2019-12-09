using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DecodeStaticImage : MonoBehaviour {

	public Texture2D tex;
	public Text targetText;
	// Use this for initialization
	void Start () {
		string str = QRCodeDecodeControllerForWSA.DecodeByStaticPic (tex);
	
		targetText.text = str;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

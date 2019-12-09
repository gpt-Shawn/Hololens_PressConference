using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionByLatitudeAndLongitude : MonoBehaviour
{
    [TextArea]
    public string currentPosition;

    Vector2 ancher1;
    Vector2 ancher2;
    Vector2 ancher3;
    public Text AncherObj1;
    public Text AncherObj2;
    public Text AncherObj3;
    float Latitude;
    float Longitude;

    // Start is called before the first frame update
    void Start()
    {
        Latitude = float.Parse(currentPosition.Split(',')[0]);//經度座標
        Longitude = float.Parse(currentPosition.Split(',')[1]);//緯度座標
        ancher1 = new Vector2(float.Parse(AncherObj1.text.Split(',')[0]), float.Parse(AncherObj1.text.Split(',')[1]));
        ancher2 = new Vector2(float.Parse(AncherObj2.text.Split(',')[0]), float.Parse(AncherObj2.text.Split(',')[1]));
        ancher3 = new Vector2(float.Parse(AncherObj3.text.Split(',')[0]), float.Parse(AncherObj3.text.Split(',')[1]));

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localPosition = new Vector3( Latitude.Remap( ancher1.x, ancher3.x, AncherObj1.transform.localPosition.x, AncherObj3.transform.localPosition.x), 
                                                          gameObject.transform.localPosition.y,
                                                          Longitude.Remap(ancher1.y, ancher2.y, AncherObj1.transform.localPosition.z, AncherObj2.transform.localPosition.z));

    }
}
public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}

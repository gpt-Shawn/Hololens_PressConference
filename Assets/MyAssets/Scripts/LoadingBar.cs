using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingBar : MonoBehaviour
{

    public TextMeshProUGUI loadText;
    public Slider loadSlider;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.developerConsoleVisible = false;
        StartCoroutine(LoadingScene("Office"));
    }
    
    IEnumerator LoadingScene(string sceneName)
    {
        yield return new WaitForSeconds(1.0f);

        AsyncOperation async = Application.LoadLevelAsync(sceneName);
        async.allowSceneActivation = false;//先設定不跳場景

        while (async.progress < 0.9f)//如果讀取進度尚未到 90%
        {
            while (loadSlider.value < async.progress)//讓顯示進度條跟上實際讀取進度
            {
                loadSlider.value += 0.02f;
                loadText.text = (loadSlider.value * 100).ToString("0") + "%";
                yield return new WaitForEndOfFrame();
            }
        }

        yield return new WaitForEndOfFrame();

        while (loadSlider.value < 1f)//如果超過90%但還沒滿
        {
            loadSlider.value += 0.02f;
            loadText.text = (loadSlider.value * 100).ToString("0") + "%";
            yield return new WaitForEndOfFrame();
        }
        async.allowSceneActivation = true;//讀取條到100%時跳場景
        //while (!async.isDone)
        //{
        //    loadText.text = (async.progress * 100).ToString() + "%";
        //    loadSlider.value = async.progress;
        //    yield return null;
        //}

    }
}

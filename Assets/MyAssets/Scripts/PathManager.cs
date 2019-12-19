using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private Vector3 StartPosition;
    private Vector3 TargetPosition;
    public float MoveDistance;
    public GameObject EscapeIcon;
    public GameObject PathArrow;
    public GameObject Office;
    public float LifeTime;
    private float CreatPathTime;

    // Start is called before the first frame update
    void Start()
    {
        StartPosition = Camera.main.transform.position;
        StartPosition = new Vector3( StartPosition.x, Office.transform.position.y, StartPosition.z);
        TargetPosition = EscapeIcon.transform.position;
        TargetPosition = new Vector3( TargetPosition.x,Office.transform.position.y,TargetPosition.z);
        CreatPathTime = 0f;
        StartCoroutine(CreatPath());
    }

    // Update is called once per frame
    void Update()
    {
        CreatPathTime += Time.deltaTime;
        if (CreatPathTime >= LifeTime)
        {
            StartPosition = Camera.main.transform.position;
            StartPosition = new Vector3(StartPosition.x, Office.transform.position.y, StartPosition.z);
            TargetPosition = EscapeIcon.transform.position;
            TargetPosition = new Vector3(TargetPosition.x, Office.transform.position.y, TargetPosition.z);
            StartCoroutine(CreatPath());
            CreatPathTime = 0f;
        }
        
    }

    IEnumerator CreatPath()
    {
        Vector3 PrefabPosition;
        PrefabPosition = StartPosition;
        while (Vector3.Distance(PrefabPosition, TargetPosition) > MoveDistance) //如果還沒到終點的話，繼續製造箭頭
        {
            PrefabPosition = Vector3.Lerp(PrefabPosition, TargetPosition, MoveDistance / Vector3.Distance(PrefabPosition, TargetPosition));
            PrefabPosition = new Vector3(PrefabPosition.x, Office.transform.position.y, PrefabPosition.z);//算出箭頭新增位置
            var arrow = Instantiate(PathArrow, PrefabPosition, Quaternion.identity);
            arrow.transform.LookAt(EscapeIcon.transform);
            arrow.transform.eulerAngles = new Vector3(90, arrow.transform.eulerAngles.y, 0);
            Destroy(arrow, LifeTime);//創造後一秒就消滅
            yield return new WaitForSeconds(0.05f);//隔0.05秒後新增下一個

        }
        
    }

}

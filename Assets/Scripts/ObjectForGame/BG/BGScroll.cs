using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGScroll : MonoBehaviour
{
    [SerializeField]
    List<Image> FarBGList;

    [SerializeField]
    List<Image> NearBGList;

    [SerializeField]
    float FarBGSpeed = 0.5f;

    [SerializeField]
    float FarBGWidth = 2048;

    [SerializeField]
    float NearBGSpeed = 2f;

    [SerializeField]
    float NearBGWidth = 2048;


    void Update()
    {
        foreach (var img in FarBGList)
        {
            float x = img.transform.localPosition.x;
            float y = img.transform.localPosition.y;

            x -= FarBGSpeed * Time.deltaTime;
            if (x < -FarBGWidth) x += FarBGWidth * 2;
            img.transform.localPosition = new Vector3(x, y);
        }
        foreach (var img in NearBGList)
        {
            float x = img.transform.localPosition.x;
            float y = img.transform.localPosition.y;

            x -= NearBGSpeed * Time.deltaTime;
            if (x < -NearBGWidth) x += NearBGWidth * 2;
            img.transform.localPosition = new Vector3(x, y);
        }
    }
}

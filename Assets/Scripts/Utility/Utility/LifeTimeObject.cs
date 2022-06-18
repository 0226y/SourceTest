using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeObject : MonoBehaviour
{
    [SerializeField, Tooltip("オブジェクトの寿命")]
    float LifeTime = 1f;


    void Start()
    {
        StartCoroutine(CoStart());
    }

    IEnumerator CoStart()
    {
        yield return new WaitForSeconds(LifeTime);
        Destroy(gameObject);
    }
}

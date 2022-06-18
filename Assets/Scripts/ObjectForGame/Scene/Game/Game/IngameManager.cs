using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    [SerializeField, Tooltip("UIルート")]
    GameObject _UIRoot;

    [SerializeField, Tooltip("スコアマネージャ")]
    ScoreManager _ScoreManager;

    public void DisActive()
    {
        _UIRoot.SetActive(false);
    }

    public void GameStart()
    {
        _UIRoot.SetActive(true);
        _ScoreManager.SetScore(0);
        _ScoreManager.GameStart();
    }

    public void GameEnd()
    {
        _ScoreManager.GameEnd(()=>
        {
            _UIRoot.SetActive(false);
        });
    }
}

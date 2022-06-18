using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    [SerializeField, Tooltip("UIルート")]
    GameObject _UIRoot;

    [SerializeField, Tooltip("アニメ")]
    Animator _Anime;

    [SerializeField, Tooltip("アニメまでのディレイ")]
    float GameStartAnimeDurationDelay = 2;

    [SerializeField, Tooltip("出現アニメ尺")]
    float GameStartAnimeDurationAppear = 2;

    [SerializeField, Tooltip("ウエイトアニメ尺")]
    float GameStartAnimeDurationWait = 3;

    [SerializeField, Tooltip("退場アニメ尺")]
    float GameStartAnimeDurationFinish = 2;

    public delegate void FinishEvent();

    public void DisActive()
    {
        _UIRoot.SetActive(false);
    }

    public void GameStart(FinishEvent finish)
    {
        _UIRoot.SetActive(true);
        StartCoroutine(CoStart(finish));
    }

    IEnumerator CoStart(FinishEvent finish)
    {
        // プレイヤーキャラ生成まで待つ
        bool isFinish = false;
        PlayerCharaManager.Instance.CreatePlayerChara(() =>
        {
            isFinish = true;
        });
        while (!isFinish) yield return null;

        Sound.Instance.Play(16, Sound.CH.SE_ONESHOT);

        // ディレイ
        yield return new WaitForSeconds(GameStartAnimeDurationDelay);
        Sound.Instance.Play(13, Sound.CH.SE_ONESHOT);

        // アニメ再生
        _Anime.Play("appear", 0, 0);
        yield return new WaitForSeconds(GameStartAnimeDurationAppear);
        yield return new WaitForSeconds(GameStartAnimeDurationWait);
        _Anime.Play("end", 0, 0);
        yield return new WaitForSeconds(GameStartAnimeDurationFinish);

        if (finish != null) finish.Invoke();
    }
}

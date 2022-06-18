using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField, Tooltip("UIルート")]
    GameObject _UIRoot;

    [SerializeField, Tooltip("アニメ")]
    Animator _Anime;

    [SerializeField, Tooltip("アニメまでのディレイ")]
    float GameOverAnimeDurationDelay = 2;

    [SerializeField, Tooltip("出現アニメ尺")]
    float GameOverAnimeDurationAppear = 2;

    [SerializeField, Tooltip("ウエイトアニメ尺")]
    float GameOverAnimeDurationWait = 3;

    [SerializeField, Tooltip("退場アニメ尺")]
    float GameOverAnimeDurationFinish = 2;

    public delegate void FinishEvent();

    public void DisActive()
    {
        _UIRoot.SetActive(false);
    }

    public void StartGameOver(FinishEvent finish)
    {
        _UIRoot.SetActive(true);
        StartCoroutine(CoStart(finish));
    }

    IEnumerator CoStart(FinishEvent finish)
    {
        // ディレイ
        yield return new WaitForSeconds(GameOverAnimeDurationDelay);

        Sound.Instance.Play(17, Sound.CH.SE_ONESHOT);

        // アニメ再生
        _Anime.Play("appear", 0, 0);
        yield return new WaitForSeconds(GameOverAnimeDurationAppear);

        // リザルトBGMをフェードイン
        Sound.Instance.Play(2, Sound.CH.BGM2, true, 0);
        Sound.Instance.Fade(Sound.CH.BGM2, 3f, 1f, false);
        yield return new WaitForSeconds(GameOverAnimeDurationWait);

        _Anime.Play("end", 0, 0);
        yield return new WaitForSeconds(GameOverAnimeDurationFinish);

        if (finish != null) finish.Invoke();
    }
}

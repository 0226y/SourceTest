using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField, Tooltip("UIルート")]
    GameObject _UIRoot;

    [SerializeField, Tooltip("アニメ")]
    Animator _Anime;

    [SerializeField, Tooltip("リザルト開始演出尺")]
    float ResultStartDuration = 3f;

    [SerializeField, Tooltip("リザルト終了演出尺")]
    float ResultEndDuration = 3f;

    [SerializeField, Tooltip("リザルトスコア")]
    TextMeshProUGUI _ResultScore;

    [SerializeField, Tooltip("ハイスコア")]
    TextMeshProUGUI _HiScore;

    [SerializeField, Tooltip("ランクインRoot")]
    GameObject _RankInRoot;

    [SerializeField, Tooltip("ランクイン順位")]
    TextMeshProUGUI _Rank;

    public delegate void FinishEvent();

    static ResultManager _Instance = null;
    public static ResultManager Instance { get { return _Instance; } }

    private void Start()
    {
        _Instance = this;
    }

    public void SetScore(int score)
    {
        _ResultScore.text = "SCORE:" + $"{score:0000}";
    }

    public void SetHiScore(int score)
    {
        _HiScore.text = "HI-SCORE:" + $"{score:0000}";
    }

    public void SetRank(bool isRankin, string rankString)
    {
        _RankInRoot.SetActive(isRankin);
        _Rank.text = rankString;
    }

    public void DisActive()
    {
        _UIRoot.SetActive(false);
    }

    public void StartResult()
    {
        _UIRoot.SetActive(true);
        StartCoroutine(CoStartResult());
    }

    IEnumerator CoStartResult()
    {
        // アニメ開始
        _Anime.Play("appear", 0, 0);
        yield return new WaitForSeconds(ResultStartDuration);
    }

    // タイトル画面へ戻るボタン
    public void BackToTitle(FinishEvent finish)
    {
        StartCoroutine(CoBackToTitle(finish));
    }

    IEnumerator CoBackToTitle(FinishEvent finish)
    {
        // アニメ開始
        _Anime.Play("finish", 0, 0);
        yield return new WaitForSeconds(ResultEndDuration);
        finish.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField, Tooltip("スコアUI")]
    TextMeshProUGUI _ViewScore;

    [SerializeField, Tooltip("アニメ")]
    Animator _Anime;

    [SerializeField, Tooltip("出現アニメーション尺")]
    float AppearDuration = 0.5f;

    [SerializeField, Tooltip("消滅アニメーション尺")]
    float FinishDuration = 0.5f;

    public delegate void FinishEvent();

    enum State
    {
        None,
        Active,
    }
    State _State = State.None;

    // 現在のスコア
    int _Score = -9999;
    public int Score { get { return _Score; } }

    static ScoreManager _Instance = null;
    public static ScoreManager Instance { get { return _Instance; } }

    private void Start()
    {
        _Instance = this;
    }

    public void SetScore(int score)
    {
        if (score == _Score) return;
        if (_ViewScore == null) return;
        _ViewScore.text = "SCORE " + $"{score:0000}";
        _Score = score;
    }

    public void GameStart()
    {
        if (_State != State.None) return;
        _Anime.Play("Appear", 0, 0);
        _State = State.Active;
    }

    public void GameEnd(FinishEvent finish)
    {
        if (_State != State.Active) return;
        _State = State.None;
        StartCoroutine(CoEnd(finish));
    }

    IEnumerator CoEnd(FinishEvent finish)
    {
        _Anime.Play("Finish", 0, 0);
        yield return new WaitForSeconds(FinishDuration);
    }

    public void AddScore(int add)
    {
        SetScore(_Score + add);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField, Tooltip("UIルート")]
    GameObject _UIRoot;

    [SerializeField, Tooltip("ボタン")]
    Button _GameStartButton;

    [SerializeField, Tooltip("ボタン画面外位置")]
    float GameStartButtonFirstY = -800;

    [SerializeField, Tooltip("ボタン移動尺")]
    float GameStartButtonMoveDuration = 0.5f;

    [SerializeField, Tooltip("ボタン移動ディレイ")]
    float GameStartButtonMoveDelay = 1.5f;

    [SerializeField, Tooltip("ボタン移動イージング")]
    Ease GameStartButtonEase = Ease.OutExpo;

    [SerializeField, Tooltip("タイトルロゴ")]
    TextMeshProUGUI _TitleLogoText;

    [SerializeField, Tooltip("タイトルフェード尺")]
    float TitleLogoTextDuration = 1f;

    [SerializeField, Tooltip("タイトルフェードディレイ")]
    float TitleLogoTextDelay = 1f;

    [SerializeField, Tooltip("タイトルフェードイージング")]
    Ease TitleLogoTextEase = Ease.OutQuad;

    [SerializeField, Tooltip("タイトル開始時ウェイト")]
    float TitleStartWait = 2f;

    [SerializeField, Tooltip("タイトル終了時ウェイト")]
    float TitleFinishWait = 2f;

    [SerializeField, Tooltip("デフォルトボタン位置")]
    Vector2 DefaultButtonPosition;

    public delegate void FinishEvent();

    public void StartTitle(FinishEvent finish)
    {
        _UIRoot.SetActive(true);
        StartCoroutine(CoStart(finish));
    }

    IEnumerator CoStart(FinishEvent finish)
    {
        // ボタンを画面外から画面内下側へ移動
        float dest = DefaultButtonPosition.y;
        _GameStartButton.transform.localPosition = new Vector3(0, GameStartButtonFirstY);
        _GameStartButton.transform
            // 移動先・尺を指定
            .DOLocalMoveY(dest, GameStartButtonMoveDuration)
            // 開始位置指定
            .From(GameStartButtonFirstY)
            // イージング指定
            .SetEase(GameStartButtonEase)
            // ディレイ指定
            .SetDelay(GameStartButtonMoveDelay);

        // ロゴをフェードイン
        _TitleLogoText.DOFade(1, TitleLogoTextDuration)
            // 開始アルファ
            .From(0)
            // イージング指定
            .SetEase(TitleLogoTextEase)
            // ディレイ指定
            .SetDelay(TitleLogoTextDelay);

        // 待った後イベント発火
        yield return new WaitForSeconds(TitleStartWait);
        if (finish != null) finish.Invoke();
    }

    public void FinishTitle(FinishEvent finish)
    {
        StartCoroutine(CoFinish(finish));
    }

    IEnumerator CoFinish(FinishEvent finish)
    {
        _GameStartButton.transform
            // 移動先・尺を指定
            .DOLocalMoveY(GameStartButtonFirstY, GameStartButtonMoveDuration)
            // イージング指定
            .SetEase(GameStartButtonEase)
            // ディレイ指定
            .SetDelay(TitleLogoTextDelay);

        // ロゴをフェードアウト
        _TitleLogoText.DOFade(0, TitleLogoTextDuration)
            // イージング指定
            .SetEase(TitleLogoTextEase);

        // ウエイト
        yield return new WaitForSeconds(TitleFinishWait);

        // 壁プール完了待ち
        while (!WallManager.Instance.PoolFlag) yield return null;

        if (finish != null) finish.Invoke();
    }

    public void DisActive()
    {
        _UIRoot.SetActive(false);
    }
}

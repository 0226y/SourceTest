using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : MonoBehaviour
{
    public enum WallType
    {
        Top,
        MidTop,
        Mid,
        MidBottom,
        Bottom,
    }

    public enum WallState
    {
        None,
        Active,
    }

    [SerializeField, Tooltip("消滅X")]
    float VanishX;

    [SerializeField, Tooltip("1秒間に進む速度")]
    float SpeedPerSecond;

    [SerializeField, Tooltip("スコアに反映するX位置")]
    float ScorePosition;

    [SerializeField, Tooltip("壁タイプ")]
    WallType _Type;
    public WallType Type { get { return _Type; } }

    WallState _State;
    public WallState State { get { return _State; } }

    // スコア加算済みフラグ
    bool _IsAddScore = false;


    // プールされた壁を初期化
    public void Initialize()
    {
        _State = WallState.Active;
        _IsAddScore = false;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_State != WallState.Active) return;

        float x = transform.localPosition.x;
        float y = transform.localPosition.y;

        // 移動
        transform.localPosition = new Vector3(x + -SpeedPerSecond * Time.deltaTime, y);

        // スコア加算
        // [TODO] スコア加算

        // 消滅
        if(transform.localPosition.x <= VanishX)
        {
            _State = WallState.None;
            gameObject.SetActive(false);
        }
    }
}

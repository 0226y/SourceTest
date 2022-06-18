using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharaObject : MonoBehaviour
{
    public enum State
    {
        None,
        Init,       // 初期状態
        StartWait,  // ゲーム開始待ち
        Active,     // アクティブ
        GameOver,   // ゲームオーバー
    }
    State _State = State.None;
    public State GetState() { return _State; }

    [SerializeField, Tooltip("リジッドボディ")]
    Rigidbody2D _RigidBody;

    [SerializeField, Tooltip("生成位置からデフォ位置まで移動する尺")]
    float InitMoveDuration = 2.5f;

    [SerializeField, Tooltip("生成位置からデフォ位置まで移動するカーブ")]
    AnimationCurve InitMoveCurve;

    [SerializeField, Tooltip("落下とみなされる座標")]
    float LowestPosition = -500f;

    [SerializeField, Tooltip("これ以上は上昇できない高さ")]
    float HighestPosition = 500f;

    [SerializeField, Tooltip("マウス押下時のベロシティ加算")]
    float AddVelocity;

    [SerializeField, Tooltip("最大ベロシティ")]
    float MaxVelocity;

    [SerializeField, Tooltip("HITエフェクト名")]
    string HitFxName = "DamageParticle";

    [SerializeField, Tooltip("ジャンプエフェクト名")]
    string JumpFxName = "JumpParticle";

    [SerializeField, Tooltip("壁用レイヤー名")]
    string WallLayerName = "Wall";

    [SerializeField, Tooltip("アニメ")]
    Animator _Anime;

    [SerializeField, Tooltip("ゲームオーバー時のウエイト")]
    float GameOverWait;

    [SerializeField, Tooltip("ゲームオーバー時の回転(1秒につき)")]
    float GameOverRot;

    [SerializeField, Tooltip("ゲームオーバー時の落下移動値")]
    float GameOverFall;

    [SerializeField, Tooltip("ゲームオーバー時の落下移動尺")]
    float GameOverFallDuration;

    // 初期位置
    Vector2 _InitPosition;

    // デフォ位置
    Vector2 _DefaultPosition;


    private void Start()
    {
        // オブジェクト生成の瞬間、リジッドボディを無効にする(落下阻止)
        _RigidBody.simulated = false;
    }

    public void SetDefaultPosition(Vector2 vec)
    {
        _DefaultPosition = vec;
    }

    // 初期化
    public void Init(Vector2 vec)
    {
        if (_State != State.None) return;

        _InitPosition = vec;
        transform.localPosition = vec;
        _State = State.Init;

        _Anime.Play("Idle", 0, 0);

        StartCoroutine(CoInit());
    }

    IEnumerator CoInit()
    {
        float start = Time.time;
        float end = start + InitMoveDuration;

        // 終了するまで
        // [TODO] 一定時間かけて所定の位置まで移動
        yield return null;

        transform.localPosition = _DefaultPosition;
        _State = State.StartWait;
    }

    public void Update()
    {
        if (_State != State.Active) return;
        var pos = transform.localPosition;

        // ジャンプ
        // [TODO] ジャンプ処理

        // ミス判定
        if (IsMiss())
        {
            _State = State.GameOver;
            StartCoroutine(CoGameOver());
            _RigidBody.simulated = false;
            SceneGame.Instance.OnMissSalamander();
        }
    }

    // ゲームオーバー時の挙動
    IEnumerator CoGameOver()
    {
        yield return new WaitForSeconds(GameOverWait);

        // [TODO] ゲームオーバー時の挙動を作ろう
    }

    // Active状態のとき、障害物に当たるか最大まで落下するかでtrueを返す
    public bool IsMiss()
    {
        // [TODO] 落下した判定

        return false;
    }

    public void SetStateActive()
    {
        _State = State.Active;
        _RigidBody.simulated = true;
    }

    // [TODO] 壁との接触判定
}

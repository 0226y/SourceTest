using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharaManager : MonoBehaviour
{
    [SerializeField, Tooltip("プレイヤー初期位置")]
    Vector2 InitializePosition;

    [SerializeField, Tooltip("プレイヤーデフォ位置")]
    Vector2 DefaultPosition;

    [SerializeField, Tooltip("プレイヤー生成ルート")]
    Transform CreateRoot;

    // シングルトンインスタンス
    public static PlayerCharaManager Instance { get { return _Instance; } }
    static PlayerCharaManager _Instance = null;

    // プレイヤーキャラ
    PlayerCharaObject _PlayerChara;
    public PlayerCharaObject PlayerChara { get { return _PlayerChara; } }

    // プレイヤー生成完了イベント
    public delegate void OnCreatePlayerChara();

    private void Start()
    {
        _Instance = this;
    }

    // プレイヤー生成
    public void CreatePlayerChara(OnCreatePlayerChara onFinish)
    {
        PrefabCreater.Instance.Create(PrefabCreater.Category.PlayerChara, "PlayerChara", (GameObject obj) => {
            if (obj == null) return;
            if (onFinish == null) return;

            obj.transform.SetParent(CreateRoot, true);

            var player = obj.GetComponent<PlayerCharaObject>();
            if (player == null) return;

            _PlayerChara = player;

            _PlayerChara.SetDefaultPosition(DefaultPosition);
            _PlayerChara.Init(InitializePosition);
            onFinish.Invoke();
        });
    }

    // ゲーム開始時
    public void OnGameStart()
    {
        if (_PlayerChara == null) return;
        if (_PlayerChara.GetState() != PlayerCharaObject.State.StartWait) return;

        _PlayerChara.SetStateActive();
    }

    public void DestroyChara()
    {
        if (_PlayerChara == null) return;
        Destroy(_PlayerChara.gameObject);
        _PlayerChara = null;
    }
}

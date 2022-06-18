using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField, Tooltip("壁オブジェクトのコピー元リスト")]
    List<string> WallSourcePrefabs;

    [SerializeField, Tooltip("プールする数")]
    int PoolLength;

    // プールしたオブジェクトのリスト
    List<WallObject> _WallList = new List<WallObject>();

    // プール済みフラグ
    bool _PoolFlag = false;
    public bool PoolFlag { get { return _PoolFlag; } }

    [SerializeField, Tooltip("壁出現テーブル<(何回目までの出現で適用するか, 出現間隔>")]
    List<CustomPair<int, float>> WallAppearTable;

    [SerializeField, Tooltip("テーブルに存在しない回数のデフォ出現間隔")]
    float DefaultAppearTime = 1f;

    [SerializeField, Tooltip("生成ルート")]
    Transform CreateRoot;

    [SerializeField, Tooltip("生成座標")]
    Vector3 CreatePosition;

    public enum State
    {
        None,
        Active,
    }
    State _State = State.None;

    static WallManager _Instance = null;
    public static WallManager Instance { get { return _Instance; } }

    // プールオブジェクトカウント
    int _PoolCount = 0;

    // 初期化済みフラグ
    bool _Initialized = false;

    // 壁生成数
    int _CreateCount;

    // 次に生成する時間
    float _NextCreateTime;

    // 前回生成時間
    float _PrevCreateTime;

    private void Start()
    {
        _Instance = this;
    }

    // 壁オブジェクトをプールする
    public void PoolWall()
    {
        if (_PoolFlag) return;
        if (_Initialized) return;

        _Initialized = true;
        foreach (var prefabName in WallSourcePrefabs)
        {
            for (int i = 0; i < PoolLength; ++i)
            {
                PrefabCreater.Instance.Create(PrefabCreater.Category.Wall, prefabName, (GameObject obj) =>
                {
                    if (obj == null) return;
                    var wall = obj.GetComponent<WallObject>();
                    if (wall == null) return;

                    wall.transform.SetParent(CreateRoot, false);
                    _WallList.Add(wall);
                    wall.gameObject.SetActive(false);

                    _PoolCount++;

                    if (WallSourcePrefabs.Count * PoolLength <= _PoolCount)
                    {
                        _PoolFlag = true;
                    }
                });
            }
        }
    }

    // 壁抽選
    // [TODO] 壁抽選処理

    private void Update()
    {
        if (_State != State.Active) return;


        // 生成
        // [TODO] 壁生成処理
    }

    // 次回生成時間を取得
    float GetNextCreateTime()
    {
        // テーブルから現在の出現間隔を取得
        float time = DefaultAppearTime;
        var table = WallAppearTable
            .Where(it => _CreateCount < it.key)
            .FirstOrDefault();
        if (table != null)
        {
            time = table.value;
        }
        return _PrevCreateTime + time;
    }

    // ゲーム開始時
    public void OnGameStart()
    {
        _State = State.Active;
        _CreateCount = 0;
        _PrevCreateTime = Time.time;
        _NextCreateTime = GetNextCreateTime();
    }

    // ゲームオーバー時
    public void OnGameOver()
    {
        _State = State.None;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGame : MonoBehaviour
{
    public enum GameState
    {
        Init,
        Title,
        GameStart,
        Game,
        GameOver,
        Result,
        Ranking,
        ToTitle,
    }
    GameState _State = GameState.Init;

    static SceneGame _Instance = null;
    public static SceneGame Instance { get { return _Instance; } }

    [SerializeField, Tooltip("タイトルマネージャ")]
    TitleManager _TitleManager;

    [SerializeField, Tooltip("ゲームスタートマネージャ")]
    GameStartManager _GameStartManager;

    [SerializeField, Tooltip("ゲームマネージャ")]
    IngameManager _GameManager;

    [SerializeField, Tooltip("ゲームオーバーマネージャ")]
    GameOverManager _GameOverManager;

    [SerializeField, Tooltip("リザルトマネージャ")]
    ResultManager _ResultManager;

    [SerializeField, Tooltip("ランキングマネージャ")]
    RankingManager _RankingManager;

    [SerializeField, Tooltip("ポストプロセスマネージャ")]
    PostProcessManager _PostProcessManager;

    [SerializeField, Tooltip("被写界深度デフォルト")]
    float DepthDefault = 32f;

    [SerializeField, Tooltip("被写界深度有効時")]
    float DepthEnabled = 3f;

    [SerializeField, Tooltip("被写界深度変更尺")]
    float DepthChangeDuration = 0.5f;

    void Start()
    {
        _Instance = this;

        // BGM
        Sound.Instance.Play(1, Sound.CH.BGM1, true, 1);

        // セーブデータロード
        SaveData.CreateInstance();
        SaveData.Instance.Load();

        _PostProcessManager.TweenDepthOfFieldApeture( DepthDefault, 0, DG.Tweening.Ease.Linear);

        _GameStartManager.DisActive();
        _GameManager.DisActive();
        _GameOverManager.DisActive();
        _ResultManager.DisActive();
        _RankingManager.DisActive();

        _TitleManager.StartTitle(()=>
        {
            _State = GameState.Title;
            WallManager.Instance.PoolWall();
        });
    }

    // GAMESTARTボタンが押されたとき
    public void OnClickGameStart()
    {
        if(_State != GameState.Title)
        {
            return;
        }

        _TitleManager.FinishTitle(() =>
        {
            _State = GameState.GameStart;
            _TitleManager.DisActive();

            _GameManager.GameStart();

            // ゲーム開始演出が終わった
            _GameStartManager.GameStart(()=>
            {
                _State = GameState.Game;
                _GameStartManager.DisActive();
                PlayerCharaManager.Instance.OnGameStart();
                WallManager.Instance.OnGameStart();
            });
        });
    }

    // サラマンダーがミスしたとき
    public void OnMissSalamander()
    {
        if (_State != GameState.Game)
        {
            return;
        }
        _State = GameState.GameOver;
        _GameManager.GameEnd();

        // ミスSE
        Sound.Instance.Play(12, Sound.CH.SE_ONESHOT);

        // 既存BGMをフェードアウト
        Sound.Instance.Fade(Sound.CH.BGM1, 3f, 0f, false);

        // ゲームオーバー演出
        _GameOverManager.StartGameOver(() =>
        {
            // 画面をぼかす
            _PostProcessManager.TweenDepthOfFieldApeture(DepthEnabled, DepthChangeDuration, DG.Tweening.Ease.Linear);
            _GameOverManager.DisActive();

            // プレイヤーキャラ破棄
            PlayerCharaManager.Instance.DestroyChara();
            WallManager.Instance.OnGameOver();

            // セーブデータ更新
            SaveData.Instance.AddNewScore(ScoreManager.Instance.Score);
            SaveData.Instance.Save();

            // リザルト開始
            _State = GameState.Result;
            _ResultManager.SetHiScore(SaveData.Instance.GetHiScore());
            _ResultManager.SetScore(ScoreManager.Instance.Score);
            string rank = SaveData.Instance.GetRank(ScoreManager.Instance.Score);
            bool isRankIn = (!string.IsNullOrEmpty(rank));
            _ResultManager.SetRank(isRankIn, rank);
            _ResultManager.StartResult();
        });
    }

    // リザルトからタイトルに戻るボタンが押された
    public void OnBackToTitle()
    {
        if (_State != GameState.Result)
        {
            return;
        }

        // メインBGMをフェードアウト
        Sound.Instance.Fade(Sound.CH.BGM1, 3f, 1f, false);

        // リザルトBGMをフェードアウト
        Sound.Instance.Fade(Sound.CH.BGM2, 3f, 0f, true);

        _State = GameState.ToTitle;

        _ResultManager.BackToTitle(()=>
        {
            _ResultManager.DisActive();
            _PostProcessManager.TweenDepthOfFieldApeture(DepthDefault, DepthChangeDuration, DG.Tweening.Ease.Linear);

            _TitleManager.StartTitle(()=>
            {
                _State = GameState.Title;
            });
        });
    }

    // リザルトからランキングを開くボタンが押された
    public void OnOpenRanking()
    {
        if (_State != GameState.Result)
        {
            return;
        }

        _State = GameState.Ranking;
        _ResultManager.BackToTitle(() =>
        {
            _ResultManager.DisActive();
            _RankingManager.StartRanking();
            _RankingManager.UpdateRankingData();
        });
    }

    // ランキングからタイトルに戻るボタンが押された
    public void OnBackToTitleRanking()
    {
        if (_State != GameState.Ranking)
        {
            return;
        }

        // メインBGMをフェードアウト
        Sound.Instance.Fade(Sound.CH.BGM1, 3f, 1f, false);

        // リザルトBGMをフェードアウト
        Sound.Instance.Fade(Sound.CH.BGM2, 3f, 0f, true);

        _State = GameState.ToTitle;

        _RankingManager.BackToTitle(() =>
        {
            _RankingManager.DisActive();
            _PostProcessManager.TweenDepthOfFieldApeture(DepthDefault, DepthChangeDuration, DG.Tweening.Ease.Linear);

            _TitleManager.StartTitle(() =>
            {
                _State = GameState.Title;
            });
        });
    }
}

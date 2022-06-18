using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sound : MonoBehaviour
{
    // サウンドイベントオブジェクト
    public class SoundEvent
    {
        public enum Type
        {
            PLAY,
            PAUSE,
            STOP,
            CHANGE_VOL,
            FADE,
        }

        public Type ev;
        public int ID;
        public CH CH;
        public bool loop;
        public float vol;
        public float duration;
        public bool isStop;

        public SoundEvent(Type ev, int _id, CH _ch, bool loop, float _volume, float _duration, bool isStop)
        {
            this.ev = ev;
            ID = _id;
            CH = _ch;
            this.loop = loop;
            vol = _volume;
            duration = _duration;
            this.isStop = isStop;
        }
    };

    // 停止用オブジェクト
    public class SndStopObj
    {
    }

    // ポーズ用オブジェクト
    public class SndPauseObj
    {
        public CH mCH;
    };

    // 再生チャンネル
    public enum CH
    {
        BGM1 = 0,           // BGM1
        BGM2,               // BGM2
        SE_ONESHOT,         // 一度だけ再生するSE用
        MAX,                // 最大チャンネル数
    };

    //[AssetReferenceTypeRestriction(typeof(AudioClip))]
    //public AssetReference[] m_ref;

    // マスターボリューム
    float MasterVolumeBGM = 1f;
    float MasterVolumeSE = 1f;

    [SerializeField, Tooltip("ボリュームの音量カーブ")]
    AnimationCurve MasterVolCurve;
    public AnimationCurve VolCurve { get { return MasterVolCurve; } }

    [SerializeField, Tooltip("サウンドデータベース参照")]
    SoundDatabase Database;

    [SerializeField, Tooltip("サウンドプレイヤー(コピー元)")]
    GameObject SourcePlayer;

    // サウンドプレイヤー
    List<SoundPlayer> players = null;

    // オーディオクリップ(ID/AudioClip)
    Dictionary<int, AudioClip> clips = new Dictionary<int, AudioClip>();

    // キュー
    List<SoundEvent> sndQue = new List<SoundEvent>();

    // インスタンス
    static Sound _Instance = null;
    public static Sound Instance { get { return _Instance; } }

    // 起動時
    void Awake()
    {
        _Instance = this;

        // サウンドプレイヤーとオーディオソースの登録
        players = new List<SoundPlayer>();
        for (int i = 0; i < (int)CH.MAX; i++)
        {
            var p = Instantiate(SourcePlayer);
            if (p == null)
            {
                Debug.LogError("SoundPlayerのソースが見つかりません");
                break;
            }

            var player = p.GetComponent<SoundPlayer>();
            if (player == null)
            {
                Debug.LogError("SoundPlayerのコンポーネントが見つかりません");
                break;
            }

            player.transform.SetParent(SourcePlayer.transform.parent, false);
            players.Add(player);
        }
        SourcePlayer.SetActive(false);
    }

    IEnumerator LoadAudioClip(int id)
    {
        var clip = Database.GetAudio(id);
        if (clip == null)
        {
            yield break;
        }

        clips.Add(id, clip);
        yield break;
    }

    // さらに簡易再生
    public void Play(int _id, CH _ch)
    {
        Play(_id, _ch, false, 1.0f);
    }

    // 簡易再生
    public void Play(int _id, CH _ch, bool loop)
    {
        Play(_id, _ch, loop, 1.0f);
    }

    // 再生
    public void Play(int _id, CH _ch, bool loop, float _volume)
    {
        if (_Instance == null) return;
        StartCoroutine(CoPlay(_id, _ch, loop, _volume));
    }

    IEnumerator CoPlay(int _id, CH _ch, bool loop, float _volume)
    {
        int id = (int)_id;
        int ch = (int)_ch;

        // 範囲チェック
        if (Database.GetAudio(id) == null) yield break;
        if (ch < 0) yield break;
        if (ch >= (int)CH.MAX) yield break;

        //@		// デモ録画中はBGMを鳴らさない
        //@		#if	DEMO_REC
        //@		{
        //@			if( _ch == CH.BGM ) return;
        //@		}
        //@		#endif

        // サウンドデータを取得できなければロード
        if (!clips.ContainsKey(id))
        {
            yield return StartCoroutine(LoadAudioClip(_id));
        }

        // それでも取得できなければやめる
        if (!clips.ContainsKey(id)) yield break;

        // すでに登録済みの音は多重再生を避ける
        bool daburi = false;
        foreach (SoundEvent q in sndQue)
        {
            if (q.ID == _id)
            {
                daburi = true;
                break;
            }
        }

        // ダブりがなければ再生
        if (!daburi)
        {
            sndQue.Add(new SoundEvent(SoundEvent.Type.PLAY, _id, _ch, loop, _volume, 0f, false));
        }
    }

    // ポーズ
    public void Pause(CH _ch)
    {
        int ch = (int)_ch;

        // 範囲チェック
        if (ch < 0) return;
        if (ch >= (int)CH.MAX) return;

        sndQue.Add(new SoundEvent(SoundEvent.Type.PAUSE, -1, _ch, false, 0f, 0f, false));

    }

    // ポーズ(全ソース)
    public void Pause()
    {
        for (int i = 0; i < (int)CH.MAX; i++)
        {
            Pause((CH)i);
        }
    }

    // ストップ
    public void Stop(CH _ch)
    {
        int ch = (int)_ch;

        // 範囲チェック
        if (ch < 0) return;
        if (ch >= (int)CH.MAX) return;

        sndQue.Add(new SoundEvent(SoundEvent.Type.STOP, -1, (CH)ch, false, 0f, 0f, false));
    }

    // ストップ(全ソース)
    public void Stop()
    {
        for (int i = 0; i < (int)CH.MAX; i++)
        {
            Stop((CH)i);
        }
    }

    // ボリューム調整
    public void SetVolume(CH _ch, float vol)
    {
        if (vol < 0f) vol = 0f;
        if (vol > 1f) vol = 1f;

        int ch = (int)_ch;

        // 範囲チェック
        if (ch < 0) return;
        if (ch >= (int)CH.MAX) return;

        sndQue.Add(new SoundEvent(SoundEvent.Type.CHANGE_VOL, -1, (CH)ch, false, vol, 0f, false));
    }

    // マスタボリューム調整
    public void SetMasterVolume(float vol)
    {
        if (vol < 0f) vol = 0f;
        if (vol > 1f) vol = 1f;

        MasterVolumeBGM = MasterVolCurve.Evaluate(vol);
        MasterVolumeSE = MasterVolumeBGM;

        for (int i = 0; i < (int)CH.MAX; i++)
        {
            players[i].SetMasterVolume(vol);
        }
    }

    // マスタボリューム調整(BGM)
    public void SetMasterVolumeBGM(float vol)
    {
        if (vol < 0f) vol = 0f;
        if (vol > 1f) vol = 1f;

        MasterVolumeBGM = MasterVolCurve.Evaluate(vol);
        players[(int)CH.BGM1].SetMasterVolume(vol);
        players[(int)CH.BGM2].SetMasterVolume(vol);
    }

    // マスタボリューム調整(SE)
    public void SetMasterVolumeSE(float vol)
    {
        if (vol < 0f) vol = 0f;
        if (vol > 1f) vol = 1f;

        MasterVolumeSE = MasterVolCurve.Evaluate(vol);

        for (int i = 0; i < (int)CH.MAX; i++)
        {
            if (i == (int)CH.BGM1) continue;
            if (i == (int)CH.BGM2) continue;
            players[i].SetMasterVolume(vol);
        }
    }

    // フェード
    public void Fade(CH _ch, float duration, float dest, bool isStop)
    {
        int ch = (int)_ch;

        // 範囲チェック
        if (ch < 0) return;
        if (ch >= (int)CH.MAX) return;

        sndQue.Add(new SoundEvent(SoundEvent.Type.FADE, -1, (CH)ch, false, dest, duration, isStop));
    }

    // 更新
    void Update()
    {
        // サウンドキュー処理
        foreach (SoundEvent q in sndQue)
        {
            switch (q.ev)
            {
                // 再生
                case SoundEvent.Type.PLAY:
                    {
                        // ワンショット用再生
                        if (CH.SE_ONESHOT == q.CH)
                        {
                            AudioClip clip = clips[q.ID];
                            players[(int)q.CH].PlayOneShot(clip, q.vol, MasterVolumeSE);
                        }
                        // その他再生
                        else
                        {
                            players[(int)q.CH].Stop();
                            AudioClip clip = clips[q.ID];
                            float volume = (q.CH == CH.BGM1 || q.CH == CH.BGM2) ? MasterVolumeBGM : MasterVolumeSE;
                            players[(int)q.CH].Play(clip, q.vol, volume, q.loop);
                        }
                    }
                    break;

                // ポーズ
                case SoundEvent.Type.PAUSE:
                    {
                        players[(int)q.CH].Pause();
                    }
                    break;

                // 停止
                case SoundEvent.Type.STOP:
                    {
                        players[(int)q.CH].Stop();
                    }
                    break;

                // ボリューム調整
                case SoundEvent.Type.CHANGE_VOL:
                    {
                        players[(int)q.CH].SetVolume(q.vol);
                    }
                    break;

                // フェード
                case SoundEvent.Type.FADE:
                    {
                        players[(int)q.CH].Fade(q.duration, q.isStop, q.vol);
                    }
                    break;
            }
        }
        sndQue.Clear();
    }
}

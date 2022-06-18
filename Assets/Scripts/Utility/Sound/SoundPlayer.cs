using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("オーディオソース")]
    AudioSource _Source = null;

    float _Volume = 1f;
    float _MstVol = 1f;
    Coroutine _CoFade;

    private enum STATE
    {
        NORMAL,
        FADE,
    };
    STATE _State = STATE.NORMAL;

    private void Start()
    {
        _State = STATE.NORMAL;
    }

    // ワンショット再生
    public void PlayOneShot(AudioClip clip, float volume, float mstVol)
    {
        _Volume = volume;
        _MstVol = mstVol;
        float vol = _Volume * _MstVol;

        _Source.PlayOneShot(clip, vol);
    }

    // 再生
    public void Play(AudioClip clip, float volume, float mstVol, bool loop)
    {
        // 状態を強制的に初期化
        _State = STATE.NORMAL;

        _Volume = volume;
        _MstVol = mstVol;
        float vol = _Volume * _MstVol;

        _Source.Stop();
        _Source.clip = clip;
        _Source.loop = loop;
        _Source.Play();
        _Source.volume = vol;
    }

    // ポーズ
    public void Pause()
    {
        _Source.Pause();
    }

    // ストップ
    public void Stop()
    {
        _Source.Stop();
    }

    // ボリューム調整
    public void SetVolume(float vol)
    {
        _Volume = vol;
        _Source.volume = _Volume * _MstVol;
    }

    // マスターボリューム調整
    public void SetMasterVolume(float mstVol)
    {
        _MstVol = mstVol;
        float vol = _Volume * _MstVol;

        _Source.volume = vol;
    }

    // フェード
    public void Fade(float duration, bool isStop, float dest)
    {
        // 既にフェード中ならコルーチンを止める
        if(_State == STATE.FADE)
        {
            if (_CoFade != null) StopCoroutine(_CoFade);
            _CoFade = null;
        }

        _State = STATE.FADE;
        _CoFade = StartCoroutine(CoFade(duration, isStop, dest));
    }

    IEnumerator CoFade(float duration, bool isStop, float dest)
    {
        float firstVol = _Volume;
        float start = Time.time;
        float end = start + duration;

        while (true)
        {
            // 所定の時間が来たらフェード終了
            float now = Time.time;
            float current = now - start;
            if(now >= end)
            {
                _Volume = dest;
                // ストップフラグがあればサウンド止める
                if (isStop) Stop();
                break;
            }

            // 現在の値を取得
            float t = current / duration;
            _Volume = Mathf.Lerp(firstVol, dest, Sound.Instance.VolCurve.Evaluate(t));
            yield return null;

            _State = STATE.NORMAL;
        }
    }

    private void Update()
    {
        _Source.volume = _Volume;
    }
}

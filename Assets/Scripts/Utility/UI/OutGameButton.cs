using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class OutGameButton : MonoBehaviour
{
    [SerializeField]
    AudioClip _Sound;

    AudioSource _Source;

    [SerializeField]
    float _Scale = 1.1f;

    [SerializeField]
    float _NextPushableTime = 0.5f;

    //[SerializeField]
    public UnityEvent OnClickEvent;

    bool _Pushed = false;

    Image _Image;

    bool _IsInitialized = false;

    private void Start()
    {
        if (_IsInitialized) return;
        _IsInitialized = true;

        Button button = gameObject.GetComponent<Button>();
        if (button == null) return;

        if(_Sound != null)
        {
            _Source = gameObject.AddComponent<AudioSource>();
            _Source.playOnAwake = false;
            _Source.clip = _Sound;
        }

        OnClickEvent = button.onClick;
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(OnClick);

        _Image = gameObject.GetComponent<Image>();
    }

    public void OnClick()
    {
        Debug.Log("OnClick:" + gameObject.name);
        if (_Pushed) return;

        // イベント
        if (OnClickEvent != null) OnClickEvent.Invoke();

        ClickMove();
    }

    IEnumerator NextPushWait()
    {
        yield return new WaitForSeconds(_NextPushableTime);
        _Pushed = false;

        if (_Image != null)
            _Image.raycastTarget = true;
    }

    private void OnDisable()
    {
        _Pushed = false;
        if (_Image != null) _Image.raycastTarget = true;
    }

    // クリック時動作
    void ClickMove()
    {

        // 拡大＆縮小
        RectTransform rectTran = gameObject.GetComponent<RectTransform>();
        Sequence seq = DOTween.Sequence().OnStart(() =>
        {
            gameObject.transform.localScale = Vector2.one;
        })
        .Append(rectTran.DOScale(_Scale, 0.2f)).SetEase(Ease.InBounce)
        .Append(rectTran.DOScale(1.0f, 0.2f)).SetEase(Ease.OutBounce);

        // サウンド鳴らす
        if (_Sound != null)
        {
            _Source.Play();
        }

        if (_NextPushableTime > 0)
        {
            if (_Image != null) _Image.raycastTarget = false;
            _Pushed = true;
            StartCoroutine(NextPushWait());
        }
    }
}

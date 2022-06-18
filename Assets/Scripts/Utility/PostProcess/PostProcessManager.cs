using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    [SerializeField, Tooltip("ポストプロセス")]
    PostProcessVolume _Volume;

    DepthOfField _DepthOfField;


    private void Start()
    {
        _DepthOfField = _Volume.profile.GetSetting<DepthOfField>();
    }

    // Depth Of FieldのApetureを変更
    public void TweenDepthOfFieldApeture(float end, float duration, Ease ease)
    {
        DOTween.To(() => _DepthOfField.aperture.value, (x) => _DepthOfField.aperture.value = x, end, duration)
            .SetEase(ease);
    }
}

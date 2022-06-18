using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFx : MonoBehaviour
{
    [SerializeField, Tooltip("押下時エフェクト")]
    ParticleSystem _PushParticle;

    [SerializeField, Tooltip("押下時エミッション数")]
    int EmitPush;

    [SerializeField, Tooltip("押し続け時エフェクト")]
    ParticleSystem _HoldParticle;

    [SerializeField, Tooltip("押し続け時エミッション数")]
    int EmitHold;

    [SerializeField, Tooltip("押し続け時エミッション頻度")]
    float HoldEmitWait = 0.1f;

    [SerializeField, Tooltip("カメラ")]
    Camera _Camera;

    float _Next = 0;

    private void Update()
    {
        // 時間経過
        float now = Time.time;

        // 押下
        if (Input.GetMouseButtonDown(0))
        {
            // [TODO] 押下した箇所に_PushParticleを生成
        }

        // 押し続け
        if (now >= _Next)
        {
            _Next += HoldEmitWait;

            // [TODO] 押下した箇所に_HoldParticleを生成
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingItem : MonoBehaviour
{
    [SerializeField, Tooltip("ランク")]
    TextMeshProUGUI _Rank;

    [SerializeField, Tooltip("スコア")]
    TextMeshProUGUI _Score;

    [SerializeField, Tooltip("順位")]
    int _RankValue;

    public void SetRank(int rank)
    {
        _RankValue = rank;
    }

    public void SetScore(int score)
    {
        _Rank.text = SaveData.GetRankText(_RankValue);
        _Score.text = score.ToString();
    }
}

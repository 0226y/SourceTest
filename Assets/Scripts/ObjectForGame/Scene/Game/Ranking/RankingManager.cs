using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    [SerializeField, Tooltip("UIルート")]
    GameObject _UIRoot;

    [SerializeField, Tooltip("コピー元")]
    GameObject RankItemSource;

    [SerializeField, Tooltip("ランキングに乗せる最大順位")]
    int MaxRank = 10;

    [SerializeField, Tooltip("ランキング要素のルート")]
    Transform _RankItemRoot;

    [SerializeField, Tooltip("アニメ")]
    Animator _Anime;

    [SerializeField, Tooltip("ランキング開始演出尺")]
    float StartDuration = 3f;

    [SerializeField, Tooltip("ランキング終了演出尺")]
    float EndDuration = 3f;

    [SerializeField, Tooltip("スクロール")]
    ScrollRect _Scroll;

    public delegate void FinishEvent();

    List<RankingItem> _RankList;

    public void DisActive()
    {
        _UIRoot.SetActive(false);

        if (_RankList != null)
        {
            foreach(var rank in _RankList)
            {
                Destroy(rank.gameObject);
            }
            _RankList.Clear();
        }
    }

    public void StartRanking()
    {
        _UIRoot.SetActive(true);
        _RankList = new List<RankingItem>();

        for (int i = 0; i < MaxRank; i++)
        {
            var obj = Instantiate(RankItemSource) as GameObject;
            if (obj == null) continue;

            obj.transform.SetParent(_RankItemRoot, false);

            var rank = obj.GetComponent<RankingItem>();
            if (rank == null)
            {
                Destroy(obj);
            }

            rank.SetRank(i + 1);
            rank.SetScore(Random.Range(0, 9999));

            _RankList.Add(rank);
        }
        RankItemSource.SetActive(false);

        StartCoroutine(CoStart());
    }

    IEnumerator CoStart()
    {
        _Anime.Play("appear", 0, 0);
        yield return new WaitForSeconds(StartDuration);
    }

    public void BackToTitle(FinishEvent finish)
    {
        StartCoroutine(CoBackToTitle(finish));
    }

    IEnumerator CoBackToTitle(FinishEvent finish)
    {
        _Anime.Play("end", 0, 0);
        yield return new WaitForSeconds(EndDuration);
        if (finish != null) finish.Invoke();
    }

    public void UpdateRankingData()
    {
        for (int i = 0; i < _RankList.Count; ++i)
        {
            var rank = _RankList[i];
            var data = SaveData.Instance.GetData(i);
            if (data == null) rank.gameObject.SetActive(false);
            else
            {
                rank.gameObject.SetActive(true);
                rank.SetRank(i + 1);
                rank.SetScore(data.score);
            }
        }
        StartCoroutine(AdjustScroll());
    }

    IEnumerator AdjustScroll()
    {
        yield return new WaitForEndOfFrame();
        _Scroll.verticalNormalizedPosition = 1f;
    }
}

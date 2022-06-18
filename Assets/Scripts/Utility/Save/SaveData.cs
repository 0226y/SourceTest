using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// ランキング用単体データ
[Serializable]
public class RankingData
{
    public float saveTime;
    public int score;
}

// ListはそのままではJson化できないので、シリアライズ用クラスでブリッジする
public class SerializeList<T>
{
    [SerializeField]
    List<T> data;
    public List<T> ToList() { return data; }
    public SerializeList(List<T> t)
    {
        data = t;
    }
}

// セーブデータ
// Json形式でテキストデータとして保存
// ランキングデータをリストで持つだけ
// 最大10件
[Serializable]
public class SaveData
{
    private static SaveData _Instance = null;
    public static SaveData Instance { get { return _Instance; } }
    public static void CreateInstance()
    {
        _Instance = new SaveData();
    }

    List<RankingData> ranking;

    // セーブデータ保存
    public void Save()
    {
        // [TODO] rankingをjson化してテキストファイルに保存
    }

    // セーブデータ読み込み
    public void Load()
    {
        // [TODO] jsonデータを読み込んでrankingを初期化
    }

    // 新規データ追加
    public void AddNewScore(int score)
    {
        // [TODO] 新しいスコアを追加
    }

    // ハイスコアを取得
    public int GetHiScore()
    {
        if (ranking == null) return 0;
        if (ranking.Count == 0) return 0;
        return ranking.Max(it => it.score);
    }

    public static string GetRankText(int rank)
    {
        string ret = rank.ToString();

        switch (rank % 10)
        {
            case 1:
                ret += "st";
                break;

            case 2:
                ret += "nd";
                break;

            case 3:
                ret += "rd";
                break;

            default:
                ret += "th";
                break;
        }

        return ret;
    }

    // 該当スコアの順位情報を取得
    public string GetRank(int score)
    {
        if (ranking == null) return "";
        if (ranking.Count == 0) return "";

        var data = ranking.FirstOrDefault(it => it.score == score);
        if (data == null) return "";

        int index = ranking.IndexOf(data);
        return GetRankText(index + 1);
    }

    // 指定インデックスのランキングデータを取得
    public RankingData GetData(int index)
    {
        if (index < 0) return null;
        if (ranking.Count <= index) return null;
        return ranking[index];
    }
}


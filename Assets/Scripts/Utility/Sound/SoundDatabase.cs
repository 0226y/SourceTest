using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundDatabase : MonoBehaviour
{
    [SerializeField, Tooltip("オーディオIDと、それに紐づかせるAudioClipのリスト")]
    List<CustomPair<int, AudioClip>> AudioClipList;

    // オーディオ取得
    public AudioClip GetAudio(int id)
    {
        var data = AudioClipList
            .Where(it => it.key == id)
            .FirstOrDefault();
        if (data == null) return null;
        return data.value;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}

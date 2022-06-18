using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCreater : MonoBehaviour
{
    public static PrefabCreater Instance { get { return _Instance; } }
    static PrefabCreater _Instance = null;

    public delegate void OnLoadFinish(GameObject obj);

    public enum Category
    {
        PlayerChara,
        Wall,
        Fx,
    }

    private void Start()
    {
        _Instance = this;
    }

    public void Create(Category category, string name, OnLoadFinish onFinish)
    {
        // ディレクトリ名を決める
        string directory = "";
        switch(category)
        {
            case Category.Wall:
                directory = "Prefabs/Wall/";
                break;

            case Category.PlayerChara:
                directory = "Prefabs/PlayerChara/";
                break;

            case Category.Fx:
                directory = "Prefabs/Fx/";
                break;
        }

        // 非同期ロード
        // [TODO]プレハブの読み込み
    }

    // [TODO]プレハブの読み込み
}

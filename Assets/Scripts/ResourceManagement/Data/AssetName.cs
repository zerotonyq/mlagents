using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ResourceManagement.Data
{
    public sealed class AssetName
    {
        private readonly string _name;
        private readonly int _value;

        public static readonly AssetName Player = new(1, "PlayerDataAsset.asset");
        public static readonly AssetName TimerData = new(2, "TimerDataAsset.asset");
        public static readonly AssetName Intro = new(3, "Intro.prefab");
        public static readonly AssetName Menu = new(4, "Menu.prefab");
        public static readonly AssetName MapData = new(5, "Map.asset");
        private AssetName(int value, String name)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
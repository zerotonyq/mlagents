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
        public static readonly AssetName StartScreenPressAnyButton = new(3, "StartScreenPressAnyButton.asset");
        public static readonly AssetName StartScreenScrollView = new(4, "StartScreenScrollView.asset");
        public static readonly AssetName StartScreenFill = new(5, "StartScreenFill.asset");
        
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
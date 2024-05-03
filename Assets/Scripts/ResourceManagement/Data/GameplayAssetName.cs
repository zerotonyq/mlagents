using System;

namespace ResourceManagement.Data
{
    public sealed class GameplayAssetName
    {
        private readonly string _name;
        private readonly int _value;

        public static readonly GameplayAssetName Player = new(1, "PlayerDataAsset.asset");
        public static readonly GameplayAssetName TimerData = new(2, "TimerDataAsset.asset");
        public static readonly GameplayAssetName Intro = new(3, "Intro.prefab");
        public static readonly GameplayAssetName Menu = new(4, "Menu.prefab");
        public static readonly GameplayAssetName MapData = new(5, "Map.asset");
        public static readonly GameplayAssetName TerrainMaterial = new(5, "TerrainMaterial.asset");
        private GameplayAssetName(int value, String name)
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
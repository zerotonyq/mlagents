using UnityEngine;

namespace Player.Data
{
    [CreateAssetMenu(menuName = "DataAssets/PlayerDataAsset", fileName = "PlayerDataAsset")]
    public class CharacterDataAsset : ScriptableObject
    {
        public GameObject prefab;
    }
}
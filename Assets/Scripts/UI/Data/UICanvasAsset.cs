using UnityEngine;

namespace UI.Data
{
    [CreateAssetMenu(menuName = "DataAssets/UICanvasAsset", fileName = "UICanvasAsset")]
    public class UICanvasAsset : ScriptableObject
    {
        public Canvas Canvas;
    }
}
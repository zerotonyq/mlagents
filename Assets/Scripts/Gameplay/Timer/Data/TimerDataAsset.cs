using TMPro;
using UI.Data;
using UnityEngine;

namespace DefaultNamespace.UIManagement.GameplayHud.Data
{
    [CreateAssetMenu(menuName = "DataAssets/TimerDataAsset", fileName = "TimerDataAsset")]
    public class TimerDataAsset : ScriptableObject
    {
        public UICanvasAsset TimerTextCanvas;
        public float Time;
    }
}
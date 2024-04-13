using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StartScreen.Fsm.States.Data
{
    [CreateAssetMenu(menuName = "UI/Data/MenuSelectionData")]
    public class MenuSelectionData : ScriptableObject
    {
        public Button SettingsButton;
        public Button IntroButton;
        public Button Customization;
        public List<Button> WorldButtons = new();
    }
}
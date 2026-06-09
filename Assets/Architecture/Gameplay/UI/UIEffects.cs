/*
 * Description:  Example class that holds some UI effects
 */

using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    public class UIEffects
    {
        public static string ApplyStrikeThrough(string textToEffect, string displayText)
        {
            return textToEffect = $"<s>{displayText}</s>";
        }

        public static string ApplyColor(string textToEffect, string displayText, string color)
        {
            return textToEffect = $"<color={color}>{displayText}</color>";
        }

        public static string ApplySubtext(string textToEffect, string subText)
        {
            string newDisplayText = $"{textToEffect}  {subText}";
            return textToEffect = newDisplayText;
        }
    }
}
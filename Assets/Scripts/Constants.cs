using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    public const string apiKey = "7638c74f8f7cfbbe061fca540834e40e403e81026e9af2b7aacd0d2a88dcbe12";
    public const string secretKey = "d609511a6172417cdd972310f489d0229db30b7bb859bc069d6f690f16ba74a4";

    public const string HEALTH_BAR_COLOR_GREEN = "58DD8A";
    public const string HEALTH_BAR_COLOR_ORANGE = "FDE835";
    public const string HEALTH_BAR_COLOR_RED = "F85838";
    public const string MAIN_MENU_REGULAR_COLOR = "FFFFFF";
    public const string MAIN_MENU_SELECTED_COLOR = "9ED9FB";

    public static Color GetColorFromHexString(string hexString)
    {
        byte red = System.Convert.ToByte(hexString.Substring(0, 2), 16);
        byte green = System.Convert.ToByte(hexString.Substring(2, 2), 16);
        byte blue = System.Convert.ToByte(hexString.Substring(4, 2), 16);
        return new Color32(red, green, blue, 255);
    }
}

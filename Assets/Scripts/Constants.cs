using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    public const string apiKey = "86ae4bb80c68fe4b3b172a1ae8fc4018d552ae44280659e94d37de905accfe0e";
    public const string secretKey = "2fc99fd915cdddf3bba33317297223f3c9327c20d9be93786ff12a70770187bd";

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

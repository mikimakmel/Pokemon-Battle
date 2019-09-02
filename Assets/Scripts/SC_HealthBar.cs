using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_HealthBar : MonoBehaviour
{
    void Awake()
    {
        GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        GetComponent<Image>().color = GetColorFromHexString(Constants.HEALTH_BAR_COLOR_GREEN);
    }

    public void SetHealthBarScale(float HP)
    {
        GetComponent<RectTransform>().localScale = new Vector3(HP, 1f, 1f);
        SetHealthBarColor(HP);
    }

    public void SetHealthBarColor(float HP)
    {
        if(HP < 1f && HP > 0.5f)
            GetComponent<Image>().color = GetColorFromHexString(Constants.HEALTH_BAR_COLOR_GREEN);
        else if (HP < 0.5f && HP > 0.25f)
            GetComponent<Image>().color = GetColorFromHexString(Constants.HEALTH_BAR_COLOR_ORANGE);
        else if (HP < 0.25f && HP > 0f)
            GetComponent<Image>().color = GetColorFromHexString(Constants.HEALTH_BAR_COLOR_RED);
    }

    private Color GetColorFromHexString(string hexString)
    {
        byte red = System.Convert.ToByte(hexString.Substring(0, 2), 16);
        byte green = System.Convert.ToByte(hexString.Substring(2, 2), 16);
        byte blue = System.Convert.ToByte(hexString.Substring(4, 2), 16);
        return new Color32(red, green, blue, 255);
    }
}

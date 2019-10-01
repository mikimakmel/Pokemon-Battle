using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SC_HealthBar : MonoBehaviour
{
    void Awake()
    {
        GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        GetComponent<Image>().color = Constants.GetColorFromHexString(Constants.HEALTH_BAR_COLOR_GREEN);
    }

    public IEnumerator SetHealthBarScale(float _oldHP, float _newHP)
    {
        if (_oldHP == _newHP)
        {
            GetComponent<RectTransform>().localScale = new Vector3(_newHP, 1f, 1f);
            SetHealthBarColor(_newHP);
        }
        else
        {
            while (_oldHP > _newHP)
            {
                _oldHP -= 0.01f;

                if (_oldHP <= 0)
                {
                    GetComponent<RectTransform>().localScale = new Vector3(0f, 1f, 1f);
                    break;
                }
                else
                {
                    GetComponent<RectTransform>().localScale = new Vector3(_oldHP, 1f, 1f);
                    SetHealthBarColor(_oldHP);
                    yield return new WaitForSeconds(0.05f);
                }

            }
        }
    }

    public void SetHealthBarColor(float HP)
    {
        if(HP <= 1f && HP > 0.5f)
            GetComponent<Image>().color = Constants.GetColorFromHexString(Constants.HEALTH_BAR_COLOR_GREEN);
        else if (HP <= 0.5f && HP > 0.25f)
            GetComponent<Image>().color = Constants.GetColorFromHexString(Constants.HEALTH_BAR_COLOR_ORANGE);
        else if (HP <= 0.25f && HP > 0f)
            GetComponent<Image>().color = Constants.GetColorFromHexString(Constants.HEALTH_BAR_COLOR_RED);
    }
}

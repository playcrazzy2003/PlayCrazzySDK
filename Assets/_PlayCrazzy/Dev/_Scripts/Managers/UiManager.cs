using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class UiManager : MonoBehaviour
{
    public static bool bIsUiOn;

    [Header("HUD")] 
    public Canvas canvas;
    public RectTransform hudPanel;

    [Tooltip("This will use for ui opne and close timing")]
    public float popUpDuraction;
    
    public void OpenPanel(RectTransform backgroundPanel = null, Image bgImg = null, RectTransform mainPanel = null)
    {
        if (bIsUiOn) return;
        hudPanel.gameObject.SetActive(false);
        if (backgroundPanel != null)
        {
            backgroundPanel.gameObject.SetActive(true);
        }

        if (bgImg != null)
        {
            bgImg.DOFade(0.9f, popUpDuraction * 2);
        }

        if (mainPanel != null)
        {
            mainPanel.localScale = Vector3.zero;
            mainPanel.gameObject.SetActive(true);
            mainPanel.DOScale(Vector3.one, popUpDuraction).SetEase(Ease.OutBounce);
        }

        bIsUiOn = true;
    }

    public void ClosePanel(RectTransform backgroundPanel = null, Image bgImg = null, RectTransform mainPanel = null)
    {
        if (bgImg != null)
        {
            bgImg.DOFade(0f, popUpDuraction).OnComplete(() =>
            {
                if (backgroundPanel != null)
                {
                    backgroundPanel.gameObject.SetActive(false);
                }
            });
        }

        if (mainPanel != null)
        {
            mainPanel.DOScale(Vector3.zero, popUpDuraction).SetEase(Ease.OutSine).OnComplete(() =>
            {
                mainPanel.gameObject.SetActive(false);
                hudPanel.gameObject.SetActive(true);
                bIsUiOn = false;
            });
        }
    }

    public string ScoreShow(double Score)
    {
        string result;
        string[] ScoreNames = new string[]
        {
            "", "k", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an",
            "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf",
            "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx",
            "by", "bz",
        };
        int i;

        for (i = 0; i < ScoreNames.Length; i++)
            if (Score < 999)
                break;
            else Score = /* Math.Floor*/(Score / 100d) / 10d;

        if (Score == Math.Floor(Score))
            result = Score.ToString() + " " + ScoreNames[i];
        else result = Score.ToString("F1") + " " + ScoreNames[i];
        return result;
    }
}
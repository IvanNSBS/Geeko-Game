using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameAnalyticsSDK;
using TMPro;
public class StartButton : MonoBehaviour
{
    TextMeshProUGUI start;
    // Start is called before the first frame update
    void Start()
    {
        GameAnalytics.Initialize();
        start = this.GetComponent<TextMeshProUGUI>();
        AnimateStartButton();
    }

    void AnimateStartButton()
    {
        Sequence m_Sequence = DOTween.Sequence();
        m_Sequence.SetLoops(-1);
        m_Sequence.Append(start.DOFade(0f, 1f));
        m_Sequence.Append(start.DOFade(1f, 1f));
    }

}

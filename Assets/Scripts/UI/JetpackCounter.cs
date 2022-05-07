using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.DebugTool;
using UnityEngine.UI;

public class JetpackCounter : MonoBehaviour
{
    [Tooltip("Image component representing jetpack fuel")]
    public Image JetpackFillImage;

    [Tooltip("Canvas group that contains the whole UI for the jetack")]
    public CanvasGroup MainCanvasGroup;

    [Tooltip("Component to animate the color when empty or full")]
    public FillBarColorChange FillBarColorChange;

    Jetpack m_Jetpack;

    void Awake()
    {
        m_Jetpack = FindObjectOfType<Jetpack>();
        DebugUtility.HandleErrorIfNullFindObject<Jetpack, JetpackCounter>(m_Jetpack, this);

        FillBarColorChange.Initialize(1f, 0f);
    }

    void Update()
    {
        MainCanvasGroup.gameObject.SetActive(m_Jetpack.IsJetpackUnlocked);

        if (m_Jetpack.IsJetpackUnlocked)
        {
            JetpackFillImage.fillAmount = m_Jetpack.CurrentFillRatio;
            FillBarColorChange.UpdateVisual(m_Jetpack.CurrentFillRatio);
        }
    }
}


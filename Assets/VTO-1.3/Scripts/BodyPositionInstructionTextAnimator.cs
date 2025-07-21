using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BodyPositionInstructionTextAnimator : MonoBehaviour
{
    private Tween currentTween;
    private TextMeshProUGUI bodyPositionInstructionText;
    private CanvasGroup canvasGroup;
    private void Start()
    {
        bodyPositionInstructionText = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }
    public void ShowBodyPositioningInstruction(String message)
    {
        if(currentTween != null) currentTween.Kill(); 
        currentTween = canvasGroup.DOFade(1f, 0.5f);
        bodyPositionInstructionText.text = message;
    }
    public void HideBodyPositioningInstruction()
    {
        if(currentTween != null) currentTween.Kill(); 
        currentTween = canvasGroup.DOFade(0f, 0.5f).
            OnComplete(() => bodyPositionInstructionText.text = "");
    }
}

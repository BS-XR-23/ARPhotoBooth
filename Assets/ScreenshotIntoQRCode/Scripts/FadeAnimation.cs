using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class FadeAnimation : MonoBehaviour
{
  private CanvasGroup canvasGroup;
  [SerializeField]
  private float duration=1;
  [SerializeField]
  private FadeType fadeType=FadeType.FADE_IN;
  private void Awake()
  {
    canvasGroup = GetComponent<CanvasGroup>();
  }
  private void OnEnable()
  {
    if (fadeType == FadeType.FADE_IN) FadeIn();
    else FadeOut();
  }
  private void OnDisable()
  {
    //FadeOut();
  }
  private void FadeIn()
  {
    Debug.Log("Fade In");
    canvasGroup.alpha = 0;
    canvasGroup.DOFade(1, duration);
  }
  private void FadeOut()
  {
    canvasGroup.alpha = 1;
    canvasGroup.DOFade(0, duration);
  }
  private enum FadeType
  {
    FADE_IN,
    FADE_OUT
  }
}

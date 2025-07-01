using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance;
    public bool IsHoverActive;
    
    [HideInInspector]
    public HoverDetector latestHoverDetector;
    public CountDownTimer globalCountDownTimer;
    [SerializeField]
    public TextMeshProUGUI log;
    private void Awake()
    {
        Instance = this;
    }
    
    public void StartTimer()
    {
        globalCountDownTimer.StartTimer();
    }
    public void StopTimer()
    {
        globalCountDownTimer.Resume();
    }
}

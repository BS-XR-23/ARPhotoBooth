using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    public static CountDownTimer Instance;
    [SerializeField]
    private TextMeshProUGUI _timerText;
    [SerializeField]
    private Image outline;
    public float totalTime = 30.0f;
    private float timeCount;
    public UnityEvent OnFinished;
    public bool timerIsRunning;
    private CanvasGroup _canvasGroup;

    [SerializeField] private Transform startingLocalPosition;
    [SerializeField]
    private bool reverse;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(timerIsRunning)
        {
            UpdateTime();
        }
    }

    private void Start()
    { 
      _canvasGroup = GetComponent<CanvasGroup>();
      if (_canvasGroup) _canvasGroup.alpha = 0;
    }
   
    private void UpdateTime()
    {
        if (timeCount <= totalTime)
        {
            timeCount += Time.deltaTime;
            outline.fillAmount = reverse?1- (timeCount / totalTime) : timeCount / totalTime;
            int time= reverse ? (int)totalTime - (int)timeCount : (int)timeCount;
          _timerText.text = time.ToString("D2");
        }
        else
        {
            OnFinished?.Invoke();
            timerIsRunning = false;
            if (_canvasGroup) _canvasGroup.alpha = 0;
        }
    }
    [ContextMenu("Start Timer")]
    public void StartTimer(bool isVisible = true)
    {
        gameObject.SetActive(isVisible);
        if (_canvasGroup) _canvasGroup.alpha = 1; 
        Debug.Log("Start timer");
        outline.fillAmount = 0;
        timerIsRunning = true;
        timeCount = 0;
    }
    [ContextMenu("Pause Timer")]
    public void Pause()
    {
        timerIsRunning = false;
    }
    [ContextMenu("Resume Timer")]
    public void Resume()
    {
        gameObject.SetActive(true);
        timerIsRunning = true;
    }
    
    public void StopTimer()
    {
        Debug.Log($"Reset Timer:{name}");
        transform.localPosition = startingLocalPosition.position;
        _timerText.text = $"{(int)totalTime}";
        outline.fillAmount = 0;
        timerIsRunning = false;
        timeCount = 0;
        gameObject.SetActive(false);
        if (_canvasGroup) _canvasGroup.alpha = 0;
    }

    public void SetTimerText()
    {
        _timerText.text = $"{(int)totalTime}";
    }
}

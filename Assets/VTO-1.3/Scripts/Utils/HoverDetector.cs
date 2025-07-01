using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoverDetector : MonoBehaviour
{
  [SerializeField]
  private RectTransform hoverTaget;
  private bool[] hover;
  public UnityEvent OnHoverEnter;
  public UnityEvent OnHoverExit;
  public UnityEvent OnTimerFinish;
  public List<RectTransform> cursors = new List<RectTransform>();
  public bool dressItem;
  public bool showTimer;
  public RectTransform activeInSide;
  void Start()
  {
    if(hoverTaget == null) hoverTaget = GetComponent<RectTransform>();
    hover = new bool[cursors.Count];
  }

 
  // Update is called once per frame
  void Update()
  {
    if(ApplicationManager.Instance.IsHoverActive&&hoverTaget.gameObject.activeSelf)
    {
      for(int i=0;i< cursors.Count;i++)
      {
        if ((activeInSide != null ? RectTransformUtility.RectangleContainsScreenPoint(activeInSide, cursors[i].position) : true) )
        {
          if (!hover[i]&&(dressItem ? RectTransformUtility.RectangleContainsScreenPoint(hoverTaget, cursors[i].position) : RectTransformUtility.RectangleContainsScreenPoint(cursors[i], hoverTaget.position)))
          {
            
            if (!ApplicationManager.Instance.globalCountDownTimer.timerIsRunning && hover[i])
            {
              ResetListener(i);
            }
            if (!hover[i])
            {
              hover[i] = true;
              ResetListener(i);
              break;
            }
            //ApplicationManager.Instance.log.text = $"Hover Enter:{ApplicationManager.Instance.globalCountDownTimer.gameObject.activeSelf}";
          }
          else if ((dressItem ? !RectTransformUtility.RectangleContainsScreenPoint(hoverTaget, cursors[i].position) : !RectTransformUtility.RectangleContainsScreenPoint(cursors[i], hoverTaget.position)) && hover[i])
          {
            hover[i] = false;
            if (ApplicationManager.Instance.latestHoverDetector == this)
            {
                OnHoverExit?.Invoke();
                ApplicationManager.Instance.globalCountDownTimer.StopTimer();

            }
          }
        }
      }
    }
  }
    public void Reset()
    {
        
    }

    public void ResetListener(int HoverIndex)
    {
        OnHoverEnter?.Invoke();
        ApplicationManager.Instance.globalCountDownTimer.StartTimer();
        ApplicationManager.Instance.latestHoverDetector = this;
        if (showTimer)
        {
            ApplicationManager.Instance.globalCountDownTimer.OnFinished.RemoveAllListeners();
            ApplicationManager.Instance.globalCountDownTimer.OnFinished.AddListener(() =>
            {
                if (!dressItem) hover[HoverIndex] = false;
                ApplicationManager.Instance.globalCountDownTimer.StopTimer();
                ApplicationManager.Instance.globalCountDownTimer.gameObject.SetActive(false);
                OnTimerFinish.Invoke();
            });
            ApplicationManager.Instance.globalCountDownTimer.transform.position = hoverTaget.transform.position;
        }
        else
        {
            ApplicationManager.Instance.globalCountDownTimer.Resume();
        }
    }

  
}

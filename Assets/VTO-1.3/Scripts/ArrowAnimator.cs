using System;
using UnityEngine;
using DG.Tweening;

public enum ArrowMoveDirection
{
    Left,
    Right,
    Up,
    Down,
}

public class ArrowAnimator : MonoBehaviour
{
    public ArrowMoveDirection direction;
    private RectTransform rectTransform;
    public RectTransform startPos;
    public CanvasGroup canvasGroup;
    public float moveDistance = 200f;
    public float duration = 1.5f;

    private Sequence arrowSequence;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void HideArrow()
    {
        if (arrowSequence != null) arrowSequence.Kill();
        canvasGroup.DOFade(0f, 0.5f);
    }
    public void SetDirection(string direction)
    {
        if(!Enum.TryParse(direction, out this.direction))
        {
            Debug.Log("Invalid Direction");
        }
    }
    public void SetStartingPositionAndPlayAnimation(RectTransform startingPosition)
    {
        startPos = startingPosition;
        PlayArrowAnimation();
    }
    [ContextMenu("Play Arrow Animation")]
    public void PlayArrowAnimation()
    {
        if (arrowSequence != null) arrowSequence.Kill();

        Vector3 moveVector = direction switch
        {
            ArrowMoveDirection.Right => Vector3.right,
            ArrowMoveDirection.Left => Vector3.left,
            ArrowMoveDirection.Up => Vector3.up,
            ArrowMoveDirection.Down => Vector3.down,
            _ => Vector3.right
        };

        Vector3 startLocalPos = startPos.localPosition;
        Vector3 targetLocalPos = startLocalPos + moveVector * moveDistance;

        rectTransform.localRotation = direction switch
        {
            ArrowMoveDirection.Right => Quaternion.Euler(0, 0, 0),
            ArrowMoveDirection.Left => Quaternion.Euler(0, 0, 180),
            ArrowMoveDirection.Up => Quaternion.Euler(0, 0, 90),
            ArrowMoveDirection.Down => Quaternion.Euler(0, 0, -90),
            _ => Quaternion.identity
        };

        canvasGroup.alpha = 0;
        rectTransform.localPosition = startLocalPos;

        arrowSequence = DOTween.Sequence();
        arrowSequence.Append(canvasGroup.DOFade(1f, duration * 0.5f).SetEase(Ease.InOutQuad));
        arrowSequence.Join(rectTransform.DOLocalMove(targetLocalPos, duration).SetEase(Ease.Linear));
        arrowSequence.Insert(duration * 0.7f, canvasGroup.DOFade(0f, duration * 0.3f).SetEase(Ease.InOutQuad));
        arrowSequence.SetLoops(-1, LoopType.Restart);
    }
}

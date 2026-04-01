using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Transitioner : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private RectTransform _transitioner;

    [Header("PARAMETERS")]
    [SerializeField] private float _baseTransitionTime = 1f;
    [SerializeField] private AnimationCurve _transitionCurve = new AnimationCurve();

    private Coroutine _transitionCo = null;
    private Action _onTransitionEnd = null;

    public void Mask(float transitionDuration = -1f, Action onTransitionEnd = null, Action onTransitionStart = null)
    {
        if (transitionDuration <= 0)
        {
            transitionDuration = _baseTransitionTime;
        }

        _transitionCo = StartCoroutine(TransitionCo(false, transitionDuration, onTransitionEnd, onTransitionStart));
    }

    public void Reveal(float transitionDuration = -1f, Action onTransitionEnd = null, Action onTransitionStart = null)
    {
        if (transitionDuration <= 0)
        {
            transitionDuration = _baseTransitionTime;
        }

        _transitionCo = StartCoroutine(TransitionCo(true, transitionDuration, onTransitionEnd, onTransitionStart));
    }

    private IEnumerator TransitionCo(bool reveal, float transitionTime = -1f, Action onTransitionEndAction = null, Action onTransitionStartAction = null)
    {
        float startingPoint = reveal ? 0f : 4100f;
        float endPoint = reveal ? -2150f : 1950f;
        float xScale = reveal ? 1f : -1f;

        Vector2 startingPos = new Vector2(startingPoint, 0);
        Vector2 endPos = new Vector2(endPoint, 0);

        float duration = transitionTime;
        float time = 0f;
        _onTransitionEnd = onTransitionEndAction;

        if (onTransitionStartAction != null)
        {
            onTransitionStartAction();
        }

        _transitioner.anchoredPosition = startingPos;
        _transitioner.localScale = new Vector3(xScale, 1f, 1f);

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = _transitionCurve.Evaluate(time / duration);
            Vector2 lerpPos = Vector2.Lerp(startingPos, endPos, alpha);
            _transitioner.anchoredPosition = lerpPos;
            yield return null;
        }

        _transitioner.anchoredPosition = endPos;

        if (_onTransitionEnd != null)
        {
            _onTransitionEnd();
            _onTransitionEnd = null;
        }

        _transitionCo = null;
    }
}

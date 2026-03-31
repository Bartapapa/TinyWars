using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAnimationHandler : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] protected Animator _anim;
    public Animator Animator { get { return _anim; } }

    public void PlayAnimation(string stateName, int layer = -1)
    {
        _anim.Play(stateName, 0);
    }
    public void PlayAnimationWithBlend(string stateName, float transitionDuration = .1f)
    {
        _anim.CrossFade(stateName, transitionDuration);
    }

    public void PlayAdditiveAnimation(string stateName, int layer)
    {
        _anim.Play(stateName, layer);
    }
}

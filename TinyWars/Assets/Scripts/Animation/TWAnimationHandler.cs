using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWAnimationHandler : AAnimationHandler
{
    protected virtual void Update()
    {
        SetAnimatorPlayRate();
        SetAnimatorValues();
    }

    private void SetAnimatorPlayRate()
    {
        GameManager instance = GameManager.Instance;
        if (!instance)
        {
            Animator.SetFloat("PlayRate", 1f);
            return;
        }
        Animator.SetFloat("PlayRate", instance.ActionRate);
    }

    protected virtual void SetAnimatorValues()
    {

    }
}

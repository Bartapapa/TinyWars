using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TW_AH_C_Combat : TW_AH_Character
{
    [Header("COMBAT OBJECT REFERENCES")]
    [SerializeField] protected CombatHandler _handler;
    public CombatHandler Handler { get { return _handler; } }

    protected override void Update()
    {
        base.Update();
    }

    protected override void SetAnimatorValues()
    {
        base.SetAnimatorValues();

        Animator.SetBool("isMoving", _handler.IsMoving);
        Animator.SetBool("isDead", _handler.TagHandler.HasTag(CombatState.Dead));
    }
}

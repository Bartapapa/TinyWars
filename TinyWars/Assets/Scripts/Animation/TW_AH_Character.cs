using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TW_AH_Character : TWAnimationHandler
{
    [Header("CHARACTER OBJECT REFERENCES")]
    [SerializeField] protected Character _char;
    public Character Character { get { return _char; } }

    protected override void Update()
    {
        base.Update();
    }

    protected override void SetAnimatorValues()
    {
        base.SetAnimatorValues();
    }
}

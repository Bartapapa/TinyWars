using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Action_Attack : TWAction
{
    public override void EnactAction()
    {
        base.EnactAction();

        foreach(CombatHandler target in _tCombatHandlers.Values)
        {
            float damageDealt = _iCombatHandler.DamageTarget(target);
            if (EventDispatcher.Instance)
            {
                EventDispatcher.Instance.Message_HandlerAttack(_iCombatHandler, target, damageDealt);
            }
        }
    }
}

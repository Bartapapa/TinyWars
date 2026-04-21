using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "TinyWars/Action/Attack", fileName = "Attack_ActionData")]
public class Action_Attack : TWAction
{
    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets, int actionLevel = 1)
    {
        Action_Attack newAction = CreateInstance<Action_Attack>();
        newAction._actionType = _actionType;
        newAction._simultaneousAction = _simultaneousAction;
        newAction._initiator = initiator;
        newAction._targets = targets;

        newAction._iCombatHandler = newAction._initiator.GetComponent<CombatHandler>();
        foreach (GameObject target in newAction._targets)
        {
            CombatHandler tCombatHandler = target.GetComponent<CombatHandler>();
            newAction._tCombatHandlers.Add(target, tCombatHandler);
        }
        newAction._generated = true;

        return newAction;
    }

    public override void EnactAction()
    {
        base.EnactAction();

        _iCombatHandler.AnimationHandler.PlayAnimationWithBlend("Attack");

        _iCombatHandler.AttackTargets(_tCombatHandlers.Values.ToList<CombatHandler>(), _iCombatHandler.Attack.Value, .66f);

        if (EventDispatcher.Instance)
        {
            AttackContext context = new AttackContext(_iCombatHandler);
            EventDispatcher.Instance.Message_FighterAttacked(ref context);
        }
    }
}

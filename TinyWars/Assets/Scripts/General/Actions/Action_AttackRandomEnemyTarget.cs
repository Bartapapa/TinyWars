using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/AttackRandomEnemyTarget", fileName = "AttackRandomEnemyTarget_ActionData")]

public class Action_AttackRandomEnemyTarget : TWAction
{
    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets)
    {
        Action_AttackRandomEnemyTarget newAction = CreateInstance<Action_AttackRandomEnemyTarget>();
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

        Debug.Log(444);
        _iCombatHandler.AttackTargets(_tCombatHandlers.Values.ToList<CombatHandler>(), .66f);

        if (EventDispatcher.Instance)
        {
            Debug.Log(555);
            AttackContext context = new AttackContext(_iCombatHandler);
            EventDispatcher.Instance.Message_FighterAttacked(ref context);
        }
    }
}

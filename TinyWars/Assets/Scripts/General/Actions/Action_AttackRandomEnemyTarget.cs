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

        _iCombatHandler.AnimationHandler.PlayAnimationWithBlend("Cast1");

        List<CombatHandler> target = new List<CombatHandler>();
        CombatRow enemyRow = _tCombatHandlers[_targets[0]].CurrentRow;
        List<CombatHandler> aliveTargets = enemyRow.GetCurrentFighters(true);
        int randomInt = Random.Range(0, aliveTargets.Count);
        CombatHandler randomTarget = aliveTargets[randomInt];
        target.Add(randomTarget);

        _iCombatHandler.AttackTargets(target, _iCombatHandler.Attack.Value, .66f, true);

        if (EventDispatcher.Instance)
        {
            AttackContext context = new AttackContext(_iCombatHandler);
            EventDispatcher.Instance.Message_FighterAttacked(ref context);
        }
    }
}

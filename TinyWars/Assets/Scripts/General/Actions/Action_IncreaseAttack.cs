using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/IncreaseAttack", fileName = "IncreaseAttack_ActionData")]

public class Action_IncreaseAttack : TWAction
{
    [Header("PARAMETERS")]
    [SerializeField] private float _increaseToAttack = 1f;

    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets, int actionLevel = 1)
    {
        Action_IncreaseAttack newAction = CreateInstance<Action_IncreaseAttack>();
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

        newAction._increaseToAttack = _increaseToAttack;

        newAction._generated = true;

        return newAction;
    }

    public override void EnactAction()
    {
        base.EnactAction();

        _iCombatHandler.AnimationHandler.PlayAnimationWithBlend("Cast1");

        TWStatisticModifier newMod = new TWStatisticModifier(_increaseToAttack, StatisticModifierType.Flat, ModifierApplicationType.Standard, TWStatisticModifierDuration.Combat, _iCombatHandler.Character);
        _iCombatHandler.Attack.AddModifier(newMod);

        Debug.Log(_iCombatHandler.gameObject.name + " has increased their attack by " + _increaseToAttack + "!");
    }
}

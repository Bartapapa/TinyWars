using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/Wait", fileName = "Wait_ActionData")]

public class Action_Wait : TWAction
{
    [Header("WAIT PARAMETERS")]
    [SerializeField] private float _baseWaitDuration = 1f;

    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets, int actionLevel = 1)
    {
        Action_Wait newAction = CreateInstance<Action_Wait>();
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

        Debug.Log("Waited for a base duration of " + _baseWaitDuration + " seconds, or an adjusted " + GameManager.Instance.ActionTime * _baseWaitDuration + " seconds.");
    }
}

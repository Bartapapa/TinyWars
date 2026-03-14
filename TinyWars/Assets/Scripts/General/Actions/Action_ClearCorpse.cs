using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/ClearCorpse", fileName = "ClearCorpse_ActionData")]
public class Action_ClearCorpse : TWAction
{
    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets)
    {
        Action_ClearCorpse newAction = CreateInstance<Action_ClearCorpse>();
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

        //Play corpse animation.
        Vector3 randomDirection = (_iCombatHandler.transform.up * (Random.Range(.7f, 1f)) + (-_iCombatHandler.transform.forward * (Random.Range(.2f, 1f))));
        randomDirection = randomDirection.normalized;
        float forceIntensity = Random.Range(.5f, 1f) * 10f;
        Vector3 randomTorque = new Vector3(0, 0, Random.Range(-1f, 1f) * 5f);
        _iCombatHandler.Character.Mesh.Yeet(randomDirection * forceIntensity, randomTorque);

        //Clear currently occupied slot.
        int currentSlot = _iCombatHandler.CurrentRow.GetFighterSlotIndex(_iCombatHandler);
        _iCombatHandler.CurrentRow.Slots[currentSlot] = null;

        if (EventDispatcher.Instance)
        {
            FighterContext context = new FighterContext(_iCombatHandler);
            EventDispatcher.Instance.Message_FighterCorpseCleared(ref context);
        }

        //TODO: Remove this later.
        //Destroy(Initiator);
    }
}

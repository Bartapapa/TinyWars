using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Character/Abilities/LittleCrow_RandomDamage", fileName = "LittleCrow_RandomDamage_AbilityData")]
public class Ability_LittleCrow_RandomDamage : TWAbility
{
    public override TWAbility GenerateAbility(AbilityHandler handler)
    {
        Ability_LittleCrow_RandomDamage newAbility = CreateInstance<Ability_LittleCrow_RandomDamage>();

        newAbility._internalName = this._internalName;
        newAbility._listenedMessages = this._listenedMessages;
        newAbility._abilityActions = this._abilityActions;

        newAbility._abilityHandler = handler;
        newAbility._generated = true;

        return newAbility;
    }

    public override void OnMessage_FighterDamagedDefender(AttackContext context)
    {
        base.OnMessage_FighterDamagedDefender(context);

        //Check if defender is the user of this ability's abilityHandler.
        if (context.Defender.gameObject != _abilityHandler.gameObject) return;

        //If true, generate an action of AttackRandomEnemyTarget, which uses defender.AttackTargets to attack a random living enemy from the attacker's current row.
        GameObject randomTarget = null;
        List<CombatHandler> targets = context.Attacker.CurrentRow.GetCurrentFighters(true);
        if (targets.Count <= 0) return;

        int randomInt = Random.Range(0, targets.Count);
        randomTarget = targets[randomInt].gameObject;
        List<GameObject> target = new List<GameObject>();
        target.Add(randomTarget);

        foreach(TWAction action in _abilityActions)
        {
            if (EventDispatcher.Instance)
            {
                TWAction newAction = action.GenerateAction(context.Defender.gameObject, target);
                ActionContext newContext = new ActionContext(newAction);
                EventDispatcher.Instance.Message_ActionCalled(ref newContext);
            }
        }
    }
}

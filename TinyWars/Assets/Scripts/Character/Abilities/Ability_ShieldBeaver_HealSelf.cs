using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Character/Abilities/ShieldBeaver_HealSelf", fileName = "ShieldBeaver_HealSelf_AbilityData")]
public class Ability_ShieldBeaver_HealSelf : TWAbility
{
    public override TWAbility GenerateAbility(AbilityHandler handler)
    {
        Ability_ShieldBeaver_HealSelf newAbility = CreateInstance<Ability_ShieldBeaver_HealSelf>();

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

        //Check if defender is the user of this ability's abilityHandler and isn't dead.
        if (context.Defender.gameObject != _abilityHandler.gameObject) return;
        if (context.Defender.TagHandler.HasTag(CombatState.Dead)) return;

        foreach (TWAction action in _abilityActions)
        {
            if (EventDispatcher.Instance)
            {
                List<GameObject> target = new List<GameObject>();
                target.Add(context.Defender.gameObject);
                TWAction newAction = action.GenerateAction(context.Defender.gameObject, target);
                ActionContext newContext = new ActionContext(newAction);
                EventDispatcher.Instance.Message_ActionCalled(ref newContext);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Character/Abilities/Ability_SpawnFighterOnDeath", fileName = "User_SpawnFighterOnDeath_AbilityData")]
public class Ability_SpawnFighterOnDeath : TWAbility
{
    public override TWAbility GenerateAbility(AbilityHandler handler)
    {
        Ability_SpawnFighterOnDeath newAbility = CreateInstance<Ability_SpawnFighterOnDeath>();

        newAbility._internalName = this._internalName;
        newAbility._abilityName = this._abilityName;
        newAbility._abilityIcon = this._abilityIcon;
        newAbility._listenedMessages = this._listenedMessages;
        newAbility._abilityActions = this._abilityActions;
        newAbility._abilityLevel = _abilityLevel;
        newAbility._canLevelUp = this._canLevelUp;

        newAbility._abilityHandler = handler;
        newAbility._generated = true;

        return newAbility;
    }

    public override void OnMessage_FighterHealthReachedZero(FighterContext context)
    {
        base.OnMessage_FighterHealthReachedZero(context);

        if (context.Fighter.gameObject != _abilityHandler.gameObject) return;

        foreach (TWAction action in _abilityActions)
        {
            if (EventDispatcher.Instance)
            {
                List<GameObject> target = new List<GameObject>();
                target.Add(context.Fighter.gameObject);
                TWAction newAction = action.GenerateAction(context.Fighter.gameObject, target);
                ActionContext newContext = new ActionContext(newAction);
                EventDispatcher.Instance.Message_ActionCalled(ref newContext);

                AbilityContext abilityContext = new AbilityContext(_abilityHandler.Character, this);
                EventDispatcher.Instance.Message_OnCharacterUsedAbility(ref abilityContext);
            }
        }
    }
}

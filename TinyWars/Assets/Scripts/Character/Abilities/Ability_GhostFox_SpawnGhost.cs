using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Character/Abilities/GhostFox_SpawnGhost", fileName = "GhostFox_SpawnGhost_AbilityData")]
public class Ability_GhostFox_SpawnGhost : TWAbility
{
    public override TWAbility GenerateAbility(AbilityHandler handler)
    {
        Ability_GhostFox_SpawnGhost newAbility = CreateInstance<Ability_GhostFox_SpawnGhost>();

        newAbility._internalName = this._internalName;
        newAbility._listenedMessages = this._listenedMessages;
        newAbility._abilityActions = this._abilityActions;

        newAbility._abilityHandler = handler;
        newAbility._generated = true;

        return newAbility;
    }

    public override void OnMessage_FighterCorpseCleared(FighterContext context)
    {
        base.OnMessage_FighterCorpseCleared(context);

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
            }
        }
    }
}

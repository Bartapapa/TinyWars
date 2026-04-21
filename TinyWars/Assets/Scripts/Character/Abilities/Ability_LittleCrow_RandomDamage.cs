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

    public override void OnMessage_FighterDamagedDefender(AttackContext context)
    {
        base.OnMessage_FighterDamagedDefender(context);

        //Check if defender is the user of this ability's abilityHandler and isn't dead.
        if (context.Defender.gameObject != _abilityHandler.gameObject) return;
        if (context.Defender.TagHandler.HasTag(CombatState.Dead)) return;

        //If true, generate an action of AttackRandomEnemyTarget, which uses defender.AttackTargets to attack a random living enemy from the attacker's current row.
        //GameObject randomTarget = null;
        List<CombatHandler> targets = context.Attacker.CurrentRow.GetCurrentFighters();
        if (targets.Count <= 0) return;
        List<GameObject> target = new List<GameObject>();

        foreach (CombatHandler livingEnemy in targets)
        {
            target.Add(livingEnemy.gameObject);
        }

        //int randomInt = Random.Range(0, targets.Count);
        //randomTarget = targets[randomInt].gameObject;
        //target.Add(randomTarget);

        foreach(TWAction action in _abilityActions)
        {
            if (EventDispatcher.Instance)
            {
                TWAction newAction = action.GenerateAction(context.Defender.gameObject, target);
                ActionContext newContext = new ActionContext(newAction);
                EventDispatcher.Instance.Message_ActionCalled(ref newContext);

                AbilityContext abilityContext = new AbilityContext(_abilityHandler.Character, this);
                EventDispatcher.Instance.Message_OnCharacterUsedAbility(ref abilityContext);
            }
        }
    }
}

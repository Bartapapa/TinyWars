using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "TinyWars/Character/Abilities/LittleCrow_IncreaseAttack", fileName = "LittleCrow_IncreaseAttack_AbilityData")]
public class Ability_IncreaseAttack : TWAbility
{
    public override TWAbility GenerateAbility(AbilityHandler handler)
    {
        Ability_IncreaseAttack newAbility = CreateInstance<Ability_IncreaseAttack>();

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

    public override void OnMessage_CombatStarted(CombatContext context)
    {
        base.OnMessage_CombatStarted(context);

        List<GameObject> targets = new List<GameObject>();
        GameObject initiator = _abilityHandler.gameObject;
        targets.Add(initiator);

        foreach (TWAction action in _abilityActions)
        {
            if (EventDispatcher.Instance)
            {
                TWAction newAction = action.GenerateAction(initiator, targets);
                ActionContext newContext = new ActionContext(newAction);
                EventDispatcher.Instance.Message_ActionCalled(ref newContext);

                AbilityContext abilityContext = new AbilityContext(_abilityHandler.Character, this);
                EventDispatcher.Instance.Message_OnCharacterUsedAbility(ref abilityContext);
            }
        }
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
    }
}

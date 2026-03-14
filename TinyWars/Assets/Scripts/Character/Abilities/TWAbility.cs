using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Playables;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "TinyWars/Character/AbilityData", fileName = "AbilityName_AbilityData")]
public class TWAbility : ScriptableObject
{
    [Header("ABILITY ID")]
    [SerializeField] protected string _internalName = "abilityName";
    public string InternalName { get { return _internalName; } }

    [Header("MESSAGE LISTEN TYPES")]
    [SerializeField] protected List<EventMessageType> _listenedMessages = new List<EventMessageType>();
    public List<EventMessageType> ListenedMessages { get { return _listenedMessages; } }

    [Header("ASSOCIATED ACTIONS")]
    [SerializeField] protected List<TWAction> _abilityActions = new List<TWAction>();
    public List<TWAction> AbilityActions { get { return _abilityActions; } }

    protected AbilityHandler _abilityHandler;
    public AbilityHandler AbilityHandler { get { return _abilityHandler; } }
    protected bool _generated = false;
    public bool Generated { get { return _generated; } }

    public virtual TWAbility GenerateAbility(AbilityHandler handler)
    {
        TWAbility newAbility = CreateInstance<TWAbility>();

        newAbility._internalName = this._internalName;
        newAbility._listenedMessages = this._listenedMessages;
        newAbility._abilityActions = this._abilityActions;

        newAbility._abilityHandler = handler;
        newAbility._generated = true;

        return newAbility;
    }

    public virtual void OnMessage_ActionCalled(ActionContext context)
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }
        //override with checks & conditions from context if necessary. Return in override if checks don't work out.
        //generate actions with info from context and send them via EventDispatcher.Message_ActionCalled.
    }

    public virtual void OnMessage_FighterHealthReachedZero(FighterContext context)
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }
    }

    public virtual void OnMessage_FighterAttacked(AttackContext context)
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }
    }

    public virtual void OnMessage_FighterDamagedDefender(AttackContext context)
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }
    }

    public virtual void OnMessage_FighterMoved(MoveContext context)
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }
    }
}

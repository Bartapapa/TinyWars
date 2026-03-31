using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "TinyWars/Character/AbilityData", fileName = "AbilityName_AbilityData")]
public class TWAbility : ScriptableObject
{
    [Header("ABILITY ID")]
    [SerializeField] protected string _internalName = "abilityName";
    public string InternalName { get { return _internalName; } }

    [Header("USER-FACING")]
    [SerializeField] protected string _abilityName = "name";
    public string AbilityName { get { return _abilityName; } }
    [SerializeField] protected Sprite _abilityIcon;
    public Sprite AbilityIcon { get { return _abilityIcon; } }

    [Header("MESSAGE LISTEN TYPES")]
    [SerializeField] protected List<EventMessageType> _listenedMessages = new List<EventMessageType>();
    public List<EventMessageType> ListenedMessages { get { return _listenedMessages; } }

    [Header("ASSOCIATED ACTIONS")]
    [SerializeField] protected List<TWAction> _lvl1_abilityActions = new List<TWAction>();
    public List<TWAction> Lvl1AbilityActions { get { return _lvl1_abilityActions; } }

    [Header("LEVEL UP")]
    [SerializeField] protected bool _canLevelUp = true;
    public bool CanLevelUp { get { return _canLevelUp; } }

    protected int _abilityLevel = 1;
    public int AbilityLevel { get { return _abilityLevel; } }

    protected AbilityHandler _abilityHandler;
    public AbilityHandler AbilityHandler { get { return _abilityHandler; } }
    protected bool _generated = false;
    public bool Generated { get { return _generated; } }

    public virtual TWAbility GenerateAbility(AbilityHandler handler)
    {
        TWAbility newAbility = CreateInstance<TWAbility>();

        newAbility._internalName = this._internalName;
        newAbility._abilityName = this._abilityName;
        newAbility._abilityIcon = this._abilityIcon;
        newAbility._listenedMessages = this._listenedMessages;
        newAbility._lvl1_abilityActions = this._lvl1_abilityActions;
        newAbility._abilityLevel = this._abilityLevel;

        newAbility._abilityHandler = handler;
        newAbility._canLevelUp = this._canLevelUp;

        newAbility._generated = true;

        return newAbility;
    }

    public virtual void LevelUpAbility()
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }

        _abilityLevel++;

        //Override with specific level up logic if need be;
    }

    #region MESSAGES_LISTEN
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

    public virtual void OnMessage_FighterSpawned(MoveContext context)
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }
    }

    public virtual void OnMessage_FighterCorpseCleared(FighterContext context)
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EventMessageType
{
    None,
    ActionCalled,
    FighterHealthReachedZero,
    FighterAttacked,
    FighterDamagedDefender,
    FighterMoved,
    FighterSpawned,
    FighterCorpseCleared,
    FighterLevelUp,
}

public struct ActionContext
{
    public TWAction Action;

    public ActionContext(TWAction action)
    {
        Action = action;
    }
}

public struct FighterContext
{
    public CombatHandler Fighter;

    public FighterContext(CombatHandler fighter)
    {
        Fighter = fighter;
    }
}

public struct AttackContext
{
    public CombatHandler Attacker;
    public CombatHandler Defender;
    public float DamageDealt;

    public AttackContext(CombatHandler attacker, CombatHandler defender, float damageDealt)
    {
        Attacker = attacker;
        Defender = defender;
        DamageDealt = damageDealt;
    }

    public AttackContext(CombatHandler attacker) : this(attacker, null, 0f) { }
    public AttackContext(CombatHandler attacker, CombatHandler defender) : this(attacker, defender, 0f) { }
}

public struct MoveContext
{
    public CombatHandler Mover;
    public CombatRow Row;
    public int ToSlotIndex;

    public MoveContext(CombatHandler mover, CombatRow row, int toSlotIndex)
    {
        Mover = mover;
        Row = row;
        ToSlotIndex = toSlotIndex;
    }

    public MoveContext(CombatHandler mover, int toSlotIndex) : this(mover, null, toSlotIndex) { }
}

public struct AbilityContext
{
    public Character Character;
    public TWAbility Ability;

    public AbilityContext(Character character, TWAbility ability)
    {
        Character = character;
        Ability = ability;
    }
}

public class EventDispatcher : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static EventDispatcher _instance = null;

    public static EventDispatcher Instance { get { return _instance; } }

    #region Events

    //Replace event arguments with multiple struct types relaying bits of information.
    //Such as 'DamageContext' struct, 'DeathContext' struct, etc etc...

    public delegate void ActionEvent(ActionContext context);
    public event ActionEvent ActionCalled;
    public delegate void FighterEvent(FighterContext context);
    public event FighterEvent FighterHealthReachedZero;
    public event FighterEvent FighterCorpseCleared;
    public event FighterEvent FighterLevelUp;
    public delegate void AttackEvent(AttackContext context);
    public event AttackEvent FighterAttacked;
    public event AttackEvent FighterDamagedDefender;
    public delegate void MoveEvent(MoveContext context);
    public event MoveEvent FighterMoved;
    public event MoveEvent FighterSpawned;
    public delegate void AbilityEvent(AbilityContext context);
    public event AbilityEvent CharacterUsedAbility;
    #endregion

    public void Initialize()
    {
        if (!_instance)
        {
            lock (_lockingObject)
            {
                if (!_instance)
                {
                    _instance = this;
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Message_FighterAttacked(ref AttackContext context)
    {
        FighterAttacked?.Invoke(context);
    }

    public void Message_FighterDamagedDefender(ref AttackContext context)
    {
        FighterDamagedDefender?.Invoke(context);
    }

    public void Message_FighterMoved(ref MoveContext context)
    {
        FighterMoved?.Invoke(context);
    }

    public void Message_FighterHealthReachedZero(ref FighterContext context)
    {
        FighterHealthReachedZero?.Invoke(context);
    }

    public void Message_ActionCalled(ref ActionContext context)
    {
        ActionCalled?.Invoke(context);
    }

    public void Message_FighterSpawned(ref MoveContext context)
    {
        FighterSpawned?.Invoke(context);
    }

    public void Message_FighterCorpseCleared(ref FighterContext context)
    {
        FighterCorpseCleared?.Invoke(context);
    }

    public void Message_FighterLevelUp(ref FighterContext context)
    {
        FighterLevelUp?.Invoke(context);
    }

    public void Message_OnCharacterUsedAbility(ref AbilityContext context)
    {
        CharacterUsedAbility?.Invoke(context);
    }
}

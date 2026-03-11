using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventDispatcher : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static EventDispatcher _instance = null;

    public static EventDispatcher Instance { get { return _instance; } }

    #region Events
    public delegate void ActionEvent(TWAction action);
    public event ActionEvent ActionCalled;
    public delegate void CombatHandlerEvent(CombatHandler combatHandler);
    public event CombatHandlerEvent HandlerHealthReachedZero;
    public delegate void AttackEvent(CombatHandler attacker, CombatHandler defender, float damageDealt);
    public event AttackEvent HandlerAttack;
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

    public void Message_HandlerAttack(CombatHandler attacker, CombatHandler defender, float damageDealt)
    {
        HandlerAttack?.Invoke(attacker, defender, damageDealt);
    }

    public void Message_HandlerHealthReachedZero(CombatHandler handler)
    {
        HandlerHealthReachedZero?.Invoke(handler);
    }

    public void Message_ActionCalled(TWAction action)
    {
        ActionCalled?.Invoke(action);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class CombatRow : MonoBehaviour
{
    [Header("MAXIMUM SLOTS")]
    private int _maxSlots = -1;
    public int MaxSlots { get { return _maxSlots; } }

    [Header("ROW POSITIONS")]
    [SerializeField] private Vector3[] _rowPositions;
    [SerializeField] private float _positionSeparation = 1.2f;

    [Header("FIGHTER SLOTS")]
    [ReadOnlyInspector][SerializeField] private CombatHandler[] _slots;
    public CombatHandler[] Slots { get { return _slots; } }

    public void Initialize(int maxSlots, List<CombatHandler> team)
    {
        if (maxSlots <= 0) return;

        _slots = new CombatHandler[maxSlots];
        _rowPositions = CreateRowPositions(maxSlots);

        for (int i = 0; i < team.Count; i++)
        {
            if (i >= maxSlots) break;
            else
            {
                CombatHandler newFighter = Instantiate<CombatHandler>(team[i], _rowPositions[i], transform.rotation, this.transform);
                newFighter.Initialize();
                newFighter.SetCurrentCombatRow(this);
                newFighter.gameObject.name = "Fighter " + i;
                _slots[i] = newFighter;
            }
        }
    }

    public void ClearRow()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] != null)
            {
                Destroy(_slots[i].gameObject);
            }
        }

        _slots = null;
        _rowPositions = null;
    }

    private Vector3[] CreateRowPositions(int slots)
    {
        Vector3[] positions = new Vector3[slots];
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 pos = transform.position - (transform.forward * i * _positionSeparation);
            positions[i] = pos;
        }
        return positions;
    }

    public CombatHandler GetFighterAtIndex(int index)
    {
        return _slots[index];
    }

    public int GetFighterSlotIndex(CombatHandler fighter)
    {
        int foundIndex = -1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == fighter)
            {
                foundIndex = i;
                break;
            }
        }
        return foundIndex;
    }

    public void PlaceFighterAtSlot(CombatHandler fighter, int i)
    {
        if (i >= _slots.Length || i < 0) return;
        _slots[i] = fighter;
    }

    public void PlaceFighter(CombatHandler fighter)
    {
        int fighterSlotIndex = -1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                fighterSlotIndex = i;
                break;
            }
        }

        if (fighterSlotIndex >= 0)
        {
            PlaceFighterAtSlot(fighter, fighterSlotIndex);
        }
    }

    public void RemoveFighterAtSlot(int i)
    {
        if (i >= _slots.Length || i < 0) return;
        _slots[i] = null;
    }

    public void RemoveFighter(CombatHandler fighter)
    {
        int fighterIndex = -1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == fighter)
            {
                fighterIndex = i;
                break;
            }
        }

        if (fighterIndex >= 0)
        {
            RemoveFighterAtSlot(fighterIndex);
        }
    }

    public void MoveFighterToSlotIndex(CombatHandler fighter, int toSlotIndex)
    {
        if (_slots[toSlotIndex] != null)
        {
            SwitchFighterSlots(fighter, GetFighterAtIndex(toSlotIndex));
        }
        else
        {
            int fighterIndex = GetFighterSlotIndex(fighter);
            _slots[fighterIndex] = null;
            _slots[toSlotIndex] = fighter;

            fighter.transform.position = _rowPositions[toSlotIndex];

            if (EventDispatcher.Instance)
            {
                EventDispatcher.Instance.Message_HandlerMoved(fighter, toSlotIndex);
            }

            Debug.Log(fighter.gameObject.name + " moved to slot " + toSlotIndex + "!");
        }
    }

    public void SwitchFighterSlots(CombatHandler fighter1, CombatHandler fighter2)
    {
        int fighter1Slot = GetFighterSlotIndex(fighter1);
        int fighter2Slot = GetFighterSlotIndex(fighter2);

        _slots[fighter1Slot] = null;
        _slots[fighter2Slot] = null;
        MoveFighterToSlotIndex(fighter1, fighter2Slot);
        MoveFighterToSlotIndex(fighter2, fighter1Slot);

        Debug.Log(fighter1.gameObject.name + " switched positions with " + fighter1.gameObject.name + "!");
    }

    public void MoveFighterUp(CombatHandler fighter)
    {
        int fighterIndex = GetFighterSlotIndex(fighter);
        int frontmostEmptySlot = -1;

        for (int i = 0; i < fighterIndex; i++)
        {
            if (_slots[i] == null)
            {
                frontmostEmptySlot = i;
                break;
            }
        }

        if (frontmostEmptySlot >= 0)
        {
            MoveFighterToSlotIndex(fighter, frontmostEmptySlot);
        }
    }

    public void MoveAllFightersUp()
    {
        bool openSlotInFront = false;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] != null && openSlotInFront)
            {
                CombatHandler fighter = _slots[i];
                MoveFighterUp(fighter);
            }
            else
            {
                openSlotInFront = true;
            }
        }
    }

    public List<CombatHandler> GetDeadCombatants()
    {
        List<CombatHandler> deadCombatants = new List<CombatHandler>();

        foreach(CombatHandler combatant in _slots)
        {
            if (combatant != null && combatant.TagHandler.HasTag(CombatState.Dead))
            {
                deadCombatants.Add(combatant);
            }
        }

        return deadCombatants;
    }

    public bool AreAllCombatantsDead()
    {
        bool areAllDead = true;
        foreach(CombatHandler combatant in _slots)
        {
            if (combatant != null && !combatant.TagHandler.HasTag(CombatState.Dead))
            {
                areAllDead = false;
                break;
            }
        }
        return areAllDead;
    }

    private void OnDrawGizmosSelected()
    {
        if (_rowPositions.Length < 1) return;

        for (int i = 0; i < _rowPositions.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_rowPositions[i], .25f);
        }
    }
}

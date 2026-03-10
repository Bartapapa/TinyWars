using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatRow
{
    [Header("MAXIMUM SLOTS")]
    [SerializeField] private int _maxSlots = 5;
    public int MaxSlots { get { return _maxSlots; } }

    [ReadOnlyInspector][SerializeField] private CombatHandler[] _slots;

    public void Initialize()
    {
        _slots = new CombatHandler[_maxSlots - 1];
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

                openSlotInFront = false;
            }
            else
            {
                openSlotInFront = true;
            }
        }
    }
}

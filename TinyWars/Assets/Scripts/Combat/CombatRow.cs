using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatRowSpawnPosition
{
    None,
    FrontMost,
    BackMost,
    AtContextSlot,
}

public enum CombatRowType
{
    Player,
    Enemy,
}

public class CombatRow : MonoBehaviour
{
    [Header("MAXIMUM SLOTS")]
    private int _maxSlots = -1;
    public int MaxSlots { get { return _maxSlots; } }

    [Header("ROW POSITIONS")]
    [SerializeField] private Vector3[] _rowPositions;
    [SerializeField] private float _positionSeparation = 1.2f;
    public Vector3[] RowPositions { get { return _rowPositions; } }

    [Header("FIGHTER SLOTS")]
    [ReadOnlyInspector][SerializeField] private CombatHandler[] _slots;
    public CombatHandler[] Slots { get { return _slots; } }

    private bool _enemySide = false;
    public bool EnemySide { get { return _enemySide; } }

    public void Initialize(int maxSlots, List<CombatHandler> team, bool enemySide)
    {
        if (maxSlots <= 0) return;

        _slots = new CombatHandler[maxSlots];
        _rowPositions = CreateRowPositions(maxSlots);
        _enemySide = enemySide;

        for (int i = 0; i < team.Count; i++)
        {
            if (i >= maxSlots) break;
            else
            {
                if (team[i] != null)
                {
                    InitializeFighter(team[i], i, enemySide);                   
                }
                else
                {
                    _slots[i] = null;
                }
            }
        }
    }

    public void ResetRowPositions(int maxSlots)
    {
        _rowPositions = CreateRowPositions(maxSlots);
    }

    private CombatHandler InitializeFighter(CombatHandler fighter, int slotIndex, bool enemySide)
    {
        CombatHandler newFighter = Instantiate<CombatHandler>(fighter, _rowPositions[slotIndex], Quaternion.identity, this.transform);
        //newFighter.Initialize();

        if (enemySide)
        {
            //newFighter.Character.Mesh.MeshPivot.localScale = new Vector3()
            newFighter.transform.localEulerAngles = new Vector3(0, 180, 0);
            newFighter.Character.Mesh.FlipFacing();
        }
        else
        {
            newFighter.transform.localEulerAngles = Vector3.zero;
        }

        newFighter.SetCurrentCombatRow(this);
        string teamName = "";
        if (_enemySide)
        {
            teamName = "Enemy";
        }
        else
        {
            teamName = "Player";
        }
        newFighter.gameObject.name = teamName + "_" + newFighter.Character.CharacterData.Character.ToString();
        _slots[slotIndex] = newFighter;

        return newFighter;
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

    public List<CombatHandler> GetCurrentFighters(bool mustBeLiving = true)
    {
        List<CombatHandler> currentFighters = new List<CombatHandler>();
        for (int i = 0; i < _slots.Length -1; i++)
        {
            if (_slots[i] != null)
            {
                if (mustBeLiving)
                {
                    if (!_slots[i].TagHandler.HasTag(CombatState.Dead))
                    {
                        currentFighters.Add(_slots[i]);
                    }
                }
                else
                {
                    currentFighters.Add(_slots[i]);
                }
            }
        }

        return currentFighters;
    }

    public List<CombatHandler> GetCurrentDeadFighters()
    {
        List<CombatHandler> currentFighters = new List<CombatHandler>();
        for (int i = 0; i < _slots.Length - 1; i++)
        {
            if (_slots[i] != null)
            {
                if (_slots[i].TagHandler.HasTag(CombatState.Dead))
                {
                    currentFighters.Add(_slots[i]);
                }
            }
        }

        return currentFighters;
    }

    public int GetFrontmostOpenSlot()
    {
        int openSlot = -1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots == null)
            {
                openSlot = i;
                break;
            }
        }

        return openSlot;
    }

    public int GetBackmostOpenSlot()
    {
        int openSlot = -1;
        for (int i = _slots.Length -1; i >= 0; i--)
        {
            if (_slots[i] == null)
            {
                openSlot = i;
            }
            else
            {
                break;
            }
        }

        return openSlot;
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

    public void ClearAllFighterCorpses()
    {
        foreach(CombatHandler fighter in GetCurrentDeadFighters())
        {
            ClearFighterCorpse(fighter);
        }
    }

    public void ClearFighterCorpse(CombatHandler fighter)
    {
        RemoveFighter(fighter);

        if (EventDispatcher.Instance)
        {
            FighterContext context = new FighterContext(fighter);
            EventDispatcher.Instance.Message_FighterCorpseCleared(ref context);
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

            //Move fighter to new position
            fighter.MoveToPosition(_rowPositions[toSlotIndex]);

            if (EventDispatcher.Instance)
            {
                MoveContext context = new MoveContext(fighter, this, toSlotIndex);
                EventDispatcher.Instance.Message_FighterMoved(ref context);
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

    public bool MoveFighterDown(CombatHandler fighter)
    {
        int fighterIndex = GetFighterSlotIndex(fighter);
        int emptySlotBehindIndex = -1;
        bool enoughSpace = false;

        if (fighterIndex == _slots.Length - 1) return enoughSpace;
        int slotToCheck = fighterIndex + 1;

        if (_slots[slotToCheck] != null)
        {
            if (_slots[slotToCheck].TagHandler.HasTag(CombatState.Dead))
            {
                ClearFighterCorpse(_slots[slotToCheck]);
            }
        }

        if (_slots[slotToCheck] == null)
        {
            emptySlotBehindIndex = slotToCheck;
            enoughSpace = true;
        }

        if (emptySlotBehindIndex >= 0)
        {
            MoveFighterToSlotIndex(fighter, emptySlotBehindIndex);
        }

        return enoughSpace;
    }

    public bool MoveAllFightersDown()
    {
        bool openSlotInBack = false;

        if (GetCurrentFighters().Count == _slots.Length) return openSlotInBack;

        for (int i = _slots.Length -1; i >= 0; i--)
        {
            if (_slots[i] != null && openSlotInBack)
            {
                CombatHandler fighter = _slots[i];
                MoveFighterDown(fighter);
            }
            else
            {
                openSlotInBack = true;
            }
        }

        return openSlotInBack;
    }

    public void MoveFighterUp(CombatHandler fighter)
    {
        int fighterIndex = GetFighterSlotIndex(fighter);
        int frontmostEmptySlot = -1;

        for (int i = 0; i < fighterIndex; i++)
        {
            if (_slots[i] != null)
            {
                if (_slots[i].TagHandler.HasTag(CombatState.Dead))
                {
                    ClearFighterCorpse(_slots[i]);
                }
            }

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
                i--;
                openSlotInFront = false;
            }
            else
            {
                openSlotInFront = true;
            }
        }
    }

    public bool SpawnFighter(CombatHandler originalFighter, int atSlot)
    {
        //If slot isn't empty, don't spawn.
        if (_slots[atSlot] != null) return false;
        else
        {
            CombatHandler newFighter = InitializeFighter(originalFighter, atSlot, _enemySide);
            newFighter.AnimationHandler.PlayAnimationWithBlend("Spawn");
            //MoveFighterUp(newFighter);
            if (EventDispatcher.Instance)
            {
                MoveContext newContext = new MoveContext(newFighter, this, atSlot);
                EventDispatcher.Instance.Message_FighterSpawned(ref newContext);
            }
        }
        return true;
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

    public bool AreAllCombatantsCleared()
    {
        bool areAllCleared = true;
        foreach (CombatHandler combatant in _slots)
        {
            if (combatant != null)
            {
                areAllCleared = false;
                break;
            }
        }
        return areAllCleared;
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

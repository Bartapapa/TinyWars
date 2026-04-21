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
    [Header("OBJECT REFERENCES")]
    [SerializeField] private CombatRowSlot _combatRowSlotPrefab;
    public CombatRowSlot CombatRowSlotPrefab { get { return _combatRowSlotPrefab; } }

    [Header("MAXIMUM SLOTS")]
    private int _maxSlots = -1;
    public int MaxSlots { get { return _maxSlots; } }

    [Header("SLOT POSITIONS")]
    [SerializeField] private Vector3[] _slotPositions;
    [SerializeField] private float _positionSeparation = 1.2f;
    public Vector3[] SlotPositions { get { return _slotPositions; } }

    [Header("FIGHTER SLOTS")]
    [ReadOnlyInspector][SerializeField] private CombatRowSlot[] _slots;
    public CombatRowSlot[] Slots { get { return _slots; } }

    private bool _enemySide = false;
    public bool EnemySide { get { return _enemySide; } }

    public void PlaceFighters(List<CombatHandler> team, bool enemySide)
    {
        if (_maxSlots <= 0) return;

        _enemySide = enemySide;

        for (int i = 0; i < team.Count; i++)
        {
            if (i >= _maxSlots) break;
            else
            {
                if (team[i] != null)
                {
                    InitializeFighter(team[i], i, enemySide);                   
                }
                else
                {
                    _slots[i].Fighter = null;
                }
            }
        }
    }

    public void InitializeSlots(int maxSlots)
    {
        if (maxSlots <= 0) return;

        _maxSlots = maxSlots;
        _slots = new CombatRowSlot[_maxSlots];

        for (int i = 0; i < _slots.Length; i++)
        {
            //Create individual combatrowslots.
            CombatRowSlot newSlot = Instantiate<CombatRowSlot>(_combatRowSlotPrefab, this.transform);
            _slots[i] = newSlot;
        }

        _slotPositions = CreateRowPositions(_maxSlots);
    }

    public void ResetRowPositions(int maxSlots)
    {
        _slotPositions = CreateRowPositions(maxSlots);
    }

    private CombatHandler InitializeFighter(CombatHandler fighter, int slotIndex, bool enemySide)
    {
        fighter.transform.parent = this.transform.parent;
        fighter.transform.position = _slotPositions[slotIndex];

        fighter.AbilityHandler.AbilitiesStartListen();
        fighter.TagHandler.AddTagIfNotPresent(HolderState.CannotBeDragged);

        if (enemySide)
        {
            //newFighter.Character.Mesh.MeshPivot.localScale = new Vector3()
            fighter.transform.localEulerAngles = new Vector3(0, 90, 0);
            fighter.Character.Mesh.FlipFacing();
        }
        else
        {
            fighter.transform.localEulerAngles = new Vector3(0, 90, 0); ;
        }

        fighter.SetCurrentCombatRow(this);

        //DEBUG
        string teamName = "";
        if (_enemySide)
        {
            teamName = "Enemy";
        }
        else
        {
            teamName = "Player";
        }
        fighter.gameObject.name = teamName + "_" + fighter.Character.CharacterData.Character.ToString();


        _slots[slotIndex].Fighter = fighter;

        return fighter;
    }

    public void ClearRow()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Fighter != null)
            {
                Destroy(_slots[i].Fighter.gameObject);
            }
            Destroy(_slots[i].gameObject);
        }

        _slots = null;
        _slotPositions = null;
    }

    private Vector3[] CreateRowPositions(int slots)
    {
        Vector3[] positions = new Vector3[slots];
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 pos = transform.position - (transform.forward * i * _positionSeparation);
            positions[i] = pos;
            _slots[i].transform.position = pos;
            _slots[i].transform.rotation = this.transform.rotation;
        }
        return positions;
    }

    public CombatHandler GetFighterAtIndex(int index)
    {
        return _slots[index].Fighter;
    }

    public int GetFighterSlotIndex(CombatHandler fighter)
    {
        int foundIndex = -1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Fighter == fighter)
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
            if (_slots[i].Fighter != null)
            {
                if (mustBeLiving)
                {
                    if (!_slots[i].Fighter.TagHandler.HasTag(CombatState.Dead))
                    {
                        currentFighters.Add(_slots[i].Fighter);
                    }
                }
                else
                {
                    currentFighters.Add(_slots[i].Fighter);
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
            if (_slots[i].Fighter != null)
            {
                if (_slots[i].Fighter.TagHandler.HasTag(CombatState.Dead))
                {
                    currentFighters.Add(_slots[i].Fighter);
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
            if (_slots[i].Fighter == null)
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
            if (_slots[i].Fighter == null)
            {
                openSlot = i;
            }
            else
            {
                if (_slots[i].Fighter.TagHandler.HasTag(CombatState.Dead))
                {
                    openSlot = i;
                }
                else
                {
                    break;
                }             
            }
        }

        return openSlot;
    }

    public void PlaceFighterAtSlot(CombatHandler fighter, int i)
    {
        if (i >= _slots.Length || i < 0) return;
        _slots[i].Fighter = fighter;
    }

    public void PlaceFighter(CombatHandler fighter)
    {
        int fighterSlotIndex = -1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Fighter == null)
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
        _slots[i].Fighter = null;
    }

    public void RemoveFighter(CombatHandler fighter)
    {
        int fighterIndex = -1;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Fighter == fighter)
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
        if (_slots[toSlotIndex].Fighter != null)
        {
            SwitchFighterSlots(fighter, GetFighterAtIndex(toSlotIndex));
        }
        else
        {
            int fighterIndex = GetFighterSlotIndex(fighter);
            _slots[fighterIndex].Fighter = null;
            _slots[toSlotIndex].Fighter = fighter;

            //Move fighter to new position
            fighter.MoveToPosition(_slotPositions[toSlotIndex]);

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

        _slots[fighter1Slot].Fighter = null;
        _slots[fighter2Slot].Fighter = null;
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

        if (_slots[slotToCheck].Fighter != null)
        {
            if (_slots[slotToCheck].Fighter.TagHandler.HasTag(CombatState.Dead))
            {
                ClearFighterCorpse(_slots[slotToCheck].Fighter);
            }
        }

        if (_slots[slotToCheck].Fighter == null)
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
            if (_slots[i].Fighter != null && openSlotInBack)
            {
                CombatHandler fighter = _slots[i].Fighter;
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
            if (_slots[i].Fighter != null)
            {
                if (_slots[i].Fighter.TagHandler.HasTag(CombatState.Dead))
                {
                    ClearFighterCorpse(_slots[i].Fighter);
                }
            }

            if (_slots[i].Fighter == null)
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
            if (_slots[i].Fighter != null && openSlotInFront)
            {
                CombatHandler fighter = _slots[i].Fighter;
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
        //If slot isn't occupied by dead character or isn't empty, don't spawn.
        CombatHandler corpseCharacter = null;
        if (_slots[atSlot].Fighter != null)
        {
            if (!_slots[atSlot].Fighter.TagHandler.HasTag(CombatState.Dead))
            {
                return false;
            }
            else
            {
                corpseCharacter = _slots[atSlot].Fighter;
            }
        }

        if (corpseCharacter != null)
        {
            ClearFighterCorpse(corpseCharacter);
        }

        CombatHandler newFighter = Instantiate<CombatHandler>(originalFighter);
        newFighter.Initialize();
        InitializeFighter(newFighter, atSlot, _enemySide);
        newFighter.AnimationHandler.PlayAnimationWithBlend("Spawn");

        if (EventDispatcher.Instance)
        {
            MoveContext newContext = new MoveContext(newFighter, this, atSlot);
            EventDispatcher.Instance.Message_FighterSpawned(ref newContext);
        }
        return true;
    }

    public List<CombatHandler> GetDeadCombatants()
    {
        List<CombatHandler> deadCombatants = new List<CombatHandler>();



        foreach(CombatRowSlot slot in _slots)
        {
            if (slot.Fighter != null)
            {
                if (slot.Fighter.TagHandler.HasTag(CombatState.Dead))
                {
                    deadCombatants.Add(slot.Fighter);
                }
            }
        }

        return deadCombatants;
    }

    public bool AreAllCombatantsDead()
    {
        bool areAllDead = true;
        foreach(CombatRowSlot slot in _slots)
        {
            if (slot.Fighter != null)
            {
                if (!slot.Fighter.TagHandler.HasTag(CombatState.Dead))
                {
                    areAllDead = false;
                    break;
                }
            }
        }
        return areAllDead;
    }

    public bool AreAllCombatantsCleared()
    {
        bool areAllCleared = true;
        foreach (CombatRowSlot slot in _slots)
        {
            if (slot.Fighter != null)
            {
                areAllCleared = false;
                break;
            }
        }
        return areAllCleared;
    }

    private void OnDrawGizmosSelected()
    {
        if (_slotPositions == null) return;
        if (_slotPositions.Length < 1) return;

        for (int i = 0; i < _slotPositions.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_slotPositions[i], .25f);
        }
    }
}

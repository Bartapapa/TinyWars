using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static CombatManager _instance = null;

    public static CombatManager Instance { get { return _instance; } }

    [Header("BATTLEFIELD")]
    [SerializeField] private BattleField _battleField;
    public BattleField CurrentBattleField { get { return _battleField; } }
    private List<CombatHandler> _currentEnemyParty = new List<CombatHandler>();
    public List<CombatHandler> CurrentEnemyParty { get { return _currentEnemyParty; } }

    [Header("UNIVERSAL ACTIONS")]
    public Action_Attack AttackAction;
    public Action_ClearCorpse ClearCorpseAction;

    private Coroutine _phaseSequenceCo = null;
    public bool UnderGoingPhaseSequence { get { return _phaseSequenceCo != null; } }

    private CombatPhase _currentPhase = null;
    private CombatPhase _nextPhase = null;

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

    public void SetEnemyParty(List<CombatHandler> enemyParty)
    {
        _currentEnemyParty = enemyParty;
    }

    public void StartCombat(BattleField battleField, List<CombatHandler> playerTeam, List<CombatHandler> enemyTeam)
    {
        //Listen for action call events.
        if (EventDispatcher.Instance != null)
        {
            EventDispatcher.Instance.ActionCalled -= OnActionCalled;
            EventDispatcher.Instance.ActionCalled += OnActionCalled;
        }

        int maxTeamSlots = GameManager.Instance.UniversalData.MaxTeamSlots;

        _battleField = battleField;
        _battleField.PlayerRow.transform.position = _battleField.PlayerRowPreFightPos;
        _battleField.EnemyRow.transform.position = _battleField.EnemyRowPreFightPos;
        _battleField.InitializeBattlefield(maxTeamSlots, playerTeam, enemyTeam);

        TransitionIntoPreCombat();
    }

    private void TransitionIntoPreCombat()
    {
        StartCoroutine(PreCombatTransitionCo());
    }

    private IEnumerator PreCombatTransitionCo()
    {
        float baseDuration = 3f;
        float transitionDuration = baseDuration * GameManager.Instance.ActionTime;
        float transitionTimer = 0f;

        _battleField.PlayerRow.transform.position = _battleField.PlayerRowPreFightPos;
        _battleField.EnemyRow.transform.position = _battleField.EnemyRowPreFightPos;
        _battleField.ResetRowPositions();

        for (int i = 0; i < _battleField.PlayerRow.Slots.Length; i++)
        {
            if (_battleField.PlayerRow.Slots[i].Fighter != null)
            {
                _battleField.PlayerRow.Slots[i].Fighter.transform.position = _battleField.PlayerRowInitialPos;
                _battleField.PlayerRow.Slots[i].Fighter.MoveToPosition(_battleField.PlayerRow.SlotPositions[i], baseDuration);
            }
        }

        for (int i = 0; i < _battleField.PlayerRow.Slots.Length; i++)
        {
            if (_battleField.EnemyRow.Slots[i].Fighter != null)
            {
                _battleField.EnemyRow.Slots[i].Fighter.transform.position = _battleField.EnemyRowInitialPos;
                _battleField.EnemyRow.Slots[i].Fighter.MoveToPosition(_battleField.EnemyRow.SlotPositions[i], baseDuration);
            }
        }

        while (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.deltaTime;
            yield return null;
        }

        CombatContext context = new CombatContext(_battleField.PlayerRow, _battleField.EnemyRow, true);
        EventDispatcher.Instance.Message_OnCombatStarted(ref context);

        StartPreCombatSequence();
    }

    private void StartPreCombatSequence()
    {
        if (PhaseCheck())
        {
            _phaseSequenceCo = StartCoroutine(PhaseSequenceCo(true));
        }
        else
        {
            _battleField.PlayerRow.MoveAllFightersUp();
            _battleField.EnemyRow.MoveAllFightersUp();

            StartCoroutine(CombatTransitionCo());
        }
    }

    private IEnumerator CombatTransitionCo()
    {
        float baseDuration = 1f;
        float transitionDuration = baseDuration * GameManager.Instance.ActionTime;
        float transitionTimer = 0f;

        _battleField.PlayerRow.transform.position = _battleField.PlayerRowBasePos;
        _battleField.EnemyRow.transform.position = _battleField.EnemyRowBasePos;
        _battleField.ResetRowPositions();

        for (int i = 0; i < _battleField.PlayerRow.Slots.Length; i++)
        {
            if (_battleField.PlayerRow.Slots[i].Fighter != null)
            {
                _battleField.PlayerRow.Slots[i].Fighter.MoveToPosition(_battleField.PlayerRow.SlotPositions[i], baseDuration);
            }
        }

        for (int i = 0; i < _battleField.PlayerRow.Slots.Length; i++)
        {
            if (_battleField.EnemyRow.Slots[i].Fighter != null)
            {
                _battleField.EnemyRow.Slots[i].Fighter.MoveToPosition(_battleField.EnemyRow.SlotPositions[i], baseDuration);
            }
        }

        while (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.deltaTime;
            yield return null;
        }

        StartCombatSequence();
    }

    private void StartCombatSequence()
    {
        //Have participants, turn by turn, attack each other simultaneously until one side has no more participants.
        //Each 'turn' of combat is deconstructed in Phases, where both Player and Enemy team actions are listed.
        //Phases are lists of actions, and Enemy and Player phase actions are done simultaneously. (All actions of a Phase take the same amount of time to 'enact')
        //Once all actions of both teams are done, there is a new phase check, to see if additional actions were generated by the actions of the previous phase.
        //If there is a new phase, all actions of that phase are done.
        //Sometimes, a team can 'skip' a phase, depending on the actions done by the other team. This is not an issue, as even if a team's phase has no actions, they are still technically participating in a phase.
        //This continues until there are no more actions in either phase, at which point a clash happens.
        //During a clash, both frontline participants hit each other for their attack values.
        //Following a clash, there is another phase check.
        //Once no more actions are in either phase, participants who have 0 Health or lower die, leaving their slot open.
        //If there are open slots that are found in front of characters, they move into that slot.

        //EVENTS can call phase checks. The end of a phase check is an event.
        //Clashes, a character dying, a character being hit, a character being healed - all of these can cause phase check requests.
        //If a phase check request is detected, the actions of the following phase are checked - if there are any, generate a phase for both teams.



        if (PhaseCheck())
        {
            _phaseSequenceCo = StartCoroutine(PhaseSequenceCo(false));
        }
        else
        {
            //If everyone dead is cleared, end combat.
            bool playersCleared = _battleField.PlayerRow.AreAllCombatantsCleared();
            bool enemiesCleared = _battleField.EnemyRow.AreAllCombatantsCleared();
            if (playersCleared || enemiesCleared)
            {
                EndCombat();
                if (playersCleared && !enemiesCleared)
                {
                    Debug.LogWarning("The players were defeated!");
                }
                else if (enemiesCleared && !playersCleared)
                {
                    Debug.LogWarning("The enemies were defeated!");
                }
                else if (enemiesCleared && playersCleared)
                {
                    Debug.LogWarning("Everyone was defeated! It was a tie!");
                }
                return;
            }

            //If some combatants are dead, clear their corpses and move everyone alive up.
            List<CombatHandler> deadPlayercombatants = _battleField.PlayerRow.GetDeadCombatants();
            List<CombatHandler> deadEnemycombatants = _battleField.EnemyRow.GetDeadCombatants();
            if (deadPlayercombatants.Count > 0 || deadEnemycombatants.Count > 0)
            {
                _currentPhase = new CombatPhase();
                foreach (CombatHandler deadPlayer in deadPlayercombatants)
                {
                    TWAction playerClearCorpseAction = ClearCorpseAction.GenerateAction(deadPlayer.gameObject, new List<GameObject>());
                    _currentPhase.PhaseActions.Add(playerClearCorpseAction);
                }

                foreach (CombatHandler deadEnemy in deadEnemycombatants)
                {
                    TWAction enemyClearCorpseAction = ClearCorpseAction.GenerateAction(deadEnemy.gameObject, new List<GameObject>());
                    _currentPhase.PhaseActions.Add(enemyClearCorpseAction);
                }

                _phaseSequenceCo = StartCoroutine(PhaseSequenceCo(false));

                _battleField.PlayerRow.MoveAllFightersUp();
                _battleField.EnemyRow.MoveAllFightersUp();
            }      
            else
            {
                //Otherwise, attack and proceed as normal.
                _currentPhase = new CombatPhase();
                List<GameObject> playerTarget = new List<GameObject>();
                playerTarget.Add(_battleField.EnemyRow.Slots[0].Fighter.gameObject);
                List<GameObject> enemyTarget = new List<GameObject>();
                enemyTarget.Add(_battleField.PlayerRow.Slots[0].Fighter.gameObject);
                TWAction playerAttackAction = AttackAction.GenerateAction(_battleField.PlayerRow.Slots[0].Fighter.gameObject, playerTarget);
                TWAction enemyAttackAction = AttackAction.GenerateAction(_battleField.EnemyRow.Slots[0].Fighter.gameObject, enemyTarget);
                _currentPhase.PhaseActions.Add(playerAttackAction);
                _currentPhase.PhaseActions.Add(enemyAttackAction);

                _phaseSequenceCo = StartCoroutine(PhaseSequenceCo(false));
            }
        }
    }

    private bool PhaseCheck()
    {
        bool proceedToNextPhase = false;
        if (_nextPhase != null)
        {
            _currentPhase = _nextPhase;
            _nextPhase = null;
            proceedToNextPhase = true;
        }

        return proceedToNextPhase;
    }

    private IEnumerator PhaseSequenceCo(bool preCombat)
    {
        _currentPhase.SortPhaseActions();

        _currentPhase.EnactNextPhaseAction();

        float currentPhaseTimer = GameManager.Instance.ActionTime;

        while (currentPhaseTimer > 0f)
        {
            currentPhaseTimer -= Time.deltaTime;
            yield return null;
        }

        _phaseSequenceCo = null;
        OnActionEnded(preCombat);
    }

    private void OnActionEnded(bool preCombat)
    {
        //Check if current phase still has actions to process.
        if (_currentPhase.HasActionsToProcess)
        {
            //If yes, process next action.
            _phaseSequenceCo = StartCoroutine(PhaseSequenceCo(preCombat));
        }
        else
        {
            //If no, start new combat sequence.
            if (preCombat)
            {
                StartPreCombatSequence();
            }
            else
            {
                StartCombatSequence();
            }
        }
    }

    public void EndCombat()
    {
        //Stop listening for action call events.
        if (EventDispatcher.Instance != null)
        {
            EventDispatcher.Instance.ActionCalled -= OnActionCalled;
        }

        //Transition to end of combat screen / game over screen depending on fight results.
        //Call end of combat event, allow abilities based on end of combat to apply.
        //Initiate recruitment phase if possible.

        //Remove combatants from combat. Change this, only used for debugging.
        //_battleField.PlayerRow.ClearRow();

        for (int i = 0; i < PartyManager.Instance.CurrentParty.Count; i++)
        {
            CombatHandler fighter = PartyManager.Instance.CurrentParty[i];
            if (fighter != null)
            {
                fighter.Character.Mesh.ResetState();
            }
        }

        for (int i = 0; i < _currentEnemyParty.Count; i++)
        {
            CombatHandler fighter = _currentEnemyParty[i];
            if (fighter != null)
            {
                fighter.Character.Mesh.ResetState();
            }
        }

        //Should destroy all fighters that aren't needed - essentially any enemy that appeared DURING the fight. Perhaps a value to set when spawning a character mid-combat
        _battleField.PlayerRow.PlaceFighters(PartyManager.Instance.CurrentParty, false);
        for (int i = 0; i < _battleField.PlayerRow.Slots.Length; i++)
        {
            CombatHandler fighter = _battleField.PlayerRow.Slots[i].Fighter;
            if (fighter != null)
            {
                fighter.HealToFull(true);
                fighter.RemoveAllCombatModifiers();
                fighter.TagHandler.ClearTagsOfHandle(HolderState.CannotBeDragged);
            }
        }

        _battleField.EnemyRow.PlaceFighters(_currentEnemyParty, false);
        for (int i = 0; i < _battleField.EnemyRow.Slots.Length; i++)
        {
            CombatHandler fighter = _battleField.EnemyRow.Slots[i].Fighter;
            if (fighter != null)
            {
                fighter.HealToFull(true);
                fighter.RemoveAllCombatModifiers();
                fighter.TagHandler.ClearTagsOfHandle(HolderState.CannotBeDragged);
            }
        }

        _battleField = null;
    }

    private void OnActionCalled(ActionContext context)
    {
        //If no next phase has been created yet, make one.
        if (_nextPhase == null)
        {
            _nextPhase = new CombatPhase();
        }
        //Add action to next phase's list of actions.
        _nextPhase.PhaseActions.Add(context.Action);
    }
}

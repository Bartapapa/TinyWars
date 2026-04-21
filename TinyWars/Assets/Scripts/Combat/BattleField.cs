using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BattleFieldFighterDescriptor
{
    public CombatHandler Fighter;
    public FighterSetupDescriptor Setup;
}

public class BattleField : MonoBehaviour
{
    [Header("COMBAT ROWS")]
    [SerializeField] private CombatRow _playerRow;
    [SerializeField] private CombatRow _enemyRow;
    public CombatRow PlayerRow { get { return _playerRow; } }
    public CombatRow EnemyRow { get { return _enemyRow; } }

    [Header("ENVIRONMENT PARAMETERS")]
    [SerializeField] private float _finalRowSeparation = 3f;
    [SerializeField] private float _preFightRowSeparation = 5.5f;
    [SerializeField] private float _initialRowSeparation = 10f;

    [Header("DEBUG COMBAT")]
    public bool StartCombatOnLoad = false;
    public List<BattleFieldFighterDescriptor> PlayerTeamDescriptor = new List<BattleFieldFighterDescriptor>();
    public List<BattleFieldFighterDescriptor> EnemyTeamDescriptor = new List<BattleFieldFighterDescriptor>();

    public Vector3 EnemyRowBasePos { get { return transform.position + (transform.right * (_finalRowSeparation * .5f)); } }
    public Vector3 PlayerRowBasePos { get { return transform.position - (transform.right * (_finalRowSeparation * .5f)); } }
    public Vector3 EnemyRowPreFightPos { get { return transform.position + (transform.right * (_preFightRowSeparation * .5f)); } }
    public Vector3 PlayerRowPreFightPos { get { return transform.position - (transform.right * (_preFightRowSeparation * .5f)); } }
    public Vector3 EnemyRowInitialPos { get { return transform.position + (transform.right * (_initialRowSeparation * .5f)); } }
    public Vector3 PlayerRowInitialPos { get { return transform.position - (transform.right * (_initialRowSeparation * .5f)); } }

    private int _maxSlots = 0;


    private void Start()
    {
        if (StartCombatOnLoad && PlayerTeamDescriptor.Count > 0 && EnemyTeamDescriptor.Count > 0)
        {
            //Mimics having a pre-generated team beforehand. Re-use this when actually creating a team.

            List<CombatHandler> instantiatedPlayerTeam = new List<CombatHandler>();
            List<CombatHandler> instantiatedEnemyTeam = new List<CombatHandler>();

            foreach(BattleFieldFighterDescriptor fighterDescriptor in PlayerTeamDescriptor)
            {
                CombatHandler newFighter = null;
                if (fighterDescriptor.Fighter != null)
                {
                    newFighter = Instantiate<CombatHandler>(fighterDescriptor.Fighter, GameManager.Instance.transform);
                    newFighter.Initialize();
                    newFighter.Setup(fighterDescriptor.Setup);
                }

                instantiatedPlayerTeam.Add(newFighter);
            }
            PartyManager.Instance.SetCurrentParty(instantiatedPlayerTeam);

            foreach (BattleFieldFighterDescriptor fighterDescriptor in EnemyTeamDescriptor)
            {
                CombatHandler newFighter = null;
                if (fighterDescriptor.Fighter != null)
                {
                    newFighter = Instantiate<CombatHandler>(fighterDescriptor.Fighter, GameManager.Instance.transform);
                    newFighter.Initialize();
                    newFighter.Setup(fighterDescriptor.Setup);
                }

                instantiatedEnemyTeam.Add(newFighter);
            }
            CombatManager.Instance.SetEnemyParty(instantiatedEnemyTeam);

            GameManager.Instance.DBG_StartCombat(this, instantiatedPlayerTeam, instantiatedEnemyTeam);
            UIManager.Instance.Transitioner.Reveal();
        }
    }

    public void InitializeBattlefield(int maxTeamSlots, List<CombatHandler> playerTeam, List<CombatHandler> enemyTeam)
    {
        _maxSlots = maxTeamSlots;

        _playerRow.transform.position = PlayerRowBasePos;
        _enemyRow.transform.position = EnemyRowBasePos;

        _playerRow.InitializeSlots(maxTeamSlots);
        _enemyRow.InitializeSlots(maxTeamSlots);

        _playerRow.PlaceFighters(playerTeam, false);
        _enemyRow.PlaceFighters(enemyTeam, true);
    }

    public void ResetRowPositions()
    {
        _playerRow.ResetRowPositions(_maxSlots);
        _enemyRow.ResetRowPositions(_maxSlots);
    }

    private void OnDrawGizmosSelected()
    {
        if (_playerRow == null || _enemyRow == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(EnemyRowBasePos, .35f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(PlayerRowBasePos, .35f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(EnemyRowInitialPos, .25f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(PlayerRowInitialPos, .25f);
    }
}

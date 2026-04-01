using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    [Header("COMBAT ROWS")]
    [SerializeField] private CombatRow _playerRow;
    [SerializeField] private CombatRow _enemyRow;
    public CombatRow PlayerRow { get { return _playerRow; } }
    public CombatRow EnemyRow { get { return _enemyRow; } }

    [Header("ENVIRONMENT PARAMETERS")]
    [SerializeField] private float _finalRowSeparation = 3f;
    [SerializeField] private float _initialRowSeparation = 10f;

    [Header("DEBUG COMBAT")]
    public bool StartCombatOnLoad = false;
    public List<CombatHandler> PlayerTeam = new List<CombatHandler>();
    public List<CombatHandler> EnemyTeam = new List<CombatHandler>();

    public Vector3 EnemyRowBasePos { get { return transform.position + (transform.right * (_finalRowSeparation * .5f)); } }
    public Vector3 PlayerRowBasePos { get { return transform.position - (transform.right * (_finalRowSeparation * .5f)); } }
    public Vector3 EnemyRowInitialPos { get { return transform.position + (transform.right * (_initialRowSeparation * .5f)); } }
    public Vector3 PlayerRowInitialPos { get { return transform.position - (transform.right * (_initialRowSeparation * .5f)); } }

    private int _maxSlots = 0;


    private void Start()
    {
        if (StartCombatOnLoad && PlayerTeam.Count > 0 && EnemyTeam.Count > 0)
        {
            UIManager.Instance.Transitioner.Reveal();
            GameManager.Instance.DBG_StartCombat(this, PlayerTeam, EnemyTeam);
        }
    }

    public void InitializeBattlefield(int maxTeamSlots, List<CombatHandler> playerTeam, List<CombatHandler> enemyTeam)
    {
        _maxSlots = maxTeamSlots;

        _playerRow.transform.position = PlayerRowBasePos;
        _enemyRow.transform.position = EnemyRowBasePos;

        _playerRow.Initialize(maxTeamSlots, playerTeam, false);
        _enemyRow.Initialize(maxTeamSlots, enemyTeam, true);
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

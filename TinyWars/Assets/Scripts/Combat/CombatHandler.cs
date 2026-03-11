using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum CombatState
{
    Dead,
}

public class CombatHandler : MonoBehaviour
{
    public Character Character { get { return _character; } }
    private Character _character;
    public TagHandler TagHandler { get { return _tagHandler; } }
    private TagHandler _tagHandler;

    [Header("COMBAT DATA")]
    [SerializeField] private SOCombatData _combatData;
    public SOCombatData CombatData { get { return _combatData; } }

    [Header("STATISTICS")]
    [SerializeField] private Statistic _health;
    [SerializeField] private Statistic _attack;

    public Statistic Health { get { return _health; } }
    public Statistic Attack { get { return _attack; } }

    [Header("COMBAT")]
    [ReadOnlyInspector] [SerializeField] private CombatRow _currentRow;
    public CombatRow CurrentRow { get { return _currentRow; } }

    private bool _initialized = false;

    private void Awake()
    {
        if (!_initialized)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        _character = GetComponent<Character>();
        if (!_character)
        {
            Debug.LogWarning("Warning! " + this.gameObject.name + " has no associated character. Returning.");
            return;
        }

        _tagHandler = GetComponent<TagHandler>();
        if (!_tagHandler)
        {
            Debug.LogWarning("Warning! " + this.gameObject.name + " has no associated tag handler. Returning.");
            return;
        }

        if (_combatData)
        {
            _health = new Statistic(_combatData.BaseHealth);
            _attack = new Statistic(_combatData.BaseAttack);
        }
        else
        {
            Debug.LogWarning("Warning! " + this.gameObject.name + " has no associated combat data. Returning.");
            return;
        }

        _health.StatisticValueChanged -= OnHealthValueChanged;
        _health.StatisticValueChanged += OnHealthValueChanged;

        _initialized = true;
    }

    private void OnEnable()
    {
        _health.StatisticValueChanged -= OnHealthValueChanged;
        _health.StatisticValueChanged += OnHealthValueChanged;
    }

    private void OnDisable()
    {
        _health.StatisticValueChanged -= OnHealthValueChanged;
    }

    public void SetCurrentCombatRow(CombatRow row)
    {
        _currentRow = row;
    }

    public float DamageTarget(CombatHandler target)
    {
        float damageDealt = 0;
        float oldTargetHealthValue = target.Health.Value;

        target.Health.AddModifier(new StatisticModifier(_attack.Value * -1, StatisticModifierType.Flat, ModifierApplicationType.Permanent, this));

        damageDealt = oldTargetHealthValue - target.Health.Value;

        Debug.Log(this.gameObject + " attacked " + target.gameObject + " for " + damageDealt + " damage!");

        return damageDealt;
    }

    private void OnHealthValueChanged (float from, float to)
    {
        if (to <= 0 && !_tagHandler.HasTag(CombatState.Dead))
        {
            if (EventDispatcher.Instance)
            {
                EventDispatcher.Instance.Message_HandlerHealthReachedZero(this);
            }
            Debug.Log(this.gameObject + " died!");
            _tagHandler.AddTag(CombatState.Dead, this);
        }
    }
}

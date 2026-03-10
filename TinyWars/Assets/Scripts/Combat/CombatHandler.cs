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
    public Statistic Damage { get { return _attack; } }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
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

    public void DamageTarget(CombatHandler target)
    {
        target.Health.AddModifier(new StatisticModifier(_attack.Value * -1, StatisticModifierType.Flat, ModifierApplicationType.Permanent, this));

        if (EventDispatcher.Instance)
        {
            EventDispatcher.Instance.Message_HandlerAttack(this, target, _attack.Value);
        }
    }

    private void OnHealthValueChanged (float from, float to)
    {
        if (to <= 0 && !_tagHandler.HasTag(CombatState.Dead))
        {
            if (EventDispatcher.Instance)
            {
                EventDispatcher.Instance.Message_HandlerHealthReachedZero(this);
            }
            _tagHandler.AddTag(CombatState.Dead, this);
        }
    }
}

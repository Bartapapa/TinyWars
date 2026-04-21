using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExperienceHandler : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private CombatHandler _combatHandler;
    public CombatHandler CombatHandler { get { return _combatHandler; } }
    public StatisticHandler StatisticHandler { get { return _statisticHandler; } }
    [SerializeField] private StatisticHandler _statisticHandler;


    [Header("LEVEL")]
    [SerializeField][ReadOnlyInspector] private Statistic _currentLevel;
    public int CurrentLevelInt { get { return _currentLevel.IntValue; } }
    public Statistic CurrentLevelStat { get { return _currentLevel; } }

    [Header("EXPERIENCE")]
    [SerializeField][ReadOnlyInspector] private Statistic _currentXP;
    public float CurrentXPValue { get { return _currentXP.Value; } }
    [SerializeField][ReadOnlyInspector] private Statistic _XPgainMultiplier;
    public float XPGainMultiplierValue { get { return _XPgainMultiplier.Value; } }
    [SerializeField][ReadOnlyInspector] private Statistic _nextXPThreshold;
    public float NextXPThresholdValue { get { return _nextXPThreshold.Value; } }

    private bool _initialized = false;

    public void Initialize()
    {
        _currentLevel = _statisticHandler.CreateStat("CurrentLevel", 1f, 1f);
        _currentXP = _statisticHandler.CreateStat("CurrentXP", 0f, 0f);
        _XPgainMultiplier = _statisticHandler.CreateStat("XPGainMultiplier", 1f, 0f);
        _nextXPThreshold = _statisticHandler.CreateStat("NextXPThreshold", 0f, 0f);

        SetNextXPThreshold(CurrentLevelInt);

        _currentXP.StatisticValueChanged -= OnCurrentXPChangedValue;
        _currentXP.StatisticValueChanged += OnCurrentXPChangedValue;

        _initialized = true;
    }

    private void OnCurrentXPChangedValue(float from, float to)
    {
        if (!_initialized) return;

        if (to > from)
        {
            //Increased XP, such as after combat.
            //Upon XP gain, increase health and attack by that same value.
            if (to > NextXPThresholdValue)
            {
                LevelUp();
            }
        }

        if (to < from)
        {
            //Decreased XP, such as after a level up.
        }
    }

    public void LevelUp()
    {
        //Add a level, and reset current XP.
        _currentLevel.AddModifier(new StatisticModifier(1f, StatisticModifierType.Flat, ModifierApplicationType.Permanent));
        _currentXP.AddModifier(new StatisticModifier(0f, StatisticModifierType.Force, ModifierApplicationType.Permanent));

        SetNextXPThreshold(CurrentLevelInt);

        //Apply health and damage bonuses appropriate to character combat data.
        float healthIncrease = GameManager.Instance.UniversalData.LevelUpMaxHealthIncrease;
        float attackIncrease = GameManager.Instance.UniversalData.LevelUpAttackIncrease;
        _combatHandler.MaxHealth.AddModifier(new StatisticModifier(healthIncrease, StatisticModifierType.Flat, ModifierApplicationType.Permanent));
        _combatHandler.Health.AddModifier(new StatisticModifier(healthIncrease, StatisticModifierType.Flat, ModifierApplicationType.Permanent));
        _combatHandler.Attack.AddModifier(new StatisticModifier(attackIncrease, StatisticModifierType.Flat, ModifierApplicationType.Permanent));

        if (EventDispatcher.Instance)
        {
            FighterContext context = new FighterContext(_combatHandler);
            EventDispatcher.Instance.Message_FighterLevelUp(ref context);
        }
    }

    private void SetNextXPThreshold(int level)
    {
        _nextXPThreshold.AddModifier(new StatisticModifier(GameManager.Instance.UniversalData.GetXPValueByLevel(level), StatisticModifierType.Force, ModifierApplicationType.Permanent));
    }
}

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum CombatState
{
    Dead,
}

public enum CharacterStat
{
    Health,
    Attack,
    MaxHealth,
}

[System.Serializable]
public struct AbilitySetupDescriptor
{
    public TWAbility Ability;
    public int AbilityLevel;
}

[System.Serializable]
public struct FighterSetupDescriptor
{
    public int Level;
    public int AdditionalMaxHealthIncrease;
    public int AdditionalAttackIncrease;
    public int OverrideCurrentHealth;
    public int BaseAbilityLevel;
    public List<AbilitySetupDescriptor> AdditionalAbilities;
    public List<TWItem> ConsumedItems;
    public TWItem EquippedItem;

    public FighterSetupDescriptor(int level, int additionalMaxHealthIncrease, int additionalAttackIncrease, int overrideCurrentHealth, int baseAbilityLevel,
        List<AbilitySetupDescriptor> additionalAbilities, List<TWItem> consumedItems, TWItem equippedItem)
    {
        Level = level;
        AdditionalMaxHealthIncrease = additionalMaxHealthIncrease;
        AdditionalAttackIncrease = additionalAttackIncrease;
        OverrideCurrentHealth = overrideCurrentHealth;
        BaseAbilityLevel = baseAbilityLevel;

        AdditionalAbilities = additionalAbilities;
        ConsumedItems = consumedItems;
        EquippedItem = equippedItem;
    }
}

public class CombatHandler : MonoBehaviour
{

    [Header("OBJECT REFERENCES")]
    [SerializeField] private Character _character;
    [SerializeField] private TagHandler _tagHandler;
    [SerializeField] private DeathBehavior _deathBehavior;
    [SerializeField] private StatisticHandler _statisticHandler;
    [SerializeField] private AbilityHandler _abilityHandler;
    [SerializeField] private ExperienceHandler _experienceHandler;
    [SerializeField] private AAnimationHandler _animHandler;
    public AAnimationHandler AnimationHandler { get { return _animHandler; } }
    public Character Character { get { return _character; } }
    public TagHandler TagHandler { get { return _tagHandler; } }
    public DeathBehavior DeathBehavior { get { return _deathBehavior; } }
    public StatisticHandler StatisticHandler { get { return _statisticHandler; } }
    public ExperienceHandler ExperienceHandler { get { return _experienceHandler; } }
    public AbilityHandler AbilityHandler { get { return _abilityHandler; } }

    [Header("COMBAT DATA")]
    [SerializeField] private SOCombatData _combatData;
    public SOCombatData CombatData { get { return _combatData; } }

    [Header("STATISTICS")]
    [SerializeField] private Statistic _health;
    [SerializeField] private Statistic _attack;
    [SerializeField] private Statistic _maxHealth;

    public Statistic Health { get { return _health; } }
    public Statistic Attack { get { return _attack; } }
    public Statistic MaxHealth { get { return _maxHealth; } }

    [Header("COMBAT")]
    [ReadOnlyInspector] [SerializeField] private CombatRow _currentRow;
    public CombatRow CurrentRow { get { return _currentRow; } }

    [Header("PROJECTILE")]
    [SerializeField] private Projectile _projectile;
    public Projectile Projectile { get { return _projectile; } }
    [SerializeField] private Transform _projectileSpawn;
    public Transform ProjectileSPawn { get { return _projectileSpawn; } }
    [SerializeField] private AnimationCurve _projectileParabolaCurve = new AnimationCurve();
    [SerializeField] private float _heightIntensity = 1f;
    [SerializeField] private AnimationCurve _projectileLateralPositionCurve = new AnimationCurve();

    [Header("MOVING IN COMBAT")]
    [SerializeField] private AnimationCurve _moveCurve = new AnimationCurve();
    public bool IsMoving { get { return _moveCo != null; } }

    [Header("DAMAGE AND DEATH")]
    [SerializeField] private AnimationCurve _damageShakeIntensityCurve = new AnimationCurve();
    public AnimationCurve DamageShakeIntensityCurve { get { return _damageShakeIntensityCurve; } }
    [SerializeField] private AnimationCurve _deathShakeIntensityCurve = new AnimationCurve();
    public AnimationCurve DeathShakeIntensityCurve { get { return _deathShakeIntensityCurve; } }

    [SerializeField] [HideInInspector] private bool _initialized = false;
    private Coroutine _moveCo = null;
    private Coroutine _shakeCo = null;


    public void Initialize()
    {
        if (_combatData)
        {
            _health = _statisticHandler.CreateStat("Health", _combatData.BaseHealth, 0f);
            _attack = _statisticHandler.CreateStat("Attack", _combatData.BaseAttack, 0f);
            _maxHealth = _statisticHandler.CreateStat("MaxHealth", _combatData.BaseHealth, 0f);

            //Associate MaxValue of health to _maxHealth's value.
            _health.AddModifier(new StatisticModifier(_maxHealth, StatisticModifierType.MaxValue, ModifierApplicationType.Standard, _character));
        }
        else
        {
            Debug.LogWarning("Warning! " + this.gameObject.name + " has no associated combat data. Returning.");
            return;
        }

        _health.StatisticValueChanged -= OnHealthValueChanged;
        _health.StatisticValueChanged += OnHealthValueChanged;

        ExperienceHandler.Initialize();
        AbilityHandler.Initialize();

        Canvas canv = GetComponentInChildren<Canvas>();
        if (canv)
        {
            canv.worldCamera = Camera.main;
        }

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

    public FighterSetupDescriptor GetSetup()
    {
        //Creates the setup based on the additional modifiers affecting the character.

        FighterSetupDescriptor setup = new FighterSetupDescriptor();
        return setup;
    }

    public void Setup(FighterSetupDescriptor setup)
    {
        //Here, setup the character further than just initial values.
        //Apply permanent modifiers to MaxHealth, CurrentHealth, Attack, equip it with items, new abilities, etc.
        //This should use a struct as an arg which encapsulates all of this information, taking into account that if something is of its default value, then no modification is required.
        //This should be used to spawn characters on 'preview rows' (such as scouting or recruiting) so that the shown character is the same as the one acquired.
        //Then, from that 'preview row' is spawned the actual fighter at the start of combat
        //(we don't want the fighters in combat to be the fighters we actually use, so as to allow for temporary modifications to their stats to only apply for the duration of combat).
        //For temporary modifications that the character will have before combat, but only for the duration of combat (such as consumed items) we want to show these, however the end of combat function
        //will call for the deletion of such applied modifiers before visualizing the combatants on the subsequent recruitment preview row.

        //Level should increase Health and Attack as per values in UniversalData.
        if (setup.Level > 0)
        {
            for (int i = _experienceHandler.CurrentLevelInt; i < setup.Level; i++)
            {
                _experienceHandler.LevelUp();

                //StatisticModifier healthMod = new StatisticModifier(GameManager.Instance.UniversalData.LevelUpMaxHealthIncrease, StatisticModifierType.Flat, ModifierApplicationType.Permanent, Character);
                //_health.AddModifier(healthMod);

                //StatisticModifier attackMod = new StatisticModifier(GameManager.Instance.UniversalData.LevelUpAttackIncrease, StatisticModifierType.Flat, ModifierApplicationType.Permanent, Character);
                //_attack.AddModifier(attackMod);
            }
        }

        if (setup.AdditionalMaxHealthIncrease > 0)
        {
            StatisticModifier healthMod = new StatisticModifier(setup.AdditionalMaxHealthIncrease, StatisticModifierType.Flat, ModifierApplicationType.Permanent, Character);
            _health.AddModifier(healthMod);
        }

        if (setup.AdditionalAttackIncrease > 0)
        {
            StatisticModifier attackMod = new StatisticModifier(setup.AdditionalAttackIncrease, StatisticModifierType.Flat, ModifierApplicationType.Permanent, Character);
            _attack.AddModifier(attackMod);
        }

        if (setup.OverrideCurrentHealth > 0)
        {
            StatisticModifier healthMod = new StatisticModifier(setup.OverrideCurrentHealth, StatisticModifierType.Force, ModifierApplicationType.Permanent, Character);
            _health.AddModifier(healthMod);
        }

        if (setup.BaseAbilityLevel > 0)
        {
            if (_abilityHandler.Abilities.Count > 0)
            {
                for (int i = _abilityHandler.Abilities[0].AbilityLevel; i < setup.BaseAbilityLevel; i++)
                {
                    _abilityHandler.Abilities[0].LevelUpAbility();
                }
            }
        }

        if (setup.AdditionalAbilities.Count > 0)
        {
            for (int i = 0; i < setup.AdditionalAbilities.Count; i++)
            {
                if (setup.AdditionalAbilities[i].Ability != null)
                {
                    TWAbility newAbility = _abilityHandler.AddAbility(setup.AdditionalAbilities[i].Ability);
                    if (setup.AdditionalAbilities[i].AbilityLevel > 0)
                    {
                        for (int j = newAbility.AbilityLevel; j < setup.AdditionalAbilities[i].AbilityLevel; j++)
                        {
                            newAbility.LevelUpAbility();
                        }
                    }
                }
            }
        }

        if (setup.ConsumedItems.Count > 0)
        {
            for (int i = 0; i < setup.ConsumedItems.Count; i++)
            {
                if (setup.ConsumedItems[i] != null)
                {

                }
            }
        }

        if (setup.EquippedItem != null)
        {

        }
    }

    public void HealToFull(bool force = false)
    {
        Revive(force);

        StatisticModifier mod = new StatisticModifier(_maxHealth.Value, StatisticModifierType.Force, ModifierApplicationType.Permanent, this.Character);
        _health.AddModifier(mod);
    }

    public void Revive(bool force = false)
    {
        if (!_tagHandler.HasTag(CombatState.Dead)) return;

        _deathBehavior.DeathBehaviorRevive();

        if (force)
        {
            //No SFX or VFX
            _animHandler.PlayAnimation("Idle");
        }
        else
        {
            //With SFX and VFX
            _animHandler.PlayAnimation("Spawn");
        }

        StatisticModifier mod = new StatisticModifier(1f, StatisticModifierType.Force, ModifierApplicationType.Permanent, this.Character);
        _health.AddModifier(mod);
    }
    public void RemoveAllCombatModifiers()
    {
        foreach(Statistic stat in _statisticHandler.Stats)
        {
            List<StatisticModifier> standardStatMods = stat.StandardModifiers;
            if (standardStatMods.Count > 0)
            {
                for (int i = stat.StandardModifiers.Count - 1; i >= 0; i--)
                {
                    StatisticModifier mod = stat.StandardModifiers[i];
                    if (mod is TWStatisticModifier)
                    {
                        TWStatisticModifier twStatMod = (TWStatisticModifier)stat.StandardModifiers[i];
                        if (twStatMod != null)
                        {
                            if (twStatMod.Duration == TWStatisticModifierDuration.Combat)
                            {
                                stat.RemoveModifier(twStatMod);
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetCurrentCombatRow(CombatRow row)
    {
        if (!_initialized) return;

        _currentRow = row;
    }

    public void AttackTargets(List<CombatHandler> targets, float damageValue, float delay = 0f, bool useProjectile = false)
    {
        if (!_initialized) return;

        if (delay >= 0f)
        {
            if (useProjectile)
            {
                StartCoroutine(ProjectileCo(targets, damageValue, delay * GameManager.Instance.ActionTime));
            }
            else
            {
                StartCoroutine(AttackCo(targets, damageValue, delay * GameManager.Instance.ActionTime));
            }
            
        }
        else
        {
            foreach(CombatHandler target in targets)
            {
                DamageTarget(target, Attack.Value);
            }
        }
    }

    private float DamageTarget(CombatHandler target, float damageValue)
    {
        float damageDealt = 0;
        float oldTargetHealthValue = target.Health.Value;

        target.SufferDamage(damageValue, this.Character);

        damageDealt = oldTargetHealthValue - target.Health.Value;

        if (EventDispatcher.Instance)
        {
            AttackContext context = new AttackContext(this, target, damageDealt);
            EventDispatcher.Instance.Message_FighterDamagedDefender(ref context);
        }

        Debug.Log(this.gameObject.name + " attacked " + target.gameObject.name + " for " + damageDealt + " damage!");

        return damageDealt;
    }

    public void SufferDamage(float damageValue, object source)
    {
        if (!_initialized) return;

        Health.AddModifier(new StatisticModifier(damageValue * -1, StatisticModifierType.Flat, ModifierApplicationType.Permanent, source));

        if (Health.Value > 0)
        {
            _animHandler.PlayAdditiveAnimation("Hurt", 1);
            DamageShake(.2f, _damageShakeIntensityCurve);
        }
    }

    private void OnHealthValueChanged (float from, float to)
    {
        if (!_initialized) return;

        if (to <= 0 && !_tagHandler.HasTag(CombatState.Dead))
        {
            FighterDie();
            if (EventDispatcher.Instance)
            {
                FighterContext context = new FighterContext(this);
                EventDispatcher.Instance.Message_FighterHealthReachedZero(ref context);
            }
        }
    }

    private void FighterDie()
    {
        if (!_initialized) return;

        _animHandler.PlayAnimation("Die");
        DamageShake(.6f, _deathShakeIntensityCurve);

        _deathBehavior.DeathBehaviorDie();
    }

    public void DamageShake(float actionTimeMultiplier, AnimationCurve curve)
    {
        if (!_initialized) return;

        if (_shakeCo != null)
        {
            StopCoroutine(_shakeCo);
            _shakeCo = null;
        }
        _shakeCo = StartCoroutine(ShakeCo(actionTimeMultiplier, curve));
    }

    public void MoveToPosition(Vector3 toPos, float overTime = 1f)
    {
        if (!_initialized) return;

        if (_moveCo != null)
        {
            StopCoroutine(_moveCo);
            _moveCo = null;
        }
        _moveCo = StartCoroutine(MoveCo(toPos, overTime));
    }

    private IEnumerator MoveCo(Vector3 toPos, float overTime = 1f)
    {
        Vector3 fromPos = transform.position;
        float currentMoveTimer = 0f;
        Vector3 newPos = fromPos;
        float actionTime = GameManager.Instance.ActionTime*.75f*overTime;
        float alpha = 0;

        while (currentMoveTimer <= actionTime)
        {
            currentMoveTimer += Time.deltaTime;
            alpha = _moveCurve.Evaluate(currentMoveTimer / actionTime);
            newPos = Vector3.Lerp(fromPos, toPos, alpha);
            transform.position = newPos;
            yield return null;
        }

        transform.position = toPos;

        _moveCo = null;
    }

    private IEnumerator ShakeCo(float shakeDuration = 1f, AnimationCurve intensityCurve = null)
    {
        float currentShakeTimer = 0f;
        float alpha = 0;
        float baseShakeIntensity = .05f;
        AnimationCurve curve = intensityCurve;
        Vector3 origLocalPos = this.Character.Mesh.MeshPivot.localPosition;

        while(currentShakeTimer <= shakeDuration)
        {
            currentShakeTimer += Time.deltaTime;
            if (curve != null)
            {
                alpha = curve.Evaluate(currentShakeTimer / shakeDuration);
            }
            else
            {
                alpha = currentShakeTimer / shakeDuration;
            }
            this.Character.Mesh.MeshPivot.localPosition = origLocalPos + (this.Character.Mesh.MeshPivot.forward * baseShakeIntensity * alpha * Mathf.Sin(Time.time*100f));
            yield return null;
        }

        this.Character.Mesh.MeshPivot.localPosition = origLocalPos;
        _shakeCo = null;
    }

    private IEnumerator AttackCo(List<CombatHandler> targets, float damageValue, float delay)
    {
        float currentDelayTimer = 0f;

        while (currentDelayTimer <= delay)
        {
            currentDelayTimer += Time.deltaTime;
            yield return null;
        }

        foreach(CombatHandler target in targets)
        {
            DamageTarget(target, damageValue);
        }
    }

    private IEnumerator ProjectileCo(List<CombatHandler> targets, float damageValue, float delay)
    {
        float currentDelayTimer = 0f;

        List<Projectile> projectiles = new List<Projectile>();
        foreach (CombatHandler target in targets)
        {
            Projectile newProjecitle = Instantiate<Projectile>(_projectile, _projectileSpawn.position, this.transform.rotation, this.transform);
            newProjecitle.Target = target.transform;

            projectiles.Add(newProjecitle);
        }

        float alpha = 0f;
        float lateralAlpha = 0f;
        Vector3 originalPosition = _projectileSpawn.position;

        while (currentDelayTimer <= delay)
        {
            currentDelayTimer += Time.deltaTime;
            alpha = currentDelayTimer / delay;
            lateralAlpha = _projectileLateralPositionCurve.Evaluate(alpha);
            foreach(Projectile projectile in projectiles)
            {
                float height = _heightIntensity * _projectileParabolaCurve.Evaluate(alpha);
                projectile.transform.position = Vector3.Lerp(originalPosition, projectile.Target.position + projectile.Target.up, lateralAlpha) + new Vector3(0, height, 0);
            }
            yield return null;
        }

        for (int i = projectiles.Count-1; i >= 0; i--)
        {
            Destroy(projectiles[i].gameObject);
        }

        foreach (CombatHandler target in targets)
        {
            DamageTarget(target, damageValue);
        }
    }
}

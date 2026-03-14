using System;
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

    [Header("OBJECT REFERENCES")]
    [SerializeField] private AAnimationHandler _animHandler;
    public AAnimationHandler AnimationHandler { get { return _animHandler; } }

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

    [Header("MOVING IN COMBAT")]
    [SerializeField] private AnimationCurve _moveCurve = new AnimationCurve();
    public bool IsMoving { get { return _moveCo != null; } }

    [Header("DAMAGE AND DEATH")]
    [SerializeField] private AnimationCurve _damageShakeIntensityCurve = new AnimationCurve();
    [SerializeField] private AnimationCurve _deathShakeIntensityCurve = new AnimationCurve();

    private bool _initialized = false;
    private Coroutine _moveCo = null;
    private Coroutine _shakeCo = null;
    private Coroutine _attackCo = null;

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

    public void AttackTargets(List<CombatHandler> targets, float delay = 0f)
    {
        if (delay >= 0f)
        {
            if (_attackCo != null)
            {
                StopCoroutine(_attackCo);
                _attackCo = null;
            }
            _attackCo = StartCoroutine(AttackCo(targets, delay*GameManager.Instance.ActionTime));
        }
        else
        {
            foreach(CombatHandler target in targets)
            {
                DamageTarget(target);
            }
        }
    }

    public float DamageTarget(CombatHandler target)
    {
        float damageDealt = 0;
        float oldTargetHealthValue = target.Health.Value;

        target.Health.AddModifier(new StatisticModifier(_attack.Value * -1, StatisticModifierType.Flat, ModifierApplicationType.Permanent, this));

        damageDealt = oldTargetHealthValue - target.Health.Value;

        if (EventDispatcher.Instance)
        {
            AttackContext context = new AttackContext(this, target, damageDealt);
            EventDispatcher.Instance.Message_FighterDamagedDefender(ref context);
        }

        Debug.Log(this.gameObject + " attacked " + target.gameObject + " for " + damageDealt + " damage!");

        return damageDealt;
    }

    private void OnHealthValueChanged (float from, float to)
    {
        float valueDiff = from - to;
        bool damaged = valueDiff >= 0 ? true : false;

        if (damaged)
        {
            if (to <= 0)
            {
                DamageShake(.3f, _deathShakeIntensityCurve);
            }
            else
            {
                DamageShake(.2f, _damageShakeIntensityCurve);
            }
        }

        if (to <= 0 && !_tagHandler.HasTag(CombatState.Dead))
        {
            if (EventDispatcher.Instance)
            {
                FighterContext context = new FighterContext(this);
                EventDispatcher.Instance.Message_FighterHealthReachedZero(ref context);
            }
            Debug.Log(this.gameObject + " died!");
            _tagHandler.AddTag(CombatState.Dead, this);
        }
    }

    public void DamageShake(float actionTimeMultiplier, AnimationCurve curve)
    {
        if (_shakeCo != null)
        {
            StopCoroutine(_shakeCo);
            _shakeCo = null;
        }
        _shakeCo = StartCoroutine(ShakeCo(actionTimeMultiplier, curve));
    }

    public void MoveToPosition(Vector3 toPos)
    {
        if (_moveCo != null)
        {
            StopCoroutine(_moveCo);
            _moveCo = null;
        }
        _moveCo = StartCoroutine(MoveCo(toPos));
    }

    private IEnumerator MoveCo(Vector3 toPos)
    {
        Vector3 fromPos = transform.position;
        float currentMoveTimer = 0f;
        Vector3 newPos = fromPos;
        float actionTime = GameManager.Instance.ActionTime*.75f;
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

    private IEnumerator AttackCo(List<CombatHandler> targets, float delay)
    {
        float currentDelayTimer = 0f;

        while (currentDelayTimer <= delay)
        {
            currentDelayTimer += Time.deltaTime;
            yield return null;
        }
        
        _attackCo = null;
        foreach(CombatHandler target in targets)
        {
            DamageTarget(target);
        }
    }
}

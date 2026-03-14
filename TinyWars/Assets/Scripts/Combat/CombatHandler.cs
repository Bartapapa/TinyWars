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

    public void AttackTargets(List<CombatHandler> targets, float delay = 0f, bool useProjectile = false)
    {
        if (delay >= 0f)
        {
            if (_attackCo != null)
            {
                StopCoroutine(_attackCo);
                _attackCo = null;
            }

            if (useProjectile)
            {
                _attackCo = StartCoroutine(ProjectileCo(targets, delay * GameManager.Instance.ActionTime));
            }
            else
            {
                _attackCo = StartCoroutine(AttackCo(targets, delay * GameManager.Instance.ActionTime));
            }
            
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

        target.Health.AddModifier(new StatisticModifier(_attack.Value * -1, StatisticModifierType.Flat, ModifierApplicationType.Permanent, this.Character));

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
                _animHandler.PlayAnimation("Die");
                DamageShake(.6f, _deathShakeIntensityCurve);
            }
            else
            {
                _animHandler.PlayAdditiveAnimation("Hurt", 1);
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

    private IEnumerator ProjectileCo(List<CombatHandler> targets, float delay)
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
            DamageTarget(target);
        }
    }
}

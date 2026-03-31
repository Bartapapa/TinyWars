using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static GameManager _instance = null;

    public static GameManager Instance { get { return _instance; } }

    [Header("MANAGERS")]
    [SerializeField] private EventDispatcher _eventDispatcher;
    public EventDispatcher EventDispatcher { get { return _eventDispatcher; } }
    [SerializeField] private CombatManager _combatManager;
    public CombatManager CombatManager { get { return _combatManager; } }
    [SerializeField] private RecruitmentManager _recruitmentManager;
    public RecruitmentManager RecruitmentManager { get { return _recruitmentManager; } }
    [SerializeField] private PartyManager _partyManager;
    public PartyManager PartyManager { get { return _partyManager; } }

    [Header("DATA")]
    [SerializeField] private SOUniversalData _universalData;
    public SOUniversalData UniversalData { get { return _universalData; } }

    [Header("PARAMETERS")]
    [SerializeField] private float _baseActionTime = 1f;
    public float BaseActionTime { get { return _baseActionTime; } }
    [SerializeField] private float _actionRate = 1f;
    public float ActionRate { get { return _actionRate; } }
    public float ActionTime { get { return _baseActionTime / _actionRate; } }

    private void Awake()
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

        InitializeAllManagers();
    }
    private void InitializeAllManagers()
    {
        //Be careful of the order of these. Some need to be initialized before others.

        //High priority.
        _eventDispatcher.Initialize();
        _partyManager.Initialize();

        //Low priority.
        _combatManager.Initialize();
        _recruitmentManager.Initialize();
    }

    private void Start()
    {
        _combatManager.StartCombat();
    }
}

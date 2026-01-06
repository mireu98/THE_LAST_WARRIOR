using NaughtyCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager s_instance = null;
    public static GameManager Instance
    {
        get
        {
            if (s_instance == null)
                return null;
            return s_instance;
        }
    }
    #endregion
    [SerializeField] private GameObject _deathUIPrefab;
    [SerializeField] private GameObject _playerUIPrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _cameraPrefab;

    private GameObject _deathUIInstance;
    private GameObject _playerUIInstance;
    private GameObject _playerInstance;
    private GameObject _cameraInstance;

    public GameObject Camera => _cameraInstance;
    public GameObject Player => _playerInstance;
    public PlayerUIController UI { get; private set; }

    private float _playerMaxHp = 1000f;
    private float _playerCurrentHp = 1000f;
    public float PlayerMaxHp { get => _playerMaxHp; set => _playerMaxHp = value; }
    public float PlayerCurrentHp { get => _playerCurrentHp; set => _playerCurrentHp = value; }

    private float _playerMaxMp = 800f;
    private float _playerCurrentMp = 800f;
    public float PlayerMaxMp { get => _playerMaxMp; set => _playerMaxMp = value; }
    public float PlayerCurrentMp { get => _playerCurrentMp; set => _playerCurrentMp = value; }

    private float _playerMaxExp = 500f;
    private float _playerCurrentExp = 0f;
    public float PlayerMaxExp { get => _playerMaxExp; set => _playerMaxExp = value; }
    public float PlayerCurrentExp { get => _playerCurrentExp; set => _playerCurrentExp = value; }

    private float _playerDmg = 500f;
    public float PlayerDmg { get => _playerDmg; set => _playerDmg = value; }

    private int _playerLevel = 1;
    public int PlayerLevel { get => _playerLevel; set => _playerLevel = value; }

    private bool isPlayerDead = false;
    public bool IsPlayerDead { get => isPlayerDead; set => isPlayerDead = value; }

    private bool _spawning = false;

    private bool isDefense = false;
    public bool IsDefense { get => isDefense; set => isDefense = value; }

    private bool isAttacking = false;
    public bool IsAttacking { get=>isAttacking; set => isAttacking = value; }

    public bool IsCutscene = false;

    private float _healHp = 500f;
    public float HealHp { get => _healHp; set => _healHp = value; }
    private float _healMp = 500f;
    public float HealMp { get => _healMp; set => _healMp = value; }

    private float _needQMp = 200f;
    public float NeedQMp { get => _needQMp; set => _needQMp = value; }
    private float _needEMp = 350f;
    public float NeedEMp { get => _needEMp; set=> _needEMp = value; }
    private float _needRMp = 500f;
    public float NeedRMp { get => _needRMp; set => _needRMp = value; }

    public bool isGameClear = false;
    public GameObject BossPrefab;

    public GameObject GameClearUI;
    private GameObject _clearUIInstance;

    public GameObject EscUI;
    public bool isESCClicked;

    private void Awake()
    {
        #region Singleton
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        isESCClicked = false;
        EscUI.SetActive(false);
        _playerUIInstance = Instantiate(_playerUIPrefab);
        DontDestroyOnLoad(_playerUIInstance);

        UI = _playerUIInstance.GetComponent<PlayerUIController>();
        UI.Init();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnDestroy()
    {
        if (s_instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") return;
        if (_spawning) return;
        _spawning = true;

        if (_playerInstance != null)
        {
            Destroy(_playerInstance);
            _playerInstance = null;  
        }

        if (_cameraInstance != null)
        {
            Destroy(_cameraInstance);
            _cameraInstance = null;
        }

        var esc = GameObject.Find("EscUI");
        EscUI = esc;

        if (EscUI != null) EscUI.SetActive(false);

        var spawn = GameObject.Find("SpawnPoint")?.transform;
        Vector3 pos = spawn ? spawn.position : Vector3.zero;
        Quaternion rot = spawn ? spawn.rotation : Quaternion.identity;

        _playerInstance = Instantiate(_playerPrefab, pos, rot);
        _cameraInstance = Instantiate(_cameraPrefab);

        var cam = _cameraInstance.GetComponentInChildren<PlayerCamera>(true);
        if (cam != null) cam.SetTarget(_playerInstance.transform);

        IsPlayerDead = false;
        PlayerCurrentHp = PlayerMaxHp;
        PlayerCurrentMp = PlayerMaxMp;

        _spawning = false;
    }

    private void Update()
    {
        if (IsPlayerDead && _deathUIInstance == null)
            _deathUIInstance = Instantiate(_deathUIPrefab);

        if (isGameClear && _clearUIInstance == null)
            _clearUIInstance = Instantiate(GameClearUI);
    }

    public void CloseDeathUI()
    {
        if (_deathUIInstance != null)
        {
            Destroy(_deathUIInstance);
            _deathUIInstance = null;
        }
    }
}

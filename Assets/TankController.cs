using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class TankController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 60f;

    [Header("Turret & Muzzle Settings")]
    public Transform turret;
    public Transform muzzle;
    public Transform firePoint;
    public float turretTurnSpeed = 50f;
    public float muzzleTurnSpeed = 30f;
    public float minMuzzleAngle = 10f;
    public float maxMuzzleAngle = 45f;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public float projectileForce = 700f;
    public float fireCooldown = 2f;

    [Header("Audio Settings")]
    public AudioClip fireSound;
    public float engineVolume = 0.5f;
    public float fireVolume = 1f;
    public float pitchIdle = 0.9f;

    private Rigidbody rb;
    private AudioSource engineSource;
    private float muzzleAngle;
    private float fireTimer = 0f;

    private bool controlsEnabled = false;
    private bool cursorUnlocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // ✅ Use the AudioSource already on the tank
        engineSource = GetComponent<AudioSource>();
        if (engineSource != null && engineSource.clip != null)
        {
            engineSource.loop = true;
            engineSource.volume = engineVolume;
            engineSource.pitch = pitchIdle;
            engineSource.playOnAwake = false; // don't start until panels are hidden
        }
        else
        {
            Debug.LogError("❌ Tank AudioSource missing or has no clip assigned!");
        }

        muzzleAngle = Mathf.Clamp(minMuzzleAngle, minMuzzleAngle, maxMuzzleAngle);
        if (muzzle != null)
            muzzle.localRotation = Quaternion.Euler(muzzleAngle, 0f, 0f);
    }

    void Update()
    {
        if (!controlsEnabled) return;

        fireTimer -= Time.deltaTime;

        if (!cursorUnlocked)
        {
            HandleTurret();
            HandleMuzzle();
            HandleFire();
            HandleEngineAudio();
        }

        HandleCursorToggle();
    }

    void FixedUpdate()
    {
        if (!controlsEnabled) return;
        if (!cursorUnlocked)
            HandleMovementPhysics();
    }

    void HandleMovementPhysics()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        Vector3 moveVector = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVector);

        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    void HandleTurret()
    {
        if (turret == null) return;
        float turretTurn = Input.GetAxis("Mouse X") * turretTurnSpeed * Time.deltaTime;
        turret.Rotate(Vector3.up * turretTurn, Space.Self);
    }

    void HandleMuzzle()
    {
        if (muzzle == null) return;
        float input = -Input.GetAxis("Mouse Y") * muzzleTurnSpeed * Time.deltaTime;
        muzzleAngle = Mathf.Clamp(muzzleAngle + input, minMuzzleAngle, maxMuzzleAngle);
        muzzle.localRotation = Quaternion.Euler(muzzleAngle, 0f, 0f);
    }

    void HandleFire()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && fireTimer <= 0f)
        {
            Fire();
            fireTimer = fireCooldown;
        }
    }

    void Fire()
    {
        if (firePoint != null && projectilePrefab != null)
        {
            GameObject shell = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rbShell = shell.GetComponent<Rigidbody>();
            if (rbShell != null)
                rbShell.AddForce(firePoint.forward * projectileForce);

            Destroy(shell, 5f);

            if (fireSound != null)
                AudioSource.PlayClipAtPoint(fireSound, firePoint.position, fireVolume);
        }
    }

    void HandleEngineAudio()
    {
        if (engineSource == null || engineSource.clip == null) return;

        // ✅ Engine plays only if no UI panel is active
        UIManager ui = FindObjectOfType<UIManager>();
        bool panelActive = false;
        if (ui != null)
        {
            if ((ui.mainMenuPanel != null && ui.mainMenuPanel.activeSelf) ||
                (ui.gameOverPanel != null && ui.gameOverPanel.activeSelf) ||
                (ui.missionCompletePanel != null && ui.missionCompletePanel.activeSelf))
            {
                panelActive = true;
            }
        }

        if (!panelActive)
        {
            if (!engineSource.isPlaying)
            {
                Debug.Log("▶ Engine START: " + engineSource.clip.name);
                engineSource.Play();
            }
        }
        else
        {
            if (engineSource.isPlaying)
            {
                Debug.Log("⏹ Engine STOP");
                engineSource.Stop();
            }
        }
    }

    void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cursorUnlocked)
            {
                LockCursor();
                cursorUnlocked = false;
            }
            else
            {
                UnlockCursor();
                cursorUnlocked = true;
            }
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ✅ for UIManager compatibility
    public void EnableControl()
    {
        controlsEnabled = true;
        cursorUnlocked = false;
        LockCursor();
    }

    public void DisableControl()
    {
        controlsEnabled = false;
        cursorUnlocked = true;
        UnlockCursor();

        if (engineSource != null && engineSource.isPlaying)
            engineSource.Stop();
    }

    void OnApplicationQuit()
    {
        SaveSystem saveSystem = FindObjectOfType<SaveSystem>();
        if (saveSystem != null) saveSystem.SaveGame();
    }

    public void TeleportTo(Vector3 position)
    {
        if (rb != null)
        {
            rb.position = position;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            transform.position = position;
        }
    }
}

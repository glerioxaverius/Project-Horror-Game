using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Light flashlight;
    [SerializeField] private Animator handsAnimator;

    [Header("UI References")]
    [SerializeField] private Image crosshairDot;
    [SerializeField] private Image crosshairInteract;
    [SerializeField] private Image damageVignette;
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private Text healthText;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed            = 3.0f;
    [SerializeField] private float runSpeed             = 6.0f;
    [SerializeField] private float crouchSpeed          = 1.5f;
    [SerializeField] private float gravity              = -19.62f;

    [Header("Crouch Settings")]
    [SerializeField] private float standingHeight       = 1.0f;
    [SerializeField] private float crouchHeight         = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 8f;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina           = 100f;
    [SerializeField] private float staminaDrainRate     = 20f;
    [SerializeField] private float staminaRegenRate     = 10f;
    [SerializeField] private float staminaRegenDelay    = 1.5f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity     = 2.0f;
    [SerializeField] private float maxLookAngle         = 80f;

    [Header("Flashlight Settings")]
    [SerializeField] private float flashlightRange      = 12f;
    [SerializeField] private float flashlightAngle      = 45f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange     = 2.5f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth            = 100f;
    [SerializeField] private float vignetteDecayRate    = 0.8f;

    [SerializeField] private InventoryUI inventoryUI;


    [Header("Melee Settings")]
    [SerializeField] private float axeRange             = 1.8f;
    [SerializeField] private float axeDamage            = 50f;
    [SerializeField] private float axeStaminaCost       = 25f;
    [SerializeField] private float axeCooldown          = 0.8f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Ranged Settings")]
    [SerializeField] private float gunRange             = 30f;
    [SerializeField] private float gunDamage            = 75f;
    [SerializeField] private int   maxAmmo              = 6;
    [SerializeField] private float gunCooldown          = 0.6f;
    [SerializeField] private GameObject muzzleFlashFX;

    [Header("Audio - Footsteps")]
    [SerializeField] private AudioClip[] footstepWalk;
    [SerializeField] private AudioClip[] footstepRun;
    [SerializeField] private AudioClip[] footstepCrouch;
    [SerializeField] private float footstepIntervalWalk   = 0.50f;
    [SerializeField] private float footstepIntervalRun    = 0.30f;
    [SerializeField] private float footstepIntervalCrouch = 0.75f;

    [Header("Audio - Combat")]
    [SerializeField] private AudioClip axeSwingSound;
    [SerializeField] private AudioClip axeHitSound;
    [SerializeField] private AudioClip gunShotSound;
    [SerializeField] private AudioClip gunEmptySound;
    [SerializeField] private AudioClip playerHurtSound;

    [Header("Stealth Settings")]
    [SerializeField] private float noiseCrouchMultiplier = 0.2f;
    [SerializeField] private float noiseWalkMultiplier   = 0.5f;
    [SerializeField] private float noiseRunMultiplier    = 1.0f;
    [SerializeField] private float noiseDecayRate        = 2.0f;

    public enum WeaponType { None, Axe, Firearm }

    [Header("Weapon State")]
    [SerializeField] private WeaponType currentWeapon   = WeaponType.None;

    private CharacterController _cc;
    private AudioSource         _audio;

    private Vector3 _velocity;
    private float   _currentSpeed;
    private bool    _isGrounded;
    private bool    _isCrouching;
    private float   _targetCrouchHeight;
    private float   _xRotation;
    private float   _currentStamina;
    private float   _staminaRegenTimer;
    private bool    _isRunning;
    private float   _currentHealth;
    private float   _vignetteAlpha;
    private bool    _flashlightOn = true;
    private float   _axeTimer;
    private float   _gunTimer;
    private int     _currentAmmo;
    private float   _footstepTimer;
    private float   _noiseLevel;

    public float      NoiseLevel    => _noiseLevel;
    public float      CurrentHealth => _currentHealth;
    public float      CurrentStamina => _currentStamina;
    public int        CurrentAmmo   => _currentAmmo;
    public bool       IsCrouching   => _isCrouching;
    public WeaponType ActiveWeapon  => currentWeapon;


    private void Awake()
    {
        _cc    = GetComponent<CharacterController>();
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _currentHealth      = maxHealth;
        _currentStamina     = maxStamina;
        _currentAmmo        = maxAmmo;
        _targetCrouchHeight = standingHeight;
        _cc.height          = standingHeight;

        if (flashlight != null)
        {
            flashlight.range     = flashlightRange;
            flashlight.spotAngle = flashlightAngle;
            flashlight.enabled   = _flashlightOn;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        SetCrosshair(false);
    }

    private void Update()
    {

        if (inventoryUI == null || !inventoryUI.IsOpen)
        {
            HandleLook();
        }
        HandleGroundCheck();
        HandleLook();
        HandleCrouch();
        HandleMovement();
        HandleStamina();
        HandleFlashlight();
        HandleInteraction();
        HandleCombat();
        HandleNoiseDecay();
        UpdateDamageVignette();
        UpdateUI();
        HandleInventoryInput();
    }

    private void HandleGroundCheck()
    {
        _isGrounded = _cc.isGrounded;
        if (_isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation  = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _isCrouching        = !_isCrouching;
            _targetCrouchHeight = _isCrouching ? crouchHeight : standingHeight;
        }

        _cc.height = Mathf.Lerp(_cc.height, _targetCrouchHeight,
                                 Time.deltaTime * crouchTransitionSpeed);

        Vector3 camLocal = playerCamera.transform.localPosition;
        float   targetY  = _isCrouching ? crouchHeight * 0.8f : standingHeight * 0.9f;
        camLocal.y       = Mathf.Lerp(camLocal.y, targetY,
                                       Time.deltaTime * crouchTransitionSpeed);
        playerCamera.transform.localPosition = camLocal;
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool wantsRun = Input.GetKey(KeyCode.LeftShift)
                        && !_isCrouching
                        && _currentStamina > 0f;

        _isRunning    = wantsRun && (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);
        _currentSpeed = _isCrouching ? crouchSpeed
                       : _isRunning   ? runSpeed
                                      : walkSpeed;

        Vector3 move = transform.right * h + transform.forward * v;
        if (move.magnitude > 1f) move.Normalize();

        _cc.Move(move * _currentSpeed * Time.deltaTime);

        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);

        bool isMoving = move.magnitude > 0.1f && _isGrounded;
        if (isMoving)
        {
            float interval = _isCrouching ? footstepIntervalCrouch
                           : _isRunning   ? footstepIntervalRun
                                          : footstepIntervalWalk;
            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer <= 0f)
            {
                PlayFootstep();
                UpdateNoiseLevel();
                _footstepTimer = interval;
            }
        }
    }

    private void PlayFootstep()
    {
        AudioClip[] clips = _isCrouching ? footstepCrouch
                          : _isRunning   ? footstepRun
                                         : footstepWalk;
        if (clips == null || clips.Length == 0) return;

        float vol = _isCrouching ? 0.2f : _isRunning ? 1.0f : 0.6f;
        _audio.PlayOneShot(clips[Random.Range(0, clips.Length)], vol);
    }

    private void HandleStamina()
    {
        if (_isRunning)
        {
            _currentStamina   -= staminaDrainRate * Time.deltaTime;
            _currentStamina    = Mathf.Max(_currentStamina, 0f);
            _staminaRegenTimer = staminaRegenDelay;
        }
        else
        {
            if (_staminaRegenTimer > 0f)
                _staminaRegenTimer -= Time.deltaTime;
            else
            {
                _currentStamina += staminaRegenRate * Time.deltaTime;
                _currentStamina  = Mathf.Min(_currentStamina, maxStamina);
            }
        }
    }

    private void HandleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            _flashlightOn      = !_flashlightOn;
            flashlight.enabled = _flashlightOn;
        }
    }

private ItemPickup _currentTargetItem;
private void HandleInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                SetCrosshair(true);

                ItemPickup pickup = hit.collider.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    if (_currentTargetItem != null && _currentTargetItem != pickup)
                    {
                        _currentTargetItem.ShowUI(false);
                    }

                    _currentTargetItem = pickup;
                    _currentTargetItem.ShowUI(true); 
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (_currentTargetItem != null) _currentTargetItem = null;
                    
                    interactable.Interact(this.gameObject);
                    return; 
                }
                
                return;
            }
        }

        if (_currentTargetItem != null)
        {
            _currentTargetItem.ShowUI(false);
            _currentTargetItem = null;
        }

        SetCrosshair(false);
    }

private void HandleInventoryInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.LogWarning("[TEST 1] Tombol TAB terdeteksi ditekan di keyboard!");

            if (inventoryUI != null)
            {
                Debug.LogWarning($"[TEST 2] Referensi InventoryUI aman. Status sebelum pencet: IsOpen = {inventoryUI.IsOpen}");
                
                inventoryUI.ToggleInventory();
                
                Debug.LogWarning($"[TEST 3] Fungsi Toggle sukses dipanggil. Status sesudah pencet: IsOpen = {inventoryUI.IsOpen}");
            }
            else
            {
                Debug.LogError("[ERROR] Tombol TAB ditekan, tapi script PlayerController kehilangan arah karena variabel 'inventoryUI' KOSONG (Null)!");
            }
        }
    }

    private void HandleCombat()
    {
        _axeTimer -= Time.deltaTime;
        _gunTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeapon == WeaponType.Axe)    TryMeleeAttack();
            if (currentWeapon == WeaponType.Firearm) TryShoot();
        }
    }

    private void TryMeleeAttack()
    {
        if (_axeTimer > 0f || _currentStamina < axeStaminaCost) return;

        _axeTimer        = axeCooldown;
        _currentStamina -= axeStaminaCost;

        handsAnimator?.SetTrigger("Swing");
        _audio.PlayOneShot(axeSwingSound);

        Ray ray = new Ray(playerCamera.transform.position,
                          playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, axeRange, enemyLayer))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            target?.TakeDamage(axeDamage);
            _audio.PlayOneShot(axeHitSound);
        }
    }

    private void TryShoot()
    {
        if (_gunTimer > 0f) return;
        _gunTimer = gunCooldown;

        if (_currentAmmo <= 0)
        {
            _audio.PlayOneShot(gunEmptySound);
            return;
        }

        _currentAmmo--;
        handsAnimator?.SetTrigger("Shoot");
        _audio.PlayOneShot(gunShotSound);
        _noiseLevel = 1.0f;

        if (muzzleFlashFX != null) StartCoroutine(ShowMuzzleFlash());

        Ray ray = new Ray(playerCamera.transform.position,
                          playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, gunRange, enemyLayer))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            target?.TakeDamage(gunDamage);
        }
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlashFX.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlashFX.SetActive(false);
    }

    private void UpdateNoiseLevel()
    {
        float level = _isCrouching ? noiseCrouchMultiplier
                    : _isRunning   ? noiseRunMultiplier
                                   : noiseWalkMultiplier;
        _noiseLevel = Mathf.Clamp01(Mathf.Max(_noiseLevel, level));
    }

    private void HandleNoiseDecay()
    {
        _noiseLevel = Mathf.MoveTowards(_noiseLevel, 0f,
                                         noiseDecayRate * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        _currentHealth  = Mathf.Max(_currentHealth, 0f);
        _vignetteAlpha  = 1.0f;

        if (playerHurtSound != null) _audio.PlayOneShot(playerHurtSound);
        if (_currentHealth <= 0f) OnPlayerDeath();
    }
    
        public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth); 
        Debug.Log("Darah bertambah!");
    }


    private void UpdateDamageVignette()
    {
        if (damageVignette == null) return;
        _vignetteAlpha = Mathf.MoveTowards(_vignetteAlpha, 0f,
                                            vignetteDecayRate * Time.deltaTime);
        Color c = damageVignette.color;
        c.a = _vignetteAlpha;
        damageVignette.color = c;
    }

    private void OnPlayerDeath()
    {
        Debug.Log("[PlayerController] Player meninggal. Game Over.");
        enabled = false;
    }

    private void SetCrosshair(bool showInteract)
    {
        if (crosshairDot      != null) crosshairDot.enabled      = !showInteract;
        if (crosshairInteract != null) crosshairInteract.enabled  =  showInteract;
    }

    private void UpdateUI()
    {
        if (staminaBarFill != null)
            staminaBarFill.fillAmount = _currentStamina / maxStamina;

        if (healthText != null)
        {
            healthText.text = "HP: " + Mathf.RoundToInt(_currentHealth).ToString();
        }
    }

    public void EquipAxe()
    {
        currentWeapon = WeaponType.Axe;
        Debug.Log("[PlayerController] Kapak equipped.");
    }

    public void EquipFirearm(int initialAmmo = 6)
    {
        currentWeapon = WeaponType.Firearm;
        _currentAmmo  = initialAmmo;
        Debug.Log($"[PlayerController] Firearm equipped. Ammo: {_currentAmmo}");
    }

    public void AddAmmo(int amount)
    {
        _currentAmmo = Mathf.Min(_currentAmmo + amount, maxAmmo);
        Debug.Log($"[PlayerController] Ammo: {_currentAmmo}/{maxAmmo}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        if (playerCamera == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerCamera.transform.position,
                       playerCamera.transform.forward * axeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.transform.position,
                       playerCamera.transform.forward * gunRange);
    }

}

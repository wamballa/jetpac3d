using System.Collections.Generic;
using Unity.DebugTool;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Health), typeof(Actor))]
public class EnemyController : MonoBehaviour
{
    [System.Serializable]
    public struct RendererIndexData
    {
        public Renderer Renderer;
        public int MaterialIndex;

        public RendererIndexData(Renderer renderer, int index)
        {
            Renderer = renderer;
            MaterialIndex = index;
        }
    }

    [Header("Parameters")]
    [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
    public float SelfDestructYHeight = -20f;

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    public float PathReachingRadius = 2f;

    [Tooltip("The speed at which the enemy rotates")]
    public float OrientationSpeed = 10f;

    [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
    public float DeathDuration = 0f;


    [Header("Weapons Parameters")]
    [Tooltip("Allow weapon swapping for this enemy")]
    public bool SwapToNextWeapon = false;

    [Tooltip("Time delay between a weapon swap and the next attack")]
    public float DelayAfterWeaponSwap = 0f;

    [Header("Eye color")]
    [Tooltip("Material for the eye color")]
    public Material EyeColorMaterial;

    [Tooltip("The default color of the bot's eye")]
    [ColorUsageAttribute(true, true)]
    public Color DefaultEyeColor;

    [Tooltip("The attack color of the bot's eye")]
    [ColorUsageAttribute(true, true)]
    public Color AttackEyeColor;

    [Header("Flash on hit")]
    [Tooltip("The material used for the body of the hoverbot")]
    public Material BodyMaterial;

    [Tooltip("The gradient representing the color of the flash on hit")]
    [GradientUsageAttribute(true)]
    public Gradient OnHitBodyGradient;

    [Tooltip("The duration of the flash on hit")]
    public float FlashOnHitDuration = 0.5f;

    [Header("Sounds")]
    [Tooltip("Sound played when recieving damages")]
    public AudioClip DamageTick;
    public AudioClip explosionSFX;
    public AudioSource[] explosionSources;

    [Header("Audio source")]
    public AudioSource audioSource;

    [Header("VFX")]
    [Tooltip("The VFX prefab spawned when the enemy dies")]
    public GameObject DeathVfx;

    [Tooltip("The point at which the death VFX is spawned")]
    public Transform DeathVfxSpawnPoint;

    [Header("Loot")]
    [Tooltip("The object this enemy can drop when dying")]
    public GameObject LootPrefab;

    [Tooltip("The chance the object has to drop")]
    [Range(0, 1)]
    public float DropRate = 1f;

    [Header("Debug Display")]
    [Tooltip("Color of the sphere gizmo representing the path reaching range")]
    public Color PathReachingRangeColor = Color.yellow;

    [Tooltip("Color of the sphere gizmo representing the attack range")]
    public Color AttackRangeColor = Color.red;

    [Tooltip("Color of the sphere gizmo representing the detection range")]
    public Color DetectionRangeColor = Color.blue;

    public UnityAction onAttack;
    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;
    public UnityAction onDamaged;

    List<RendererIndexData> m_BodyRenderers = new List<RendererIndexData>();
    MaterialPropertyBlock m_BodyFlashMaterialPropertyBlock;
    float m_LastTimeDamaged = float.NegativeInfinity;

    RendererIndexData m_EyeRendererData;
    MaterialPropertyBlock m_EyeColorMaterialPropertyBlock;

    //public PatrolPath PatrolPath { get; set; }
    //public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
    //public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
    //public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
    //public bool HadKnownTarget => DetectionModule.HadKnownTarget;
    //public NavMeshAgent NavMeshAgent { get; private set; }
    //public DetectionModule DetectionModule { get; private set; }

    int m_PathDestinationNodeIndex;
    EnemyManager m_EnemyManager;
    ActorsManager m_ActorsManager;
    Health m_Health;
    Actor m_Actor;
    Collider[] m_SelfColliders;
    //GameFlowManager m_GameFlowManager;
    bool m_WasDamagedThisFrame;
    float m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
    int m_CurrentWeaponIndex;
    WeaponController m_CurrentWeapon;
    WeaponController[] m_Weapons;
    //NavigationModule m_NavigationModule;

    void Start()
    {
        m_EnemyManager = FindObjectOfType<EnemyManager>();
        DebugUtility.HandleErrorIfNullFindObject<EnemyManager, EnemyController>(m_EnemyManager, this);

        m_ActorsManager = FindObjectOfType<ActorsManager>();
        DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyController>(m_ActorsManager, this);

        //m_EnemyManager.RegisterEnemy(this);

        m_Health = GetComponent<Health>();
        DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyController>(m_Health, this, gameObject);

        m_Actor = GetComponent<Actor>();
        DebugUtility.HandleErrorIfNullGetComponent<Actor, EnemyController>(m_Actor, this, gameObject);

        //NavMeshAgent = GetComponent<NavMeshAgent>();
        //m_SelfColliders = GetComponentsInChildren<Collider>();

        //m_GameFlowManager = FindObjectOfType<GameFlowManager>();
        //DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, EnemyController>(m_GameFlowManager, this);

        // Subscribe to damage & death actions
        m_Health.OnDie += OnDie;

        m_Health.OnDamaged += OnDamaged;


    }

    void Update()
    {
        EnsureIsWithinLevelBounds();

        m_WasDamagedThisFrame = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            //print("hit ground");

            OnDie();

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            print("hit ground with OnCOllissionEnter");

            OnDie();
        }
    }

    void EnsureIsWithinLevelBounds()
    {
        // at every frame, this tests for conditions to kill the enemy
        if (transform.position.y < SelfDestructYHeight)
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDamaged(float damage, GameObject damageSource)
    {
        // test if the damage source is the player
        if (damageSource && !damageSource.GetComponent<EnemyController>())
        {
            //// pursue the player
            //DetectionModule.OnDamaged(damageSource);

            onDamaged?.Invoke();
            m_LastTimeDamaged = Time.time;

            // play the damage tick sound
            if (DamageTick && !m_WasDamagedThisFrame)
                AudioUtility.CreateSFX(DamageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

            m_WasDamagedThisFrame = true;
        }
    }

    void OnDie()
    {
        //if (explosionSFX)
        //{
        //    // play sound
        //    AudioUtility.CreateSFX(
        //        explosionSFX,
        //        transform.position,
        //        AudioUtility.AudioGroups.Impact,
        //        0f);
        //}


        // spawn a particle system when dying
        var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);

        //AudioUtility.CreateSFX(explosionSFX, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

        //print(audioSource.name );
        //audioSource.PlayOneShot(explosionSFX, 0.6f);

        //explosionSources[Random.Range(0, explosionSources.Length - 1)].Play();

        //AudioUtility.CreateSFX(explosionSFX, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

        //AudioUtility.CreateSFX(explosionSFX, transform.position, AudioUtility.AudioGroups.EnemyAttack, 0f);


        Destroy(vfx, 5f);

        // tells the game flow manager to handle the enemy destuction
        m_EnemyManager.UnregisterEnemy(this);

        // loot an object
        if (TryDropItem())
        {
            Instantiate(LootPrefab, transform.position, Quaternion.identity);
        }



        // this will call the OnDestroy function
        Destroy(gameObject, DeathDuration);
    }

    //void OnDrawGizmosSelected()
    //{
    //    // Path reaching range
    //    Gizmos.color = PathReachingRangeColor;
    //    Gizmos.DrawWireSphere(transform.position, PathReachingRadius);

    //    if (DetectionModule != null)
    //    {
    //        // Detection range
    //        Gizmos.color = DetectionRangeColor;
    //        Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

    //        // Attack range
    //        Gizmos.color = AttackRangeColor;
    //        Gizmos.DrawWireSphere(transform.position, DetectionModule.AttackRange);
    //    }
    //}

    public void OrientWeaponsTowards(Vector3 lookPosition)
    {
        for (int i = 0; i < m_Weapons.Length; i++)
        {
            // orient weapon towards player
            Vector3 weaponForward = (lookPosition - m_Weapons[i].WeaponRoot.transform.position).normalized;
            m_Weapons[i].transform.forward = weaponForward;
        }
    }

    public bool TryAtack(Vector3 enemyPosition)
    {
        //if (m_GameFlowManager.GameIsEnding)
        //    return false;

        //OrientWeaponsTowards(enemyPosition);

        //if ((m_LastTimeWeaponSwapped + DelayAfterWeaponSwap) >= Time.time)
        //    return false;

        //// Shoot the weapon
        //bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

        //if (didFire && onAttack != null)
        //{
        //    onAttack.Invoke();

        //    if (SwapToNextWeapon && m_Weapons.Length > 1)
        //    {
        //        int nextWeaponIndex = (m_CurrentWeaponIndex + 1) % m_Weapons.Length;
        //        SetCurrentWeapon(nextWeaponIndex);
        //    }
        //}

        return false;
    }

    public bool TryDropItem()
    {
        if (DropRate == 0 || LootPrefab == null)
            return false;
        else if (DropRate == 1)
            return true;
        else
            return (Random.value <= DropRate);
    }

    //void FindAndInitializeAllWeapons()
    //{
    //    // Check if we already found and initialized the weapons
    //    if (m_Weapons == null)
    //    {
    //        m_Weapons = GetComponentsInChildren<WeaponController>();
    //        DebugUtility.HandleErrorIfNoComponentFound<WeaponController, EnemyController>(m_Weapons.Length, this,
    //            gameObject);

    //        for (int i = 0; i < m_Weapons.Length; i++)
    //        {
    //            m_Weapons[i].Owner = gameObject;
    //        }
    //    }
    //}

    //public WeaponController GetCurrentWeapon()
    //{
    //    FindAndInitializeAllWeapons();
    //    // Check if no weapon is currently selected
    //    if (m_CurrentWeapon == null)
    //    {
    //        // Set the first weapon of the weapons list as the current weapon
    //        SetCurrentWeapon(0);
    //    }

    //    DebugUtility.HandleErrorIfNullGetComponent<WeaponController, EnemyController>(m_CurrentWeapon, this,
    //        gameObject);

    //    return m_CurrentWeapon;
    //}

    //void SetCurrentWeapon(int index)
    //{
    //    m_CurrentWeaponIndex = index;
    //    m_CurrentWeapon = m_Weapons[m_CurrentWeaponIndex];
    //    if (SwapToNextWeapon)
    //    {
    //        m_LastTimeWeaponSwapped = Time.time;
    //    }
    //    else
    //    {
    //        m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
    //    }
    //}
}

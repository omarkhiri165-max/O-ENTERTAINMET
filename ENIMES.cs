using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private const int V = 100;
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public int health = 100;
    public int damage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Audio Clips")]
    public AudioClip spawnClip;
    public AudioClip[] alertClips;
    public AudioClip attackClip;
    public AudioClip damageClip;
    public AudioClip deathClip;

    private AudioSource audioSource;
    private float lastAttackTime;
    private bool alerted = false;
    private bool isDead = false;
    public Transform target;
    public ZombieSpawnerTMP spawner;
    public int PLERDAM = 100;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;

        audioSource = GetComponent<AudioSource>();
        PlaySound(spawnClip);

        // ✨ يلقط اللاعب أوتوماتيكياً
        GameObject playerObj = GameObject.Find("PLAYER");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > attackRange)
        {
            // ✨ يتبع اللاعب
            agent.isStopped = false;
            agent.SetDestination(player.position);

            float speed = agent.velocity.magnitude;
            animator.SetBool("walk", speed > 0.1f);

            if (distance < 8f && !alerted)
            {
                PlayRandomAlert();
                alerted = true;
            }
        }
        else
        {
            // ✨ يهاجم اللاعب
            agent.isStopped = true;
            animator.SetBool("walk", false);

            // يواجه اللاعب
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            // ✨ الهجوم كل فترة
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        if (HasParameter(animator, "shoot"))
        {
            animator.ResetTrigger("shoot");
            animator.SetTrigger("shoot");
        }

        PlaySound(attackClip);

        // ✨ نستعمل سكربت PLAER مباشرة
        PLAER playerScript = player.GetComponent<PLAER>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);

            // ✨ كل مرة يضرب اللاعب بـ 100 Damage → يزيد 1 Score
            if (damage == 100 && spawner != null)
            {
                spawner.AddScore(1);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        PlaySound(damageClip);

        if (HasParameter(animator, "F"))
        {
            animator.ResetTrigger("F");
            animator.SetTrigger("F");
        }

        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        PlaySound(deathClip);

        if (HasParameter(animator, "DEATH"))
        {
            animator.ResetTrigger("DEATH");
            animator.SetTrigger("DEATH");
        }

        agent.enabled = false;
        this.enabled = true;

        Destroy(gameObject, 0f); // ✨ يموت بعد 2 ثانية
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    void PlayRandomAlert()
    {
        if (alertClips.Length > 0 && audioSource != null)
        {
            int index = Random.Range(0, alertClips.Length);
            audioSource.PlayOneShot(alertClips[index]);
        }
    }

    bool HasParameter(Animator anim, string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
            if (param.name == paramName) return true;
        return false;
    }
}
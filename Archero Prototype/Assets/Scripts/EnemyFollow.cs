using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float timer = 5;
    private float bulletTime;

    public GameObject enemyBullet;
    public GameManager gameManager;
    public Transform spawnPoint;
    public float enemySpeed;

    public NavMeshAgent enemy;
    public Transform target;
    private PlayerMovementManager playerMovementManager;

    public HealthBar healthBar;
    public int maxHealth = 60;
    public int currentHealth;

    public UIManager manager;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(target.position);
        ShootAtPlayer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) // Assuming "Bullet" is the tag for player bullets
        {
            TakeDamage(20);
            manager.IncreaseCoins(10);
            
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            gameManager.DecreaseEnemyCount();
            Destroy(gameObject);    
        }
    }

    void ShootAtPlayer()
    {
        bulletTime -= Time.deltaTime;

        if (bulletTime > 0 ) return;

        bulletTime = timer;

        GameObject bulletObj = Instantiate(enemyBullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        Rigidbody bulletRig = bulletObj.GetComponent<Rigidbody>();
        bulletRig.AddForce(bulletRig.transform.forward * enemySpeed);
        Destroy(bulletObj, 5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementManager : MonoBehaviour
{
    public VariableJoystick joystick;
    public CharacterController controller;
    public HealthBar healthBar;
    public int maxHealth = 60;
    public int currentHealth;
    
    public float moveSpeed = 5f;
    public float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    
    public Canvas inputCanvas;
    
    public bool isJoystick;

    public GameObject playerBullet;
    public Transform bulletSpawnPoint;
    [SerializeField] private float shootTimer = 5;
    private bool isMoving = false;

    public float shootInterval = 0.5f;  // Adjust the shooting interval as needed



    private void Start()
    {
        EnableJoystickInput();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        

    }
    public void EnableJoystickInput()
    {
        isJoystick = true;
        inputCanvas.gameObject.SetActive(true);
    }

    private void Update()
    {


        if (!isMoving)
        {
            Transform nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                FaceTarget(nearestEnemy);
                ShootAtEnemy(nearestEnemy);
            }
        }


        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(100);
            GameOver();
        }

        
        
        if (collision.gameObject.CompareTag("Enemy Bullet"))
        {
            TakeDamage(20);
        }

    }

    public void GameOver()
    {
        Destroy(gameObject, 1f);
        LoadMenu();
    }

    public void GameWin()
    {
        Destroy(gameObject);
        LoadMenu();
    }

    public void DestroyPlayer()
    {
        gameObject.SetActive(false);
        Debug.Log("Hit enemy");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void FixedUpdate()
    {
        if (isJoystick)
        {
            var movementDirection = new Vector3(joystick.Direction.x, 0.0f, joystick.Direction.y).normalized;

            if (movementDirection.magnitude >= 0.1f)
            {
                isMoving = true;
                float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                controller.Move(movementDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
            }
        }

    }

    void TakeDamage(int damage)
    {
        if (currentHealth >= 0)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
        }
        else return;
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < minDistance)
            {
                minDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }
        return nearestEnemy;
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void ShootAtEnemy(Transform target)
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer > 0) return;

        shootTimer = shootInterval;

        GameObject bullet = Instantiate(playerBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        Vector3 direction = (target.position - bulletSpawnPoint.position).normalized;
        bulletRb.AddForce(direction * 1000);
        Destroy(bullet, 2f);
    }


}

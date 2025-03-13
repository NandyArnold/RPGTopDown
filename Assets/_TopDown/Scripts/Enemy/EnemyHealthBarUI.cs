using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    public Slider healthSlider;

    private EnemyCombatSystem enemyCombatSystem;
    private Transform target;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        enemyCombatSystem = GetComponentInParent<EnemyCombatSystem>();

        if (enemyCombatSystem != null)
        {
            enemyCombatSystem.onHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(enemyCombatSystem.currentHealth, enemyCombatSystem.maxHealth);
            target = enemyCombatSystem.transform;
        }
        else
        {
            Debug.LogError("EnemyHealthBarUI: EnemyCombatSystem not found on parent!");
        }
        if (target != null)
        {

            FaceCamera();

        }


    }

    private void FaceCamera()
    {
        if (mainCamera)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }


    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider == null)
        {
            return;
        }
        if (healthSlider.maxValue != maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }
        healthSlider.value = currentHealth;
    }



    private void OnBecameVisible()
    {
        gameObject.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (enemyCombatSystem != null)
        {
            enemyCombatSystem.onHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }

}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameplayHUD : MonoBehaviour
{
    [Header("Player Health Bar")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Player Mana Bar")]
    public Slider manaSlider;
    public TextMeshProUGUI manaText;

    private PlayerCombatSystem playerCombatSystem;




    private void Start()
    {
        playerCombatSystem = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCombatSystem>();

        if(playerCombatSystem != null)
        {
            playerCombatSystem.onHealthChanged.AddListener(UpdateHealthUI);
            playerCombatSystem.onManaChanged.AddListener(UpdateManaUI);

            UpdateHealthUI(playerCombatSystem.currentHealth, playerCombatSystem.maxHealth);
            UpdateManaUI(playerCombatSystem.currentMana, playerCombatSystem.maxMana);
        }
    }


    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider == null || healthText == null)
            return;
        if(healthSlider.maxValue != maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }

        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }


    private void UpdateManaUI(int currentMana, int maxMana)
    {
        if(manaSlider == null || manaText == null)
        {
            return;
        }
        if(manaSlider.maxValue != maxMana)
        {
            manaSlider.maxValue = maxMana;
        }

        manaSlider.value = currentMana;
        manaText.text = currentMana.ToString();
        

    }

    private void OnDestroy()
    {
        if (playerCombatSystem != null)
        {
            playerCombatSystem.onHealthChanged.RemoveListener(UpdateHealthUI);
            playerCombatSystem.onManaChanged.RemoveListener(UpdateManaUI);
        }

                }

    // Update is called once per frame
    void Update()
    {
        
    }
}

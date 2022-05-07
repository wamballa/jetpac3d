using UnityEngine.UI;
using UnityEngine;
using Unity.DebugTool;

public class PlayerHealthBar : MonoBehaviour
{
    [Tooltip("Image component dispplaying current health")]
    public Image HealthFillImage;

    Health playerHealth;

    void Start()
    {
        PlayerController playerCharacterController =
            GameObject.FindObjectOfType<PlayerController>();
        DebugUtility.HandleErrorIfNullFindObject<PlayerController, PlayerHealthBar>(
            playerCharacterController, this);

        playerHealth = playerCharacterController.GetComponent<Health>();
        DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerHealthBar>(playerHealth, this,
            playerCharacterController.gameObject);
    }

    void Update()
    {
        // update health bar value
        HealthFillImage.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
    }
}

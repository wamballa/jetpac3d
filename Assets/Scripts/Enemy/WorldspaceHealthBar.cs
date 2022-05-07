using UnityEngine;
using UnityEngine.UI;


public class WorldspaceHealthBar : MonoBehaviour
{
    [Tooltip("Health component to track")] public Health Health;

    [Tooltip("Image component displaying health left")]
    public Image HealthBarImage;

    [Tooltip("The floating healthbar pivot transform")]
    public Transform HealthBarPivot;

    [Tooltip("Whether the health bar is visible when at full health or not")]
    public bool HideFullHealthBar = true;

    public Transform camera;

    void Update()
    {
        // update health bar value


        HealthBarImage.fillAmount = Health.CurrentHealth / Health.MaxHealth;

        // rotate health bar to face the camera/player
        //HealthBarPivot.LookAt(Camera.main.transform.position);
        HealthBarPivot.LookAt(camera.position);

        // hide health bar if needed
        if (HideFullHealthBar)
            HealthBarPivot.gameObject.SetActive(HealthBarImage.fillAmount != 1);
    }
}

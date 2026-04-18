using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float fillWidth = 360;

    private float previousHealth = 1;
    private float targetHealth = 1;
    private float healthLerp = -1;
    [SerializeField] float healthLerpSpeed = 4;

    private RectMask2D fillCrop;
    private PlayerController player;
    [SerializeField] private Image flash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fillCrop = GetComponentInChildren<RectMask2D>();
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID)[0];
    }

    // Update is called once per frame
    void Update()
    {
        float health = player.getHealth();
        if (health != targetHealth)
        {
            if (healthLerp != -1) previousHealth = GetHealthLerped();
            targetHealth = health;
            healthLerp = 0;
        }

        if (healthLerp != -1)
        {
            healthLerp += Time.deltaTime * healthLerpSpeed;
            if (healthLerp >= 1)
            {
                healthLerp = -1;
                previousHealth = targetHealth;
                SetCrop(targetHealth);
                flash.color = new Color(1, 1, 1, 0);
            }
            else
            {
                SetCrop(GetHealthLerped());
                flash.color = new Color(1, 1, 1, 1 - Mathf.Sqrt(healthLerp));
            }
        }
    }

    private float GetHealthLerped()
    {
        return Mathf.Lerp(previousHealth, targetHealth, Mathf.Sqrt(healthLerp));
    }

    private void SetCrop(float amount)
    {
        Vector4 newCrop = fillCrop.padding;
        newCrop.z = (1 - amount) * fillWidth;
        fillCrop.padding = newCrop;
    }
}

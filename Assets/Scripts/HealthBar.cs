using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    [SerializeField] private float fillWidth = 360;

    protected float previousHealth = 1;
    private float targetHealth = 1;
    private float healthLerp = -1;
    [SerializeField] float healthLerpSpeed = 7;

    private RectMask2D fillCrop;
    [SerializeField] private Image flash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetCropMask();
    }

    protected void GetCropMask()
    {
        fillCrop = GetComponentInChildren<RectMask2D>();
    }

    // Update is called once per frame
    protected void Update()
    {
        float health = GetHealth();
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

    protected abstract float GetHealth();

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

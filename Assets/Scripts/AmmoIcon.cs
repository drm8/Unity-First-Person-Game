using UnityEngine;
using UnityEngine.UI;

public class AmmoIcon : MonoBehaviour
{
    [SerializeField] private Sprite enabledTexture;
    [SerializeField] private Sprite disabledTexture;
    private Image image;

    [SerializeField] private int countToEnable;
    private bool wasEnabled = true;

    private PlayerCombat player;

    private float flashEffectDuration = 0.2f;
    private float flashEffectDelta = -1;
    [SerializeField] private float baseScale;
    [SerializeField] private float flashScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        player = FindObjectsByType<PlayerCombat>(FindObjectsSortMode.InstanceID)[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (flashEffectDelta != -1)
        {
            flashEffectDelta += Time.deltaTime / flashEffectDuration;
            if (flashEffectDelta >= 1)
            {
                transform.localScale = Vector3.one * baseScale;
                flashEffectDelta = -1;
            }
            else
            {
                transform.localScale = Vector3.one * Mathf.Lerp(flashScale, baseScale, flashEffectDelta * flashEffectDelta);
            }
        }

        bool isEnabled = (player.GetAmmo() >= countToEnable);
        if (isEnabled != wasEnabled)
        {
            image.sprite = isEnabled ? enabledTexture : disabledTexture;
            if (isEnabled)
            {
                Color color = image.color;
                color.a = 1;
                image.color = color;
                image.CrossFadeAlpha(0.25f, flashEffectDuration, false);
                flashEffectDelta = 0;
                transform.localScale = Vector3.one * flashScale;
            }
        }
        wasEnabled = isEnabled;
    }
}

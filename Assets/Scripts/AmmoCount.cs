using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class AmmoCount : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

	// Update is called once per frame
	public void UpdateText(int current, int max)
    {
		text.text = current + "/" + max;
	}
}

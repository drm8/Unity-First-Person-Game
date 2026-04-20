using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
	public string menuScene;

	private PauseManager pause;
    private PlayerCamera playerCam;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider audioSlider;

    private void Start()
    {
        pause = FindObjectsByType<PauseManager>(FindObjectsSortMode.InstanceID)[0];
        playerCam = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.InstanceID)[0];

        audioSlider.value = PlayerPrefs.GetFloat("masterVolume", audioSlider.value);
        PlayerPrefs.SetFloat("masterVolume", audioSlider.value);
        PlayerPrefs.Save();
    }

    public void Resume()
    {
        pause.Unpause();
    }

    public void QuitRun()
    {
        pause.Unpause();
        SceneManager.LoadScene(menuScene);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

    public void OnSensitivityChange(Slider slider)
    {
        playerCam.UpdateSensitivity(slider.value);
    }

    public void OnVolumeChange(Slider slider)
    {
        mixer.SetFloat("masterVolume", slider.value);
        PlayerPrefs.SetFloat("masterVolume", slider.value);
        PlayerPrefs.Save();
    }
}

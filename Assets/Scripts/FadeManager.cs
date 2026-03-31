using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndIn(sceneName));
    }

    IEnumerator FadeOutAndIn(string sceneName)
    {
        float timer = 0;
        float startVolume = audioSource.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            
            fadeImage.color = new Color(0, 0, 0, progress);
            audioSource.volume = Mathf.Lerp(startVolume, 0, progress);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);

        timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            fadeImage.color = new Color(0, 0, 0, 1 - progress);
            audioSource.volume = Mathf.Lerp(0, startVolume, progress);
            yield return null;
        }
    }
}
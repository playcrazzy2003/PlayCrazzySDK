using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoaderManager : MonoBehaviour
{

    [SerializeField] private string currentBuildVersion = "1.0.0";

    private const string BuildVersionKey = "BuildVersion";

    [Header("Loading UI")]
    public GameObject LoadingScreen;
    public int sceneNumber;
    public RectTransform injectionBack;
    public Image slider;
    public Text percentageText;
    public float slidingTime = 2f;

    void Awake()
    {
        if (string.IsNullOrEmpty(currentBuildVersion))
        {
            Debug.LogError("Current build version is not set in the DataManager. Please set it in the Inspector.");
            return;
        }

        CheckBuildVersion();
    }

    void OnSupersonicWisdomReady()
    {
        LoadLevel(sceneNumber);
    }

    public void LoadLevel(int sceneIndex)
    {
        slider.fillAmount = 0f;
        StartCoroutine(SlideInjectionBack());
        StartCoroutine(FillSlider(sceneIndex));
    }

    private IEnumerator SlideInjectionBack()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = injectionBack.anchoredPosition;
        Vector2 targetPosition = new Vector2(310, startPosition.y);

        while (elapsedTime < slidingTime)
        {
            injectionBack.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / slidingTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        injectionBack.anchoredPosition = targetPosition;
    }

    private IEnumerator FillSlider(int sceneIndex)
    {
        float elapsedTime = 0f;

        while (elapsedTime < slidingTime)
        {
            slider.fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / slidingTime);
            UpdatePercentage();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        slider.fillAmount = 1f;
        UpdatePercentage();

        SceneManager.LoadScene(sceneIndex);
    }

    private void UpdatePercentage()
    {
        percentageText.text = $"{Mathf.FloorToInt(slider.fillAmount * 100)}%";
    }

    private void CheckBuildVersion()
    {
        string savedVersion = PlayerPrefs.GetString(BuildVersionKey, null);

        if (string.IsNullOrEmpty(savedVersion))
        {
            Debug.LogWarning("No saved version found. Setting the current build version for the first time...");
            PlayerPrefs.DeleteAll();
            SaveCurrentVersion();
            return;
        }

        if (!savedVersion.Equals(currentBuildVersion))
        {
            Debug.LogWarning($"Version mismatch! Saved version: {savedVersion}, Current version: {currentBuildVersion}. Clearing PlayerPrefs...");
            PlayerPrefs.DeleteAll();
            SaveCurrentVersion();
        }
        else
        {
            Debug.Log("Version matches. No action needed.");
        }
    }

    private void SaveCurrentVersion()
    {
        PlayerPrefs.SetString(BuildVersionKey, currentBuildVersion);
        PlayerPrefs.Save();

        Debug.Log($"PlayerPrefs updated to the new version: {currentBuildVersion}");
    }


}

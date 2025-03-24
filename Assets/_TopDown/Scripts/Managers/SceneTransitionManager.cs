using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("References")]
    public GameObject loadingScreen;
    public UnityEngine.UI.Slider progressBar;

    [Header("Settings")]
    public float minimumLoadingTime = 1.0f;
    public float fadeTime = 0.5f;

    private bool isLoading = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading)
        {
            return;
        }
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;

        if(loadingScreen)
        {
            loadingScreen.SetActive(true);

            CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
            if(canvasGroup)
            {
                canvasGroup.alpha = 0;
                float timer = 0;
                while(timer<fadeTime)
                {
                    timer += Time.unscaledDeltaTime;
                    canvasGroup.alpha = timer / fadeTime;
                    yield return null;
                }
                canvasGroup.alpha = 1;
            }
        }

        SaveQuestData();

        float startTime = Time.realtimeSinceStartup;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.allowSceneActivation = false;

        while(!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            if(progressBar)
            {
                progressBar.value = progress;
            }

            if(asyncOperation.progress >= 0.9f)
            {
                float elapsedTime = Time.realtimeSinceStartup - startTime;
                if(elapsedTime >= minimumLoadingTime)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        yield return null;

        LoadQuestData();

        if(loadingScreen)
        {
            CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
            if(canvasGroup)
            {
                float timer = 0;
                while(timer < fadeTime)
                {
                    timer += Time.unscaledDeltaTime;
                    canvasGroup.alpha = 1 - (timer / fadeTime);
                    yield return null;
                }
            }

            loadingScreen.SetActive(false);
        }

        isLoading = false;
    }

    private void SaveQuestData()
    {
        if(QuestManager.Instance)
        {
            QuestManager.Instance.SaveQuestData();
        }


    }

    private void LoadQuestData()
    {
        if(QuestManager.Instance)
        {
            QuestManager.Instance.LoadQuestData();
        }
    }

}

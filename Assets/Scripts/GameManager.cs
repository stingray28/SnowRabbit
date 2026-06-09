using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public int NowScene { get; private set; }
    [SerializeField] private GameObject eventSystemPrefab;
    [SerializeField] private float transitionTime = 0.2f;

    private Slider transitionUI;
    //public GameObject PauseMenu { get; private set; }

    private float fixedDeltaTime;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        fixedDeltaTime = Time.fixedDeltaTime;

        transitionUI = GameObject.Find("Transition").GetComponent<Slider>();
        //PauseMenu = GameObject.Find("PauseMenu");

        //PauseMenu.SetActive(false);
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        NowScene = SceneManager.GetActiveScene().buildIndex;
        if (!FindAnyObjectByType<EventSystem>()) Instantiate(eventSystemPrefab);
        if (transitionUI.value == 1f)   StartCoroutine(TransitionExit());
    }

    public void LoadScene(int sceneIndex, bool transition = true)
    {
        if (transition)
        {
            StartCoroutine(TransitionEnter(sceneIndex));
        }
        else
        {
            StartCoroutine(LoadSceneDefault(sceneIndex));
        }
    }

    private IEnumerator LoadSceneDefault(int _sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield break;
    }

    private IEnumerator TransitionEnter(int _sceneIndex)
    {
        transitionUI.direction = Slider.Direction.RightToLeft;
        for (float timeCount = 0; timeCount <= transitionTime; timeCount += fixedDeltaTime)
        {
            transitionUI.value = timeCount / transitionTime;
            yield return null;
        }
        transitionUI.value = 1;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneIndex);
        if (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield break;
    }

    private IEnumerator TransitionExit()
    {
        transitionUI.direction = Slider.Direction.LeftToRight;
        for (float timeCount = transitionTime; timeCount >= 0; timeCount -= fixedDeltaTime)
        {
            transitionUI.value = timeCount / transitionTime;
            yield return null;
        }
        transitionUI.value = 0;

        yield break;
    }

    //public void Pause()
    //{
    //    bool isPause = PauseMenu.activeSelf;
    //    PauseMenu.SetActive(!isPause);
    //    TimeScaleSet(!isPause);
    //}

    public void TimeScaleSet(bool isStop)
    {
        Time.timeScale = isStop ? 0f : 1f;
        Time.fixedDeltaTime = isStop ? 0f : fixedDeltaTime;
    }

    //public void ExitPlay()
    //{
    //    PauseMenu.SetActive(false);
    //    TimeScaleSet(true);
    //    DataManager.Instance.SaveData();
    //    Destroy(PlayerManager.Instance.gameObject);
    //}
}
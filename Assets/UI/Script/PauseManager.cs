using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;
    public static PauseManager Instance { get { return instance; } }

    [Header("MenuManager")]
    [SerializeField] private MenuManager menuManager;

    [Header("Root")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject loadPanel;
    [SerializeField] private GameObject settingPanel;

    [Header("Panel Animators")]
    [SerializeField] private UIPanelSwingAnimator pauseMenuAnimator;
    [SerializeField] private UIPanelSwingAnimator loadPanelAnimator;
    [SerializeField] private UIPanelSwingAnimator settingPanelAnimator;

    [Header("Main Pause Menu")]
    [SerializeField] private RectTransform pauseSelectionBG;
    [SerializeField] private RectTransform[] pauseMenuItems;

    [Header("Load Menu")]
    [SerializeField] private RectTransform loadSelectionBG;
    [SerializeField] private RectTransform[] loadMenuItems;

    [Header("Setting Category Menu")]
    [SerializeField] private RectTransform settingSelectionBG;
    [SerializeField] private RectTransform[] settingCategoryItems;
    [SerializeField] private GameObject[] settingCategoryIcons;

    [Header("Setting Contents")]
    [SerializeField] private GameObject graphicPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject languagePanel;

    [Header("Input Lock")]
    [SerializeField] private float pointerLockDuration = 0.2f;

    private int currentIndex;
    private bool isPaused;
    private bool isLoadPanelOpen;
    private bool isSettingPanelOpen;
    private bool isTransitioning;
    private float pointerInputLockTime;

    private void Awake()
    {
        instance = this;

        pauseUI.SetActive(false);

        pauseMenuPanel.SetActive(true);
        loadPanel.SetActive(false);
        settingPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isTransitioning) return;

            if (isLoadPanelOpen || isSettingPanelOpen)
            {
                BackToPauseMenu();
            }
            else
            {
                TogglePause();
            }
        }

        if (!isPaused || isTransitioning) return;

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            MoveSelection(1);
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            MoveSelection(-1);
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Submit();
        }
    }

    public void TogglePause()
    {
        if (isTransitioning) return;

        isPaused = !isPaused;

        pauseUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            OpenPauseMenuInstant();
        }
        else
        {
            ResetPanels();
        }
    }

    private void OpenPauseMenuInstant()
    {
        LockPointerInput();

        isLoadPanelOpen = false;
        isSettingPanelOpen = false;

        pauseMenuPanel.SetActive(true);
        loadPanel.SetActive(false);
        settingPanel.SetActive(false);

        if (pauseMenuAnimator != null)
        {
            pauseMenuAnimator.PlayIn();
        }

        currentIndex = 0;
        StartCoroutine(RefreshSelectionNextFrame());
    }

    private void OpenPauseMenu()
    {
        LockPointerInput();

        isLoadPanelOpen = false;
        isSettingPanelOpen = false;

        pauseMenuPanel.SetActive(true);
        loadPanel.SetActive(false);
        settingPanel.SetActive(false);

        if (pauseMenuAnimator != null)
        {
            pauseMenuAnimator.PlayIn();
        }

        currentIndex = 0;
        StartCoroutine(RefreshSelectionNextFrame());
    }

    private void OpenLoadPanel()
    {
        if (isTransitioning) return;

        LockPointerInput();
        isTransitioning = true;

        if (pauseMenuAnimator != null)
        {
            pauseMenuAnimator.PlayOut(() =>
            {
                LockPointerInput();

                pauseMenuPanel.SetActive(false);
                loadPanel.SetActive(true);
                settingPanel.SetActive(false);

                isLoadPanelOpen = true;
                isSettingPanelOpen = false;

                currentIndex = 0;

                if (loadPanelAnimator != null)
                {
                    loadPanelAnimator.PlayIn();
                }

                StartCoroutine(RefreshSelectionNextFrame());
                isTransitioning = false;
            });
        }
        else
        {
            pauseMenuPanel.SetActive(false);
            loadPanel.SetActive(true);
            settingPanel.SetActive(false);

            isLoadPanelOpen = true;
            isSettingPanelOpen = false;

            currentIndex = 0;
            StartCoroutine(RefreshSelectionNextFrame());
            isTransitioning = false;
        }
    }

    private void OpenSettingPanel()
    {
        if (isTransitioning) return;

        LockPointerInput();
        isTransitioning = true;

        if (pauseMenuAnimator != null)
        {
            pauseMenuAnimator.PlayOut(() =>
            {
                LockPointerInput();

                pauseMenuPanel.SetActive(false);
                loadPanel.SetActive(false);
                settingPanel.SetActive(true);

                isLoadPanelOpen = false;
                isSettingPanelOpen = true;

                currentIndex = 0;
                ShowSettingContent(0);

                if (settingPanelAnimator != null)
                {
                    settingPanelAnimator.PlayIn();
                }

                StartCoroutine(RefreshSelectionNextFrame());
                isTransitioning = false;
            });
        }
        else
        {
            pauseMenuPanel.SetActive(false);
            loadPanel.SetActive(false);
            settingPanel.SetActive(true);

            isLoadPanelOpen = false;
            isSettingPanelOpen = true;

            currentIndex = 0;
            ShowSettingContent(0);

            StartCoroutine(RefreshSelectionNextFrame());
            isTransitioning = false;
        }
    }

    public void BackToPauseMenu()
    {
        if (isTransitioning) return;

        LockPointerInput();
        isTransitioning = true;

        UIPanelSwingAnimator currentAnimator = null;

        if (isLoadPanelOpen)
        {
            currentAnimator = loadPanelAnimator;
        }
        else if (isSettingPanelOpen)
        {
            currentAnimator = settingPanelAnimator;
        }

        if (currentAnimator != null)
        {
            currentAnimator.PlayOut(() =>
            {
                OpenPauseMenu();
                isTransitioning = false;
            });
        }
        else
        {
            OpenPauseMenu();
            isTransitioning = false;
        }
    }

    private void ResetPanels()
    {
        isLoadPanelOpen = false;
        isSettingPanelOpen = false;
        isTransitioning = false;

        pauseMenuPanel.SetActive(true);
        loadPanel.SetActive(false);
        settingPanel.SetActive(false);
    }

    private IEnumerator RefreshSelectionNextFrame()
    {
        yield return null;
        SetSelection(currentIndex);
    }

    private void MoveSelection(int direction)
    {
        RectTransform[] currentItems = GetCurrentMenuItems();

        currentIndex += direction;

        if (currentIndex < 0)
        {
            currentIndex = currentItems.Length - 1;
        }

        if (currentIndex >= currentItems.Length)
        {
            currentIndex = 0;
        }

        SetSelection(currentIndex);
    }

    public void SetSelection(int index)
    {
        currentIndex = index;

        RectTransform selectionBG = GetCurrentSelectionBG();
        RectTransform[] currentItems = GetCurrentMenuItems();

        if (selectionBG == null || currentItems == null || currentItems.Length == 0) return;
        if (currentIndex < 0 || currentIndex >= currentItems.Length) return;

        Vector3 targetPosition = selectionBG.position;
        targetPosition.y = currentItems[currentIndex].position.y;
        selectionBG.position = targetPosition;

        if (isSettingPanelOpen)
        {
            UpdateSettingCategoryIcons();
            ShowSettingContent(currentIndex);
        }
    }

    public void SetSelectionByPointer(int index)
    {
        if (Time.unscaledTime < pointerInputLockTime) return;
        if (isTransitioning) return;

        SetSelection(index);
    }

    public void ClickCurrentSelection()
    {
        if (Time.unscaledTime < pointerInputLockTime) return;
        if (isTransitioning) return;

        Submit();
    }

    private void Submit()
    {
        if (isLoadPanelOpen)
        {
            SubmitLoadPanel();
        }
        else if (isSettingPanelOpen)
        {
            SubmitSettingPanel();
        }
        else
        {
            SubmitPauseMenu();
        }
    }

    private void SubmitPauseMenu()
    {
        switch (currentIndex)
        {
            case 0:
                ContinueGame();
                break;

            case 1:
                OpenLoadPanel();
                break;

            case 2:
                OpenSettingPanel();
                break;

            case 3:
                menuManager.ToMain();
                break;
        }
    }

    private void SubmitLoadPanel()
    {
        switch (currentIndex)
        {
            case 0:
                menuManager.ToPlay(1);
                break;

            case 1:
                menuManager.ToPlay(2);
                break;

            case 2:
                menuManager.ToPlay(3);
                break;

            case 3:
                BackToPauseMenu();
                break;
        }
    }

    private void SubmitSettingPanel()
    {
        ShowSettingContent(currentIndex);
    }

    public void ContinueGame()
    {
        if (isTransitioning) return;

        isPaused = false;
        isLoadPanelOpen = false;
        isSettingPanelOpen = false;
        isTransitioning = false;

        pauseUI.SetActive(false);

        pauseMenuPanel.SetActive(true);
        loadPanel.SetActive(false);
        settingPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void ShowSettingContent(int index)
    {
        graphicPanel.SetActive(index == 0);
        soundPanel.SetActive(index == 1);
        controlPanel.SetActive(index == 2);
        languagePanel.SetActive(index == 3);
    }

    private void UpdateSettingCategoryIcons()
    {
        if (settingCategoryIcons == null) return;

        for (int i = 0; i < settingCategoryIcons.Length; i++)
        {
            if (settingCategoryIcons[i] == null) continue;

            settingCategoryIcons[i].SetActive(i != currentIndex);
        }
    }

    private void LockPointerInput()
    {
        pointerInputLockTime = Time.unscaledTime + pointerLockDuration;
    }

    private RectTransform GetCurrentSelectionBG()
    {
        if (isLoadPanelOpen)
        {
            return loadSelectionBG;
        }

        if (isSettingPanelOpen)
        {
            return settingSelectionBG;
        }

        return pauseSelectionBG;
    }

    private RectTransform[] GetCurrentMenuItems()
    {
        if (isLoadPanelOpen)
        {
            return loadMenuItems;
        }

        if (isSettingPanelOpen)
        {
            return settingCategoryItems;
        }

        return pauseMenuItems;
    }
}
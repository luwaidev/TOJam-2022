using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;



    [Header("References")]
    public Animator transition;
    public Animator pauseMenu;

    [Header("Scene Loading Settings")]

    [SerializeField] float sceneTransitionTime;
    public bool loaded;
    public bool loadingScene;
    public bool paused;
    float volume = 0.5f;
    float brightness = -0.5f;
    public float sens = 4;

    public Volume v;
    public ColorAdjustments c;

    [Header("Sounds")]
    public AudioSource hitSound;
    public AudioSource buildupSound;



    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameController").Length > 1) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        v.profile.TryGet(out c);
    }
    public void Load(string sceneName)
    {
        if (!loadingScene) StartCoroutine(LoadScene(sceneName));
    }

    public void LoadWithDelay(string sceneName, float delayTime)
    {
        if (!loadingScene) StartCoroutine(Delay(sceneName, delayTime));
    }

    IEnumerator Delay(string sceneName, float delayTime)
    {
        loadingScene = true;
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(LoadScene(sceneName));
    }

    public IEnumerator Kill()
    {
        if (loadingScene)
        {
            yield break;
        }
        loadingScene = true;
        yield return new WaitForSeconds(1f);
        buildupSound.Play();
        yield return new WaitForSeconds(2.25f);
        hitSound.Play();
        SceneManager.LoadScene("Kill");
        yield return new WaitForSeconds(3);
        Load("Menu Scene");
    }

    public void Exit()
    {

        StartCoroutine(Quit());
    }

    public IEnumerator Quit()
    {
        transition.SetTrigger("Transition"); // Start transitioning scene out




        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        Application.Quit();
    }
    public IEnumerator LoadScene(string sceneName)
    {
        if (paused)
        {
            TogglePause();
        }
        loaded = true;
        loadingScene = true;
        transition.SetTrigger("Transition"); // Start transitioning scene out




        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition

        // Start loading scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;
        while (!load.isDone)
        {
            if (load.progress >= 0.9f)
            {
                load.allowSceneActivation = true;
            }

            yield return null;
        }
        load.allowSceneActivation = true;

        yield return null;


        //  Set volume
        AudioListener.volume = volume;


        if (GameObject.Find("CameraHolder") != null)
        {
            GameObject.Find("CameraHolder").GetComponent<CameraController>().sens = Vector2.one * sens;
        }

        instance = this; // Reset self as GameManager instance

        transition.SetTrigger("Transition"); // Start transitioning scene back

        if (sceneName != "Menu Scene")
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;

        yield return new WaitForSeconds(1);
        instance = this;

    }

    private void Update()
    {
        //

        if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }

    public void TogglePause()
    {

        if (SceneManager.GetActiveScene().name != "Menu Scene" && !loadingScene)
        {
            paused = !paused;
            Time.timeScale = (paused) ? 0 : 1;
            pauseMenu.Play(paused ? "Paused" : "Unpaused");
            Cursor.visible = paused;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;

        }
    }

    public IEnumerator TimeEffect()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
    }

    public void OnVolumeChange(float _volume)
    {
        volume = _volume;
        AudioListener.volume = volume;
    }

    public void OnBrightnessChange(float _brightness)
    {
        brightness = _brightness;
        print(brightness);
        c.postExposure.value = brightness;

    }

    public void OnSensitivityChange(float _sens)
    {
        sens = _sens;
        if (GameObject.Find("CameraHolder") != null)
        {
            print(sens);
            GameObject.Find("CameraHolder").GetComponent<CameraController>().sens = Vector2.one * sens;
        }
    }

    public void OnToggleFullscreen(bool fullscreen)
    {

        // Toggle fullscreen
        Screen.fullScreen = fullscreen;
    }
}
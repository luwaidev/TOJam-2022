using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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


    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameController").Length > 1) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
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

    public void FirstTimeLoadingGame()
    {
        if (loadingScene) StartCoroutine(FirstTimeLoadingGameCutscene());
    }
    public IEnumerator FirstTimeLoadingGameCutscene()
    {
        yield return new WaitForSeconds(1);

        LoadScene("Game");
    }

    public IEnumerator Kill()
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.7f);
        SceneManager.LoadScene("Kill");
        transition.SetTrigger("Transition");
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




        instance = this; // Reset self as GameManager instance

        transition.SetTrigger("Transition"); // Start transitioning scene back

        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;

        yield return new WaitForSeconds(1);
        instance = this;

    }


    public void TogglePause()
    {
        paused = !paused;
        Time.timeScale = (paused) ? 0 : 1;
        pauseMenu.SetBool("Paused", paused);
    }

    public IEnumerator TimeEffect()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
    }
}
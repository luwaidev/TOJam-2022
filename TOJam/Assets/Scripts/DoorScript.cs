using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorScript : MonoBehaviour, Interactables
{
    public float MaxRange { get { return maxRange; } }

    [SerializeField] private float maxRange = 2f;
    [SerializeField] private float fadeTime = 0.01f;


    [SerializeField] private GameObject textObject;
    private TMP_Text text;

    public AudioSource openSound;
    public AudioSource openSound1;
    public AudioSource closeSound;
    public AudioSource ambience;
    public AudioSource spooky;
    public AudioSource spookysound;

    public bool open;
    public float openSpeed;
    public float openRotation;
    public float closedRotation;

    public bool toggleSound;
    public bool locked;
    public bool end;


    public void Awake()
    {
        textObject = GameObject.Find("Door Text");
        text = textObject.GetComponent<TMP_Text>();
    }

    public void OnStartHover()
    {
        //textObject.SetActive(true);
        text.color = Color32.Lerp(new Color32(225, 225, 225, 225), new Color32(225, 225, 225, 0), Time.deltaTime * fadeTime);
    }

    public void OnInteract()
    {
        if (end)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.Kill());
            return;
        }
        if (locked)
        {
            openSound.Play();
            return;
        }
        open = !open;
        if (open)
        {
            openSound.Play();
            openSound1.Play();

        }
        else
        {
            closeSound.Play();
        }
        if (toggleSound)
        {
            StartCoroutine(PlayAudio());
        }

        if (spookysound != null)
        {
            spookysound.Play();

        }

        if (end)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.Kill());
        }
    }

    public void OnEndHover()
    {
        //textObject.SetActive(false);
        text.color = Color32.Lerp(new Color32(225, 225, 225, 0), new Color32(225, 225, 225, 225), Time.deltaTime * fadeTime);
    }

    private void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, open ? openRotation : closedRotation, openSpeed));
        if (locked && open)
        {
            Lock();
        }
    }

    IEnumerator PlayAudio()
    {

        spooky.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        ambience.gameObject.SetActive(false);
        toggleSound = false;
    }

    public void Lock()
    {
        locked = true;
        open = false;
        closeSound.Play();
    }
}

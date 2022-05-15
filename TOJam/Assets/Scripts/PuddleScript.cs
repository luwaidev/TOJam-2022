using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class PuddleScript : MonoBehaviour, Interactables
{
    public float MaxRange { get { return maxRange; } }

    [SerializeField] private float maxRange = 2f;
    [SerializeField] private float fadeTime = 0.01f;


    [SerializeField] private GameObject textObject;
    private TMP_Text text;

    public void Awake()
    {
        textObject = GameObject.Find("Interact Text");
        text = textObject.GetComponent<TMP_Text>();
    }

    public void OnStartHover()
    {
        //textObject.SetActive(true);
        text.color = Color32.Lerp(new Color32(225, 225, 225, 225), new Color32(225, 225, 225, 0), Time.deltaTime * fadeTime);
    }

    public void OnInteract()
    {
        Destroy(gameObject);
    }

    public void OnEndHover()
    {
        //textObject.SetActive(false);
        text.color = Color32.Lerp(new Color32(225, 225, 225, 0), new Color32(225, 225, 225, 225), Time.deltaTime * fadeTime);
    }

}

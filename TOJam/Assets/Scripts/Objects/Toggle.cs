using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    public GameObject o;
    private void OnTriggerEnter(Collider other)
    {
        o.SetActive(true);
    }
}

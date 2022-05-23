using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteHallway : MonoBehaviour
{
    // ONLY WORKS ON Z AXIS 
    // TODO add functionality for other axis
    public Renderer obj;
    public Transform player;
    public float teleportPosition;
    public float limit;
    // Start is called before the first frame update
    void Start()
    {
        obj = obj.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // print(!obj.isVisible && player.position.z > limit);
        if (!obj.isVisible && player.position.z > limit)
        {
            // print("how");
            player.position = new Vector3(player.position.x, player.position.y, teleportPosition);
        }
    }
}

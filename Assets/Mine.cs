using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public ChunkSpawner Spawner;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer

            Vector3 pos = transform.position;
         //   pos.y += 0f;
            if (Physics.Raycast(pos, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {

                

                Spawner.MineBlock(hit.point);
                Debug.Log(hit.point);
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
            }
        }
    }
}

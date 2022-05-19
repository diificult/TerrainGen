using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

public class ChunkSpawner : MonoBehaviour
{


    public GameObject chunk;

    public Dictionary<Point, Chunk> chunks = new Dictionary<Point, Chunk>();

    public void Start()
    {
        for (int cx = 0; cx < 48; cx++)
        {
            for (int cz = 0; cz < 48; cz++)
            {
                StartCoroutine(SpawnChunk(cx, cz));
            }
        }
    }


    IEnumerator SpawnChunk (int cx, int cz)
    {
        //yield return new WaitForSecondsRealtime(Random.Range(0f, 1f));
        yield return new WaitForEndOfFrame();
        
        Chunk c = Instantiate(chunk, new Vector3(cx * 16, 0, cz * 16), new Quaternion(0,0,0,0)).GetComponent<Chunk>();
        c.Generate(cx, cz);

        chunks.Add(new Point(cx, cz), c);
        
    }

    public void MineBlock(Vector3 point)
    {

        
        int x = Mathf.FloorToInt(point.x);
        int y = Mathf.FloorToInt(point.y);  
        int z = Mathf.FloorToInt(point.z);


        int cx = x / 16;
        int cz = z / 16;

        int posx = x % 16;
        int zpos = z % 16;

        Chunk c = chunks[new Point(cx, cz)];
        c.Mineblock(posx, y, zpos);




    }

}

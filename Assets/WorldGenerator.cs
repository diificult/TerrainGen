using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SeedX = Random.Range(0, 1000);
        SeedZ = Random.Range(0, 1000);
        StartCoroutine(Generate());

        
        

    }


    private int mapSize = (20 * 16) ;
    private int SeedX;
    private int SeedZ;

    IEnumerator Generate()
    {
        for (int cx = 0; cx < 20; cx++)
        {
            for (int cy = 0; cy < 20; cy++)
            {
                GenerateChunk(cx, cy);
                yield return new WaitForEndOfFrame();

            }

        }
    }

    public GameObject block;
    public int scale;

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GenerateChunk(int Chunkx, int Chunkz)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z  < 16; z++)
            {
                int height = GenerateHeight(x,z, Chunkx, Chunkz);
                
             //   for (int y = 0; y < height; y++)
               // {
                    Instantiate(block, new Vector3(x, height, z) + new Vector3((Chunkx * 16), 0, (Chunkz * 16)), new Quaternion(0, 0, 0, 0));
               // }

            }

        }
    }


    private int GenerateHeight(int x, int z, int chunkX, int chunkZ)
    {
        int height = 0;
        //Debug.Log(Mathf.PerlinNoise(x / (16 * chunkX + 1) * scale, z / (16 * chunkZ + 1) * scale) + "," + x + "," + z);
        // height = 50 + (int)(Mathf.PerlinNoise(x / (16 * chunkX + 1) * scale, z / (16 * chunkZ + 1) * scale) * 25);

        //  Debug.Log(( (x + (chunkX * 16) ) / 1600) +  "");


        float xCoord = (float)((x + (chunkX * 16)) / (float)mapSize )* scale;
        float zCoord = (float)((z + (chunkZ * 16) ) / (float)mapSize) * scale;

        Debug.Log("x = " + x  + ",Chunk = " + chunkX + ",Location = "+  (x + (chunkX *16)));
        Debug.Log(xCoord);



        height = (int)(Mathf.PerlinNoise(xCoord, zCoord) * 50f);
        Debug.Log(height);
        

     //   height = 1 + (int) (Mathf.PerlinNoise(((float) (( (float)x  + (chunkX * 16f)) / 255f)), (float)(((float)z + (chunkZ * 16) )/ 255)) * 50);
       
        
        
        
        
        //height += (int)(Mathf.PerlinNoise(((float)(((float)x + (chunkX * 16f)) / 128f)), (float)(((float)z + (chunkZ * 16f)) / 128f)) * 25);
        //height += (int)(Mathf.PerlinNoise(((float)(((float)x + (chunkX * 16f)) / 64f)), (float)(((float)z + (chunkZ * 64f)) / 128f)) * 12);

        
        // float[] octaveFrequencies = { 1, 1.5f, 2, 2.5f };
        // float[] octaveAmplitudes = { 1, 0.9f, 0.7f, 0.1f };
        // float y = 0;
        //for (int i = 0; i < octaveFrequencies.Length; i++)
        //     y += octaveAmplitudes[i] * Mathf.PerlinNoise(
        //         octaveFrequencies[i] * x + .3f,
        //         octaveFrequencies[i] * z + .3f) / 2f;




        //Debug.Log(y);

        return  height;
    }


   // private int[,] GenerateNoiseTexture()
   // {


//
  //  }











}

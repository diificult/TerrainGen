using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{


    
    int RandomXAdj;
    int RandomYAdj;

    int width = 513;
    int height = 513;
    int depth = 20;


    // Start is called before the first frame update
    void Start()
    {


        RandomXAdj = Random.Range(-1000000, 1000000);
        RandomYAdj = Random.Range(-1000000, 1000000);
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);


        

    }


    TerrainData GenerateTerrainData(TerrainData data)
    {
        data.heightmapResolution = width + 1;
        data.size = new Vector3(width, depth, height);
        data.SetHeights(0, 0, GenerateHeight());
        return data;


    }

    private float[,] GenerateHeight()
    {
        float[,] heights = new float[513, 513];

        for (int x = 0; x < 513; x++)
        {
            for (int y = 0; y < 513; y++)
            {
                
               // for (int s = 1; s < 4; s++)
               // { 
                    heights[x, y] = calculateheight(x, y, 1);
                 //   Debug.Log("x,y " + x + ", " + y + "    " + heights[x, y]);
                 //   heights[x, y] = heights[x,y] + (calculateheight(x, y, (4))  / 4);
                    //Debug.Log("x,y " + x + ", " + y + "    " + heights[x, y]);
                 //   heights[x, y] += (calculateheight(x, y, (16)) / 16);
                 //   heights[x, y] += calculateheight(x, y, (64)) / 64;
                 //   heights[x, y] = heights[x, y];
                // }
               
             
            }
        }
        return heights;
    }


    private float calculateheight(int x, int y, int scale)
    {
        float xf = ((float)x / 513 * scale) + RandomXAdj;
        float yf = ((float)y / 513 * scale) + RandomYAdj;

        float[] octaveFrequencies = { 0.05f, 0.1f, 0.2f, 0.1f };
        float[] octaveAmplitudes = { 0.60f, 0.3f, 0.15f, 0.075f };
        float z = 0;
        for (int i = 0; i < octaveFrequencies.Length; i++)
            z += octaveAmplitudes[i] * Mathf.PerlinNoise(
                 octaveFrequencies[i] * x + .3f,
                 octaveFrequencies[i] * y + .3f) / 2f;

      //  Debug.Log(z);

        return z; //(Mathf.PerlinNoise(xf,yf));  

    }











}

using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshRenderer mRenderer;
    public MeshFilter mFilter;
    public MeshCollider mCollider;

    int size = 16;
    [SerializeField]
    public short[,,] BlockIDs;

    public int HightLimit = 256;


    public int Chunkx, Chunkz = 0;

    public float scale = 0.25f;


    public bool RequiresMeshGeneration = false;

    int textureWidth = 512;
    int textureHeight = 512;

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mCollider = GetComponent<MeshCollider>();
        mFilter = GetComponent<MeshFilter>();
        ContTest();
    }


    public void Mineblock(int x, int y, int z)
    {
        BlockIDs[x, y, z] = 0;
        RequiresMeshGeneration = true;
    }

    // Start is called before the first frame update
    public void Generate(int cx, int cz)
    {

        Chunkx = cx;
        Chunkz = cz;
        //  GenerateBlocks();
        GenerateHeightmaps();

        RequiresMeshGeneration = true;

    }
    // Update is called once per frame
    void Update()
    {
        if (RequiresMeshGeneration) GenerateMesh();
    }


    void GenerateBlocks()
    {
        BlockIDs = new short[size, size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    BlockIDs[x, y, z] = 1;
                }
            }
        }
    }


    void GenerateHeightmaps()
    {

        BlockIDs = new short[size, HightLimit, size];
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float contientalness = GenerateContientalness(x, z, Chunkx, Chunkz);
                int height = 3;
                if (contientalness <= 0.7)
                {
                    height += (int)(contientalness * 15f);
                }
                else if (contientalness > 0.7 && contientalness <= 0.8)
                {
                    height += (int)((0.7f * 15f) + ((contientalness - 0.7f) * 175f));
                }
                else
                {
                    height += (int)((0.7f * 15f) + ((0.1f) * 175f) + ((contientalness - 0.8f) * 10f));
                }



                  //   Debug.Log(height);
                for (int y = 0; y < HightLimit; y++)
                {
                    if (y < height) BlockIDs[x, y, z] = 1;
                    else BlockIDs[x, y, z] = 0;
                }

            }
        }
    }



    public void ContTest()
    {
        float contientalness = 0.75f;
        int height = 0;
        if (contientalness <= 0.7)
        {
            height = 3 + (int)(contientalness * 8f);
        }
        else if (contientalness > 0.7 && contientalness <= 0.8)
        {
            height = 3 + (int)((0.7f * 8f) + ((contientalness - 0.7f) * 125f));
        }
        else
        {
            height += 3 + (int)((0.7f * 8f) + ((0.1f) * 125f) + ((contientalness - 0.8f) * 2f));
        }
       // Debug.Log (height);
    }


    private float GenerateContientalness(int x, int z, int chunkX, int chunkZ)
    {

        int height = 0;
        float xCoorda = (float)((x + (chunkX * 16)) / 16f) * scale;
        float xCoordb = (float)((x + (chunkX * 16) + 1000) / 16f) * (scale * 2);
        float xCoordc = (float)((x + (chunkX * 16) + 2000) / 16f) * (scale * 4);
        float zCoorda = (float)((z + (chunkZ * 16)) / 16f) * scale;
        float zCoordb = (float)((z + (chunkZ * 16) + 1000) / 16f) * (scale * 2);
        float zCoordc = (float)((z + (chunkZ * 16) + 2000) / 16f) * (scale * 4);

        //  Debug.Log("x = " + x + ",Chunk = " + chunkX + ",Location = " + (x + (chunkX * 16)));
        // Debug.Log(xCoord);
        float heighta = 1 + (Mathf.PerlinNoise(xCoorda, zCoorda) * 50f);
        float heightb = 1 + (Mathf.PerlinNoise(xCoorda, zCoorda) * 25f);
        float heightc = 1 + (Mathf.PerlinNoise(xCoorda, zCoorda) * 12f);
        float contientalness = (heighta + heightb + heightc) / (50f + 25f + 12f);


        

        return contientalness;
    }

    void GenerateMesh()
    {

        Mesh newMesh = new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        int currentIndex = 0;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < HightLimit; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Vector3Int offset = new Vector3Int(x, y, z);
                    //  Debug.Log(x + ", " + y + "," + z + " block ids = " + BlockIDs[x,y,z]) ;
                    if (BlockIDs[x, y, z] == 0) continue;
                    else
                    {
                        GenerateBlock_Top(ref currentIndex, offset, verts, normals, uvs, indices, new Rect());
                        GenerateBlock_Right(ref currentIndex, offset, verts, normals, uvs, indices, new Rect());
                        GenerateBlock_Left(ref currentIndex, offset, verts, normals, uvs, indices, new Rect());
                        GenerateBlock_Forward(ref currentIndex, offset, verts, normals, uvs, indices, new Rect());
                        GenerateBlock_Back(ref currentIndex, offset, verts, normals, uvs, indices, new Rect());
                        GenerateBlock_Bottom(ref currentIndex, offset, verts, normals, uvs, indices, new Rect());
                    }
                }
            }
        }


        newMesh.SetVertices(verts);
        newMesh.SetNormals(normals);
        newMesh.SetUVs(0, uvs);
        newMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        

        newMesh.RecalculateTangents();
        mFilter.mesh = newMesh;
        
        mCollider.sharedMesh = newMesh;
        Material mat = Resources.Load("stone") as Material;
        mRenderer.material = mat;


        //setTexture

        




        RequiresMeshGeneration = false;
    }


    void GenerateBlock_Top(ref int currentIndex, Vector3Int offset, List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> Indices, Rect blockUVs)
    {
        verts.Add(new Vector3(-0.5f, 0.5f, 0.5f) + offset);
        verts.Add(new Vector3(0.5f, 0.5f, 0.5f) + offset);
        verts.Add(new Vector3(0.5f, 0.5f, -0.5f) + offset);
        verts.Add(new Vector3(-0.5f, 0.5f, -0.5f) + offset);

        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);


        uvs.Add(new Vector2(blockUVs.xMin /textureWidth, blockUVs.yMax /textureHeight));
        uvs.Add(new Vector2(blockUVs.xMax /textureWidth, blockUVs.yMax /textureHeight));
        uvs.Add(new Vector2(blockUVs.xMax /textureWidth, blockUVs.yMin /textureHeight));
        uvs.Add(new Vector2(blockUVs.xMin /textureWidth, blockUVs.yMin /textureHeight));
       // uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMax));
      ////  uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMin));
      //  uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMin));

        

        Indices.Add(currentIndex + 0);
        Indices.Add(currentIndex + 1);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 0);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 3);
        currentIndex += 4;




    }

    void GenerateBlock_Right(ref int currentIndex, Vector3Int offset, List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> Indices, Rect blockUVs)
    {
        verts.Add(new Vector3(0.5f, 0.5f, -0.5f) + offset);
        verts.Add(new Vector3(0.5f, 0.5f, 0.5f) + offset);
        verts.Add(new Vector3(0.5f, -0.5f, 0.5f) + offset);
        verts.Add(new Vector3(0.5f, -0.5f, -0.5f) + offset);

        normals.Add(Vector3.right);
        normals.Add(Vector3.right);
        normals.Add(Vector3.right);
        normals.Add(Vector3.right);


        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMin));
        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMin));

        Indices.Add(currentIndex);
        Indices.Add(currentIndex + 1);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 0);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 3);
        currentIndex += 4;




    }

    void GenerateBlock_Left(ref int currentIndex, Vector3Int offset, List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> Indices, Rect blockUVs)
    {
        verts.Add(new Vector3(-0.5f, 0.5f, 0.5f) + offset);
        verts.Add(new Vector3(-0.5f, 0.5f, -0.5f) + offset);
        verts.Add(new Vector3(-0.5f, -0.5f, -0.5f) + offset);
        verts.Add(new Vector3(-0.5f, -0.5f, 0.5f) + offset);

        normals.Add(Vector3.left);
        normals.Add(Vector3.left);
        normals.Add(Vector3.left);
        normals.Add(Vector3.left);


        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMin));
        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMin));

        Indices.Add(currentIndex);
        Indices.Add(currentIndex + 1);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 0);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 3);
        currentIndex += 4;




    }


    void GenerateBlock_Forward(ref int currentIndex, Vector3Int offset, List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> Indices, Rect blockUVs)
    {
        verts.Add(new Vector3(0.5f, 0.5f, 0.5f) + offset);
        verts.Add(new Vector3(-0.5f, 0.5f, 0.5f) + offset);
        verts.Add(new Vector3(-0.5f, -0.5f, 0.5f) + offset);
        verts.Add(new Vector3(0.5f, -0.5f, 0.5f) + offset);

        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);
        normals.Add(Vector3.forward);


        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMin));
        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMin));

        Indices.Add(currentIndex);
        Indices.Add(currentIndex + 1);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 0);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 3);
        currentIndex += 4;




    }


    void GenerateBlock_Back(ref int currentIndex, Vector3Int offset, List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> Indices, Rect blockUVs)
    {
        verts.Add(new Vector3(-0.5f, 0.5f, -0.5f) + offset);
        verts.Add(new Vector3(0.5f, 0.5f, -0.5f) + offset);
        verts.Add(new Vector3(0.5f, -0.5f, -0.5f) + offset);
        verts.Add(new Vector3(-0.5f, -0.5f, -0.5f) + offset);

        normals.Add(Vector3.back);
        normals.Add(Vector3.back);
        normals.Add(Vector3.back);
        normals.Add(Vector3.back);


        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMin));
        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMin));

        Indices.Add(currentIndex);
        Indices.Add(currentIndex + 1);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 0);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 3);
        currentIndex += 4;




    }


    void GenerateBlock_Bottom(ref int currentIndex, Vector3Int offset, List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> Indices, Rect blockUVs)
    {
        verts.Add(new Vector3(-0.5f, -0.5f, -0.5f) + offset);
        verts.Add(new Vector3(0.5f, -0.5f, -0.5f) + offset);
        verts.Add(new Vector3(0.5f, -0.5f, 0.5f) + offset);
        verts.Add(new Vector3(-0.5f, -0.5f, 0.5f) + offset);

        normals.Add(Vector3.down);
        normals.Add(Vector3.down);
        normals.Add(Vector3.down);
        normals.Add(Vector3.down);


        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMax));
        uvs.Add(new Vector2(blockUVs.xMax, blockUVs.yMin));
        uvs.Add(new Vector2(blockUVs.xMin, blockUVs.yMin));

        Indices.Add(currentIndex);
        Indices.Add(currentIndex + 1);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 0);
        Indices.Add(currentIndex + 2);
        Indices.Add(currentIndex + 3);
        currentIndex += 4;




    }


}

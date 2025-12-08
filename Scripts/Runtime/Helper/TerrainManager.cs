using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;

    private Terrain closestTerrain, terrainLastFrame;
    private TerrainData _terrainData;
    private int alphamapWidth;
    private int alphamapHeight;

    private float[,,] _splatmapData;
    private int _numTextures;


    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    private void Update()
    {
        GetTerrainProps();
    }


    private void GetTerrainProps()
    {
        if (closestTerrain == null)
        {
            if (Terrain.activeTerrain != null) closestTerrain = Terrain.activeTerrain;
            else return;
        }
        if (terrainLastFrame == closestTerrain) return;

        _terrainData = closestTerrain.terrainData;
        alphamapWidth = _terrainData.alphamapWidth;
        alphamapHeight = _terrainData.alphamapHeight;

        _splatmapData = _terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        _numTextures = _splatmapData.Length / (alphamapWidth * alphamapHeight);

        terrainLastFrame = closestTerrain;
    }
    /*private Terrain GetClosestTerrain()
    {
        float closestRange = float.MaxValue;
        float distanceFromPlayer = 0;

        if (Terrain.activeTerrains.Length <= 0) return null;

        for (int i = 0; i < Terrain.activeTerrains.Length; i++)
        {
            distanceFromPlayer = Vector3.Distance(Terrain.activeTerrains[0].GetPosition(), Player.Instance.transform.position);
            if (distanceFromPlayer < closestRange)
            {
                closestRange = distanceFromPlayer;
                closestTerrain = Terrain.activeTerrains[i];
            }
        }

        return closestTerrain;
    }*/
    private Vector3 ConvertToSplatMapCoordinate(Vector3 playerPos)
    {
        Vector3 vecRet = new Vector3();
        if (Terrain.activeTerrain.terrainData == null) return vecRet;

        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        vecRet.x = ((playerPos.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        vecRet.z = ((playerPos.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return vecRet;
    }
    private int GetActiveTerrainTextureIdx(Vector3 pos)
    {
        if (Terrain.activeTerrain.terrainData == null) return 0;

        Vector3 TerrainCord = ConvertToSplatMapCoordinate(pos);
        int ret = 0;
        float comp = 0f;
        for (int i = 0; i < _numTextures; i++)
        {
            if (comp < _splatmapData[(int)TerrainCord.z, (int)TerrainCord.x, i])
                ret = i;
        }
        return ret;
    }

    public int GetTerrainAtPosition(Vector3 pos)
    {
        if(Terrain.activeTerrain == null) return 0;
        if (Terrain.activeTerrain.terrainData == null) return 0;

        int terrainIdx = GetActiveTerrainTextureIdx(pos);
        return terrainIdx;
    }
    public TerrainData GetTerrainData() => _terrainData;
    public Terrain GetCurrentTerrain() => closestTerrain;
    public void SetCurrentTerrain(Terrain _terrain)
    {
        closestTerrain = _terrain;
        GetTerrainProps();
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageGenerate : MonoBehaviour
{
    [Header("맵 크기 설정")]
    public int size = 10; // n x n 맵 크기

    [Header("타일들")]
    public Tile floorTile;
    public Tile wallTile;
    public Tile specialTile;

    [Header("프리셋 모듈 Prefab들")]
    public GameObject[] presetModules;

    private Tilemap tilemap;
    private GameObject[,] grid;

    // 시작/보스 좌표 (예시)
    private Vector2Int startPos = new Vector2Int(0, 0);
    private Vector2Int bossPos;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        grid = new GameObject[size, size];
        bossPos = new Vector2Int(size - 1, size - 1);

        GenerateValidMap();
    }

    void GenerateValidMap()
    {
        bool valid = false;
        int attempts = 0;

        while (!valid && attempts < 20) // 최대 20번 시도
        {
            ClearMap();
            GenerateMap();
            valid = CheckConnectivity(startPos, bossPos);
            attempts++;
        }

        if (!valid)
        {
            Debug.LogError("맵 생성 실패: 연결성 확보 불가");
        }
    }

    void GenerateMap()
    {
        // 1. 프리셋 모듈 배치
        foreach (GameObject prefab in presetModules)
        {
            PresetModule preset = prefab.GetComponent<PresetModule>();
            if (preset != null)
            {
                PlacePreset(preset.targetX, preset.targetY, prefab);
            }
        }

        // 2. 랜덤 타일맵 생성
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] == null) // 프리셋 모듈이 없는 칸
                {
                    Tile chosenTile = GetRandomTile();
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    tilemap.SetTile(pos, chosenTile);
                }
            }
        }
    }

    void ClearMap()
    {
        tilemap.ClearAllTiles();
        grid = new GameObject[size, size];
    }

    void PlacePreset(int x, int y, GameObject prefab)
    {
        Vector3 pos = new Vector3(x, y, 0);
        grid[x, y] = Instantiate(prefab, pos, Quaternion.identity);
    }

    Tile GetRandomTile()
    {
        float rand = Random.value;
        if (rand < 0.7f) return floorTile;   // 70% 바닥
        else if (rand < 0.9f) return wallTile; // 20% 벽
        else return specialTile;             // 10% 특수 타일
    }

    // BFS 연결성 검사
    bool CheckConnectivity(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] dirs = {
             new Vector2Int(1, 0),
             new Vector2Int(-1, 0),
             new Vector2Int(0, 1),
             new Vector2Int(0, -1)
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == goal) return true;

            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;

                if (IsWalkable(next) && !visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }
        return false;
    }

    bool IsWalkable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= size || pos.y >= size) return false;

        Tile tile = tilemap.GetTile<Tile>(new Vector3Int(pos.x, pos.y, 0));
        if (tile == wallTile) return false; // 벽은 못 지나감
        return true;
    }

}

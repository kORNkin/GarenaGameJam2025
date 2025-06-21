using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{
    [SerializeField] List<RuleTile> tiles;
    [SerializeField] Tilemap map;
    [SerializeField] GameObject tilePrefab;

    private int N = 12;

    private int[,] grid;
    private bool[,] visited;
    private System.Random random;

    public List<GameObject> houses;
    void Start()
    {
        GenerateMap();
        //GenerateMapQueue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMap()
    {
        (int h, int w)[] types = new (int h, int w)[] {
            (1, 1), (1, 1), (1, 1), 
            (1, 2), (1, 2), (1, 2), 
            (2, 1), (2, 1), (2, 1),
            (2, 2), (2, 2), (2, 2)
        };

        int tileId = 1;


        grid = new int[N, N];
        visited = new bool[N, N];
        random = new System.Random();

        for(int r = 1; r < N - 1; r++)
        {
            for(int c = 1; c < N - 1; c++)
            {
                if (visited[r, c]) continue;

                var candidates = new List<(int h, int w, int t)>();

                for(int i = 0; i < types.Count(); i++)
                {
                    int h = types[i].h;
                    int w = types[i].w;          
                    if (Fits(r, c, h, w) /*&& Fits(c, r, w, h)*/)
                        candidates.Add((h, w, i));
                }

                if(candidates.Count == 0)
                {
                    candidates.Add((1, 1, 0));
                }

                var (bh, bw, typeIndex) = candidates[random.Next(candidates.Count)];

                Fill(r, c, bh, bw, tileId, typeIndex);
                //Fill(c, r, bw, bh, tileId, (typeIndex >= 3 && typeIndex <= 5? 11 - typeIndex: typeIndex));
                tileId++;
            }
        }
    }
    private bool Fits(int r, int c, int h, int w)
    {
        if (r + h >= N - 1 || c + w >= N - 1) return false;

        for (int dr = 0; dr < h; dr++)
        {
            for (int dc = 0; dc < w; dc++)
            {
                if (visited[r + dr, c + dc]) return false;
            }
        }

        return true;
    }

    private void Fill(int r, int c, int h, int w, int tileId, int typeIndex)
    {
        for (int dr = 0; dr < h; dr++)
        {
            for (int dc = 0; dc < w; dc++)
            {
                if (visited[r + dr, c + dc]) continue;
                visited[r + dr, c + dc] = true;
                grid[r + dr, c + dc] = tileId;
                map.SetTile(new Vector3Int(r + dr - (N/2), c + dc - (N/2), 0), tiles[typeIndex]);
                var worldPos = map.CellToWorld(new Vector3Int(r + dr - (N / 2), c + dc - (N / 2), 0)) + map.tileAnchor;

                worldPos += new Vector3(-0.13f, -0.15f, 0); // Modify Pos

                houses.Add(Instantiate(tilePrefab, worldPos, Quaternion.identity));

            }
        }
        (int x, int y)[] diff = new (int x, int y)[]
        {
            (0,1), (1,0), (1,1), (0,-1), (-1,0), (-1,-1), (-1,1), (1,-1)
        };

        for (int dr = 0; dr < h; dr++)
        {
            for (int dc = 0; dc < w; dc++)
            {
                foreach(var (x, y) in diff)
                {
                    if (Valid(r + dr + x, c + dc + y)) 
                        visited[r + dr + x, c + dc + y] = true;
                }
            }
        }
    }
    bool Valid(int x, int y)
    {
        if (x < N && y < N && x >= 0 && y >= 0) return true;
        return false;
    }

    Queue<(int r, int c)> q = new Queue<(int r, int c)> ();

    void GenerateMapQueue()
    {
        (int h, int w)[] types = new (int h, int w)[] {
            (1, 1), (1, 1), (1, 1),
            (1, 2), (1, 2), (1, 2),
            (2, 1), (2, 1), (2, 1),
            (2, 2), (2, 2), (2, 2)
        };
        
        grid = new int[N, N];
        visited = new bool[N, N];
        random = new System.Random();

        int tileId = 1;
        q.Enqueue((1, 1));
        q.Enqueue((N - 2, N - 2));
        q.Enqueue((N - 2, 1));
        q.Enqueue((1, N - 2));

        while (q.Count > 0)
        {
            (int r, int c) = q.Dequeue();
            if (r < N - 1) q.Enqueue((r + 1, c));
            if (c < N - 1) q.Enqueue((r ,c + 1));

            if (visited[r, c]) continue;

            var candidates = new List<(int h, int w, int t)>();

            for (int i = 0; i < types.Count(); i++)
            {
                int h = types[i].h;
                int w = types[i].w;
                if (Fits(r, c, h, w) /*&& Fits(c, r, w, h)*/)
                    candidates.Add((h, w, i));
            }

            if (candidates.Count == 0)
            {
                candidates.Add((1, 1, 0));
            }

            var (bh, bw, typeIndex) = candidates[random.Next(candidates.Count)];

            Fill(r, c, bh, bw, tileId, typeIndex);
            //Fill(c, r, bw, bh, tileId, (typeIndex >= 3 && typeIndex <= 8 ? 11 - typeIndex : typeIndex));
            tileId++;                  
        }
    }
}

using System.Collections.Generic; // Add this line
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the tilemap
    public RuleTile floorTile; // Assign this in the Inspector

    public int width = 100;
    public int height = 100;
    private int[,] dungeonGrid;
    private List<Room> rooms = new List<Room>();

    void Start()
    {
        dungeonGrid = new int[width, height];
        GenerateDungeon();
        RenderDungeon();
    }

    void GenerateDungeon()
    {
        BSP(new RectInt(0, 0, width, height), 4);
        ConnectRooms();
    }

    void BSP(RectInt space, int depth)
    {
        if (depth <= 0 || space.width < 10 || space.height < 10) return;

        bool splitVertically = space.width > space.height;
        int splitPoint = splitVertically
            ? Random.Range(4, space.width - 4)
            : Random.Range(4, space.height - 4);

        RectInt space1, space2;

        if (splitVertically)
        {
            space1 = new RectInt(space.x, space.y, splitPoint, space.height);
            space2 = new RectInt(space.x + splitPoint, space.y, space.width - splitPoint, space.height);
        }
        else
        {
            space1 = new RectInt(space.x, space.y, space.width, splitPoint);
            space2 = new RectInt(space.x, space.y + splitPoint, space.width, space.height - splitPoint);
        }

        BSP(space1, depth - 1);
        BSP(space2, depth - 1);

        CreateRoom(space);
    }

    void CreateRoom(RectInt space)
    {
        int roomWidth = Random.Range(4, space.width - 2);
        int roomHeight = Random.Range(4, space.height - 2);
        int roomX = Random.Range(space.x + 1, space.x + space.width - roomWidth - 1);
        int roomY = Random.Range(space.y + 1, space.y + space.height - roomHeight - 1);

        Room newRoom = new Room(roomX, roomY, roomWidth, roomHeight);
        rooms.Add(newRoom);

        for (int x = newRoom.x; x < newRoom.x + newRoom.width; x++)
        {
            for (int y = newRoom.y; y < newRoom.y + newRoom.height; y++)
            {
                dungeonGrid[x, y] = 1;
            }
        }
    }

    void ConnectRooms()
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Room roomA = rooms[i];
            Room roomB = rooms[i + 1];

            Vector2Int pointA = roomA.GetCenter();
            Vector2Int pointB = roomB.GetCenter();

            CreateCorridor(pointA, pointB);
        }
    }

    void CreateCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        while (current.x != end.x)
        {
            dungeonGrid[current.x, current.y] = 1;
            current.x += (end.x > current.x) ? 1 : -1;
        }

        while (current.y != end.y)
        {
            dungeonGrid[current.x, current.y] = 1;
            current.y += (end.y > current.y) ? 1 : -1;
        }
    }

    void RenderDungeon()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (dungeonGrid[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }
    }
}

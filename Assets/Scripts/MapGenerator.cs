using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int width = 50;
    public int height = 50;

    [Header("Room Settings")]
    public int maxRooms = 10;
    public int minRoomSize = 5;
    public int maxRoomSize = 10;

    [Header("Tilemap References")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [Header("Tiles")]
    public TileBase floorTile;
    public TileBase wallTile;

    [Header("Corridor Settings")]
    [Tooltip("������ ��(Ÿ�� ��)")]
    public int corridorWidth = 3;

    [Header("Player Spawn")]
    public Transform player;     // ���� ��ġ�� Player ������Ʈ
    public Vector3 spawnOffset;  // Ÿ�� �߽� ������(��: (0.5f,0.5f,0))

    [Header("Enemy (Mop) Spawn")]
    public GameObject mopPrefab;            // Mop ������
    public int minMopsPerRoom = 1;          // ��� �ּ� Mop ��
    public int maxMopsPerRoom = 3;          // ��� �ִ� Mop ��

    private List<Rect> rooms = new List<Rect>();

    void Start()
    {
        GenerateMap();
        PlacePlayerInFirstRoom();
    }

    void GenerateMap()
    {
        // 1) �� ��ü�� ������ ä���
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);

        // 2) �� ����
        for (int i = 0; i < maxRooms; i++)
        {
            int rw = Random.Range(minRoomSize, maxRoomSize + 1);
            int rh = Random.Range(minRoomSize, maxRoomSize + 1);
            int rx = Random.Range(1, width - rw - 1);
            int ry = Random.Range(1, height - rh - 1);

            Rect newRoom = new Rect(rx, ry, rw, rh);
            bool overlaps = false;
            foreach (var room in rooms)
                if (newRoom.Overlaps(room)) { overlaps = true; break; }

            if (!overlaps)
            {
                CreateRoom(newRoom);

                // ���� �� �߽ɰ� ���� ����
                if (rooms.Count > 0)
                {
                    Vector2Int prevCenter = GetCenter(rooms[rooms.Count - 1]);
                    Vector2Int newCenter = GetCenter(newRoom);
                    CreateCorridor(prevCenter, newCenter);
                }

                rooms.Add(newRoom);

                //�� �ȿ� Mop ���� ����
                SpawnMopsInRoom(newRoom);

                rooms.Add(newRoom);
                // ù ��° ��(�ε��� 0)���� �� �������� �ʱ�
                if (rooms.Count > 1)
                    SpawnMopsInRoom(newRoom);
            }
        }
    }

    void CreateRoom(Rect room)
    {
        for (int x = (int)room.xMin; x < (int)room.xMax; x++)
            for (int y = (int)room.yMin; y < (int)room.yMax; y++)
            {
                var cell = new Vector3Int(x, y, 0);
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
    }

    void CreateCorridor(Vector2Int a, Vector2Int b)
    {
        int half = corridorWidth / 2;
        // ����
        int xMin = Mathf.Min(a.x, b.x), xMax = Mathf.Max(a.x, b.x);
        for (int x = xMin; x <= xMax; x++)
            for (int dy = -half; dy <= half; dy++)
            {
                var c = new Vector3Int(x, a.y + dy, 0);
                floorTilemap.SetTile(c, floorTile);
                wallTilemap.SetTile(c, null);
            }
        // ����
        int yMin = Mathf.Min(a.y, b.y), yMax = Mathf.Max(a.y, b.y);
        for (int y = yMin; y <= yMax; y++)
            for (int dx = -half; dx <= half; dx++)
            {
                var c = new Vector3Int(b.x + dx, y, 0);
                floorTilemap.SetTile(c, floorTile);
                wallTilemap.SetTile(c, null);
            }
    }

    Vector2Int GetCenter(Rect room)
        => new Vector2Int(
            (int)(room.xMin + room.width / 2),
            (int)(room.yMin + room.height / 2)
        );
    void PlacePlayerInFirstRoom()
    {
        if (player == null || rooms.Count == 0) return;

        // 1) ù ���� �߾� ��������
        Vector2Int center = GetCenter(rooms[0]);

        // 2) Cell �� ���� ��ǥ ��ȯ
        // �׸��� �� �Ѱ���� Ÿ�� ũ��(1����)�� ������ 0.5�� ����
        Vector3 worldPos = floorTilemap.CellToWorld(
            new Vector3Int(center.x, center.y, 0))
            + spawnOffset;

        // 3) �÷��̾� ��ġ ����
        player.position = worldPos;
    }

    void SpawnMopsInRoom(Rect room)
    {
        if (mopPrefab == null || player == null) return;

        int count = Random.Range(minMopsPerRoom, maxMopsPerRoom + 1);
        var playerRig = player.GetComponent<Rigidbody2D>();

        for (int i = 0; i < count; i++)
        {
            int x = Random.Range((int)room.xMin + 1, (int)room.xMax - 1);
            int y = Random.Range((int)room.yMin + 1, (int)room.yMax - 1);

            Vector3 spawnPos = floorTilemap.CellToWorld(
                new Vector3Int(x, y, 0)
            ) + new Vector3(0.5f, 0.5f, 0f);

            GameObject mopGO = Instantiate(mopPrefab, spawnPos, Quaternion.identity);
            Mop mop = mopGO.GetComponent<Mop>();
            if (mop != null)
                mop.target = playerRig;
        }
    }
}


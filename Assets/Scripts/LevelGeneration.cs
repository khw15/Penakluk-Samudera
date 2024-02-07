using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelGeneration : MonoBehaviour
{
    Vector2 WorldSize = new Vector2(100, 100);
    Room[,] Rooms;
    public GameObject[,] InstantiatedRooms;
    List<Vector2> TakenPositions = new List<Vector2>();
    int GridSizeX, GridSizeY;
    public int NumRooms;
    //public GameObject MapSprite;
    //public float MapRoomGap;
    public float RoomGapX;
    public float RoomGapY;
    private Vector2 index;
    public int BossDepth;

    void Start()
    {
        if (NumRooms < Math.Abs(BossDepth))
            Debug.LogWarning("Ada lebih sedikit ruangan dari kedalaman bos yang ditentukan, tidak ada ruang bos yang akan muncul.");

        if (NumRooms >= ((WorldSize.x * 2) * (WorldSize.y * 2)) )
            NumRooms = Mathf.RoundToInt((WorldSize.x * 2) * WorldSize.y * 2);
        GridSizeX = Mathf.RoundToInt(WorldSize.x);
        GridSizeY = Mathf.RoundToInt(WorldSize.y);

        CreateRooms();
        SetRoomDoors();
        DrawMap();
        CreateTunnels();
    }

    void CreateRooms()
    {
        InstantiatedRooms = new GameObject[GridSizeX * 2, GridSizeY * 2];
        Rooms = new Room[GridSizeX * 2, GridSizeY * 2];
        Rooms[GridSizeX, GridSizeY] = new Room(Vector2.zero, 1);
        TakenPositions.Insert(0, Vector2.zero);
        Vector2 CheckPos = Vector2.zero;

        float RandomCompare = 0.2f, RandomCompareStart = 0.2f, RandomCompareEnd = 0.01f;
    
        for (int i = 0; i < NumRooms-1; i++)
        {
            float RandomPerc = ((float) i / ((float) NumRooms-1));

            if (!ApproachingRoomLimit() || GetDeepestRoom().y == BossDepth)
            {
                RandomCompare = Mathf.Lerp(RandomCompareStart, RandomCompareEnd, RandomPerc);
                CheckPos = FindNewValidRoomPos();

                if (NumberOfNeighbors(CheckPos, TakenPositions) > 1 && UnityEngine.Random.value > RandomCompare)
                {
                    int Iterations = 0;

                    while (NumberOfNeighbors(CheckPos, TakenPositions) > 1 && Iterations < 50)
                    {
                        CheckPos = FindNewValidRoomPos();
                        Iterations++;
                    }
                }
            }
            else
            {
                CheckPos = new Vector2(GetDeepestRoom().x, GetDeepestRoom().y - 1);
            }
 
            Rooms[(int)CheckPos.x + GridSizeX, (int)CheckPos.y + GridSizeY] = new Room(CheckPos, 0);
            TakenPositions.Insert(0, CheckPos);
        }
    }

    Vector2 FindNewValidRoomPos()
    {
        int x = 0, y = 0;
        Vector2 CheckingPos = Vector2.zero;
        
        while (TakenPositions.Contains(CheckingPos) || x >= GridSizeX || x < -GridSizeX || y >= GridSizeY || y < -GridSizeY)
        {
            do
            {
                int Index = Mathf.RoundToInt(UnityEngine.Random.value * (TakenPositions.Count - 1));
                
                x = (int) TakenPositions[Index].x;
                y = (int) TakenPositions[Index].y;                    
            }
            while ((y == BossDepth) && (GetDeepestRoom().y == BossDepth));

            bool Down, Right;

            Down = (UnityEngine.Random.value < 0.9f);
            Right = (UnityEngine.Random.value < 0.5f);
            
            if (Down)
                y -= 1;
            else if (Right)
                x += 1;
            else
                x -= 1;

            CheckingPos = new Vector2(x,y);
        }

        return CheckingPos;
    }

    int NumberOfNeighbors (Vector2 CheckingPos, List<Vector2> UsedPositions)
    {
        int NumNeighbors = 0;
        if (UsedPositions.Contains(CheckingPos + Vector2.right))
            NumNeighbors++;
        if (UsedPositions.Contains(CheckingPos + Vector2.left))
            NumNeighbors++;
        if (UsedPositions.Contains(CheckingPos + Vector2.up))
            NumNeighbors++;
        if (UsedPositions.Contains(CheckingPos + Vector2.down))
            NumNeighbors++;

        return NumNeighbors;
    }

    void SetRoomDoors()
    {
        for (int x = 0; x < (GridSizeX * 2); x++)
            for (int y = 0; y < (GridSizeY * 2); y++)
            {
                if (Rooms[x,y] == null)
                    continue;

                Vector2 GridPosition = new Vector2(x,y);

                // Check Above
                if (y - 1 < 0)
                    Rooms[x,y].DoorBot = false;
                else
                    Rooms[x,y].DoorBot = (Rooms[x, y-1] != null);

                // Check Below
                if (y + 1 >= GridSizeY * 2)
                    Rooms[x,y].DoorTop = false;
                else
                    Rooms[x,y].DoorTop = (Rooms[x, y + 1] != null);

                //  Check Left
                if (x - 1 < 0)
                    Rooms[x,y].DoorLeft = false;
                else
                    Rooms[x,y].DoorLeft = (Rooms[x - 1, y] != null);

                // Check Right
                if (x + 1 >= GridSizeX * 2)
                    Rooms[x,y].DoorRight = false;
                else   
                    Rooms[x,y].DoorRight = (Rooms[x + 1, y] != null);
            }
    }

    void DrawMap()
    {
        foreach (Room R in Rooms)
        {
            if (R == null)
                continue;

            Vector2 DrawPos = R.GridPos;
            DrawPos.x *= RoomGapX/100;
            DrawPos.y *= RoomGapY/100;

            R.Type = Mathf.Abs((int) R.GridPos.y / 5);
            GameObject RoomPrefab = Instantiate(GameObject.Find("Room" + R.Type.ToString()), DrawPos, Quaternion.identity);

            index = ExtensionMethods.CoordinatesOf<Room>(Rooms, R);
            InstantiatedRooms[(int)index.x, (int)index.y] = RoomPrefab;
        }
    }

    void CreateTunnels()
    {
        foreach (Room R in Rooms)
        {
            if ((R == null) || (R.Type == 4))
                continue;

            index = ExtensionMethods.CoordinatesOf<Room>(Rooms, R);
            GameObject CurrentRoom = InstantiatedRooms[(int)index.x, (int)index.y];

            if (R.DoorBot)
                Destroy(CurrentRoom.transform.GetChild(1).Find("BotDoorGrid").gameObject);
            if (R.DoorTop)
                Destroy(CurrentRoom.transform.GetChild(1).Find("TopDoorGrid").gameObject);
            if (R.DoorLeft)
                Destroy(CurrentRoom.transform.GetChild(1).Find("LeftDoorGrid").gameObject);
            if (R.DoorRight)
                Destroy(CurrentRoom.transform.GetChild(1).Find("RightDoorGrid").gameObject);
        }
    }

    private Vector2 GetDeepestRoom()
    {
        Vector2 MaxDepth = Vector2.zero;
        foreach (Vector2 Pos in TakenPositions)
        {
            if (Pos.y < MaxDepth.y)
                MaxDepth = Pos;
        }
        return MaxDepth;
    }

    private bool ApproachingRoomLimit()
    {
        int DeepestRoomDepth = (int) GetDeepestRoom().y;
        
        if (NumRooms - TakenPositions.Count <= Math.Abs(BossDepth) - Math.Abs(DeepestRoomDepth))
            return true;
        else if (DeepestRoomDepth >= BossDepth)
            return false;
        else
            return false;
    }
}
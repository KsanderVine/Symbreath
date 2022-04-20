using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///To-Do:
///Поиск в зоне
///Поиск в радиусе
///Счет в зоне всех символов
///Счет в зоне определенных символов
///Определение путей

public class Map : MonoBehaviour
{
    public static Map instance { get; private set; }
    
    protected int gameTurn;
    protected int seed;
    protected char[] mapGrid;

    public int mapSizeX = 100;
    public int mapSizeY = 100;

    private float turnDelay;
    private float delayTime = 0.025f;
    
    [Header("UI:")]
    public UIGrid mapGridUI;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        Generate();
    }

    public static void Generate()
    {
        instance._Generate();
    }
    private void _Generate ()
    {
        seed = Random.Range(0, 10000);

        gameTurn = 0;
        turnDelay = delayTime;
        mapGrid = new char[mapSizeX * mapSizeY];

        Symbol ground = Symbols.Get(0);
        for (int i = 0; i < (mapSizeX * mapSizeY); i++)
        {
            mapGrid[i] = '#';
        }

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                float noizeValue = Noise.Generate((seed + x) * 0.04f, y * 0.125f);
                if(noizeValue > 0)
                    SetMapSymbol('.', new Vector2Int(x,y));
            }
        }
        for (int i = 0; i < mapSizeX * 10; i++)
            SetMapSymbol('~', i, '.');

        for (int i = 0; i < mapSizeX * 10; i++)
            SetMapSymbol('`', 3000 + i, '.');
    }

    public static bool IsGround (int index)
    {
        if(IsOutOfRange(index) == false)
            return instance.mapGrid[index] == Symbols.groundChar;
        return false;
    }

    public static bool IsGround(Vector2Int point)
    {
        return GetOnMap(point) == Symbols.groundChar;
    }

    public static char GetOnMap(Vector2Int point)
    {
        if (!IsOutOfRange(point))
        {
            int index = ToSymbolIndex(point);
            return instance.mapGrid[index];
        }
        return ' ';
    }

    public static char GetOnMap(int index)
    {
        if (!IsOutOfRange(index))
        {
            return instance.mapGrid[index];
        }
        return ' ';
    }

    public static void SetMapSymbol (char symbolID, Vector2Int point)
    {
        instance._SetMapSymbol(symbolID, point);
    }
    public static void SetMapSymbol(char symbolID, Vector2Int point, params char[] condition)
    {
        if (IsAnyOfCondition(point, condition))
            instance._SetMapSymbol(symbolID, point);
    }
    private void _SetMapSymbol(char symbolID, Vector2Int point)
    {
        if (IsOutOfRange(point))
            return;

        int index = ToSymbolIndex(point);
        mapGrid[index] = symbolID;
    }

    public static void SetMapSymbol(char symbolID, int index)
    {
        instance._SetMapSymbol(symbolID, index);
    }
    public static void SetMapSymbol(char symbolID, int index, params char[] condition)
    {
        if (IsAnyOfCondition(index, condition))
            instance._SetMapSymbol(symbolID, index);
    }
    private void _SetMapSymbol(char symbolID, int index)
    {
        if (IsOutOfRange(index))
            return;
        mapGrid[index] = symbolID;
    }

    public static bool IsAnyOfCondition (char symbolID, params char[] condition)
    {
        for(int i = 0; i < condition.Length; i++)
        {
            if (condition[i] == symbolID)
                return true;
        }
        return false;
    }

    public static bool IsAnyOfCondition(int index, params char[] condition)
    {
        char symbolID = GetOnMap(index);
        for (int i = 0; i < condition.Length; i++)
        {
            if (condition[i] == symbolID)
                return true;
        }
        return false;
    }

    public static bool IsAnyOfCondition(Vector2Int point, params char[] condition)
    {
        char symbolID = GetOnMap(point);
        for (int i = 0; i < condition.Length; i++)
        {
            if (condition[i] == symbolID)
                return true;
        }
        return false;
    }

    public static void MoveSymbol (int indexFrom, int indexTo)
    {
        char from = GetOnMap(indexFrom);
        char to = GetOnMap(indexTo);

        if (from == ' ' || to == ' ')
            return;

        SetMapSymbol(from, indexTo);
        SetMapSymbol(to, indexFrom);
    }

    public static void MoveSymbol(Vector2Int pointFrom, Vector2Int pointTo)
    {
        char from = GetOnMap(pointFrom);
        char to = GetOnMap(pointTo);

        if (from == ' ' || to == ' ')
            return;

        SetMapSymbol(from, pointTo);
        SetMapSymbol(to, pointFrom);
    }

    public static bool IsOutOfRange (Vector2Int point)
    {
        return instance._IsOutOfRange(point.x, point.y);
    }
    public static bool IsOutOfRange (int x, int y)
    {
        return instance._IsOutOfRange(x, y);
    }
    private bool _IsOutOfRange (int x, int y)
    {
        return (x < 0 || x >= mapSizeX || y < 0 || y >= mapSizeY);
    }

    public static bool IsOutOfRange(int index)
    {
        return instance._IsOutOfRange(index);
    }
    private bool _IsOutOfRange(int index)
    {
        return (index < 0 || index >= mapGrid.Length);
    }

    public static Vector2Int ToGridCoordinates (int index)
    {
        int x = GetX(index);
        int y = GetY(index);
        return new Vector2Int(x, y);
    }

    public static int ToSymbolIndex(Vector2Int gridCoordinates)
    {
        return gridCoordinates.y * instance.mapSizeX + gridCoordinates.x;
    }

    public static int GetX(int index)
    {
        return Mathf.Clamp(Mathf.FloorToInt(index % instance.mapSizeX), 0, instance.mapSizeX - 1);
    }

    public static int GetY(int index)
    {
        return Mathf.Clamp(Mathf.FloorToInt(index / instance.mapSizeX), 0, instance.mapSizeY - 1);
    }

    public static int Offset (int index, Vector2Int offset)
    {
        index += (offset.y * instance.mapSizeX);
        index += offset.x;
        return index;
    }

    private void Tick ()
    {
        string mapGrid = string.Concat(this.mapGrid);

        for(int i = 0; i < mapGrid.Length; i++)
        {
            Symbol symbol = Symbols.Get(mapGrid[i]);
            if (symbol != null)
                symbol.Simulate(gameTurn, i);
            else
                SetMapSymbol('.', i);
        }

        string mapText = string.Empty;

        for (int i = 0; i < mapGrid.Length; i += mapSizeX)
        {
            mapText += "\n" + mapGrid.Substring(i, mapSizeX);
        }

        mapGridUI.map.text = mapText;
    }

    public void FixedUpdate()
    {
        if(mapGridUI.map.isFocused)
        {
            return;
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            Generate();
            Tick();
            return;
        }

        turnDelay -= Time.fixedDeltaTime;
        if(turnDelay < 0)
        {
            turnDelay = delayTime;
            gameTurn++;
            Tick();
        }
    }
}

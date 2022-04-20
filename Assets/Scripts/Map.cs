using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public static Map instance;
    public void Awake() { instance = this; }

    public int gameTurn;
    public string mapGrid;

    public int mapSizeX = 100;
    public int mapSizeY = 100;

    private float turnDelay;
    private float delayTime = 0.1f;
    
    [Header("UI:")]
    public Text mapGridUI;
    
    public void Start()
    {
        Generate();
    }

    public static void Generate() { instance._Generate(); }
    private void _Generate ()
    {
        gameTurn = 0;
        turnDelay = delayTime;
        mapGrid = string.Empty;

        Symbol ground = Symbols.Get(0);
        for (int i = 0; i < (mapSizeX * mapSizeY); i++)
        {
            mapGrid += ground.symbolID;
        }

        SetMapSymbol('@', ToGridCoordinates(Random.Range(0, (mapSizeX * mapSizeY) - 1)));
        
        for (int i = 0; i < mapSizeX * 14; i++)
            SetMapSymbol('~', ToGridCoordinates(Random.Range(0, (mapSizeX * mapSizeY) - 1)));


        /*SetMapSymbol('i', ToGridCoordinates(Random.Range(0, (mapSizeX * mapSizeY) - 1)));
        SetMapSymbol('i', ToGridCoordinates(Random.Range(0, (mapSizeX * mapSizeY) - 1)));
        SetMapSymbol('i', ToGridCoordinates(Random.Range(0, (mapSizeX * mapSizeY) - 1)));
        SetMapSymbol('i', ToGridCoordinates(Random.Range(0, (mapSizeX * mapSizeY) - 1)));
        SetMapSymbol('i', ToGridCoordinates(Random.Range(0, (mapSizeX * mapSizeY) - 1)));*/
    }

    //Поиск в зоне, в радиусе
    //Счет в зоне всех символов
    //Счет в зоне определенных символов
    //Определение путей
    //Акселерометр

    public static bool IsGround (Vector2Int point)
    {
        return GetOnMap(point) == Symbols.groundChar;
    }

    public static char GetOnMap(Vector2Int point)
    {
        if (!IsOutOfRange(point))
        {
            int index = instance.ToSymbolIndex(point);
            return instance.mapGrid[index];
        }
        return ' ';
    }

    public static void SetMapSymbol (char symbolID, Vector2Int point)
    {
        instance._SetMapSymbol(symbolID, point);
    }
    public static void SetMapSymbol(char symbolID, Vector2Int point, char condition)
    {
        if(GetOnMap(point) == condition)
            instance._SetMapSymbol(symbolID, point);
    }
    private void _SetMapSymbol(char symbolID, Vector2Int point)
    {
        if (IsOutOfRange(point)) return;
        int index = ToSymbolIndex(point);
        System.Text.StringBuilder grid = new System.Text.StringBuilder(mapGrid);
        grid[index] = symbolID;
        mapGrid = grid.ToString();
    }

    public static void MoveSymbol (Vector2Int pointFrom, Vector2Int pointTo)
    {
        instance._MoveSymbol(pointFrom, pointTo);
    }
    private void _MoveSymbol (Vector2Int pointFrom, Vector2Int pointTo)
    {
        char from = GetOnMap(pointFrom);
        char to = GetOnMap(pointTo);
        if (from == ' ' || to == ' ') return;

        //Debug.Log(from + " > " + to);

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

    private Vector2Int ToGridCoordinates (int index)
    {
        int x = Mathf.Clamp(Mathf.FloorToInt(index % mapSizeX), 0, mapSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(index / mapSizeX), 0, mapSizeY - 1);
        return new Vector2Int(x, y);
    }
    private int ToSymbolIndex(Vector2Int gridCoordinates)
    {
        return gridCoordinates.y * mapSizeX + gridCoordinates.x;
    }

    private void Tick ()
    {
        string mapGrid = this.mapGrid;

        for(int i = 0; i < mapGrid.Length; i++)
        {
            Vector2Int gridCoordinates = ToGridCoordinates(i);
            Symbols.Get(mapGrid[i]).Simulate(gameTurn, gridCoordinates);
            /*if (mapGrid[i] == '@')
            {
                Debug.Log(i + " " + ToGridCoordinates(i) + " == " + ToSymbolIndex(ToGridCoordinates(i)));
            }*/
        }

        string mapText = string.Empty;

        for (int i = 0; i < mapGrid.Length; i += mapSizeX)
        {
            mapText += "\n" + mapGrid.Substring(i, mapSizeX);
        }

        mapGridUI.text = mapText;
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Generate();
            Tick();
            return;
        }

        turnDelay -= Time.deltaTime;
        if(turnDelay < 0)
        {
            turnDelay = delayTime;
            gameTurn++;
            Tick();
        }
    }
}

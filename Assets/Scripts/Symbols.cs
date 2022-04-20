using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbols : MonoBehaviour
{
    public static Symbols instance;
    public void Awake()
    {
        instance = this;
        Reload();
    }

    public static Dictionary<char, Symbol> symbolsBase { get; private set; }
    public static char groundChar { get; private set; } = '.';

    public static Symbol Get (char symbolID)
    {
        return symbolsBase[symbolID];
    }

    public static Symbol SafeGet(char symbolID)
    {
        if (symbolsBase.TryGetValue(symbolID, out Symbol symbol))
        {
            return symbol;
        }
        return null;
    }

    public static Symbol Get(int index)
    {
        return new List<Symbol>(symbolsBase.Values)[index];
    }

    private Symbol AddSymbol (Symbol symbol)
    {
        char symbolID = symbol.symbolID;
        if (symbolsBase.ContainsKey(symbolID))
        {
            Debug.Log("[Symbols] Symbol ''" + symbolID + "'' already existed!");
            return null;
        }
        else
        {
            symbolsBase.Add(symbolID, symbol);
            return symbol;
        }
    }

    private void Reload()
    {
        symbolsBase = new Dictionary<char, Symbol>();

        AddSymbol(new Symbol('.'));

        AddSymbol(new Symbol('~',
            (turn, point) =>
            {
                if (turn % 4 == 0)
                {
                    if (Map.IsGround(point + new Vector2Int(0, 1)))
                        Map.MoveSymbol(point, point + new Vector2Int(0, 1));
                    else if (Map.IsGround(point + new Vector2Int(1, 0)))
                        Map.MoveSymbol(point, point + new Vector2Int(1, 0));
                    else if (Map.IsGround(point + new Vector2Int(-1, 0)))
                        Map.MoveSymbol(point, point + new Vector2Int(-1, 0));
                }
            }));



        AddSymbol(new Symbol('i',
            (turn, point) => {
                if (turn % 100 == 0)
                    Map.SetMapSymbol('r', point);
            }));
        AddSymbol(new Symbol('r',
            (turn, point) => {
                if (turn % 220 == 0)
                    Map.SetMapSymbol('y', point);
            }));
        AddSymbol(new Symbol('y',
            (turn, point) => {
                if (turn % 290 == 0)
                    Map.SetMapSymbol('Y', point);
            }));
        AddSymbol(new Symbol('Y',
            (turn, point) => {
                if (turn % 380 == 0)
                {
                    for (int i = 0; i < Random.Range(0, 3); i++)
                    {
                        int x = Mathf.FloorToInt((Random.Range(-3, 4)));
                        int y = Mathf.FloorToInt((Random.Range(-3, 4)));
                        Map.SetMapSymbol('i', point + new Vector2Int(x, y), '.');
                    }
                }
            }));
        AddSymbol(new Symbol('@', 
            (turn, point) => {
                if (turn % 100 == 0)
                {
                    int x = Mathf.FloorToInt((Random.Range(-1, 2)));
                    int y = Mathf.FloorToInt((Random.Range(-1, 2)));
                    Map.SetMapSymbol('@', point + new Vector2Int(x, y), '.');
                }

                int moveX = Mathf.FloorToInt((Random.Range(-1, 2)));
                int moveY = Mathf.FloorToInt((Random.Range(-1, 2)));
                Vector2Int target = point + new Vector2Int(moveX, moveY);
                if (Map.IsGround(target))
                    Map.MoveSymbol(point, target);
            }));
    }
}


public class Symbol
{
    public readonly char symbolID;
    public readonly System.Action<int, Vector2Int> action;

    public Symbol(char symbolID)
    {
        this.symbolID = symbolID;
    }

    public Symbol(char symbolID, System.Action<int, Vector2Int> action)
    {
        this.symbolID = symbolID;
        this.action = action;
    }

    public virtual void Simulate(int turn, Vector2Int point)
    {
        if (action != null)
            action(turn, point);
    }
}
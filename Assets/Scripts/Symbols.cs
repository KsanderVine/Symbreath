using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbols : MonoBehaviour
{
    public static Symbols instance { get; private set; }

    public static Dictionary<char, Symbol> symbolsBase { get; private set; }
    public static char groundChar { get; private set; } = '.';

    protected static Vector2Int up = Vector2Int.down;
    protected static Vector2Int down = Vector2Int.up;
    protected static Vector2Int left = Vector2Int.left;
    protected static Vector2Int right = Vector2Int.right;

    protected static Vector2Int upLeft = Vector2Int.down + Vector2Int.left;
    protected static Vector2Int downLeft = Vector2Int.up + Vector2Int.left;
    protected static Vector2Int upRight = Vector2Int.down + Vector2Int.right;
    protected static Vector2Int downRight = Vector2Int.up + Vector2Int.right;

    public void Awake()
    {
        instance = this;
        ReloadSymbols();
    }

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
            Debug.Log($"[{nameof(Symbols)}] {nameof(Symbol)} ''{symbolID}'' already existed!");
            return null;
        }
        else
        {
            symbolsBase.Add(symbolID, symbol);
            return symbol;
        }
    }

    private void ReloadSymbols()
    {
        symbolsBase = new Dictionary<char, Symbol>();

        //Ground
        AddSymbol(new Symbol('.'));

        //Dirt
        AddSymbol(new Symbol('#'));

        //Water
        AddSymbol(new Symbol('~',
            (turn, index) =>
            {
                if (turn % 2 == 0)
                {
                    if (Map.IsAnyOfCondition(Map.Offset(index, down), '.', '`'))
                        Map.MoveSymbol(index, Map.Offset(index, down));

                    if (Map.IsAnyOfCondition(Map.Offset(index, downLeft), '.', '`'))
                        Map.MoveSymbol(index, Map.Offset(index, downLeft));

                    if (Map.IsAnyOfCondition(Map.Offset(index, downRight), '.', '`'))
                        Map.MoveSymbol(index, Map.Offset(index, downRight));

                    if (Map.IsAnyOfCondition(Map.Offset(index, left), '.', '`'))
                        Map.MoveSymbol(index, Map.Offset(index, left));

                    if (Map.IsAnyOfCondition(Map.Offset(index, right), '.', '`'))
                        Map.MoveSymbol(index, Map.Offset(index, right));
                }
            }));

        //Air
        AddSymbol(new Symbol('`',
            (turn, index) =>
            {
                if (turn % 2 == 0)
                {
                    if (Map.IsGround(Map.Offset(index, up)))
                        Map.MoveSymbol(index, Map.Offset(index, up));

                    if (Map.IsGround(Map.Offset(index, upRight)))
                        Map.MoveSymbol(index, Map.Offset(index, upRight));

                    if (Map.IsGround(Map.Offset(index, upLeft)))
                        Map.MoveSymbol(index, Map.Offset(index, upLeft));

                    if (Map.IsGround(Map.Offset(index, right)))
                        Map.MoveSymbol(index, Map.Offset(index, right));

                    if (Map.IsGround(Map.Offset(index, left)))
                        Map.MoveSymbol(index, Map.Offset(index, left));

                    if (Map.IsGround(Map.Offset(index, down)))
                        Map.MoveSymbol(index, Map.Offset(index, down));

                    if (Map.IsGround(Map.Offset(index, downLeft)))
                        Map.MoveSymbol(index, Map.Offset(index, downLeft));

                    if (Map.IsGround(Map.Offset(index, downRight)))
                        Map.MoveSymbol(index, Map.Offset(index, downRight));
                }
            }));

        AddSymbol(new Symbol('v',
            (turn, index) =>
            {
                if (turn % 220 == 0)
                {
                    if (Map.IsGround(Map.Offset(index, down)))
                        Map.SetMapSymbol('v', Map.Offset(index, down));
                }
            }));

        AddSymbol(new Symbol('i',
            (turn, index) => {
                if (turn % 100 == 0)
                    Map.SetMapSymbol('r', index);
            }));

        AddSymbol(new Symbol('r',
            (turn, index) => {
                if (turn % 220 == 0)
                    Map.SetMapSymbol('y', index);
            }));

        AddSymbol(new Symbol('y',
            (turn, index) => {
                if (turn % 290 == 0)
                    Map.SetMapSymbol('Y', index);
            }));

        AddSymbol(new Symbol('Y',
            (turn, index) => {
                if (turn % 380 == 0)
                    Map.SetMapSymbol('i', index);
            }));
    }
}


public class Symbol
{
    public readonly char symbolID;
    public readonly System.Action<int, int> action;

    public Symbol(char symbolID)
    {
        this.symbolID = symbolID;
    }

    public Symbol(char symbolID, System.Action<int, int> action)
    {
        this.symbolID = symbolID;
        this.action = action;
    }

    public virtual void Simulate(int turn, int index)
    {
        if (action != null)
            action(turn, index);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject squarePrefab;
    public Overlay overlay;
    public Player[] players;
    public Color[] colorPool = {
        Color.blue,
        Color.red,
        Color.green,
        Color.gray,
        Color.cyan,
        Color.magenta
    };

    Square[,] squares;
    Puzzle puzzle;
    bool finished = false;
    // Start is called before the first frame update
    void Start()
    {
        NewGame();
        overlay.OnNewGame.AddListener(NewGame);
    }

    public void NewGame()
    {
        overlay.gameObject.SetActive(false);
        // Reset Board
        squares?.SerializeContent().ToList().ForEach(x => Destroy(x.gameObject));

        // New Game
        squares = new Square[9, 9];
        LoadJson();
        SpawnBoardSquares();
        for (var i = 0; i < players.Length; i++)
        {
            players[i].Initialize(i, colorPool[i % colorPool.Length]);
            players[i].OnPlayerLeftSpace.AddListener(PlayerLeftSpace);
            players[i].OnPlayerSuggestIncrease.AddListener(PlayerIncreaseSuggestion);
            players[i].OnPlayerSuggestMove.AddListener(PlayerMoveSuggestion);
        }
    }

    void LoadJson()
    {
        using (StreamReader r = new StreamReader("Assets/Puzzles/s1.json"))
        {
            string fulljson = r.ReadToEnd();
            PuzzleList sudokuList = JsonUtility.FromJson<PuzzleList>(fulljson);
            puzzle = sudokuList.Random();
        }
    }

    void SpawnBoardSquares()
    {
        for (var i = -4; i <= 4; i++)
        {
            for (var j = -4; j <= 4; j++)
            {
                squares[i + 4, j + 4] = Instantiate(
                    squarePrefab,
                    new Vector3(i, j, 0f),
                    Quaternion.identity
                ).GetComponent<Square>();
                squares[i + 4, j + 4].Initialize(
                    puzzle.start[i + 4 + (j + 4) * 9],
                    puzzle.solution[i + 4 + (j + 4) * 9]
                );
            }
        }
    }

    public void PlayerMoveSuggestion(int id, Vector2Int position)
    {
        if (position.x < 0 || position.x > 8) return;
        if (position.y < 0 || position.y > 8) return;
        if (!squares[position.x, position.y].PlayerMove()) return;

        players[id].MoveAccepeted(position);
    }

    public void PlayerIncreaseSuggestion(int id, Vector2Int position)
    {
        if (position.x < 0 || position.x > 8) return;
        if (position.y < 0 || position.y > 8) return;
        squares[position.x, position.y].Increment();
    }

    public void PlayerLeftSpace(int id, Vector2Int position)
    {
        if (position.x < 0 || position.x > 8) return;
        if (position.y < 0 || position.y > 8) return;
        var status = squares[position.x, position.y].PlayerExit();
        if (status == Square.PlayStatus.FAIL)
            players[id].ChangeScore(-1);
        else if (status == Square.PlayStatus.SUCCESS)
        {
            players[id].ChangeScore(1);
            Invoke("CheckEndGame", 0f);
        }
    }

    void CheckEndGame()
    {
        var squaresLeft = squares.SerializeContent().Count(x => !x.finished);
        var bestPlayers = players.OrderByDescending(x => x.Score).Take(2);
        if (bestPlayers.First().Score - bestPlayers.Last().Score >= squaresLeft)
        {
            overlay.PlayerWin(bestPlayers.First().Id);
        }
    }

    [Serializable]
    public class Puzzle
    {
        public int[] start;
        public int[] solution;
    }

    [Serializable]
    public class PuzzleList
    {
        public Puzzle[] items;

        public Puzzle Random()
        {
            return items.OrderBy(s => Guid.NewGuid()).First();
        }
    }
}

public static class MultiArrayExtension {
    public static IEnumerable<T> SerializeContent<T>(this T[,] array)
    {
        for (int i = 0; i < array.GetLength(0); i++)
            for (int j = 0; j < array.GetLength(1); j++)
                yield return array[i, j];
    }
}

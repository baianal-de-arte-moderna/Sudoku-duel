using System.IO;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject squarePrefab;
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
    // Start is called before the first frame update
    void Start()
    {
        squares = new Square[9, 9];
        LoadJson();
        SpawnBoardSquares();
        for (var i = 0; i < players.Length; i++)
        {
            players[i].SetId(i, colorPool[i % colorPool.Length]);
            players[i].OnPlayerLeftSpace.AddListener(PlayerLeftSpace);
            players[i].OnPlayerSuggestIncrease.AddListener(PlayerIncreaseSuggestion);
            players[i].OnPlayerSuggestMove.AddListener(PlayerMoveSuggestion);
        }
    }

    public void LoadJson()
    {
        using (StreamReader r = new StreamReader("Assets/Puzzles/s1.json"))
        {
            string json = r.ReadToEnd();
            puzzle = JsonUtility.FromJson<Puzzle>(json);
        }
    }

    private void SpawnBoardSquares()
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
            players[id].ChangeScore(1);
    }

    public class Puzzle
    {
        public int[] start;
        public int[] solution;
    }
}

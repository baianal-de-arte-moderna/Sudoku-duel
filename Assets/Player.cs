using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuggestMove : UnityEvent<int, Vector2Int> { }
public class SuggestIncrease : UnityEvent<int, Vector2Int> { }

public class Player : MonoBehaviour
{
    public Vector2Int boardPosition = Vector2Int.one * 4;
    public TMPro.TMP_Text scoreElement;

    public SuggestMove OnPlayerSuggestMove = new SuggestMove();
    public SuggestMove OnPlayerLeftSpace = new SuggestMove();
    public SuggestIncrease OnPlayerSuggestIncrease = new SuggestIncrease();

    [Header("Commands")]
    public KeyCode Up;
    public KeyCode Down;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Increment;

    Vector2Int move = Vector2Int.zero;
    bool incrementing = false;

    int score = 0;
    int id = -1;
    SpriteRenderer render;
    // Start is called before the first frame update
    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        transform.localPosition = Vector2.one * -4 + boardPosition;
    }

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetKeyDown(Right) ? 1 : Input.GetKeyDown(Left) ? -1 : 0;
        move.y = Input.GetKeyDown(Up) ? 1 : Input.GetKeyDown(Down) ? -1 : 0;

        incrementing = Input.GetKeyDown(Increment);

        MoveBy(move);
        IncrementSpace(incrementing);
    }

    void IncrementSpace(bool incrementing)
    {
        if (incrementing)
            OnPlayerSuggestIncrease?.Invoke(id, boardPosition);
    }

    void MoveBy(Vector2Int move)
    {
        if (id >= 0 && move.magnitude != 0)
            OnPlayerSuggestMove?.Invoke(id, boardPosition + move);
    }

    public void MoveAccepeted(Vector2Int move)
    {
        OnPlayerLeftSpace?.Invoke(id, boardPosition);
        boardPosition = move;
        transform.localPosition = Vector2.one * -4 + boardPosition;
    }

    public void SetId(int id, Color playerColor)
    {
        this.id = id;
        render.color = playerColor;
        scoreElement.color = playerColor;
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        scoreElement.text = "" + score;
    }
}

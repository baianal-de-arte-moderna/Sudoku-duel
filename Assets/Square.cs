using UnityEngine;
using TMPro;

public class Square : MonoBehaviour
{
    public enum PlayStatus
    {
        EMPTY,
        FAIL,
        SUCCESS
    }

    public TMP_Text text;

    int solutionNumber = 0;
    int currentNumber = 0;
    bool hasPlayer = false;
    bool finished = false;

    public void Initialize(int start, int solution)
    {
        solutionNumber = solution;
        currentNumber = start;
        
        if (currentNumber == solutionNumber)
        {
            text.text = "" + currentNumber;
            finished = true;
        }
    }

    public void Increment()
    {
        if (!finished && hasPlayer)
        {
            currentNumber++;
            currentNumber %= 10;
            if (currentNumber == 0)
                text.text = "";
            else
                text.text = currentNumber.ToString();
        }
    }

    public PlayStatus PlayerExit()
    {
        hasPlayer = false;
        if (finished)
            return PlayStatus.EMPTY;
        if (currentNumber <= 0)
        {
            text.text = "";
            currentNumber = 0;
            return PlayStatus.EMPTY;
        }
        if (currentNumber == solutionNumber)
        {
            finished = true;
            return PlayStatus.SUCCESS;
        }
        text.text = "";
        currentNumber = 0;
        return PlayStatus.FAIL;
    }

    public bool PlayerMove()
    {
        bool canMove = !hasPlayer;
        hasPlayer = true;
        return canMove;
    }
}

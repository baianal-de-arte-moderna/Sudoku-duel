using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Overlay : MonoBehaviour
{
    public TMP_Text winnerText;
    public UnityEvent OnNewGame;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnNewGame?.Invoke();
        }
    }

    public void PlayerWin(int id)
    {
        winnerText.text = "Player " + id + " Wins!!!";
        gameObject.SetActive(true);
    }
}

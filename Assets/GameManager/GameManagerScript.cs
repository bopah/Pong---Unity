using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    // References to player/computer text scores
    [SerializeField]
    private Text playerScoreText;
    [SerializeField]
    private Text computerScoreText;
    private int playerScore = 0;
    private int computerScore = 0;

    // References to reset positions when a ball hits left/right wall
    [SerializeField]
    private Ball_script ball;
    [SerializeField]
    private PlayerPaddle playerPaddle;
    [SerializeField]
    private ComputerPaddle computerPaddle;

    // Start is called before the first frame update
    void Start()
    {
        //playerScoreText = GetComponent<Text>();
        //computerScoreText = GetComponent<Text>();

        // Initialize scores to 0
        playerScoreText.text = playerScore.ToString();
        computerScoreText.text = computerScore.ToString();
    }


    public void addPlayerScore()
    {
        playerScore++;
        playerScoreText.text = playerScore.ToString();
    }


    public void addComputerScore()
    {
        computerScore++;
        computerScoreText.text = computerScore.ToString();
    }

    public void resetPositions()
    {
        playerPaddle.resetPaddlePosition();
        computerPaddle.resetPaddlePosition();
        ball.ResetPaddlePosition();
    }

    public void BallHitWall(string wallName)
    {
        if (wallName == "Left")
        {
            addComputerScore();
        }
        else if (wallName == "Right")
        {
            addPlayerScore();
        }
        resetPositions();
    }
    
}

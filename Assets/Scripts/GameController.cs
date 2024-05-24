/*******
 * DUCK HUNT
 * Version : 1.0 (part 2 of tutorial)
 * Hoang Minh Quan
 * http://khoahocvui.vn
 * *****/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject beginLayer;
    public GameObject blackLayer;
    public GameObject whiteBlock;
    public GameObject gameLayer;

    public GameObject Background;
    public GameObject GamePaused;
    public GameObject Tutorial;

    public GameObject lbScore;
    public GameObject lbTime;
    public GameObject ScoreTextPrefap;
    public GameObject gameOverLayer;
    public GameObject topMenuLayer;

    public bool isReadSensor = false;
    public List<GameObject> duckArr;
    public GameObject duckInstance;
    public GameObject duck;
    public GameObject lbOverScore;
    public List<AudioClip> audioArr;
    private int gameScore;
    private int gameTime;
    public int gameStatus;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        HideBlackScreen();
        gameOverLayer.SetActive(false);
        beginLayer.SetActive(true);
        topMenuLayer.SetActive(false);
        gameLayer.SetActive(false);
        gameScore = 0;
        gameStatus = -1;
        Background.SetActive(false);
        GamePaused.SetActive(false);
        Tutorial.SetActive(false);
    }

    // start game
    public void StartGame()
    {
        gameStatus = 0;
        gameTime = 30; //AQUI SE CAMBIA EL TIEMPO
        beginLayer.SetActive(false);
        gameLayer.SetActive(true);
        gameOverLayer.SetActive(false);
        topMenuLayer.SetActive(true);
        AddNewBird();
        setScore(0);
        updateTime(gameTime);
        Background.SetActive(true); 
        GamePaused.SetActive(false);
        ShowTutorialForDuration(10.0f);
    }

    // Método para mostrar el texto del tutorial durante un tiempo específico
    public void ShowTutorialForDuration(float duration)
    {
        Tutorial.SetActive(true);
        StartCoroutine(HideTutorialTextAfterTime(duration));
    }

    // Coroutine para ocultar el texto del tutorial después de un tiempo
    private IEnumerator HideTutorialTextAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Tutorial.SetActive(false);
    }

    // Add a new bird
    public void AddNewBird()
    {
        Debug.Log("new bird");
        duck = Instantiate(duckInstance, gameLayer.transform);
        randomDuck();
    }

    // random duck
    public void randomDuck()
    {
        duck.GetComponent<Duck>().randomFly();
    }


    // show black screen
    public void ShowBlackScreen()
    {
        blackLayer.SetActive(true);
        whiteBlock.SetActive(true);
    }

    // hide black screen
    public void HideBlackScreen()
    {
        blackLayer.SetActive(false);
        whiteBlock.SetActive(false);

    }

    // trigger press begin
    public void ShootBegin()
    {
        blackLayer.SetActive(true);
        whiteBlock.SetActive(false);
        GetComponent<AudioSource>().clip = audioArr[0];
        GetComponent<AudioSource>().Play();
    }

    // onDamage to the bird
    public void OnDamage()
    {
        Debug.Log("on damage");
        if (gameStatus != 0)
        {
            return;
        }
        GameObject lb = Instantiate(ScoreTextPrefap, gameLayer.transform);
        lb.transform.localPosition = new Vector3(duck.transform.localPosition.x, duck.transform.localPosition.y + 1, duck.transform.localPosition.z);
        SetTimeOut(1.0f, () => {
            Destroy(lb);
        });
        setScore(gameScore + 100);
        duck.GetComponent<Duck>().die();
        GetComponent<AudioSource>().clip = audioArr[1];
        GetComponent<AudioSource>().Play();
        AddNewBird();
    }
    // Update is called once per frame
    float eTime = 0;
    void Update()
    {
        // Check if the "P" key is pressed to pause/resume the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Toggle pause state
            isPaused = !isPaused;
            if (isPaused)
            {
                // Pause the game
                Time.timeScale = 0f;
                GamePaused.SetActive(true);
                Background.SetActive(false);
            }
            else
            {
                // Resume the game
                Time.timeScale = 1f;
                GamePaused.SetActive(false);
                Background.SetActive(true);
            }
        }

        // Skip the rest of the update if the game is paused
        if (isPaused || gameStatus < 0)
            return;

        if (duck != null)
        {
            whiteBlock.transform.localPosition = duck.transform.localPosition;
        }
        eTime += Time.deltaTime;
        if (eTime > 1)
        {
            eTime = 0;
            updateTime(gameTime - 1);
        }

        if (gameTime == 0)
        {
            GameOver();
        }
    }
    public void updateTime(int gameTime_)
    {
        gameTime = gameTime_;
        int m = gameTime / 60;
        int s = gameTime % 60;
        string ss = s.ToString();
        if (s < 10) ss = "0" + s;
        string str = "0" + m + ":" + ss;
        lbTime.GetComponent<Text>().text = str;
    }

    public void setScore(int score_)
    {
        gameScore = score_;
        string str = score_.ToString();
        if (score_ >= 100 && score_ < 1000)
        {
            str = "00" + score_;
        }
        else if (score_ >= 1000 && score_ < 10000)
        {
            str = "0" + score_;
        }
        if (score_ >= 1000 && score_ < 10000)
        {
            str = "0" + score_;
        }
        else if (score_ < 10)
        {
            str = "0000" + score_;
        }
        lbScore.GetComponent<Text>().text = str;
    }

    public void GameOver()
    {
        gameOverLayer.SetActive(true);
        Background.SetActive(false);
        lbOverScore.GetComponent<Text>().text = lbScore.GetComponent<Text>().text;
        GetComponent<AudioSource>().clip = audioArr[3];
        GetComponent<AudioSource>().Play();
        gameStatus = 1;
        SetTimeOut(5, () => {
            if (gameStatus == 1)
            {
                gameStatus = 2;
            }
        });
    }

    public void PlayAgain()
    {
        if(duck != null)
        {
            Destroy(duck);
        }
        StartGame();
    }

    // hen thoi gian sau do call 1 action
    public Coroutine SetTimeOut(float delay, System.Action action)
    {
        return StartCoroutine(WaitAndDo(delay, action));
    }

    // doi 1 thoi gian chay ham.
    private IEnumerator WaitAndDo(float time, System.Action action = null)
    {
        yield return new WaitForSeconds(time);
        if (action != null)
            action();
    }

}

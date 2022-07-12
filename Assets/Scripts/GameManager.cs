using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int smallScore = 100;
    public int mediumScore = 50;
    public int bigScore = 20;
    public int UFOscore = 200;

    public GameObject spawner;
    public Player player;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI liveText;

    public Button continueGameButton;
    public Button newGameButton;
    public Button settingsButton;
    public Button exitButton;
    public GameObject pauseMenu;
    public bool isControlsKeyboard = true;
    public bool isUFOdestroyed = true;
    public float respawnInvulnerabilityTime = 3.0f;
    private int lives = 3;
    private int score = 0;

    private float respawnTime = 0.1f;

    //Number of asteroids to spawn variables
    private int spawnAmount = 2;
    private int numberOfAsteroidsLeft = 2;
    private int level = 0;
    private int lengthOfPrefab = 0;
    private bool _isPaused = false;

    private float spawnUFOTimer;

    private bool isGameStarted = false;

    //continue button
    public void ContinueGame()
    {
            UnPauseGameState();
    }

    //new game button
    public void NewGame()
    {
        if (isGameStarted == true)
        {
            UnPauseGameState();
        }
        if (isGameStarted == false)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //control setup button
    public void ControlSettings()
    {
        if (isControlsKeyboard == true)
        {
            settingsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Mouse";
            isControlsKeyboard = false;
        }
        else if (isControlsKeyboard == false)
        {
            settingsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Keyboard";
            isControlsKeyboard = true;
        }
    }

    //Exit app funtion
    public void Exit()
    {
        Application.Quit();
    }


    private void Start() 
    {
        spawnUFOTimer = Random.Range(20,41);
        PauseGameState();
    }

    //Function for score
    public void AsteroidDestroyed(Asteroid asteroid)
    {
        if (asteroid.size == 0.5f)
        {
            this.score += smallScore;
            numberOfAsteroidsLeft --;
        }
        else if (asteroid.size == 1.0f)
        {
            this.score += mediumScore;
            numberOfAsteroidsLeft --;
        }
        else if (asteroid.size == 1.5f)
        {
            this.score += bigScore;
            numberOfAsteroidsLeft --;
        }
    }

    public void UFODestroyed(UFO ufo)
    {
        this.score += UFOscore;
        isUFOdestroyed = true;
    }

    public void PlayerDied()
    {
        this.lives --;
        if (this.lives <= 0)
        {
            GameOver();
        }
        else
        {
        Invoke("Respawn", respawnTime);
        }
    }

    private void Respawn()
    {
        this.player.transform.position = Vector3.zero;
        this.player.gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");

        //Invulnerability on respawn
        Invoke("TurnOnCollisions", this.respawnInvulnerabilityTime);

    }

    private void TurnOnCollisions()
    {
        this.player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void GameOver()
    {
        PauseGameState();
        isGameStarted = false;
        NewGame();
    }

    private void SpawnNewAsteroids()
    {
        if (numberOfAsteroidsLeft == 0)
        {
            spawnAmount ++;
            spawner.GetComponent<AsteroidSpawner>().Spawn(spawnAmount, lengthOfPrefab);
            numberOfAsteroidsLeft = spawnAmount;
            level++;
            if (level == 1)
            {
                lengthOfPrefab = 1;
            }
            else if (level == 2)
            {
                lengthOfPrefab = 2;
            }
        }
    }

    private IEnumerator spawnUFO()
    {
        while (isUFOdestroyed == true)
        {
            spawner.GetComponent<AsteroidSpawner>().SpawnUfo();
            isUFOdestroyed = false;
            yield return new WaitForSeconds (1);

        }
        spawnUFOTimer = Random.Range(20,41);
    }

    //First spawn wave
    private void Awake() 
    {
        spawner.GetComponent<AsteroidSpawner>().Spawn(spawnAmount, lengthOfPrefab);
    }
    //Every other spawn wave
    private void Update()
    {
        SpawnNewAsteroids();

        spawnUFOTimer -= Time.deltaTime;
        if (isUFOdestroyed == true && spawnUFOTimer <=0)
        {
            StartCoroutine(spawnUFO());
        }

        //Updating score and lives
        UpdateScore();
        UpdateLives();

        //pause game
        PauseGame();
    }
    private void FixedUpdate()
    {
        
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }
    private void UpdateLives()
    {
        liveText.text = "Lives: " + lives;
    }

    //funciton to manage pausegame input
    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _isPaused == false)
        {
            continueGameButton.interactable = true;
            PauseGameState();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _isPaused == true)
        {
            UnPauseGameState();
        }
        PauseMenu();
    }

    //logic for the pause menu to appear
    private void PauseMenu()
    {
        if (_isPaused == true)
        {
            pauseMenu.SetActive(true);
        }
        else if (_isPaused == false)
        {
            pauseMenu.SetActive(false);
        }
    }

    //logic to pause game state, well theoretically
    private void PauseGameState()
    {
        _isPaused = true;
        player.Frozen();

        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject asteroid in asteroids)
        {
            asteroid.GetComponent<Asteroid>().Frozen();
        }

        //freeze bullets
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            bullet.GetComponent<Bullet>().Frozen();
        }

        //freeze ufobullets
        GameObject[] UFObullets = GameObject.FindGameObjectsWithTag("UFOBullet");
        foreach (GameObject bullet in UFObullets)
        {
            bullet.GetComponent<Bullet>().Frozen();
        }

        //freeze ufo
        if (GameObject.FindGameObjectWithTag("UFO") != null)
        {
            GameObject ufo = GameObject.FindGameObjectWithTag("UFO");
            ufo.GetComponent<UFO>().Frozen();
        }

        isGameStarted = true;
    }

    //logic to pause game state, well theoretically
    private void UnPauseGameState()
    {
        _isPaused = false;
        player.UnFroze();

        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject asteroid in asteroids)
        {
            asteroid.GetComponent<Asteroid>().UnFroze();
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            bullet.GetComponent<Bullet>().UnFroze();
        }

        GameObject[] UFObullets = GameObject.FindGameObjectsWithTag("UFOBullet");
        foreach (GameObject bullet in bullets)
        {
            bullet.GetComponent<Bullet>().UnFroze();
        }

        if (GameObject.FindGameObjectWithTag("UFO") != null)
        {
        GameObject ufo = GameObject.FindGameObjectWithTag("UFO");
        ufo.GetComponent<UFO>().UnFroze();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject[] asteroidPrefab;
    public GameObject UFOprefab;

    private Camera mainCam;
    private int _prefabIndex;
    private int UFOspawnBorder;
    private Vector3 UFOspawnPosition;


    public float trajectoryVariants = 15.0f;
    public float spawnDistance = 15.0f;
    public float spawnRate = 2.0f;
    public int spawnAmount = 1;
    public Vector3 spawnDirection;

    private void Awake() 
    {

    }

    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(int spawnAmount, int typeOfAsteroids)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            //Setting up the direction of the spawwable asteroid, its spawn point, trajectory variance 
            Vector3 spawnDirection = Random.insideUnitCircle.normalized * this.spawnDistance;
            Vector3 spawnPoint = this.transform.position + spawnDirection;
            float variance = Random.Range(-trajectoryVariants, trajectoryVariants);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            //Selecting what kind of asteroid to spawn
            _prefabIndex = Random.Range(0, typeOfAsteroids);
            GameObject asteroid = Instantiate(asteroidPrefab[_prefabIndex], spawnPoint, rotation);

            //give new spawn asteroid a velocity and a size
            asteroid.GetComponent<Asteroid>().SetTrajectory(rotation * -spawnDirection);
            asteroid.GetComponent<Asteroid>().transform.localScale = Vector3.one * asteroid.GetComponent<Asteroid>().asteroidSize;
        }
    }

    public void SpawnUfo()
    {
        float sceneWidth = mainCam.orthographicSize * 2 * mainCam.aspect;
        float sceneHeight = mainCam.orthographicSize * 2;

        float sceneRightEdge = sceneWidth/2;
        float sceneLeftEdge = sceneRightEdge * -1;
        float sceneTopEdge = (sceneHeight/2)-1;
        float sceneBottomEdge = (sceneTopEdge * -1)+1;

        UFOspawnBorder = Random.Range(1,3);
        if (UFOspawnBorder == 1)
        {
            Vector3 UFOspawnPosition = new Vector3 (sceneLeftEdge, Random.Range(sceneBottomEdge, sceneTopEdge));
            GameObject UFO = Instantiate(UFOprefab, UFOspawnPosition, transform.rotation);
            UFO.GetComponent<UFO>().endPosition = new Vector3 (sceneRightEdge, Random.Range(sceneBottomEdge, sceneTopEdge));
        }
        if (UFOspawnBorder == 2)
        {
            Vector3 UFOspawnPosition = new Vector3 (sceneRightEdge, Random.Range(sceneBottomEdge, sceneTopEdge));
            GameObject UFO = Instantiate(UFOprefab, UFOspawnPosition, transform.rotation);
            UFO.GetComponent<UFO>().endPosition = new Vector3 (sceneLeftEdge, Random.Range(sceneBottomEdge, sceneTopEdge));
        }
    }
}

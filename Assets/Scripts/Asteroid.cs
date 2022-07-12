using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float asteroidSize;
    public float asteroidSpeed;
    public Sprite[] sprites;
    public float asteroidSpeed_1 = 1f;
    public float maxLifetime = 30.0f;

    //Asteroids sizes
    public float size = 1.0f;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    private float offset = 0.1f;
    private Camera mainCam;

    private bool isInScreen = false;
    private Vector2 directionMemory;

    private void Awake() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        AsteroidRandomizer();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPosition();
    }

    private void AsteroidRandomizer()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, Random.value * 360.0f);
    }

    public void SetTrajectory (Vector2 direction)
    {
        directionMemory = direction;
        _rigidbody.AddForce(direction * asteroidSpeed);

        Destroy(this.gameObject, this.maxLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (asteroidSize == 1.0f)
            {
                CreateSplit(.5f);
                CreateSplit(.5f);
            }
            if (asteroidSize == 1.5f)
            {
                CreateSplit(1.0f);
                CreateSplit(1.0f);
            }
            FindObjectOfType<GameManager>().AsteroidDestroyed(this);
            Destroy(this.gameObject);
        }
    }

    private void CreateSplit(float size)
    {
        Vector2 position = this.transform.position;
        position += Random.insideUnitCircle * 0.5f;

        Asteroid half = Instantiate(this, position, this.transform.rotation);
        half.transform.localScale = Vector3.one * size;
        half.asteroidSize = size;
        half.SetTrajectory(Random.insideUnitCircle.normalized * asteroidSpeed_1 * Random.Range(10, 30));
    }

    private void CheckPosition()
    {
        float sceneWidth = mainCam.orthographicSize * 2 * mainCam.aspect;
        float sceneHeight = mainCam.orthographicSize * 2;

        float sceneRightEdge = sceneWidth/2;
        float sceneLeftEdge = sceneRightEdge * -1;
        float sceneTopEdge = sceneHeight/2;
        float sceneBottomEdge = sceneTopEdge * -1;

        if (transform.position.x < sceneRightEdge - offset && transform.position.x > sceneLeftEdge + offset &&
            transform.position.y < sceneTopEdge - offset && transform.position.y > sceneBottomEdge + offset)
        {
            isInScreen = true;
        }

        if(isInScreen == true)
        {
            if (transform.position.x > sceneRightEdge)
            {
                transform.position = new Vector2(sceneLeftEdge, transform.position.y);
            }
            if (transform.position.x < sceneLeftEdge)
            {
                transform.position = new Vector2(sceneRightEdge, transform.position.y);
            }
            if (transform.position.y > sceneTopEdge)
            {
                transform.position = new Vector2(transform.position.x, sceneBottomEdge);
            }
            if (transform.position.y < sceneBottomEdge)
            {
                transform.position = new Vector2(transform.position.x, sceneTopEdge);
            }            
        }
    }

    public void Frozen()
    {
        _rigidbody.Sleep();
    }
    public void UnFroze()
    {
        _rigidbody.WakeUp();
        SetTrajectory(directionMemory);
    }

}

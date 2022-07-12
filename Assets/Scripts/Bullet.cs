using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 500.0f;
    private float currentLifeTime;
    private Rigidbody2D _rigidbody;
    private Camera mainCam;
    private Vector2 directionMemory;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckPosition();

        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <=0)
        {
            this.gameObject.SetActive(false);
        }

    }

    public void Project(Vector2 direction)
    {
        directionMemory = direction;
        _rigidbody.AddForce(direction * bulletSpeed);
        currentLifeTime = 3.0f;
        //Destroy(this.gameObject, this.maxLifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }
        private void CheckPosition()
    {
        float sceneWidth = mainCam.orthographicSize * 2 * mainCam.aspect;
        float sceneHeight = mainCam.orthographicSize * 2;

        float sceneRightEdge = sceneWidth/2;
        float sceneLeftEdge = sceneRightEdge * -1;
        float sceneTopEdge = sceneHeight/2;
        float sceneBottomEdge = sceneTopEdge * -1;

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

    public void Frozen()
    {
        _rigidbody.Sleep();
    }
    public void UnFroze()
    {
        _rigidbody.WakeUp();
        Project(directionMemory);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{


    public Vector2 direction;
    public float speed;

    //delay between shoots in seconds
    public float shootingDelay;
    public float lastTimeShot = 0f;
    public Transform player;
    public Bullet bullet;

    public Vector3 endPosition;
    private GameManager gameManagerScript;
    private Rigidbody2D _rigidbody;
    private bool freeze = false;
    private void Start() 
    {
        player = GameObject.FindWithTag("Player").transform;
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ShootUFO();
        OnEndPositionDestroyUFO();
    }

    private void FixedUpdate() 
    {
        MovingToOtherScreen(endPosition);
    }

    public void MovingToOtherScreen(Vector3 endPoint)
    {
        direction = (endPoint - transform.position).normalized;
        if (freeze == false)
        {
            _rigidbody.MovePosition (_rigidbody.position + direction * speed * Time.deltaTime);
        }

    }

    private void ShootUFO()
    {
        if (Time.time > lastTimeShot + shootingDelay)
        {
            //shoot
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg * -90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            Bullet bullet = Instantiate(this.bullet, this.transform.position, this.transform.rotation);
            bullet.Project(player.position);
            
            lastTimeShot = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            gameManagerScript.isUFOdestroyed = true;
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.tag == "Bullet")
        {
            gameManagerScript.UFODestroyed(this);
            Destroy(this.gameObject);
        }
    }
    
    public void OnEndPositionDestroyUFO()
    {
        if (endPosition.x > 0)
        {
            if (this.transform.position.x >= endPosition.x - 0.1)
            {
                gameManagerScript.isUFOdestroyed = true;
                Destroy(this.gameObject);
            }
        }
        else if (endPosition.x < 0)
        {
            if (this.transform.position.x <= endPosition.x - 0.1)
            {
                gameManagerScript.isUFOdestroyed = true;
                Destroy(this.gameObject);
            }
        }
    }

    public void Frozen()
    {
        _rigidbody.Sleep();
        freeze = true;
    }
    public void UnFroze()
    {
        _rigidbody.WakeUp();
        freeze = false;
    }

}

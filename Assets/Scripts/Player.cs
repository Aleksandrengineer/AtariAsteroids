using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Public variable for the adjusting
    public Bullet bulletPrefab;
    public float thrustSpeed = 1.0f;
    public float turnSpeed = 0.1f;

    private GameManager gameManager;

    //This player rb to do movements
    private Rigidbody2D _rigidbody;
    private bool _thrusting;
    private float _turnDirection;

    //Camera and border controls
    private Camera mainCam;

    //Value fro blinking effect
    private float spriteBlinkingTimer = 0.0f;
    private float spriteBlinkingMiniDuration = 0.1f;
    private float spriteBlinkingTotalTimer = 0.0f;
    private float spriteBlinkingTotalDuration = 3.0f;
    private bool startBlinking = false;

    //values for shooting
    private float shootTimer = 0;
    public float shootDelay = 1;
    public int maxShootCount = 3;
    private int shootCount = 0;

    //Rtation values
    float angle1; // angle1 will be negative to normal angle
    float angle;  

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        CheckPosition();
        if (startBlinking == true)
        {
            SpriteBlinkingEffect();
        }
        PlayerMovementInput();
        PlayerMovement();
    }


    //Input of user to control the player
    private void PlayerMovementInput()
    {
        if (gameManager.isControlsKeyboard == true)
        {

            //controller with keyboard
            _thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _turnDirection = 1.0f;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _turnDirection = -1.0f;
            }
            else
            {
                _turnDirection = 0.0f;
            }

            //Shooting input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }

        else if (gameManager.isControlsKeyboard == false)
        {
            _thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetMouseButton(1);
            
            //rotation logic
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);     
            angle = Mathf.Atan2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y) * 180 / Mathf.PI; //Get mouse angle
            _rigidbody.rotation %= 360;
            angle = (angle + _rigidbody.rotation); // Sum up rigidbody and mouse angle
            if (angle < 0) angle1 = 360.0f + angle;
            else angle1 = 360.0f - angle; // calculates negative angle
            if (Mathf.Abs(angle) > Mathf.Abs(angle1) && angle < 0)
            angle = angle1;
            if (Mathf.Abs(angle) > Mathf.Abs(angle1) && angle > 0)
            angle = angle1 * -1; // failcheck
            _rigidbody.AddTorque(-angle / 180 * turnSpeed);

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }
    }
    
    //calculator for the thrusting and torque
    private void PlayerMovement()
    {
        if (_thrusting)
        {
            _rigidbody.AddForce(this.transform.up * thrustSpeed);
        }

        if (_turnDirection != 0.0f)
        {
            _rigidbody.AddTorque(_turnDirection * turnSpeed);
        }
    }
    //Shooting logic
    private void Shoot()
    {
        if (Time.time < shootTimer + shootDelay && shootCount < maxShootCount)
        {
            //Bullet bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);

            GameObject bullet = ObjectPool.SharedInstance.GetPooledObject(); 
            if (bullet != null) {
                bullet.transform.position = this.transform.position;
                bullet.transform.rotation = this.transform.rotation;
                bullet.SetActive(true);
                bullet.GetComponent<Bullet>().Project(this.transform.up);
            }

            //bullet.Project(this.transform.up);
            shootTimer = Time.time;
            shootCount ++;
        }
        else if (Time.time > shootTimer + shootDelay)
        {
            shootCount = 0;
            shootTimer = Time.time;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "UFOBullet")
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = 0.0f;
            startBlinking = true;
            gameManager.PlayerDied();
            //this.gameObject.SetActive(false);
        }
    }

    //Border reposition
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

    //Logic of the blinking effect

    private void SpriteBlinkingEffect()
      {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if(spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
             spriteBlinkingTotalTimer = 0.0f;
             this.gameObject.GetComponent<SpriteRenderer> ().enabled = true;   // according to 
                      //your sprite
             return;
          }
     
     spriteBlinkingTimer += Time.deltaTime;
        if(spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (this.gameObject.GetComponent<SpriteRenderer> ().enabled == true) {
                this.gameObject.GetComponent<SpriteRenderer> ().enabled = false;  //make changes
            } else {
                this.gameObject.GetComponent<SpriteRenderer> ().enabled = true;   //make changes
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
    }
}

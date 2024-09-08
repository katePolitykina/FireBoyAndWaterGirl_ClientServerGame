using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour //даем системе понять, что это сетевой объект
{
    public bool isBoy;
    public float moveSpeed;
    public float jumpSpeed;

    private Rigidbody2D rb;
    private PlayerState playerState;

    private Animator headAnimator;
    private Animator bodyAnimator;
    

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        headAnimator = transform.GetChild(0).GetComponent<Animator>();
        bodyAnimator = transform.GetChild(1).GetComponent<Animator>();

        moveSpeed = moveSpeed < 0.001f ? 6f : moveSpeed;
        jumpSpeed = jumpSpeed < 0.001f ? 8f : jumpSpeed;

        playerState = PlayerState.Idle;
    }


    private void Update()
    {
                        
        if (!isLocalPlayer)
            return;
            PlayerMove();

            PlayerAnimation();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }
    private void PlayerMove()
    {
        Vector2 inputVelocity;
        inputVelocity = rb.velocity;
        float horizontal = Input.GetAxis("Horizontal");
        inputVelocity.x = horizontal * moveSpeed;
        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
                
                float jump = Input.GetAxis("Jump");
                if (jump > 0.001f)
                {
                    playerState = PlayerState.Jump;
                    inputVelocity.y = rb.velocity.y + jump * jumpSpeed;
                }
                else if (playerState == PlayerState.Idle && Mathf.Abs(inputVelocity.x) > 0.001f)
                {
                    playerState = PlayerState.Run;
                }
                else if (playerState == PlayerState.Run && Mathf.Abs(inputVelocity.x) < 0.001f)
                {
                    playerState = PlayerState.Idle;
                }

                break;
            case PlayerState.Jump:
                if (rb.velocity.y < -0.01f)
                {
                    playerState = PlayerState.Fall;
                }
                break;
            case PlayerState.Fall:
                if (Mathf.Abs(rb.velocity.y) < 0.001f)
                {
                    playerState = PlayerState.Idle;
                }
                break;

            default:
                break;
        }        
        rb.velocity = inputVelocity;
    }

    private void PlayerAnimation()
    {
        headAnimator.SetBool("Idle", playerState == PlayerState.Idle);
        headAnimator.SetBool("Run", playerState == PlayerState.Run);
        headAnimator.SetBool("Jump", playerState == PlayerState.Jump);
        headAnimator.SetBool("Fall", playerState == PlayerState.Fall);
        bodyAnimator.SetBool("Idle", playerState == PlayerState.Idle);
        bodyAnimator.SetBool("Run", playerState == PlayerState.Run);
        bodyAnimator.SetBool("Jump", playerState == PlayerState.Jump);
        bodyAnimator.SetBool("Fall", playerState == PlayerState.Fall);
    }

    [Client]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBoy)
        {
            if (collision.name == "Bluewater" || collision.name == "Greenwater")
            { 
                if (isLocalPlayer)
                {
                    cmdSceneload();                  
                }                             
            }
        }
        else {
            if (collision.name == "Redwater" || collision.name == "Greenwater")
            {
              if (isLocalPlayer)
                {
                    cmdSceneload();                   
                }
            }
        }
    }
  
    [Command]
    public void cmdSceneload()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "LevelScene")
            NetworkManager.singleton.ServerChangeScene("LevelScene");
        else if (scene.name == "LevelScene1")
            NetworkManager.singleton.ServerChangeScene("LevelScene1");
    }

    public enum PlayerState
    {
        None,
        Idle, Run, Jump, Fall,
    }



}

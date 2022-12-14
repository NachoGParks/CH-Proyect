using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rigidBody;
    public float speed = 1;
    Vector3 lookPos;
    Animator anim;
    Transform cam;
    Vector3 camForward;
    Vector3 move;
    Vector3 moveInput;
    float forwardAmount;
    float turnAmount;
    public PlayerManager playerManager;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;     
    }

    
    void Update()
    {
        playerAnimDies();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100))
        {
            lookPos = hit.point;
        }

        Vector3 lookDir = lookPos - transform.position;
        lookDir.y = 0;

        transform.LookAt(transform.position + lookDir, Vector3.up);
        
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(cam != null)
        {
            camForward = Vector3.Scale(cam.up, new Vector3(1, 0, 1)).normalized;
            move = vertical * camForward + horizontal * cam.right;
        }
        else
        {
            move = vertical * Vector3.forward + horizontal * Vector3.right;
        }

        if(move.magnitude >= 1)
        {
            move.Normalize();
        }

        this.moveInput = move;

       

        ConvertMoveInput();
        UpdateAnimator();


        Vector3 movement = new Vector3(horizontal, 0, vertical);

        rigidBody.AddForce(movement * speed / Time.deltaTime);

    }

    void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);

        turnAmount = localMove.x;
        forwardAmount = localMove.z; 
        
    }
    void UpdateAnimator()
    {
        anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Sideways", turnAmount, 0.1f, Time.deltaTime);
    }

    public void playerAnimDies()
    {
        if (playerManager.playerHP <= 0f)
        {
            anim.SetBool("PlayerDied", true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.gameObject.tag == "Coin")
        {
            playerManager.TagsFound++;
            playerManager.playerHP = 100f;
            Destroy(other.transform.gameObject);
            Debug.Log("Cantidad de Tags encontrados: " + playerManager.TagsFound);
        }
    }
}
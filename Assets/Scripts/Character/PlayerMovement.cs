using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private Animator animatorController;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float smoothVelocity;
    [SerializeField]
    private float turnSmoothTime = 0.1f;
    [SerializeField]
    private Transform mainCamera;
    [SerializeField]
    private Transform isGroundCheckSphereSensor;
    [SerializeField]
    private float radiusCheckSphere = 5f;
    [SerializeField]
    private LayerMask grounds;
    [SerializeField]
    private string name;
    [SerializeField]
    private string uid;
    [SerializeField]
    private int score = 0;

    WebSocket webSocket;
    DataPlayer dataPlayer;

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer("ws://localhost:3000");
        animatorController = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        animatorController.SetBool("Grounded", true);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDirection.normalized * speed * Time.deltaTime);
            animatorController.SetFloat("MoveSpeed", 1);
        }
        else
        {
            animatorController.SetFloat("MoveSpeed", 0);
        }

        if (!isGround() && direction.y < 0)
        {
            direction.y += Physics.gravity.y * Time.deltaTime;
            characterController.Move(direction * Time.deltaTime);
        }
    }

    bool isGround()
    {
        return Physics.CheckSphere(isGroundCheckSphereSensor.position, radiusCheckSphere, grounds);
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Egg")
		{
            Debug.Log("Player collected!");
            Destroy(other.gameObject);
            score += 1;
            dataPlayer.SetScore(score);
            dataPlayer.SetVectorState(this.transform.position);
            webSocket.Send(JsonUtility.ToJson(dataPlayer));
		}
	}

    private void ConnectToServer(string host)
	{
        dataPlayer = new DataPlayer(name, uid);
        webSocket = new WebSocket(host);
        webSocket.Connect();
    }
}

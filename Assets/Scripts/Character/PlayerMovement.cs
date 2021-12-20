using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        animatorController = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        animatorController.SetBool("Grounded", true);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
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

}

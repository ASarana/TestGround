using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class heromove : MonoBehaviour
{

    // Use this for initialization
    private CharacterController hero; //контроллер чтобы его двигать
    public float speed; //скорость и камеры
    private Vector3 move; //вектор движения 
    private Transform cam; //это чтобы рассчитать движение относительно камеры
    private Vector3 camForward;
    private float v;
    private float h;
    private float Y;
    Vector3 gravi;
    bool jump;
    bool injump;
    float g;
    public float V0;
    float V;
    //public Transform zeropos;

    void Start()
    {
        hero = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        jump = false;
        injump = false;
        gravi = -Vector3.up;
        g = (float)9.8;
    }

    // Update is called once per frame
    void Update()
    {

        h = CrossPlatformInputManager.GetAxis("Horizontal");
        v = CrossPlatformInputManager.GetAxis("Vertical");
        jump = CrossPlatformInputManager.GetButton("Jump");

        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        //рассчитаем вектор движения 
        move = (v * camForward + h * cam.right).normalized;
        hero.Move(speed * move);
        

        if (move != Vector3.zero)
            transform.forward = move;
        if (!injump)
        {
            hero.Move(-Vector3.up);
        }
        if (jump && hero.isGrounded && !injump)
          {
            injump = true;
            V = V0;
            V = V - g * Time.deltaTime;
            gravi.y = V;
            hero.Move(gravi); 
        }
          if (injump && !hero.isGrounded)
          {
            V = V - g * Time.deltaTime;
            gravi.y = V;
            hero.Move(gravi);
        }
          if (injump && hero.isGrounded)
          {
            injump = false;
          }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "box" || hit.gameObject.name == "box (1)" || hit.gameObject.name == "box (2)" || hit.gameObject.name == "container")
        {
            hit.rigidbody.velocity = move * 2;
        }
    }
}
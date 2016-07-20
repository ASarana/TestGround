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
    public float m;
    float g;
    public float V0;
    //public Transform zeropos;

    void Start()
    {
        hero = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        jump = false;
        gravi = -Vector3.up;
        g = (float)9.8;
        m = 50;
        V0 = 1;
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
        hero.Move(speed * move); //двигаем 
        hero.Move(gravi); // прижимаем к земле
        if (move != Vector3.zero)
            transform.forward = move;
        if(jump)
        {
            V0 = V0 - g * Time.deltaTime;
            gravi.y = V0;
        }

        if (!jump) V0 = 5;

    }
}
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
    //public Transform zeropos;

    void Start()
    {
        hero = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {

        h = CrossPlatformInputManager.GetAxis("Horizontal");
        v = CrossPlatformInputManager.GetAxis("Vertical");
       
        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        //рассчитаем вектор движения 
        move = (v * camForward + h * cam.right).normalized;
        Y = h * 90 - v * 90;
        hero.Move(speed * move); //двигаем 
        hero.Move(-Vector3.up); // прижимаем к земле
      //  hero.transform.LookAt(move*500);
         this.transform.rotation = Quaternion.Euler(new Vector3(0,Y,0)); //поворачиваем по направлению

    }
}

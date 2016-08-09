using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class heromove : MonoBehaviour
{

    // Use this for initialization
    private CharacterController hero; //контроллер чтобы его двигать
    public Animator anima;
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
    public float g;
    public float V0;
    float V;
    //public Transform zeropos;
    public TextMesh debtext;
    string hitname;
    

    void Start()
    {
        hero = GetComponent<CharacterController>();
        anima = GetComponent<Animator>();
        cam = Camera.main.transform;
        jump = false;
        injump = false;
        gravi = -Vector3.up;
       // g = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        h = CrossPlatformInputManager.GetAxis("Horizontal");
        v = CrossPlatformInputManager.GetAxis("Vertical");
        jump = CrossPlatformInputManager.GetButton("Jump");
        //anima.SetBool("idletowalk", false);
        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        //рассчитаем вектор движения 
        move = (v * camForward + h * cam.right).normalized;
        debtext.transform.position = this.transform.position + new Vector3(-10, -11, -20);

        debtext.text = "hero.isGrounded:" + hero.isGrounded.ToString() + '\n'
            + "injump:" + injump.ToString() + '\n'
            + "jump:" + jump.ToString() + '\n'
            + "V:" + V.ToString() + '\n'
            + "gravi" + gravi.ToString() + '\n'
            + "anima idletowalk:" + anima.GetBool("idletowalk") + '\n'
            + "anima jumpup:" + anima.GetBool("jumpup") + '\n'
            + "anima jumpdown:" + anima.GetBool("jumpdown") + '\n'
            + "anima fall:" + anima.GetBool("fall") + '\n'
            + "anima push:" + anima.GetBool("push") + '\n'
            + "hit object:" + hitname + '\n';
        if (move != Vector3.zero)
        {
            transform.forward = move;
            if(!injump && hero.isGrounded)
            anima.SetBool("idletowalk", true);
            hero.Move(speed * move);
        }
        if (move == Vector3.zero)
        {
            anima.SetBool("idletowalk", false);
             anima.SetBool("push", false);
        }
            
        if (!injump && !hero.isGrounded)
        {
            // hero.Move(-Vector3.up);
            // anima.SetBool("jumpdown", false);
          //  V = 0;
            V = V - g * Time.deltaTime;
            gravi.y = V;
            hero.Move(gravi);
         //   anima.SetBool("idletowalk", false);
            anima.SetBool("fall", true);
        }
        if (jump && hero.isGrounded && !injump)
          {
            injump = true;
            V = V0;
            V = V - g * Time.deltaTime;
            gravi.y = V;
            hero.Move(gravi);
            anima.SetBool("jumpup", true);
          //  anima.SetBool("idletowalk", false);
        }
          if (injump && !hero.isGrounded)
          {
            V = V - g * Time.deltaTime;
            gravi.y = V;
            hero.Move(gravi);
            if (V <= 0)
            {
                anima.SetBool("jumpup", false);
                anima.SetBool("fall", true);
            }
        }
        if (!injump && hero.isGrounded)
        {
            anima.SetBool("jumpdown", false);
            anima.SetBool("fall", false);
            V = 0;
        }
        if (injump && hero.isGrounded)
          {
            injump = false;
            V = 0;
            anima.SetBool("fall", false);
            anima.SetBool("jumpdown", true);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitname = hit.gameObject.name;
        if (hit.gameObject.tag == "box")
        {
           /* Ray[,] ray = new Ray[5, 5];
            for(int i=0; i<5; i++)
                for (int j = 0; j < 5; j++)
                {

                }*/


             RaycastHit rhit;
            Ray ray = new Ray();
            ray.origin = new Vector3(hit.transform.position.x + hit.transform.lossyScale.x/2, hit.transform.position.y + hit.transform.lossyScale.y/2, hit.transform.position.z + hit.transform.lossyScale.z/2);
            ray.direction = hit.moveDirection; 
            if (!hit.collider.Raycast(ray, out rhit, 1))
            {
                hit.transform.position += move / 4;
            }
            Debug.DrawRay(ray.origin, ray.direction);
            anima.SetBool("push", true);
        }
        //else anima.SetBool("push", false);
    }
}
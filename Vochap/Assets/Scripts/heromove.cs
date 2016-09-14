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
    bool testkey;
    public float g;
    public float V0;
    float V;
    //public Transform zeropos;
    public TextMesh debtext;
    string hitname;
    public float pushPower = 2.0F;
    bool horpush;
    bool vertpush;
    Vector3 lefts;
    Vector3 rightss;
    Vector3 forwards; 
    Vector3 backs;
    Vector3 hitpoint;
    static GameObject rightgun;
    static GameObject backgun;
    GameObject leftgun;

    void Start()
    {
        hero = GetComponent<CharacterController>();
        anima = GetComponent<Animator>();
        cam = Camera.main.transform;
        jump = false;
        injump = false;
        testkey = false;
        gravi = -Vector3.up;
        // g = 5f;
         horpush = false;
         vertpush=false;
        // shotgun_vochap, machete_vochap, hand.R, neck

        rightgun = GameObject.Find("machete_vochap1");
        backgun = GameObject.Find("machete_vochap");
        Debug.Log(rightgun);
    }

    // Update is called once per frame
    void Update()
    {
        if(!vertpush) h = CrossPlatformInputManager.GetAxis("Horizontal");
        if (!horpush) v = CrossPlatformInputManager.GetAxis("Vertical");
        jump = CrossPlatformInputManager.GetButton("Jump");
        testkey = CrossPlatformInputManager.GetButton("Fire1");
        if (testkey)
        {
            // anima.SetBool("testanim", true);
           //  GameObject.Find("machete_vochap").SetActive(false);
           //  GameObject.Find("machete_vochap1").SetActive(true);
          //  Object.Instantiate(rightgun);
            rightgun.SetActive(true);
            backgun.SetActive(false);
        }
        else
        {
            //anima.SetBool("testanim", false);
            //  GameObject.Find("machete_vochap").SetActive(true);
            // GameObject.Find("machete_vochap1").SetActive(false);
            rightgun.SetActive(false);
            backgun.SetActive(true);
        }

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
            + "hit object:" + hitname + '\n'
            + "hit point left:" + lefts.ToString() + '\n'
            + "hit point right:" + rightss.ToString() + '\n'
            + "hit point forw:" + forwards.ToString() + '\n'
            + "hit point back:" + backs.ToString() + '\n'
            + "hit point:" + hitpoint.ToString() + '\n';
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
            horpush = false;
            vertpush = false;
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
        if (jump && hero.isGrounded && !injump && !anima.GetBool("push"))
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
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic || body.gameObject.name=="Terrain")
             return; 

        else hitname = hit.gameObject.name;

        if (hit.moveDirection.y < -0.3F)
            return;

        if (hero.isGrounded)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
           

             forwards = new Vector3(hit.collider.transform.position.x - hit.collider.transform.lossyScale.x/2, hit.collider.transform.position.y, hit.collider.transform.position.z);
             backs = new Vector3(hit.collider.transform.position.x + hit.collider.transform.lossyScale.x / 2, hit.collider.transform.position.y, hit.collider.transform.position.z);
             rightss = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z + hit.collider.transform.lossyScale.z / 2);
             lefts = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z - hit.collider.transform.lossyScale.z / 2);
            hitpoint = hit.point;

            if ((hit.point.z == lefts.z && ((hit.point.x >= lefts.x-3) && (hit.point.x <= lefts.x + 3)) || (hit.point.z == rightss.z && (hit.point.x >= rightss.x - 3) && (hit.point.x <= rightss.x + 3)) || (hit.point.x == forwards.x && (hit.point.z >= forwards.z - 3) && (hit.point.z <= forwards.z + 3)) || (hit.point.x == backs.x && (hit.point.z >= backs.z - 3) && (hit.point.z <= backs.z + 3))) 
                && pushDir != Vector3.zero 
                && (pushDir == Vector3.forward || pushDir == Vector3.back || pushDir == Vector3.right || pushDir == Vector3.left))
            {
                anima.SetBool("push", true);
                if (pushDir.normalized == Vector3.forward || pushDir.normalized == Vector3.back) horpush = true;
                if (pushDir.normalized == Vector3.left || pushDir.normalized == Vector3.right) vertpush = true;
                body.velocity = pushDir * pushPower;
            }

        }
    }

}
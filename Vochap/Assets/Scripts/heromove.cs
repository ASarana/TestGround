using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class heromove : MonoBehaviour
{

    // Use this for initialization
    public MyDebug Mydebug; // скрипт для отладки, котрорый выводит в тестовое поле все отлаживаемые переменые
    private CharacterController hero; //контроллер чтобы двигать персонажа
    Animator anima;
    public float RunSpeed; //множитель скорости персонажа
    private Vector3 move; //вектор движения 
    private Transform cam; //это чтобы рассчитать движение относительно камеры
    private Vector3 camForward; //это чтобы рассчитать движение относительно камеры
    private float v; //для хранения вертикальной оси ввода
    private float h; //для хранения горизонтальной оси ввода
    Vector3 gravi; //движение персонажа под гравитацией
    bool jump; //хранит нажатие кнопки прыжка
    bool injump; //хранит состояние персожажа, в прыжке-ли он
    bool testkey; 
    public float g; //ускорение свободного падения
    public float JumpSpeed;  //сила толчка при прыжке
    float V; //вертикальная скорость персонажа припрыжке (может быть отрицательная когда летит вниз)
    //public Transform zeropos;
    public TextMesh debtext; //текстовое поле для дебага
    public float pushPower; //множитель скорости толкания предмета
    bool horpush; //если персонаж уже толкает по горизонтали
    bool vertpush; //если персонаж уже толкает по вертикали
    static GameObject rightgun; //контейнр под оружие в правой руке
    static GameObject backgun; //контейнер под оружие за спиной
    static GameObject backgun2; //контейнер под второе оружие за спиной
    static GameObject leftgun; //контейнер под оружие в левой руке

    void Start()
    {
        hero = GetComponent<CharacterController>(); //инициализация компонента
        anima = GetComponent<Animator>(); //инициализация компонента
        cam = Camera.main.transform;
        jump = false;
        injump = false;
        testkey = false;
        gravi = -Vector3.up;
         horpush = false;
         vertpush=false;

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
       

        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        //рассчитаем вектор движения 
        move = (v * camForward + h * cam.right).normalized;

        WeaponChange(testkey);
        MyHeroMove(move, jump, gravi, anima);

        debtext.transform.position = this.transform.position + new Vector3(-10, -11, -20);

        Mydebug.AddParamDebug("hero.isGrounded", hero.isGrounded.ToString());
        Mydebug.AddParamDebug("injump", injump.ToString());
        Mydebug.AddParamDebug("jump", jump.ToString());
        Mydebug.AddParamDebug("V", V.ToString());
        Mydebug.AddParamDebug("gravi", gravi.ToString());
        Mydebug.AddParamDebug("idletowalk", anima.GetBool("idletowalk").ToString());
        Mydebug.AddParamDebug("jumpup", anima.GetBool("jumpup").ToString());
        Mydebug.AddParamDebug("jumpdown", anima.GetBool("jumpdown").ToString());
        Mydebug.AddParamDebug("fall", anima.GetBool("fall").ToString());
        Mydebug.AddParamDebug("push", anima.GetBool("push").ToString());
        Mydebug.ShowDebug();

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic || body.gameObject.name=="Terrain")
             return; 

        if (hit.moveDirection.y < -0.3F)
            return;

        if (hero.isGrounded)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            Vector3 lefts;
            Vector3 rightss;
            Vector3 forwards;
            Vector3 backs;
            Vector3 hitpoint;

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

    void MyHeroMove(Vector3 _move, bool _jump, Vector3 _gravi, Animator _anima)
    {
        if (_move != Vector3.zero)
        {
            transform.forward = _move;
            if (!injump && hero.isGrounded)
                _anima.SetBool("idletowalk", true);
            hero.Move(RunSpeed * move);
        }
        if (_move == Vector3.zero)
        {
            _anima.SetBool("idletowalk", false);
            _anima.SetBool("push", false);
            horpush = false;
            vertpush = false;
        }

        if (!injump && !hero.isGrounded)
        {
            V = V - g * Time.fixedDeltaTime;
            _gravi.y = V;
            hero.Move(_gravi);
            _anima.SetBool("fall", true);
        }
        if (_jump && hero.isGrounded && !injump && !anima.GetBool("push"))
        {
            injump = true;
            V = JumpSpeed;
            V = V - g * Time.fixedDeltaTime;
            _gravi.y = V;
            hero.Move(_gravi);
            _anima.SetBool("jumpup", true);
        }
        if (injump && !hero.isGrounded)
        {
            V = V - g * Time.fixedDeltaTime;
            _gravi.y = V;
            hero.Move(_gravi);
            if (V <= 0)
            {
                _anima.SetBool("jumpup", false);
                _anima.SetBool("fall", true);
            }
        }
        if (!injump && hero.isGrounded)
        {
            _anima.SetBool("jumpdown", false);
            _anima.SetBool("fall", false);
            V = 0;
        }
        if (injump && hero.isGrounded)
        {
            injump = false;
            V = 0;
            _anima.SetBool("fall", false);
            _anima.SetBool("jumpdown", true);
        }
    }

    void WeaponChange(bool change)
    {
        if (change)
        {
            anima.SetBool("testanim", true);
            rightgun.SetActive(true);
            backgun.SetActive(false);
        }
        else
        {
            rightgun.SetActive(false);
            backgun.SetActive(true);
        }
    }

}
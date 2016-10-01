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

    Vector3 gravi; //движение персонажа под гравитацией

    bool injump; //хранит состояние персожажа, в прыжке-ли он
    //кнопки ввода
    bool Fire1;
    float Fire2;
    bool Throw1;
    float Throw2;
    bool jump; //хранит нажатие кнопки прыжка
    float WeaponSwich;
    float ItemSwichUp;
    float ItemSwichDown;
    float ThrowSwich;
    bool Run;
    bool Use;
    bool Super;
    bool back;
    bool start;
    bool UseItem;
    private float v; //для хранения вертикальной оси ввода
    private float h; //для хранения горизонтальной оси ввода
    float v2; //правый стик
    float h2; //правый стик

    public float g; //ускорение свободного падения
    public float JumpSpeed;  //сила толчка при прыжке
    float V; //вертикальная скорость персонажа припрыжке (может быть отрицательная когда летит вниз)
    //public Transform zeropos;
    public TextMesh debtext; //текстовое поле для дебага
    public float pushPower; //множитель скорости толкания предмета
   // bool horpush; //если персонаж уже толкает по горизонтали
   // bool vertpush; //если персонаж уже толкает по вертикали
    int nothorpush; //если персонаж уже толкает по горизонтали
    int notvertpush; //если персонаж уже толкает по вертикали
    static GameObject rightgun; //контейнр под оружие в правой руке
    static GameObject backgun; //контейнер под оружие за спиной
    static GameObject backgun2; //контейнер под второе оружие за спиной
    static GameObject leftgun; //контейнер под оружие в левой руке

    void Start()
    {
        hero = GetComponent<CharacterController>(); //инициализация компонента
        anima = GetComponent<Animator>(); //инициализация компонента
        cam = Camera.main.transform; //положение камеры
        gravi = -Vector3.up; //гравитацию ставлю
        rightgun = GameObject.Find("machete_vochap1"); //запоминаю объекты
        backgun = GameObject.Find("machete_vochap");
      //  Debug.Log(rightgun);
    }

    // Update is called once per frame
    void Update()
    {
        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        
        ReadInput(); // считываю кнопки
        move = (v * camForward * nothorpush + h * cam.right*notvertpush).normalized;  //рассчитаем вектор движения 
        MyHeroMove(); //двигаем персонажа по вектору

        WeaponChange(WeaponSwich); //вркменно тут для смены оружия

        
           

        debtext.transform.position = this.transform.position + new Vector3(-10, -11, -20); // текствовое поле отладки следует за персонажем

        //наполняем дебаг
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
        Mydebug.ShowDebug(); // выводим дебаг
    }

    void OnControllerColliderHit(ControllerColliderHit hit) // это при столкновении персонажа с объктами
    {
        Rigidbody body = hit.collider.attachedRigidbody; //запоминаем тело, в которое уткнулись
        if (body == null || body.isKinematic || body.gameObject.name=="Terrain")
             return;  // если земля или кинематический, но на выход

        if (hit.moveDirection.y < -0.3F) //хз зачем оно
            return;

        if (hero.isGrounded) //если персонаж на земле, то двигаем
        {
            //тут идёт рассчет стороны и точки приложения силы, чтобы правильно обработать отпускание. Так же чтобы не менять направление при движении
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
                if (pushDir.normalized == Vector3.forward || pushDir.normalized == Vector3.back) nothorpush = 0;
                if (pushDir.normalized == Vector3.left || pushDir.normalized == Vector3.right) notvertpush = 0;
                body.velocity = pushDir * pushPower; // собственно двигаем предмет
            }

        }
    }

    void MyHeroMove() //сам метод перемещения и прыжков персонажа со сменой анимации 
    {
        if (move != Vector3.zero)
        {
            transform.forward = move;
            if (!injump && hero.isGrounded)
                anima.SetBool("idletowalk", true);
            hero.Move(RunSpeed * move);
        }
        if (move == Vector3.zero)
        {
            anima.SetBool("idletowalk", false);
            anima.SetBool("push", false);
            nothorpush = 1;
            notvertpush = 1;
        }

        if (!injump && !hero.isGrounded)
        {
            V = V - g * Time.fixedDeltaTime;
            gravi.y = V;
            hero.Move(gravi);
            anima.SetBool("fall", true);
        }
        if (jump && hero.isGrounded && !injump && !anima.GetBool("push"))
        {
            injump = true;
            V = JumpSpeed;
            V = V - g * Time.fixedDeltaTime;
            gravi.y = V;
            hero.Move(gravi);
            anima.SetBool("jumpup", true);
        }
        if (injump && !hero.isGrounded)
        {
            V = V - g * Time.fixedDeltaTime;
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

    void WeaponChange(float change) //смена оружия
    {
        if (change>0)
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

    void ReadInput() //метод чтения ввода с устройства
    {
        h = CrossPlatformInputManager.GetAxis("Horizontal");
        v = CrossPlatformInputManager.GetAxis("Vertical");
        h2 = CrossPlatformInputManager.GetAxis("Horizontal2");
        v2 = CrossPlatformInputManager.GetAxis("Vertical2");
        jump = CrossPlatformInputManager.GetButton("Jump");
        Fire1 = CrossPlatformInputManager.GetButton("Fire1");
        Fire2 = CrossPlatformInputManager.GetAxis("Fire2");
        Throw1 = CrossPlatformInputManager.GetButton("Throw1");
        Throw2 = CrossPlatformInputManager.GetAxis("Throw2");

        WeaponSwich = CrossPlatformInputManager.GetAxis("WeaponSwich");
        ItemSwichUp = CrossPlatformInputManager.GetAxis("ItemSwichUp");
        ItemSwichDown = CrossPlatformInputManager.GetAxis("ItemSwichDown");
        ThrowSwich = CrossPlatformInputManager.GetAxis("ThrowSwich");
        Run = CrossPlatformInputManager.GetButton("Run");
        Use = CrossPlatformInputManager.GetButton("Use");
        UseItem = CrossPlatformInputManager.GetButton("UseItem");
        Super = CrossPlatformInputManager.GetButton("Super");
        back = CrossPlatformInputManager.GetButton("back");
        start = CrossPlatformInputManager.GetButton("start");



        /*для джоя следующая раскладка:
      1. Крестовина вверх - предыдущий педмет
      2. Крестовина вниз - следующий предмет
      3. Крестовина вправо - сменить оружие
      4. Крестовина влево (зажать) - убрать оружие
      5. Крестовина влево - сметить метательное
      6. Кнопка A - прыжок
      7. Кнопка B - бег
      8. Кнопка X - использовать предмет
      9. Кнопка Y - Взаимодействие
      9. RB - слабая атака
      10. RT - сильная атака
      11. LB - слабый бросок
      12. LT - сильный бросок
      13. Нажать правый стик - суперка
      14. Нажать левый стик - ?
      15. Back - журнал/карта
      16. Start - menu

    */
    }

}
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class beh : MonoBehaviour {

    NavMeshAgent agent;
    public Animator anima;
    public float speed; //скорость и камеры
    private float v;
    private float h;
    private float Y;
    Vector3 gravi;
    bool jump;
    bool injump;
    public float g;
    public float V0;
    float V;
    public setka setka;
    Vector3[,] point = new Vector3[600, 300];
    int ii =5;
    int jj = 5;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        anima = GetComponent<Animator>();
        this.transform.position = setka.setk[ii,jj];
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(CrossPlatformInputManager.GetAxis("Vertical")==0)
            h = CrossPlatformInputManager.GetAxis("Horizontal");

        if (CrossPlatformInputManager.GetAxis("Horizontal") == 0)
         v = CrossPlatformInputManager.GetAxis("Vertical");

        jump = CrossPlatformInputManager.GetButton("Jump");


        if(agent.remainingDistance>=0 && agent.remainingDistance<= (setka.setk[2, 1].z - setka.setk[1, 1].z)/2)
        {
            if (h > 0 && ii<setka.kletx) ii++;
            if (h < 0 && ii > 1) ii--;
            if (v > 0 && jj > 1) jj--;
            if (v < 0 && jj < setka.kletz) jj++;
            agent.destination = setka.setk[ii, jj];
        }

        print(setka.setk[2, 1].z - setka.setk[1, 1].z);

        if (agent.remainingDistance == 0) anima.SetBool("idletowalk", false);
        else anima.SetBool("idletowalk", true);

    }
}

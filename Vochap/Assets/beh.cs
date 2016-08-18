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
    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        anima = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        h = CrossPlatformInputManager.GetAxis("Horizontal");
        v = CrossPlatformInputManager.GetAxis("Vertical");
        jump = CrossPlatformInputManager.GetButton("Jump");

        if(agent.remainingDistance>=0 && agent.remainingDistance < 10)
        {
            if (h > 0)
                agent.destination = this.transform.position + new Vector3(0,0,20);
            if (h < 0)
                agent.destination = this.transform.position + new Vector3(0, 0, -20);
            if (v > 0)
                agent.destination = this.transform.position + new Vector3(-20, 0, 0);
            if (v < 0)
                agent.destination = this.transform.position + new Vector3(20, 0, 0);
        }

        if (agent.remainingDistance == 0) anima.SetBool("idletowalk", false);
        else anima.SetBool("idletowalk", true);

    }
}

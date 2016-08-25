using UnityEngine;
using System.Collections;

public class setka : MonoBehaviour {

    public int kletx = 50;
    public int kletz = 50;
    public Vector3[,] setk = new Vector3[600,300];
    Terrain tc;

    // Use this for initialization
    void Awake()
    {
      //  kletx = 50;
      //  kletz = 50;
        tc = GetComponent<Terrain>();
        for (int i = 0; i < kletz; i++)
            for (int j = 0; j < kletx; j++)
            {
                setk[i, j].z = (tc.terrainData.size.z / kletz) * i;
                setk[i, j].x = (tc.terrainData.size.x / kletx) * j;
                setk[i, j].y = 25;

            }

    }
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < kletz; i++)
            for (int j = 0; j < kletx; j++)
            {
                Debug.DrawLine(setk[i, j], setk[i, j] + new Vector3(0,20), Color.green);
            }

    }
}

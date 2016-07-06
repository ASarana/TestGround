using UnityEngine;
using System.Collections;

public class CamMove : MonoBehaviour
{

    private Vector3 camzero;
    public Transform hero;

    void Start()
    {
        camzero = hero.position - this.transform.position; //начальное положение камера относительно пакмана
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = hero.position - camzero; //двигаем камеру в след за пакманом
    }
}

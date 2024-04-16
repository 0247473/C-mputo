using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject first;
    public GameObject third;
    //Este código es un script en C# que controla la rotación de la cámara en primera persona en Unity. Aquí se detalla lo que hace cada línea:
    public Transform playerBody; //Declara una variable para el cuerpo del jugador
    public float mouseSensitivity = 3.7f; //Declara una variable para la sensibilidad del mouse
    float xRotation = 0; //Inicializa la variable xRotation en cero
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Establece el estado del cursor al bloqueado
        Cursor.visible = false; //Oculta el cursor
    }

    void Update()
    {    


        //Obtiene el movimiento del mouse y calcula la rotación vertical y horizontal de la cámara
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        
         //Limita la rotación vertical a no más de90 hacia arriba o abajo
                                                       //Actualiza la rotación de la cámara en función de los valores calculados
        if (Time.timeScale==1)
        {
            if (gameObject.name=="Main Camera")
            {
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                first.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                //Gira el cuerpo del jugador horizontalmente
                playerBody.Rotate(Vector3.up * mouseX);
            }
            else if (gameObject.name=="tercera")
            {
                xRotation = Mathf.Clamp(xRotation, -10f, 10f);
                third.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
                //Gira el cuerpo del jugador horizontalmente
                playerBody.Rotate(Vector3.up * mouseX);
            }
            
        }
    }
}


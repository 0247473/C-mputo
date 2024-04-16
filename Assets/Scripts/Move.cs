using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class Move : MonoBehaviour
{
    
    public CharacterController controller;
    public TextMeshProUGUI score;
    GameObject image;
    public float speed;
    public static int puntos;
    public AudioSource pasos;
    public AudioSource daño;
    bool Hactivo,Vactivo;
    public static int vida;
    private void Start()
    {
        Time.timeScale = 1;
        vida = 3;
        puntos=0;
    }

    // Update es llamado una vez por frame
    void Update()
    {
        if (vida<=0)
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
            Time.timeScale = 0;
            SceneManager.LoadScene(2);
        }
        if (puntos>=LoadGame.forW)
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
            Time.timeScale = 0;
            SceneManager.LoadScene(3);
        }
        score.text=puntos+" / "+LoadGame.forW;
        
        // Decrementar el timer en cada frame.
        // Obtener el valor del eje horizontal y vertical.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calcular la dirección del movimiento del jugador.
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Horizontal")) {
            Hactivo=true; 
            pasos.Play();
        }
        if(Input.GetButtonDown("Vertical")) {
            Vactivo=true; 
            pasos.Play();
        }
        if(Input.GetButtonUp ("Horizontal")) {
            Hactivo= false;
            if (Vactivo==false)
            {
                pasos.Pause();
            }
        }
        if (Input.GetButtonUp ("Vertical"))
        {
            Vactivo = false;
            if(Hactivo==false){
                pasos.Pause();}
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("cactus"))
        {
            print("hola");
            daño.Play();
            image = GameObject.Find("vida"+vida);
            vida--;
            Destroy(image);
            
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class desetnterrar : MonoBehaviour
{
    public GameObject player;
    public GameObject particula;
    public AudioSource tesoro;
    public AudioSource dirt;
    GameObject image;
    bool cerca,saliendo,sono;
    GameObject premio;
    GameObject detroyPArt;
    
    
    // Start is called before the first frame update
    void Start()
    {
        sono=false;
        cerca = false;
        saliendo=false;
        player = GameObject.Find("Player"); 
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Abs(player.transform.position.x-transform.position.x);
        float z = Mathf.Abs(player.transform.position.z-transform.position.z);
        if (x<=3 && z<=3)
        {
            cerca=true;
        }
        else if((x>3&&x<7) || (z>3&&z<7))
        {
            cerca=false;
        }
        if ((Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.E))&&cerca==true){
            dirt.Play();
            Instantiate(particula,new Vector3(transform.position.x,.7f,transform.position.z), transform.rotation);
            Invoke("sal",1.4f);

        }
        if (saliendo)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x,2f,transform.position.z), 5f*Time.deltaTime);
        }
        if (transform.position.y>=0.04f&&sono==false)
        {
            tesoro.Play();
            sono=true;
            if ( name=="ankh(Clone)")
            {
                Move.puntos++;
                name="ankh(Clone)_USADA";
            }
            if ( name=="lupa(Clone)")
            {

                premio = GameObject.Find("flechaVerde(Clone)");
                premio.transform.localScale =new Vector3(2f,2f,2f);
                premio.name="flechaVerde(Clone)_USADO";
                
            }
            if ( name=="SCORPION(Clone)")
            {
                image = GameObject.Find("vida"+Move.vida);
                Move.vida--;
                Destroy(image);

            }
            
        }
        transform.LookAt(player.transform.position);
    }
    void sal(){
        saliendo=true;
        detroyPArt=GameObject.Find("polvito(Clone)");
        Destroy(detroyPArt);
    }

}

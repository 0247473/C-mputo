using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tesoro;
    public GameObject escorpion;
    public GameObject lupa;
    public GameObject arenita2;
    public GameObject arenita;
    public GameObject flechaR;
    public GameObject flechaV;
    public static int forW;
    void Start()
    {
        forW=0;
        int maxEsc=6;
        int maxLupas=5;
        for(int i=0; i<20; i++){
            
            float x = Random.Range(-180f,190f);
            float y= Random.Range(-190f,190f);
            int op = Random.Range(0,3);
            if (op==0)
            {
                Instantiate(tesoro,new Vector3(x,-0.25f,y),transform.rotation);
                Instantiate(arenita2,new Vector3(x,0.064f,y),transform.rotation);   
                Instantiate(flechaR,new Vector3(x,3f,y),transform.rotation);   
                Instantiate(flechaV,new Vector3(x,3f,y),transform.rotation);   
                forW++;
                
            }
            if (op==1&&maxEsc>0)
            {
                Instantiate(escorpion,new Vector3(x,-0.718f,y),transform.rotation);
                Instantiate(arenita,new Vector3(x,0.064f,y),transform.rotation);   
                Instantiate(flechaR,new Vector3(x,3f,y),transform.rotation);  
                maxEsc--;
                if (maxEsc<=0)
                {
                    i--;
                }
            }
            if (op==2&&maxLupas>0)
            {
                Instantiate(lupa,new Vector3(x,-0.25f,y),transform.rotation);
                Instantiate(arenita,new Vector3(x,0.064f,y),transform.rotation);   
                Instantiate(flechaR,new Vector3(x,3,y),transform.rotation);  
                maxLupas--;
                if (maxLupas<=0)
                {
                    i--;
                }
            } 
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

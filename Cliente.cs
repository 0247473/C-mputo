using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Cliente : MonoBehaviour
{
    public GameObject menu;
    public GameObject auth;
    public GameObject play;
    public GameObject[] error;
    public GameObject Username;
    public GameObject Password;
    public GameObject LgIn;
    public GameObject snIn;
    public GameObject lgOut;
    private bool login=false;

    public TMP_InputField  User;
    public TMP_InputField  Pass;

    public GameObject seingMal;
    public GameObject seingBien;
    


    public TMP_InputField  RegUser;
    public TMP_InputField  RegPass;
    public TMP_InputField  RegPassVal;
    public bool conected=true;

    private GameObject sameObjectName;
    private Dictionary<string, string> credentials = new Dictionary<string, string>();
    private bool p=false;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        // Agrega las credenciales al diccionario
        credentials.Add("Hola", "1234");
        credentials.Add("admin", "4321");
    }
    private void Start()
    {
        
        DontDestroyOnLoad(gameObject); // Asegúrate de que este objeto no se destruya al cambiar de escena
        
    }
    private void Update(){

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (p==true)
        {
            sameObjectName = GameObject.Find("Canvas");
            Destroy(sameObjectName);
        }
        
        if (currentSceneIndex != 0 && conected)
        {
            p=true;
            this.gameObject.name="ServerConection";
            error[1].SetActive(false);
            error[0].SetActive(false);
            error[2].SetActive(false);
            error[3].SetActive(false);;
            auth.SetActive(false);
            play.SetActive(false);
            Username.SetActive(false);
            Password.SetActive(false);
            menu.SetActive(false);
        }
        // Verificar si la escena actual no es la escena 0
        if (currentSceneIndex != 0 && !conected)
        {
            // Cargar la escena 0
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
            menu.SetActive(true);
            error[1].SetActive(true);
            error[0].SetActive(false);
            error[2].SetActive(false);
            error[3].SetActive(false);
            auth.SetActive(false);
            play.SetActive(false);
            Username.SetActive(false);
            Password.SetActive(false);

        }
        if (!conected)
        {
            error[1].SetActive(true);
            error[0].SetActive(false);
            error[2].SetActive(false);
            error[3].SetActive(false);
            auth.SetActive(false);
            play.SetActive(false);
            Username.SetActive(false);
            Password.SetActive(false);
        }
        else
        {
            error[1].SetActive(false);
        }
    }

    public void LogOut()
    {
        lgOut.SetActive(false);
        auth.SetActive(true);
        play.SetActive(false);
        Username.SetActive(true);
        Password.SetActive(true);
        User.text="";
        Pass.text="";
        login=false;
    }

    public void SignIn()
    {
        
        error[5].SetActive(false);
        error[4].SetActive(false);
        error[2].SetActive(false);
        error[3].SetActive(false);
        if (RegUser.text=="" || RegPass.text=="" || RegPassVal.text=="")
        {
            error[5].SetActive(true);
        
        }else
        {
            Availavility();
            RegUser.text="";
            RegPass.text="";
            RegPassVal.text="";
        }
        
        
    }

    public void SignInChange()
    {
        error[0].SetActive(false);
        RegUser.text="";
        RegPass.text="";
        RegPassVal.text="";
        snIn.SetActive(true);
        LgIn.SetActive(false);
    }

    public void LogInChange()
    {
        error[5].SetActive(false);
        error[4].SetActive(false);
        error[2].SetActive(false);
        error[3].SetActive(false);
        User.text="";
        Pass.text="";
        snIn.SetActive(false);
        LgIn.SetActive(true);
    }

    public void LogIn()
    {
        if (User.text=="" || Pass.text=="")
        {
            error[0].SetActive(true);
        }
        else
        {
            error[0].SetActive(false);
            Authenticate();
            if(login){
                lgOut.SetActive(true);
                auth.SetActive(false);
                play.SetActive(true);
                Username.SetActive(false);
                Password.SetActive(false);
            }
            else{
                auth.SetActive(true);
                play.SetActive(false);
                Username.SetActive(true);
                Password.SetActive(true);
            }
            User.text="";
            Pass.text="";
        }
    }

    private void Authenticate()
    {
        string inputUsername = User.text;
        string inputPassword = Pass.text;

        // Verifica si el usuario existe en el diccionario
        if (credentials.ContainsKey(inputUsername))
        {
            // Verifica si la contraseña es correcta
            if (credentials[inputUsername] == inputPassword)
            {
                error[0].SetActive(false);
                login=true;
            }
            else
            {
                User.text="";
                Pass.text="";
                error[0].SetActive(true);
            }
        }
        else
        {
            error[0].SetActive(true);
            User.text="";
            Pass.text="";
            
        }
    }
    private void Availavility()
    {
        seingMal.SetActive(false);
        seingBien.SetActive(true);
        string inputUsername = RegUser.text;
        string inputPassword = RegPass.text;
        string inputPasswordVal = RegPassVal.text;

        error[5].SetActive(false);
        error[4].SetActive(false);
        error[3].SetActive(false);
        error[2].SetActive(false);
        // Verifica si el usuario existe en el diccionario
        
        if(!credentials.ContainsKey(inputUsername))
        {
            error[5].SetActive(false);
            error[4].SetActive(false);
            error[3].SetActive(false);
            error[2].SetActive(false);
            
            if (inputPassword==inputPasswordVal)
            {
                error[3].SetActive(false);
                error[4].SetActive(true);
                credentials.Add(inputUsername, inputPassword);
            }
            else
            {
                RegUser.text="";
                RegPass.text="";
                RegPassVal.text="";
                error[5].SetActive(false);
                error[4].SetActive(false);
                error[2].SetActive(false);
                error[3].SetActive(true);
            }
            
        }
        else
        {
            RegUser.text="";
            RegPass.text="";
            RegPassVal.text="";
            error[2].SetActive(true);
        }
        
    }
}

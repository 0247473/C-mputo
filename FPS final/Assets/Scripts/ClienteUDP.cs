using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Net.NetworkInformation;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
public class ClienteUDP : MonoBehaviour
{
    const int PORT1 = 6666;
    const int PORT2 = 6667;
    const int BUFFER_SIZE = 6144;
    const int TIMEOUT = 5;
    public Text txt1;
    public Text txt2;
    public Text txt3;
    public Text txt4;
    public Text txt5;
    public Text txt6;
    private string serverIpAddress = "10.7.7.164";
    private string filename = "HowToPlay.txt";
    private float tiempoInicio;

    string clave= "DesBunduquia";
    private Socket socket;
    private IPEndPoint remoteEndPoint;
    private bool isConnectedToFirstPort = true;
    

    
    private void Start()
    {
        tiempoInicio = Time.time;
        txt1.text="Servicio no disponible";
        txt2.text="Servicio no disponible";
        txt3.text="Servicio no disponible";
        txt4.text="Servicio no disponible";
        txt5.text="Servicio no disponible";
        txt6.text="Servicio no disponible";
    }
    private void Update(){
        float tiempoTranscurrido = Time.time - tiempoInicio;
        if (tiempoTranscurrido >= 5f)
        {   
            int conect=TryConnectToServer(serverIpAddress);
            if(conect==1){
                TryConnectToServer(serverIpAddress, filename);
                FileInstruction(filename);
            }else{
                txt1.text="Servicio no disponible";
                txt2.text="Servicio no disponible";
                txt3.text="Servicio no disponible";
                txt4.text="Servicio no disponible";
                txt5.text="Servicio no disponible";
                txt6.text="Servicio no disponible";
            }
            tiempoInicio = Time.time;
        }
    }

    void XorCifrado(char[] palabra, char[] clave)
    {
        int palabraLen = palabra.Length;
        int claveLen = clave.Length;

        for (int i = 0; i < palabraLen; i++)
        {
            palabra[i] ^= clave[i % claveLen];
        }
    }

    public void TryConnectToServer(string ip, string filename)
    {
        try
        {
            /*
            Socket sockfd = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint servaddr = new IPEndPoint(IPAddress.Parse(ip), PORT);
            */
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), isConnectedToFirstPort ? PORT1 : PORT2);
            Debug.Log("Conectado al puerto: " + (isConnectedToFirstPort ? PORT1 : PORT2));

            char[] name = filename.ToCharArray();
            char[] claveArray = clave.ToCharArray();

            XorCifrado(name, claveArray);
            string resultado = "";
            foreach (char c in name)
            {
                resultado += string.Format("{0:X2}", (byte)c);
            }
            Debug.Log($"Mande al server: {resultado}");
            socket.SendTo(System.Text.Encoding.ASCII.GetBytes(resultado), remoteEndPoint);
            FileStream file = new FileStream(filename, FileMode.Create);
            byte[] buffer = new byte[BUFFER_SIZE*6];
            byte[] byteArray = new byte[BUFFER_SIZE*6];
            int bytesReceived;
            do
            {
                bytesReceived = socket.Receive(buffer);
                if (bytesReceived == 0)
                    break;
                string responseHex = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                Debug.Log($"Obtuve de server: {responseHex}");
                char[] responseC = ConvertHexStringToCharArray(responseHex);
                Debug.Log($"Respuesta convertida Caracter: {new string(responseC)}");
                XorCifrado(responseC,claveArray);
                string response = $"{new string(responseC)}";
                Debug.Log($"Respuesta convertida String: {response}");
                if (response == "FIN")
                {
                    break;
                }
                byteArray = System.Text.Encoding.ASCII.GetBytes(response);
                file.Write(byteArray, 0, byteArray.Length);
            } while (true);
            Debug.Log("Archivo recibido del servidor.");
            file.Close();
            socket.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al conectar: " + ex.Message);
            isConnectedToFirstPort = !isConnectedToFirstPort; // Intenta conectar al otro puerto
            TryConnectToServer(ip,filename);
        }
    }

    char[] ConvertHexStringToCharArray(string hex)
        {
            int length = hex.Length / 2;
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = (char)Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return buffer;
        }
    public int TryConnectToServer(string ip)
    {
        try
        {   /*
            Socket sockfd = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint servaddr = new IPEndPoint(IPAddress.Parse(ip), PORT);
            sockfd.Connect(servaddr);
            */
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), isConnectedToFirstPort ? PORT1 : PORT2);
            Debug.Log("Conectado al puerto: " + (isConnectedToFirstPort ? PORT1 : PORT2));
            socket.Close();
            return 1; // Conexión exitosa

        }
        catch (Exception ex)
        {
            Debug.LogError("Error al conectar: " + ex.Message);
            isConnectedToFirstPort = !isConnectedToFirstPort; // Intenta conectar al otro puerto
            int num_res= TryConnectToServer(ip); // Intentar nuevamente
            txt1.text="Servicio no disponible";
            txt2.text="Servicio no disponible";
            txt3.text="Servicio no disponible";
            txt4.text="Servicio no disponible";
            txt5.text="Servicio no disponible";
            txt6.text="Servicio no disponible";
            return num_res; // Error al conectar
        }
    }

    public void PrintFileContent(string filename)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Debug.Log(line);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error al leer el archivo: {ex.Message}");
            txt1.text="Servicio no disponible";
            txt2.text="Servicio no disponible";
            txt3.text="Servicio no disponible";
            txt4.text="Servicio no disponible";
            txt5.text="Servicio no disponible";
            txt6.text="Servicio no disponible";
        }
    }

    public void FileInstruction(string filename)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filename))
            {   
                txt1.text=reader.ReadLine();
                txt2.text=reader.ReadLine();
                txt3.text=reader.ReadLine();
                txt4.text=reader.ReadLine();
                txt5.text=reader.ReadLine();
                txt6.text=reader.ReadLine(); 
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error al leer el archivo: {ex.Message}");
            txt1.text="Servicio no disponible";
            txt2.text="Servicio no disponible";
            txt3.text="Servicio no disponible";
            txt4.text="Servicio no disponible";
            txt5.text="Servicio no disponible";
            txt6.text="Servicio no disponible";
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Net.NetworkInformation;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    public GameObject player;
    [Space]
    public Transform[] spawn;
    [Space]
    public GameObject roomCam;
    [Space]
    public GameObject nameUI;
    public GameObject connectingUI;
    private string nickname = "Anonymous";
    public string roomNameToJoin = "test";
    public string roomPassword = "";

    private List<Player> playersInRoom = new List<Player>();

    const int PORT1 = 5555;
    const int PORT2 = 5556;
    private string serverIpAddress = "10.7.7.164";
    string clave= "DesBunduquia";
    private bool isConnectedToFirstPort = true;
    private int num_lobbys = 0;
    const int MAX_BUFFER = 1024;
    private RoomList roomList;

    void Awake(){
        instance = this;
    }

    public void ChangeNickName(string _name){
        nickname = _name;
    }

    public void JoinRoomBtnPress(){
        string instructionC="Lobbys " + "Nombre:Test " + "Contraseña:1234";
        int ansC=SendRequestToServer(serverIpAddress,instructionC);
        Debug.Log($"Lobbys en Server: {ansC}");
        num_lobbys = ansC;

        if(num_lobbys <10){
            Debug.Log("Connecting...");
            Debug.Log(roomPassword);
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 6 };
            if (roomPassword != "")
            {
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "password", roomPassword } };
                roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };
                Debug.Log(roomNameToJoin +"  Nueva pass: " + roomPassword);
            }else
            {
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "NoPassword", roomPassword } };
                roomOptions.CustomRoomPropertiesForLobby = new string[] { "NoPassword" };
                Debug.Log(roomNameToJoin +"  No pass: " + roomPassword);
            }
            PhotonNetwork.JoinOrCreateRoom(roomNameToJoin,  roomOptions, TypedLobby.Default);
            nameUI.SetActive(false);
            connectingUI.SetActive(true);
            string instruction="Lobbys " + "Nombre:"+roomNameToJoin +" " + "Contraseña:" + roomPassword;
            
            int ans=SendRequestToServer(serverIpAddress,instruction);
            Debug.Log($"Respuesta server: {ans}");
            num_lobbys = ans;
        }else if(num_lobbys%2 == 0){
            roomList = FindObjectOfType<RoomList>();
            int act_num_lobbys= 0;
            if(roomList != null){
                act_num_lobbys = roomList.num_lobbys;
            }
            Debug.Log($"Actualizacion cada 2 Lobbys_Creados {act_num_lobbys}");
            string instruction=$"Actualizacion Lobbys_Creados {act_num_lobbys}";
            int ans=SendRequestToServer(serverIpAddress,instruction);
            Debug.Log($"Respuesta server: {ans}");
            num_lobbys = ans;
            if(num_lobbys >= 10){
                Debug.Log("No hay mas espacios");
                Debug.Log("Maximo de lobbys alcanzados");
            }
            else{
                Debug.Log("Se actualizo la cantidad de servidores\nIntentalo de nuevo");
            }
        }else{
            roomList = FindObjectOfType<RoomList>();
            int act_num_lobbys= 0;
            if(roomList != null){
                act_num_lobbys = roomList.num_lobbys;
            }
            Debug.Log($"Actualizacion Lobbys_Creados {act_num_lobbys}");
            string instruction=$"Actualizacion Lobbys_Creados {act_num_lobbys}";
            int ans=SendRequestToServer(serverIpAddress,instruction);
            Debug.Log($"Respuesta server: {ans}");
            num_lobbys = ans;
            if(num_lobbys >= 10){
                Debug.Log("No hay mas espacios");
                Debug.Log("Maximo de lobbys alcanzados");
            }
            else{
                Debug.Log("Se actualizo la cantidad de servidores\nIntentalo de nuevo");
            }
            
        }
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();
        Debug.Log("We're connected and in a room now");
        roomCam.SetActive(false);
        playersInRoom.Add(PhotonNetwork.LocalPlayer);
        RespawnPlayer();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        base.OnPlayerEnteredRoom(newPlayer);
        playersInRoom.Add(newPlayer);
        Debug.Log(newPlayer.NickName + " has joined the room.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);
        playersInRoom.Remove(otherPlayer);
        Debug.Log(otherPlayer.NickName + " has left the room.");
    }

    public void RespawnPlayer(){
        Transform spawns = spawn[UnityEngine.Random.Range(0, spawn.Length)];
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawns.position, Quaternion.identity);
        _player.GetComponent<PPlayerSetup>().IsLocalPLayer();
        _player.GetComponent<Health>().IsLocalPLayer = true;
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

[PunRPC]
public void KickPlayer(string playerNameToKick)
{
    Debug.Log("Trying to kick " + playerNameToKick);
    if (PhotonNetwork.LocalPlayer.IsMasterClient)
    {
        Player playerToKick = null;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerNameToKick)
            {
                playerToKick = player;
                break;
            }
        }

        if (playerToKick != null)
        {
            //Lobby.SetActive(true);

            
            // Expulsar al jugador de la sala
            PhotonNetwork.CloseConnection(playerToKick);
            Debug.Log("Player kicked: " + playerNameToKick);

        }
        else
        {
            Debug.LogWarning("Player " + playerNameToKick + " not found in the room.");
        }
    }
    else
    {
        Debug.LogWarning("Only the MasterClient can kick players from the room.");
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

string ExtractFirstValue(string input)
    {
        // Remove the curly braces and single quotes
        input = input.Trim(new char[] { '{', '}', '\'' });
        
        // Split the string by the colon
        string[] parts = input.Split(':');
        
        // Return the first part, which is the desired value
        return parts[0];
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

public int SendRequestToServer(string ip, string request)
    {
        int result1 = TrySendRequest(ip, request, PORT1);
        int result2 = TrySendRequest(ip, request, PORT2);

        if (result1 != 0)
        {
            return result1;
        }
        else if (result2 != 0)
        {
            return result2;
        }
        else
        {
            Debug.Log("Ambos servidores están caídos o no respondieron correctamente.");
            return 0; 
        }
    }

private int TrySendRequest(string ip, string request, int port)
    {
        TcpClient client = null;
        NetworkStream stream = null;
        try
        {
            client = new TcpClient(ip, port);
            Debug.Log("Conectado al puerto: " + port);
            Debug.Log($"La petición del cliente es: {request}");
            if (client.Connected)
            {
                stream = client.GetStream();
                char[] buffer = request.ToCharArray();
                char[] claveArray = clave.ToCharArray();

                XorCifrado(buffer, claveArray);
                string resultado = "";
                foreach (char c in buffer)
                {
                    resultado += string.Format("{0:X2}", (byte)c);
                }
                Debug.Log($"Mande al server: {resultado}");
                byte[] bufferEn = System.Text.Encoding.ASCII.GetBytes(resultado);
                stream.Write(bufferEn, 0, bufferEn.Length);
                byte[] responseBuffer = new byte[MAX_BUFFER];
                int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                string responseHex = Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
                Debug.Log($"Obtuve de server: {responseHex}");
                string resultEn = ExtractFirstValue(responseHex);
                char[] responseC = ConvertHexStringToCharArray(resultEn);
                Debug.Log($"Respuesta convertida Caracter: {new string(responseC)}");
                XorCifrado(responseC,claveArray);
                string response = $"{new string(responseC)}";
                Debug.Log($"Respuesta convertida String: {response}");
                int result;
                if (int.TryParse(response, out result))
                {
                    return result;
                }
                return 0;
            }
            else
            {
                return 33; 
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error: {ex.Message}");
            isConnectedToFirstPort = !isConnectedToFirstPort; 
            return 0;
        }
        finally
        {
            if (stream != null)
            {
                stream.Close();
            }
            if (client != null)
            {
                client.Close();
            }
        }
    }


    

}

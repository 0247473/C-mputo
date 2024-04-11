using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    const int PORT = 5555;
    const int MAX_BUFFER = 1024;
    const int TIMEOUT = 5;

    static void Main(string[] args)
    {
        TcpClient client = null;
        NetworkStream stream = null;

        try
        {
            // Crear el socket
            client = new TcpClient();

            // Configurar el tiempo de espera
            client.SendTimeout = TIMEOUT * 1000;
            client.ReceiveTimeout = TIMEOUT * 1000;

            // Conectar al servidor
            IPAddress ipAddress = IPAddress.Parse("192.168.0.105");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);
            client.Connect(remoteEP);

            if (client.Connected)
            {
                Console.WriteLine($"Conexión exitosa con la IP: {ipAddress}");
                stream = client.GetStream();

                while (true)
                {
                    Console.Write("Ingrese una palabra: ");
                    string palabra = Console.ReadLine();

                    if (palabra == "close")
                    {
                        break;
                    }

                    // Enviar la palabra al servidor
                    byte[] buffer = Encoding.ASCII.GetBytes(palabra);
                    stream.Write(buffer, 0, buffer.Length);

                    // Recibir la respuesta del servidor
                    byte[] responseBuffer = new byte[MAX_BUFFER];
                    int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                    string response = Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);

                    if (response == "close")
                    {
                        break;
                    }

                    // Procesar la respuesta del servidor
                    string numberError = "";
                    if (!string.IsNullOrEmpty(response))
                    {
                        string[] parts = response.Split(':');
                        if (parts.Length > 1)
                        {
                            numberError = parts[1].Trim('\'');
                        }
                    }

                    Console.WriteLine($"Respuesta del servidor: {numberError}");
                }
            }
            else
            {
                Console.WriteLine("Error al conectar con el servidor");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Cerrar la conexión
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
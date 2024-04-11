require 'socket'
require 'thread'

PORT = 6666
BUFFER_SIZE = 1024
MAX_CLIENTS = 15

def handle_client(client_info)
  cliaddr = client_info[:cliaddr]
  filename = client_info[:filename]

  sockfd = Socket.new(:AF_INET, :SOCK_DGRAM, 0)

  file = File.open(filename, 'r') # Abrir el archivo en modo de lectura de texto
  if file.nil?
    puts "Error al abrir el archivo"
    return
  end

  # Enviar datos del archivo al cliente en bloques
  buffer = ''
  while (line = file.gets)
    sockfd.send(line, 0, cliaddr)
    print"#{line}\n"
  end
  sockfd.send("FIN", 0, cliaddr)
  puts "Archivo enviado al cliente."

  # Cerrar el archivo y el socket
  file.close
  sockfd.close
rescue => e
  puts "Error: #{e.message}"
end

def main
  buffer = ''
  threads = []
  client_count = 0

  puts "Escuchando en el puerto número: #{PORT}"

  # Crear socket UDP
  sockfd = Socket.new(:AF_INET, :SOCK_DGRAM, 0)

  # Configurar dirección del servidor
  servaddr = Socket.pack_sockaddr_in(PORT, '0.0.0.0')

  # Enlazar dirección del servidor al socket
  sockfd.bind(servaddr)

  # Obtener e imprimir la dirección IP en la que está escuchando
  ip_str = sockfd.local_address.ip_address
  puts "Escuchando en la IP: #{ip_str}"

  loop do
    # Recibir nombre del archivo del cliente
    buffer, cliaddr = sockfd.recvfrom(BUFFER_SIZE)
    puts "Recibiendo del cliente: #{buffer}"

    # Crear un nuevo hilo para manejar la solicitud del cliente
    if client_count < MAX_CLIENTS
      client_info = { cliaddr: cliaddr, filename: buffer.strip }
      thread = Thread.new { handle_client(client_info) }
      threads << thread
      client_count += 1
    else
      puts "Número máximo de clientes alcanzado. Solicitud rechazada."
    end
  end

  # Esperar a que todos los hilos terminen
  threads.each(&:join)

  sockfd.close
rescue => e
  puts "Error: #{e.message}"
end

main


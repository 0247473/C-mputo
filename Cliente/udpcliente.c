#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <unistd.h>
#include <errno.h>
#include <sys/time.h>

#define BUFFER_SIZE 1024
#define PORT 6666

int main() {
    char ip[16];
    char filename[50];
    struct sockaddr_in servaddr;
    //sprintf(ip, "10.7.3.223");
    sprintf(ip, "172.18.2.3");
    sprintf(filename, "HowToPlay.txt");

    // Crear socket
    int sockfd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (sockfd == -1) {
        perror("Fallo en la creación del socket");
        exit(EXIT_FAILURE);
    }

    // Configurar dirección del servidor
    memset(&servaddr, 0, sizeof(servaddr));
    servaddr.sin_family = AF_INET;
    servaddr.sin_addr.s_addr = inet_addr(ip);
    servaddr.sin_port = htons(PORT);

    // Enviar nombre del archivo al servidor
    sendto(sockfd, filename, strlen(filename), 0,
           (struct sockaddr *)&servaddr, sizeof(servaddr));

    FILE *file = fopen(filename, "w"); // Abrir el archivo en modo de escritura de texto
    if (file == NULL) {
        perror("Error al abrir el archivo");
        exit(EXIT_FAILURE);
    }

    // Recibir datos del archivo del servidor en bloques
    char buffer[BUFFER_SIZE];
    int bytes_received;
    while ((bytes_received = recvfrom(sockfd, buffer, BUFFER_SIZE - 1, 0, NULL, NULL)) > 0) {
        buffer[bytes_received] = '\0'; // Agregar el terminador de cadena
        if (strcmp(buffer, "FIN") == 0) {
            // El servidor ha terminado de enviar datos, salir del bucle
            break;
        }
        fprintf(file, "%s", buffer); // Escribir en el archivo
    }

    if (bytes_received == -1) {
        perror("recvfrom failed");
        exit(EXIT_FAILURE);
    }

    printf("Archivo recibido del servidor.\n");
    fclose(file);

    close(sockfd);

    return 0;
} 
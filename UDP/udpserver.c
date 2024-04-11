#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <unistd.h>

#define PORT 15000
#define BUFFER_SIZE 1024

int main() {
    char buffer[BUFFER_SIZE];
    struct sockaddr_in servaddr, cliaddr;
    int sockfd, len;

    printf("Escuchando en el puerto número: %d\n", PORT);

    // Crear socket UDP
    sockfd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (sockfd == -1) {
        perror("Fallo en la creación del socket");
        exit(EXIT_FAILURE);
    }

    // Configurar dirección del servidor
    memset(&servaddr, 0, sizeof(servaddr));
    servaddr.sin_family = AF_INET;
    servaddr.sin_addr.s_addr = htonl(INADDR_ANY);
    servaddr.sin_port = htons(PORT);

    // Enlazar dirección del servidor al socket
    if (bind(sockfd, (struct sockaddr *)&servaddr, sizeof(servaddr)) < 0) {
        perror("Fallo en el enlace del socket");
        exit(EXIT_FAILURE);
    }

    // Recibir nombre del archivo del cliente
    len = sizeof(cliaddr);
    int n = recvfrom(sockfd, buffer, BUFFER_SIZE, 0,
                     (struct sockaddr *)&cliaddr, (socklen_t *)&len);
    if (n < 0) {
        perror("recvfrom failed");
        exit(EXIT_FAILURE);
    } else {
        buffer[n] = '\0';
        printf("Recibiendo del cliente: %s\n", buffer);
    }

    char filename[50];
    strcpy(filename, buffer);

    // Abrir archivo a enviar
    FILE *file = fopen(filename, "rb");
    if (file == NULL) {
        perror("Error al abrir el archivo");
        exit(EXIT_FAILURE);
    }

    // Enviar datos del archivo al cliente en bloques
    char bufferArc[BUFFER_SIZE];
    int bytes_read;
    while ((bytes_read = fread(bufferArc, sizeof(char), BUFFER_SIZE, file)) > 0) {
        sendto(sockfd, bufferArc, bytes_read, 0,
               (struct sockaddr *)&cliaddr, sizeof(cliaddr));
    }

    printf("Archivo enviado al cliente.\n");

    // Cerrar el archivo y el socket
    fclose(file);
    close(sockfd);

    return 0;
}

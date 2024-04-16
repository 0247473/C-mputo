#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <unistd.h>
#include <sys/wait.h>

#define PORT 6666
#define BUFFER_SIZE 1024
#define MAX_CLIENTS 10

int main() {
    char buffer[BUFFER_SIZE];
    struct sockaddr_in servaddr, cliaddr;
    int sockfd, len, num_clients = 0;
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
    
    // Obtener e imprimir la dirección IP en la que está escuchando
    char ip_str[INET_ADDRSTRLEN];
    inet_ntop(AF_INET, &(servaddr.sin_addr), ip_str, INET_ADDRSTRLEN);
    printf("Escuchando en la IP: %s\n", ip_str);
    
    while (1) {
        // Recibir nombre del archivo del cliente
        len = sizeof(cliaddr);
        int n = recvfrom(sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&cliaddr, (socklen_t *)&len);
        if (n < 0) {
            perror("recvfrom failed");
            exit(EXIT_FAILURE);
        }
        buffer[n] = '\0';
        printf("Recibiendo del cliente: %s\n", buffer);
        
        // Crear un proceso hijo para atender al cliente
        pid_t pid = fork();
        if (pid < 0) {
            perror("fork failed");
            exit(EXIT_FAILURE);
        } else if (pid == 0) {
            // Código del proceso hijo
            FILE *file = fopen(buffer, "r");
            if (file == NULL) {
                perror("Error al abrir el archivo");
                exit(EXIT_FAILURE);
            }
            
            char bufferArc[BUFFER_SIZE];
            while (fgets(bufferArc, BUFFER_SIZE, file) != NULL) {
                sendto(sockfd, bufferArc, strlen(bufferArc), 0, (struct sockaddr *)&cliaddr, sizeof(struct sockaddr_in));
            }
            sendto(sockfd, "FIN", 3, 0, (struct sockaddr *)&cliaddr, sizeof(struct sockaddr_in));
            printf("Archivo enviado al cliente.\n");
            fclose(file);
            exit(0); // Salir del proceso hijo
        } else {
            // Código del proceso padre
            num_clients++;
            if (num_clients >= MAX_CLIENTS) {
                // Esperar a que un proceso hijo termine
                int status;
                waitpid(-1, &status, 0);
                num_clients--;
            }
        }
    }
    
    close(sockfd);
    return 0;
}

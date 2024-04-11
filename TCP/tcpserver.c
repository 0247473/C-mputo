#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <errno.h>
#include <fcntl.h>
#include <sys/time.h>
#include <netdb.h>
#include <signal.h>
#include <unistd.h>
#include <pthread.h>

#define PUERTO 5050
#define MAX_MSG_SIZE 2048
#define MAX_CLIENTS 10

int sd;
struct sockaddr_in sind, pin;
char sigue = 'S';

void aborta_handler(int sig) {
    printf("....abortando el proceso servidor %d\n", sig);
    close(sd);
    exit(1);
}

void ActivityLog(char *ins, char *hour, char *date){
    FILE *archivo = fopen("ActivityLog.txt", "a"); 
        if (archivo == NULL) {
            perror("Error al abrir el archivo");
            exit(1);
        }
        fprintf(archivo, "\n");
        fprintf(archivo, "%s %s %s", ins,hour,date);
        fclose(archivo);
}

int UserDate( char *ins, char *userins, char *passwordins) {
    FILE *archivo;
    archivo = fopen("UserData.txt", "r");
    if (archivo == NULL) {
        perror("Error al abrir el archivo");
        exit(1);
    }
    char linea[MAX_MSG_SIZE];
    int encontrado = 0; 
    while (fgets(linea, MAX_MSG_SIZE, archivo) != NULL) {
        linea[strcspn(linea, "\n")] = 0;
        char *user = NULL;
        char *password = NULL;
        if (linea != NULL && linea[0] != '\0') {
            char *temp = strdup(linea);
            char *token = strtok(linea, " ");
            if (token != NULL) {
                user = strdup(token);
                token = strtok(NULL, " ");
                if (token != NULL) {
                    password = strdup(token); 
                }
            }
        }
        if (strcmp(user,userins)==0){
            encontrado+=2;
            if (strcmp(ins,"SignIn")==0){
                encontrado=76;
                fclose(archivo);
                return encontrado;
            }
        }
        if (strcmp(password,passwordins)==0){
            encontrado+=3;
            if (strcmp(ins,"SignIn")==0){
                encontrado=87;
                fclose(archivo);
                return encontrado;
            }
        }
        if (encontrado==5){
            if (strcmp(ins,"LogIn")==0){
                fclose(archivo);
                return encontrado;
            }
        }else{
            encontrado=0;
        }
    }
    fclose(archivo);
    if(strcmp(ins,"SignIn")==0){
        FILE *archivo = fopen("UserData.txt", "a"); 
        if (archivo == NULL) {
            perror("Error al abrir el archivo");
            exit(1);
        }
        fprintf(archivo, "\n");
        fprintf(archivo, "%s %s", userins, passwordins);
        fclose(archivo);
        encontrado=5;
        return encontrado;
    }
    return encontrado;
}


void *handle_client(void *arg) {
    int sd_actual = *(int *)arg;
    char *msg = malloc(MAX_MSG_SIZE * sizeof(char));
    char *json = malloc(MAX_MSG_SIZE * sizeof(char));

    memset(msg, 0, MAX_MSG_SIZE);
    memset(json, 0, MAX_MSG_SIZE);

    int n = recv(sd_actual, msg, MAX_MSG_SIZE - 1, 0);
    if (n == -1) {
        perror("recv");
        exit(1);
    }

    msg[n] = '\0';
    printf("Client sent: %s\n", msg);
    char *instruction = NULL;
    char *user = NULL;
    char *password = NULL;
    if (strcmp(msg,"")==0) {
    }
    else{
        char *token = strtok(msg, " ");
        if (token != NULL) {
            instruction = strdup(token);
            token = strtok(NULL, " ");
            if (token != NULL) {
                user = strdup(token); 
                token = strtok(NULL, " ");
                if (token != NULL) {
                    password = strdup(token); 
                }
            }
        }
    }
    if (instruction != NULL){
        if (strcmp(instruction, "LogIn") == 0) {
            int ans1=UserDate(instruction, user, password);
            snprintf(json, MAX_MSG_SIZE, "{'%d':'%d'},", ans1,ans1);
        }
        else if(strcmp(instruction, "SignIn") == 0){
            int ans=UserDate(instruction, user, password);
            snprintf(json, MAX_MSG_SIZE, "{'%d':'%d'},", ans,ans);
        }else if(strcmp(instruction, "Activity")==0){
            ActivityLog(instruction,user, password);
        }else{
            snprintf(json, MAX_MSG_SIZE, "{'Vacio':'Vacio'}");
        }
    }
    
    int sent;
    if ((sent = send(sd_actual, json, strlen(json), 0)) == -1) {
        perror("send");
        exit(1);
    }

    close(sd_actual);
    free(msg);
    free(json);
    free(instruction);
    free(user);
    free(password);
    pthread_exit(NULL);
}


int main() {
    if (signal(SIGINT, aborta_handler) == SIG_ERR) {
        perror("Could not set signal handler");
        return 1;
    }

    sind.sin_family = AF_INET;
    sind.sin_addr.s_addr = INADDR_ANY;
    sind.sin_port = htons(PUERTO);

    if ((sd = socket(AF_INET, SOCK_STREAM, 0)) == -1) {
        perror("socket");
        exit(1);
    }

    if (bind(sd, (struct sockaddr *)&sind, sizeof(sind)) == -1) {
        perror("bind");
        exit(1);
    }

    if (listen(sd, MAX_CLIENTS) == -1) {
        perror("listen");
        exit(1);
    }

    while (sigue == 'S') {
        int addrlen;
        int sd_actual;
        pthread_t thread;

        if ((sd_actual = accept(sd, (struct sockaddr *)&pin, &addrlen)) == -1) {
            perror("accept");
            exit(1);
        }

        if (pthread_create(&thread, NULL, handle_client, &sd_actual) != 0) {
            perror("pthread_create");
            close(sd_actual);
        }
    }

    close(sd);
    printf("\nConexion cerrada\n");
    return 0;
}

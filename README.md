# SISTEMA-DE-GESTI-N-DE-SOLICITUDES-INTERNAS
Sistema Full-Stack diseñado para la gestión y seguimiento de solicitudes internas. El proyecto ha sido desarrollado bajo una arquitectura orientada a capas, integrando protocolos de autenticación seguros y un diseño de interfaz responsivo.
Tecnologías Utilizadas
•	Backend: .NET 8 (C#) con Entity Framework Core.
•	Base de Datos: PostgreSQL.
•	Frontend: JavaScript nativo (Vanilla JS) y Tailwind CSS.
•	Seguridad: JWT (JSON Web Tokens).
Instrucciones para la puesta en marcha
Para ejecutar el proyecto en su entorno local, por favor siga los pasos descritos a continuación:
1. Requisitos Previos
•	.NET 8 SDK instalado.
•	PostgreSQL instalado y en ejecución.
•	Editor de código (se recomienda Visual Studio Code).
2. Configuración de la Base de Datos
1.	Cree una base de datos en su instancia de PostgreSQL denominada IntercoopDB.
2.	Ejecute el script SQL proporcionado en la carpeta /scripts del repositorio para realizar la creación de las tablas y la definición de las relaciones de integridad.
3.	Localice el archivo appsettings.json en la carpeta IntercoopAPI/ y actualice la cadena de conexión con los parámetros de su entorno local:
JSON
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=IntercoopDB;Username=SU_USUARIO;Password=SU_CONTRASEÑA"
}
3. Ejecución del Backend
1.	Abra una terminal en el directorio IntercoopAPI/.
2.	Restaure las dependencias del proyecto:
Bash
dotnet restore
3.	Inicie el servidor de la API:
Bash
dotnet run
El servicio estará disponible de manera predeterminada en el puerto 5284.
4. Ejecución del Frontend
1.	Diríjase a la carpeta Frontend/.
2.	Abra el archivo index.html en su navegador de preferencia. Se recomienda utilizar el servidor local de su editor de código para asegurar la correcta comunicación con la API.
Consideraciones Técnicas
•	Eliminación Lógica: El sistema implementa una política de eliminación lógica en todos sus módulos. La visibilidad de los registros en la interfaz depende del valor booleano en la columna Activo dentro de la base de datos.
•	Seguridad: Se utiliza autenticación basada en tokens JWT. Es imperativo que el archivo de configuración (appsettings.json) no sea compartido públicamente si contiene credenciales de acceso reales.
Nota final
Este sistema ha sido desarrollado bajo los estándares solicitados en la prueba técnica, garantizando la escalabilidad del backend y la eficiencia en la gestión de datos.

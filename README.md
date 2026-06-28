# Sistema de Gestión de Expedientes (SGE) - Fase 2 (Entrega Final)

Este proyecto representa la evolución final del Sistema de Gestión de Expedientes (SGE) desarrollado en .NET, aplicando de forma estricta los principios de **Arquitectura Limpia (Clean Architecture)** y **Diseño Guiado por el Dominio (DDD)**. Se ha reemplazado la interfaz de consola previa por una arquitectura moderna basada en servicios web, almacenamiento persistente real con **SQLite**, control transaccional mediante el patrón **Unit of Work** y un esquema robusto de seguridad basado en tokens **JWT**.

## 🚀 Instrucciones de Ejecución y Acceso

Para levantar el servidor de la API y comenzar a probar los servicios, seguí estos pasos:

1. Abrí tu terminal.
2. Posicionate dentro de la carpeta del proyecto de presentación (Web API):
   ```bash
   cd SGE.WebApi
   ```
3. Ejecutá el proyecto con el siguiente comando:
   dotnet run
4. Una vez que la consola indique que el servidor está corriendo, abrí tu navegador web e ingresá al panel interactivo de documentación en el siguiente enlace:
   http://localhost:5013/swagger

👥 Usuarios de Prueba y Gestión de Accesos (Seeds)

Al inicializarse la base de datos a través de EnsureCreated , el sistema configura automáticamente la propiedad journal_mode=DELETE en SQLite e inyecta un conjunto de datos semilla con tres perfiles bien diferenciados para testear las restricciones de seguridad. Todas las contraseñas originales se procesan mediante una función de hash criptográfica segura (SHA-256) , por lo que nunca se almacenan en texto plano. Para probar los flujos, el evaluador cuenta con las siguientes credenciales documentadas:

Administrador
Usuario: admin@sge.com  
Contraseña: admin123
Esta cuenta tiene acceso total a todos los endpoints del sistema. El servicio de autorizaciones le concede automáticamente todos los permisos sin restricciones.

Usuario con Permisos Parciales:
Usuario: parcial@sge.com
Contraseña: user123
Esta cuenta tiene permisos asignados en base de datos para operaciones específicas (ej: ExpedienteAlta , TramiteAlta).

Usuario de Lectura (Sin Permisos):
Usuario: lectura@sge.com
Contraseña: user123
No posee ningún permiso de mutación en su colección, contando únicamente con acceso de lectura básico.

## 🧪 Guía de Prueba de Endpoints

Para evaluar el comportamiento del sistema con total tranquilidad, se aconseja seguir estrictamente este orden lógico de interacción:

### 1. Autenticación y Obtención del Token JWT

En Swagger (Scalar), buscá el módulo Usuarios y desplegá el endpoint POST /api/usuarios/login. Usá las credenciales del Administrador en el cuerpo de la petición y dale a Execute. El sistema validará el hash e interceptará la petición devolviendo un 200 OK con un JSON que contiene la identidad y el Token JWT. Copiá todo ese token de acceso.

![alt text](/misc/Screenshot_1.png)
![alt text](/misc/Screenshot_2.png)

### 2. Activar el Candado Global de Seguridad

Subí arriba de todo en la interfaz de Swagger y dale click al botón "Authorize" (el ícono del candado). En el cuadro de texto, pegá el Token JWT que copiaste y dale a Authorize. A partir de este momento, tu identidad básica (UserId) estará vinculada de forma invisible en la cabecera Authorization de todas las peticiones protegidas.

![alt text](/misc/Screenshot_3.png)
![alt text](/misc/Screenshot_4.png)

### 3. Testeo del Módulo de Expedientes y Trámites

Tanto en la pestaña Expedientes como en Trámites vas a poder probar todos los endpoints que permiten hacer uso del sistema de gestión. Algunos casos son:

Alta de Expediente (POST /api/expedientes): Verificá que el cuerpo de la petición solo pide la carátula (ningún ID de usuario se viaja por el JSON body, el UserId se extrae automáticamente del claim del token protegiendo el diseño del software).

Gestión de Trámites (POST /api/tramites): Creá un trámite vinculándolo al Guid del expediente generado. Esto disparará en segundo plano la orquestación del servicio de actualización automática del estado del expediente.

Consultas Detalladas (GET /api/expedientes/{id}): Recupera toda la información del expediente junto con la colección completa de los trámites asociados en un único objeto plano.

### 4. Operaciones de Administración Exclusivas

Al estar autenticado con la cuenta admin@sge.com, el sistema validará de forma efectiva que poseés el privilegio mayor y te permitirá operar en los siguientes endpoints exclusivos:

GET /api/usuarios (Listar todos los usuarios del sistema).
PUT /api/usuarios/{id}/permisos (Asignar o remover colecciones de permisos enviando listas de strings correspondientes al enumerativo)
DELETE /api/usuarios/{id} (Dar de baja cuentas de usuario del sistema).

Para probar: Si repetís este flujo logueándote con lectura@sge.com, al intentar usar cualquiera de estos tres endpoints, la API te denegará la acción de inmediato lanzando una respuesta de autorización.

### 5. Comportamiento y Validación de Errores

La Web API tiene configurado un Manejador Global de Excepciones (UseExceptionHandler) en el inicio de su pipeline. Cualquier fallo de lógica o validación es interceptado para devolver una respuesta bajo el formato industrial estándar ProblemDetails con su respectivo código de estado HTTP:

1. Error 401 Unauthorized (Sin Token / Token Expirado)
   Cómo probarlo: Desactivá el candado de Swagger (haciendo Logout en el modal de autorización) e intentá realizar un listado de expedientes.  
   Salida esperada: El middleware de ASP.NET Core frena la petición de raíz y devuelve un 401 Unauthorized.

2. Error 403 Forbidden (Falta de Permisos / Regla de Implicancia)
   Cómo probarlo: Logueate con el usuario de prueba sin permisos (lectura@sge.com), activá el candado e intentá dar de alta un expediente.
   Salida esperada: El Caso de Uso invoca al IAutorizacionService , comprueba que la colección no tiene el enum necesario y lanza una AutorizacionException , la cual el middleware traduce en un 403 Forbidden con el mensaje de acceso denegado.

3. Error 404 Not Found (Entidad No Encontrada)
   Cómo probarlo: En cualquier endpoint que reciba un ID por la ruta, ingresá un Guid aleatorio que no exista en las tablas.
   Salida esperada: El sistema arroja una EntidadNoEncontradaException, devolviendo una estructura limpia con código 404 Not Found.

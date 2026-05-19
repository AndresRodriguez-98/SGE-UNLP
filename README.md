# Sistema de Gestión de Expedientes (SGE)

Este proyecto es una implementación de un Sistema de Gestión de Expedientes (SGE) desarrollado en .NET, aplicando los principios de **Arquitectura Limpia (Clean Architecture)** y **Diseño Guiado por el Dominio (DDD)**.

## 🚀 Instrucciones de Ejecución

Para probar el software y ver su funcionamiento en la consola, seguí estos pasos:

1. Abrí tu terminal
2. Posicionate dentro de la carpeta del proyecto, y luego en la carpeta de consola:
   cd SGE.Consola
3. Ejecutá el proyecto con el siguiente comando:
    dotnet run
4. Una vez que el programa termine de ejecutarse, podés abrir los archivos expedientes.txt y tramites.txt generados en esa misma carpeta para corroborar cómo se guardaron y modificaron los datos en tiempo real.

🛑 ¡CONSIDERACIÓN IMPORTANTE PARA LA PRUEBA! 🛑

El archivo Program.cs está configurado para ejecutar todos los Casos de Uso de corrido como una demostración completa. Esto incluye los casos de uso de Eliminar Expediente y Eliminar Trámite al final de la ejecución.

Si querés inspeccionar los archivos .txt y ver los datos persistidos, DEBÉS COMENTAR las líneas de código correspondientes a la eliminación en el Program.cs antes de ejecutar "dotnet run" (PASO 3). De lo contrario, el programa creará, modificará y finalmente eliminará los registros tan rápido que los archivos .txt quedarán vacíos al finalizar.

## 🧪 Pruebas de Funcionalidad (`Program.cs`)

A continuación, se detalla cómo el `Program.cs` orquesta la prueba de todas las funcionalidades del sistema:

### 1. Alta de Expedientes
El sistema permite crear un expediente nuevo validando que el usuario tenga los permisos necesarios.

### 2. Listado de Expedientes
Se recuperan todos los expedientes desde el repositorio de texto plano.

### 3. Modificación de Carátula y Cambio de Estado
Se prueba la mutación de la entidad.

### 4. Gestión de Trámites (Alta y Efecto Cascada)
Al agregar un trámite a un expediente existente.

### 5. Modificación de Trámites
Se actualiza la etiqueta y el contenido de un trámite

### 6. Baja de Expedientes y Trámites (Eliminación)
El sistema prueba la eliminación de un trámite.


Salida esperada:

![alt text](image.png)

expedientes.txt:

![alt text](image-1.png)

tramites.txt:

![alt text](image-2.png)

Eliminación en cascada:

![alt text](image-3.png)

## Prueba de errores:

## 1. Error de Autorización:

Cambiar a "false" el valor de retorno del servicio de autorización AutorizacionProvisionalService:
![alt text](image-5.png)

Salida:
![alt text](image-4.png)

## 2. Error de Entidad no Encontrada:

Cambiar el id por un nuevo GUID en algun metodo que busque una entidad por ID como el "Modificar" en el 'Program.cs':
![alt text](image-7.png)

Salida:
![alt text](image-6.png)

## 3. Error de Repositorio:

Podemos crear un nuevo id que no exista en nuestro .txt e intentar llamar al método eliminar DIRECTAMENTE desde el repositorio, pasa saltarnos el caso de uso:
![alt text](image-9.png)

Salida:
![alt text](image-8.png)



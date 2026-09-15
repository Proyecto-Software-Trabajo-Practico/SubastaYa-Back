# SubastaYa - Backend Web API (.NET 8)

Backend para el sistema de subastas SubastaYa, desarrollado en C# y .NET 8 siguiendo los principios de Clean Architecture con separación en las capas Domain, Application, Data y SubastaYa API.

Para ejecutar el proyecto en un entorno local es necesario contar con .NET 8 SDK (o Visual Studio 2022 con la carga de trabajo Desarrollo de ASP.NET y web), Docker Desktop en ejecución y Git.

El primer paso para iniciar es clonar el repositorio mediante el comando `git clone https://github.com/Proyecto-Software-Trabajo-Practico/SubastaYa-Back.git` y posicionarse dentro de la carpeta del proyecto. A continuación, se debe levantar la base de datos SQL Server 2022 en Docker abriendo una terminal en la raíz de la solución y ejecutando `docker compose up -d`, lo cual creará y pondrá en marcha el contenedor escuchando en el puerto 1433[cite: 1]. Una vez activo el contenedor, se deben impactar las migraciones de Entity Framework Core para crear la base de datos SubastaYaDb y sus tablas ejecutando `Update-Database -Project Data -StartupProject SubastaYa` desde la Consola del Administrador de Paquetes en Visual Studio, o bien `dotnet ef database update --project Data --startup-project SubastaYa` desde la terminal de comandos[cite: 1]. Con la base de datos lista, la Web API se puede iniciar presionando F5 en Visual Studio o ejecutando `dotnet run --project SubastaYa` desde la consola. Al iniciar la aplicación, la documentación interactiva estará disponible a través de Swagger UI en la ruta `https://localhost:7127/swagger`. Finalmente, para detener el servicio de base de datos al concluir la sesión de trabajo sin perder la información guardada, basta con ejecutar el comando `docker compose down`[cite: 1].

Script de Automatización: Se utilizó un script en Powershell ejecutando dos pujas en paralelo

/*
param (
    [string]$Url = "url/api/Pujas", 
    [string]$Token = "Token JWT", 
    [int]$SubastaId = id,
    [decimal]$Monto = Monto
)

# Ignorar certificado SSL a nivel de sesion principal
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$headers = @{
    "Authorization" = "Bearer $Token"
    "Content-Type"  = "application/json"
}

$body = @{
    subastaId = $SubastaId
    monto     = $Monto
} | ConvertTo-Json

$scriptBlock = {
    param($endpoint, $hdrs, $payload)
    
    # Ignorar certificado SSL dentro de cada hilo secundario
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
    
    try {
        # Removido el parametro -SkipCertificateCheck incompatiible con PowerShell 5.1
        $response = Invoke-RestMethod -Uri $endpoint -Method Post -Headers $hdrs -Body $payload
        return "SUCCESS (201 Created) - Puja procesada correctamente."
    }
    catch {
        if ($_.Exception.Response) {
            $code = [int]$_.Exception.Response.StatusCode
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $bodyError = $reader.ReadToEnd()
            return "RESPUESTA HTTP $code - DETALLE: $bodyError"
        }
        return "ERROR LOCAL: $($_.Exception.Message)"
    }
}

Write-Host "Lanzando 2 peticiones simultaneas en el mismo milisegundo..." -ForegroundColor Yellow

$job1 = Start-Job -ScriptBlock $scriptBlock -ArgumentList $Url, $headers, $body
$job2 = Start-Job -ScriptBlock $scriptBlock -ArgumentList $Url, $headers, $body

$res1 = Receive-Job -Job $job1 -Wait
$res2 = Receive-Job -Job $job2 -Wait

Write-Host "`n--- RESULTADOS DEL STRESS TEST ---" -ForegroundColor Cyan
Write-Host "Peticion 1: $res1"
Write-Host "Peticion 2: $res2"

Remove-Job -Job $job1, $job2
*/

Resultado obtenido:

Lanzando 2 peticiones simultaneas en el mismo milisegundo...

--- RESULTADOS DEL STRESS TEST ---
Peticion 1: SUCCESS (201 Created) - Puja procesada correctamente.
Peticion 2: RESPUESTA HTTP 409 - DETALLE: {"error":"Otra operación modificó el recurso al mismo tiempo. Por favor actualizá y reintentá."}
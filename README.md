# SubastaYa - Backend API (.NET 8)

Backend de SubastaYa, plataforma web de subastas en tiempo real desarrollada con C# .NET 8, 
Entity Framework Core (Code-First) y SQL Server, aplicando principios de Clean Architecture y 
patrones de diseño orientados a alta concurrencia.

---

## Tecnologías y Arquitectura

* **Framework:** .NET 8 Web API
* **Persistencia:** Entity Framework Core (Enfoque Code-First)
* **Base de Datos:** SQL Server 2022 (Dockerizada)
* **Concurrencia:** Optimistic Locking (Campo `RowVersion` en Subasta y Billetera)
* **Tiempo Real:** SignalR (WebSockets)
* **Procesos en Segundo Plano:** Background Worker (`BackgroundService`) para cierre automático de subastas y liquidación atómica (Escrow)
* **Documentación:** OpenAPI / Swagger UI

---

## Requisitos Previos e Instalación

### 1. Requisitos
* Docker Desktop en ejecución
* .NET 8 SDK

### 2. Levantar la Base de Datos con Docker
Desde la raíz del proyecto backend (`SubastaYa-Back`), ejecutar en la terminal:

```bash
docker compose up -d
```

### 3. Ejecutar Migraciones y Seed Data
Las migraciones incluyen la creación del esquema y la carga de datos iniciales (usuarios, billeteras, subastas de prueba):

```bash
dotnet ef database update --project Infrastructure --startup-project SubastaYa
```

### 4. Ejecutar la Web API
```bash
dotnet run --project SubastaYa
```

La documentación interactiva y los endpoints estarán disponibles en:
* **Swagger UI:** `https://localhost:7127/swagger`

---

## Validación de Concurrencia (Stress Test)

Para verificar el control de concurrencia optimista (**Optimistic Concurrency**) ante condiciones de carrera (dos postores ofertando en el mismo milisegundo), se incluye el script en PowerShell `test_concurrencia.ps1`:

```powershell
param (
    [string]$Url = "https://localhost:7127/api/Pujas", 
    [string]$Token = "Token JWT", 
    [int]$SubastaId = 2001,
    [decimal]$Monto = 50000.00
)

# Ignorar certificado SSL a nivel de sesión principal
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
    
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
    
    try {
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
```

### Resultado obtenido ante colisión concurrente:

```text
Lanzando 2 peticiones simultaneas en el mismo milisegundo...

--- RESULTADOS DEL STRESS TEST ---
Peticion 1: SUCCESS (201 Created) - Puja procesada correctamente.
Peticion 2: RESPUESTA HTTP 409 - DETALLE: {"error":"Otra operación modificó el recurso al mismo tiempo. Por favor actualizá y reintentá."}
```

>  El código HTTP `409 Conflict` en la segunda petición demuestra que la transacción detectó una incompatibilidad en la versión (`RowVersion`) del registro, impidiendo inconsistencias de saldo o estados corruptos sin necesidad de bloqueos pesados en la base de datos.
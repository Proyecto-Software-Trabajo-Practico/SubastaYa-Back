param (
    [string]$Url = "https://localhost:7127/api/Pujas", 
    [string]$Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYwMDEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJwb3N0b3IzQHRlc3QuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6InBvc3RvcjMiLCJleHAiOjE3ODkzODQyMDEsImlzcyI6IlN1YmFzdGFZYUFQSSIsImF1ZCI6IlN1YmFzdGFZYUZyb250ZW5kIn0.6mlK3ramB8NIplG-TJIrrvw3vJ0ogFoCGAFbg5u0Dvs", 
    [int]$SubastaId = 2001,
    [decimal]$Monto = 50000.00
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
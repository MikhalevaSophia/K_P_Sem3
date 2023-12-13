# Установка модуля Git-WinEvent (если не установлен)
if (-not (Get-Module -Name Git-WinEvent -ListAvailable)) {
    Install-Module -Name Git-WinEvent -Force -Scope CurrentUser
}

# Функция для скачивания журнала событий и записи в файл
function DownloadEventLog {
    param(
        [string]$vmIpAddress,
        [string]$sshUsername,
        [string]$sshKeyPath,
        [string]$sshPassword
    )

    # Путь к файлу, в который будет записан журнал событий
    $eventLogFilePath = "путь_к_event.txt"

    # Формирование команды PowerShell для скачивания журнала событий
    $powershellCommand = "Get-WinEvent -LogName 'System', 'Application' | Out-File -FilePath $eventLogFilePath"

    # Формирование команды SSH для выполнения команды PowerShell
    $sshCommandWithPowerShell = "ssh -i $sshKeyPath $sshUsername@$vmIpAddress '$powershellCommand'"

    try {
        # Попытка скачивания журнала событий на виртуальной машине
        Invoke-Expression -Command $sshCommandWithPowerShell
        Write-Host "Журнал событий успешно скачан и сохранен в $eventLogFilePath"
    } catch {
        Write-Host "Не удалось скачать журнал событий."
        Write-Host "Ошибка: $_"
    }
}

# Параметры для подключения
$vmIpAddress = "IP_вашей_виртуальной_машины"
$sshUsername = "ваш_пользователь"
$sshKeyPath = "путь_к_вашему_ключу"
$sshPassword = "ваш_пароль"

# Вызов функции для скачивания журнала событий из виртуальной машины
DownloadEventLog -vmIpAddress $vmIpAddress -sshUsername $sshUsername -sshKeyPath $sshKeyPath -sshPassword $sshPassword
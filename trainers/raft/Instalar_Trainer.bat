@echo off
setlocal
title Instalador de Raft Trainer
cls
echo ========================================================
echo          INSTALADOR DE RAFT ULTIMATE TRAINER
echo ========================================================
echo.

set "GAME_DIR=D:\SteamLibrary\steamapps\common\Raft"

if not exist "%GAME_DIR%\Raft.exe" (
    echo [!] No se encontro en ruta por defecto. Buscando...
    if exist "C:\Program Files (x86)\Steam\steamapps\common\Raft\Raft.exe" (
        set "GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Raft"
    )
)

if not exist "%GAME_DIR%\Raft.exe" (
    echo [ERROR] No se pudo encontrar el directorio de Raft.exe
    pause
    exit /b 1
)

echo [+] Directorio detectado: %GAME_DIR%
echo [+] Copiando archivos del Trainer...

copy /y "%~dp0doorstop\x64\winhttp.dll" "%GAME_DIR%\winhttp.dll"
copy /y "%~dp0doorstop_config.ini" "%GAME_DIR%\doorstop_config.ini"
copy /y "%~dp0RaftTrainer.dll" "%GAME_DIR%\RaftTrainer.dll"

if exist "%GAME_DIR%\RaftTrainer.dll" (
    echo.
    echo ========================================================
    echo       TRAINER INSTALADO CORRECTAMENTE CON EXITO!
    echo ========================================================
    echo.
    echo Instrucciones para jugar:
    echo 1. Inicia Raft normalmente desde Steam.
    echo 2. Entra a un mundo / partida.
    echo 3. Presiona [F1] o [Insert] para abrir el menu del Trainer.
    echo.
    echo Teclas rapidas:
    echo  [F1] -> Menu Principal
    echo  [F2] -> Modo Dios
    echo  [F3] -> Hambre Infinita
    echo  [F4] -> Sed Infinita
    echo  [F5] -> Oxigeno Infinito
    echo  [F6] -> Modo Vuelo / NoClip
    echo  [K]  -> Matar Tiburon
    echo.
) else (
    echo [ERROR] No se pudieron copiar los archivos. Ejecuta como Administrador.
)

pause

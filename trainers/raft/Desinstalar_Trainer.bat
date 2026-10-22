@echo off
setlocal
title Desinstalador de Raft Trainer
cls
echo ========================================================
echo          DESINSTALADOR DE RAFT ULTIMATE TRAINER
echo ========================================================
echo.

set "GAME_DIR=D:\SteamLibrary\steamapps\common\Raft"

if not exist "%GAME_DIR%\Raft.exe" (
    if exist "C:\Program Files (x86)\Steam\steamapps\common\Raft\Raft.exe" (
        set "GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Raft"
    )
)

echo Eliminando archivos del Trainer de: %GAME_DIR%

if exist "%GAME_DIR%\winhttp.dll" del /f /q "%GAME_DIR%\winhttp.dll"
if exist "%GAME_DIR%\doorstop_config.ini" del /f /q "%GAME_DIR%\doorstop_config.ini"
if exist "%GAME_DIR%\RaftTrainer.dll" del /f /q "%GAME_DIR%\RaftTrainer.dll"

echo.
echo ========================================================
echo       TRAINER DESINSTALADO COMPLETAMENTE!
echo ========================================================
echo El juego ha vuelto a su estado original.
echo.
pause

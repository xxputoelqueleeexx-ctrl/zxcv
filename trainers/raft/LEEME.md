# ⚓ Raft Ultimate Trainer (The Final Chapter)

Trainer profesional, completo e integrado para **Raft** (Steam ID: 648800).
Diseñado específicamente para la versión oficial de Unity Mono de Raft.

---

## 🚀 Inicio Rápido (Instalación en 1 Clic)

Tienes dos formas súper sencillas de usar el Trainer:

### Opción 1: Usando la Aplicación Visual de Escritorio (Recomendado)
1. Haz doble clic en **`Abrir_Trainer_GUI.bat`**.
2. Haz clic en el botón verde **`⚡ Instalar Trainer (1-Click)`**.
3. Haz clic en **`🚀 Iniciar Raft (Steam)`**.
4. ¡Listo! Cuando entres a tu partida, presiona **`F1`** o **`Insert`** para desplegar el menú de trucos.

### Opción 2: Instalador Directo .BAT
1. Haz doble clic en **`Instalar_Trainer.bat`**.
2. Iniciará y copiará los archivos a la carpeta de Raft automáticamente.
3. Abre el juego desde Steam normalmente.

---

## ⌨️ Teclas Rápidas Globales (Hotkeys)

Puedes activar los trucos en cualquier momento mientras juegas con estas teclas:

| Tecla | Función | Descripción |
| :--- | :--- | :--- |
| **`F1`** o **`Insert`** | **Abrir / Cerrar Menú del Trainer** | Muestra la interfaz gráfica dentro del juego |
| **`F2`** | **Modo Dios (God Mode)** | Salud infinita e invulnerabilidad total |
| **`F3`** | **Hambre Infinita** | La barra de comida nunca baja |
| **`F4`** | **Sed Infinita** | Hidratación siempre al 100% |
| **`F5`** | **Oxígeno Infinito** | Buceo ilimitado sin ahogarse |
| **`F6`** | **Modo Vuelo / NoClip** | Vuela libremente por el aire y mar |
| **`K`** | **Matar al Tiburón** | Elimina al instante al tiburón activo |

> **Controles en Modo Vuelo (NoClip):**  
> `W`, `A`, `S`, `D` = Desplazamiento  
> `Espacio` = Subir  
> `Ctrl Izquierdo` o `C` = Bajar  
> `Shift Izquierdo` = Acelerar velocidad de vuelo  

---

## 📋 Características y Opciones del Menú In-Game (`F1`)

Al presionar `F1` en tu partida, aparecerá la ventana con 4 pestañas:

### 👤 1. Pestaña Jugador
- **Modo Dios**: Inmune a mordeduras de tiburón, ataques de jabalíes/osos y daño de cualquier tipo.
- **Hambre & Sed Infinitas**: Mantén tus necesidades al 100% de forma permanente.
- **Oxígeno Infinito**: Explora arrecifes sumergidos sin preocuparte por el aire.
- **Sin Daño por Caída**: Salta desde acantilados o torres sin recibir daño.
- **Multiplicadores de Velocidad**: Modifica la velocidad a 1x, 1.5x, 2x, 3x o 5x (Flash).
- **Súper Salto**: Salta a 2x o 3x de altura para alcanzar lugares elevados.
- **Botón Sanar al 100%**: Restaura toda tu vida, hambre, sed y oxígeno en 1 instante.

### 🎒 2. Pestaña Inventario y Crafteo
- **Durabilidad Infinita**: Tus herramientas, ganchos, hachas, espadas y arcos nunca se romperán.
- **Crafteo Gratis / Recetas**: Craftea cualquier objeto sin necesidad de tener los materiales.
- **Reparar Todo el Inventario**: Repara al 100% todas las herramientas y equipamiento con 1 clic.
- **Duplicar / Stacks a 99**: Rellena todas las pilas de objetos de tu inventario a 99 unidades.
- **Generador de Recursos Rápidos (1-Clic)**:
  - `+20 Madera (Plank)`
  - `+20 Plástico (Plastic)`
  - `+20 Chatarra (Scrap)`
  - `+10 Lingote de Titanio (Titanium Ingot)`
  - `+10 Lingote de Metal (Metal Ingot)`
  - `+10 Lingote de Cobre (Copper Ingot)`
  - `+5 Batería de Titanio (Advanced Battery)`
  - `+1 Gancho de Titanio`
  - `+1 Hacha de Titanio`
  - `+1 Espada de Titanio`
  - `+1 Arco + 30 Flechas de Titanio`
  - `+10 Carne de Tiburón Cocinada`
- **Buscador de Items**: Escribe el nombre de cualquier objeto del juego y la cantidad para spawnearlo al instante en tu inventario.

### 🦈 3. Pestaña Tiburón y Mundo
- **Matar Tiburón**: Elimina a los tiburones que rodean la balsa.
- **Pacificar Tiburón**: El tiburón se vuelve pasivo; no ataca la balsa ni a tu personaje.
- **Congelar Tiburón**: Detiene al tiburón en su posición.
- **Velocidad del Juego (Game Speed)**: Acelera o ralentiza el tiempo del juego (0.5x, 1x, 2x, 5x).
- **Desbloquear Comandos de Chat**: Habilita la consola y comandos de desarrollador en el chat (`/give`, `/tp`, etc.).

### 📍 4. Pestaña Teletransporte
- **Guardar Posición**: Guarda tu punto actual en el mapa (isla, fondo marino o barco).
- **Teletransportar a Posición Guardada**: Vuelve inmediatamente al punto guardado.
- **Teletransportar a la Balsa**: Si caes al mar o te quedas atrás, regresa automáticamente al centro de tu balsa.
- **Impulso hacia arriba**: Te eleva 10 metros en el aire para salir de atascos.

---

## 🗑️ Cómo Desinstalar el Trainer

Si en algún momento quieres volver a jugar en modo 100% vainilla sin trampas:
1. Abre **`Trainer_GUI.py`** y pulsa **`🗑️ Desinstalar Trainer`** o ejecuta **`Desinstalar_Trainer.bat`**.
2. El trainer eliminará los 3 archivos sin tocar ni modificar ningún archivo original de Raft ni de Steam.

---

## 🛠️ Recompilación (Opcional para Desarrolladores)

Si deseas modificar el código fuente en C# (`RaftTrainer.cs`):
1. Edita el archivo `RaftTrainer.cs`.
2. Ejecuta en PowerShell: `.\compile_trainer.ps1`
3. Se generará un nuevo `RaftTrainer.dll` listo para usar.

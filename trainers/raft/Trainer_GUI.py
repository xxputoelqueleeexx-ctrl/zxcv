"""
Raft Ultimate Trainer - Panel de Control & Gestor de Trucos
"""
import os
import sys
import shutil
import subprocess
import webbrowser
import threading
import time
import tkinter as tk
from tkinter import ttk, messagebox

# Rutas estándar del juego
DEFAULT_RAFT_PATH = r"D:\SteamLibrary\steamapps\common\Raft"
ALT_RAFT_PATH = r"C:\Program Files (x86)\Steam\steamapps\common\Raft"

def find_raft_directory():
    if os.path.exists(os.path.join(DEFAULT_RAFT_PATH, "Raft.exe")):
        return DEFAULT_RAFT_PATH
    if os.path.exists(os.path.join(ALT_RAFT_PATH, "Raft.exe")):
        return ALT_RAFT_PATH
    return DEFAULT_RAFT_PATH

def is_raft_running():
    try:
        output = subprocess.check_output('tasklist /FI "IMAGENAME eq Raft.exe" /NH', shell=True).decode('utf-8', errors='ignore')
        return "Raft.exe" in output
    except Exception:
        return False

def is_trainer_installed(game_dir):
    return (
        os.path.exists(os.path.join(game_dir, "winhttp.dll")) and
        os.path.exists(os.path.join(game_dir, "doorstop_config.ini")) and
        os.path.exists(os.path.join(game_dir, "RaftTrainer.dll"))
    )

class TrainerApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("⚓ Raft Ultimate Trainer - Panel de Control")
        self.geometry("780x620")
        self.minsize(740, 580)
        self.configure(bg="#181825")

        self.game_dir = find_raft_directory()
        self.app_dir = os.path.dirname(os.path.abspath(__file__))

        self.setup_styles()
        self.build_ui()
        self.start_status_monitor()

    def setup_styles(self):
        style = ttk.Style(self)
        style.theme_use("clam")

        # Configuración general
        style.configure(".", background="#181825", foreground="#cdd6f4", font=("Segoe UI", 10))
        style.configure("TFrame", background="#181825")
        style.configure("Card.TFrame", background="#1e1e2e", relief="flat")
        
        style.configure("TLabel", background="#181825", foreground="#cdd6f4")
        style.configure("Card.TLabel", background="#1e1e2e", foreground="#cdd6f4")
        style.configure("Title.TLabel", background="#1e1e2e", foreground="#89b4fa", font=("Segoe UI", 16, "bold"))
        style.configure("Sub.TLabel", background="#1e1e2e", foreground="#a6adc8", font=("Segoe UI", 9))
        style.configure("Header.TLabel", background="#1e1e2e", foreground="#f9e2af", font=("Segoe UI", 11, "bold"))
        
        # Botones de acción
        style.configure("Primary.TButton", background="#89b4fa", foreground="#11111b", font=("Segoe UI", 10, "bold"), borderwidth=0, padding=8)
        style.map("Primary.TButton", background=[("active", "#b4befe")])

        style.configure("Success.TButton", background="#a6e3a1", foreground="#11111b", font=("Segoe UI", 10, "bold"), borderwidth=0, padding=8)
        style.map("Success.TButton", background=[("active", "#94e2d5")])

        style.configure("Danger.TButton", background="#f38ba8", foreground="#11111b", font=("Segoe UI", 10, "bold"), borderwidth=0, padding=8)
        style.map("Danger.TButton", background=[("active", "#eba0ac")])

        style.configure("Secondary.TButton", background="#313244", foreground="#cdd6f4", font=("Segoe UI", 9), borderwidth=0, padding=6)
        style.map("Secondary.TButton", background=[("active", "#45475a")])

    def build_ui(self):
        # Header banner
        header = ttk.Frame(self, style="Card.TFrame", padding=15)
        header.pack(fill="x", padx=15, pady=(15, 10))

        title_lbl = ttk.Label(header, text="⚓ RAFT ULTIMATE TRAINER", style="Title.TLabel")
        title_lbl.pack(anchor="w")
        sub_lbl = ttk.Label(header, text="Compatible con Raft (The Final Chapter) en Steam | Menú Nativo In-Game con F1", style="Sub.TLabel")
        sub_lbl.pack(anchor="w", pady=(2, 0))

        # Status Bar Card
        status_card = ttk.Frame(self, style="Card.TFrame", padding=12)
        status_card.pack(fill="x", padx=15, pady=5)

        status_grid = ttk.Frame(status_card, style="Card.TFrame")
        status_grid.pack(fill="x")

        # Estado del juego
        self.game_status_label = tk.Label(status_grid, text="🎮 Juego: Verificando...", bg="#1e1e2e", fg="#fab387", font=("Segoe UI", 10, "bold"))
        self.game_status_label.pack(side="left", padx=10)

        # Estado del Trainer
        self.trainer_status_label = tk.Label(status_grid, text="🔧 Trainer: Verificando...", bg="#1e1e2e", fg="#fab387", font=("Segoe UI", 10, "bold"))
        self.trainer_status_label.pack(side="left", padx=20)

        # Botones de control rápido arriba
        actions_card = ttk.Frame(self, style="Card.TFrame", padding=12)
        actions_card.pack(fill="x", padx=15, pady=5)

        btn_row = ttk.Frame(actions_card, style="Card.TFrame")
        btn_row.pack(fill="x")

        self.btn_install = ttk.Button(btn_row, text="⚡ Instalar Trainer (1-Click)", style="Success.TButton", command=self.install_trainer)
        self.btn_install.pack(side="left", padx=5)

        self.btn_uninstall = ttk.Button(btn_row, text="🗑️ Desinstalar Trainer", style="Danger.TButton", command=self.uninstall_trainer)
        self.btn_uninstall.pack(side="left", padx=5)

        self.btn_launch = ttk.Button(btn_row, text="🚀 Iniciar Raft (Steam)", style="Primary.TButton", command=self.launch_game)
        self.btn_launch.pack(side="left", padx=5)

        self.btn_folder = ttk.Button(btn_row, text="📂 Abrir Carpeta", style="Secondary.TButton", command=self.open_game_folder)
        self.btn_folder.pack(side="right", padx=5)

        # Contenido principal: Guía de Trucos y Atajos
        body = ttk.Frame(self, style="Card.TFrame", padding=15)
        body.pack(fill="both", expand=True, padx=15, pady=(5, 15))

        header_cheats = ttk.Label(body, text="📋 GUÍA DE TECLAS RÁPIDAS Y FUNCIONES IN-GAME", style="Header.TLabel")
        header_cheats.pack(anchor="w", pady=(0, 10))

        # Cuadrícula de atajos
        table_frame = tk.Frame(body, bg="#11111b", bd=1, relief="solid")
        table_frame.pack(fill="both", expand=True, padx=5, pady=5)

        cheats = [
            ("F1 / Insert", "Abrir / Ocultar el Menú Visual del Trainer", "Muestra la ventana interactiva con pestañas y controles."),
            ("F2", "Modo Dios (Salud Infinita)", "Invulnerabilidad total ante ataques de tiburón, animales y caídas."),
            ("F3", "Hambre Infinita", "La barra de hambre permanece siempre llena al 100%."),
            ("F4", "Sed Infinita", "La hidratación permanece al 100% sin necesidad de beber agua."),
            ("F5", "Oxígeno Infinito", "Buceo ilimitado bajo el agua sin ahogarse jamás."),
            ("F6", "Modo Vuelo / NoClip", "Vuela por el mapa atravesando estructuras con W, A, S, D, Espacio y Shift."),
            ("K", "Matar al Tiburón", "Elimina instantáneamente al tiburón que acecha la balsa."),
            ("Menú In-Game", "Durabilidad Infinita", "Las herramientas, ganchos y armas no se desgastan al usarse."),
            ("Menú In-Game", "Crafteo Gratis / Recetas", "Permite craftear cualquier objeto sin necesidad de materiales."),
            ("Menú In-Game", "Spawn de Recursos y Armas", "Añade Madera, Plástico, Titanio, Flechas, Comida con 1 clic."),
            ("Menú In-Game", "Teletransporte a la Balsa", "Vuelve inmediatamente al centro de tu balsa si te pierdes en el mar."),
            ("Menú In-Game", "Pacificar / Congelar Tiburón", "Desactiva los ataques del tiburón o congélalo en su posición."),
        ]

        # Encabezados de tabla
        tk.Label(table_frame, text="TECLA / ORIGEN", bg="#313244", fg="#cdd6f4", font=("Segoe UI", 9, "bold"), width=16, anchor="w", padx=8, pady=4).grid(row=0, column=0, sticky="ew")
        tk.Label(table_frame, text="TRUCO / FUNCIÓN", bg="#313244", fg="#89b4fa", font=("Segoe UI", 9, "bold"), width=32, anchor="w", padx=8, pady=4).grid(row=0, column=1, sticky="ew")
        tk.Label(table_frame, text="DESCRIPCIÓN", bg="#313244", fg="#a6adc8", font=("Segoe UI", 9, "bold"), anchor="w", padx=8, pady=4).grid(row=0, column=2, sticky="ew")

        table_frame.columnconfigure(2, weight=1)

        for i, (key, func, desc) in enumerate(cheats, start=1):
            bg_color = "#181825" if i % 2 == 0 else "#1e1e2e"
            tk.Label(table_frame, text=f" {key}", bg=bg_color, fg="#fab387", font=("Segoe UI", 9, "bold"), anchor="w", padx=6, pady=3).grid(row=i, column=0, sticky="nsew")
            tk.Label(table_frame, text=f" {func}", bg=bg_color, fg="#cdd6f4", font=("Segoe UI", 9), anchor="w", padx=6, pady=3).grid(row=i, column=1, sticky="nsew")
            tk.Label(table_frame, text=f" {desc}", bg=bg_color, fg="#bac2de", font=("Segoe UI", 8), anchor="w", padx=6, pady=3).grid(row=i, column=2, sticky="nsew")

        # Footer info
        footer = ttk.Frame(body, style="Card.TFrame")
        footer.pack(fill="x", pady=(10, 0))
        foot_text = "💡 Consejo: Una vez instalado, solo inicia Raft desde Steam y presiona F1 en tu partida para ver el menú."
        ttk.Label(footer, text=foot_text, style="Card.TLabel", font=("Segoe UI", 9, "italic")).pack(side="left")

    def install_trainer(self):
        try:
            if not os.path.exists(os.path.join(self.game_dir, "Raft.exe")):
                messagebox.showerror("Error", f"No se encontró Raft.exe en:\n{self.game_dir}\nVerifica la instalación de Raft en Steam.")
                return

            winhttp_src = os.path.join(self.app_dir, "doorstop", "x64", "winhttp.dll")
            ini_src = os.path.join(self.app_dir, "doorstop_config.ini")
            dll_src = os.path.join(self.app_dir, "RaftTrainer.dll")

            if not os.path.exists(winhttp_src) or not os.path.exists(dll_src):
                messagebox.showerror("Error", "Faltan archivos del Trainer en la carpeta local.")
                return

            shutil.copy2(winhttp_src, os.path.join(self.game_dir, "winhttp.dll"))
            shutil.copy2(ini_src, os.path.join(self.game_dir, "doorstop_config.ini"))
            shutil.copy2(dll_src, os.path.join(self.game_dir, "RaftTrainer.dll"))

            messagebox.showinfo("¡Éxito!", "¡Trainer instalado correctamente en Raft!\n\nYa puedes abrir el juego y presionar [F1] o [Insert] para desplegar el menú.")
            self.update_status()
        except Exception as e:
            messagebox.showerror("Error al instalar", f"No se pudo copiar los archivos:\n{e}")

    def uninstall_trainer(self):
        try:
            files_to_remove = ["winhttp.dll", "doorstop_config.ini", "RaftTrainer.dll"]
            for f in files_to_remove:
                target = os.path.join(self.game_dir, f)
                if os.path.exists(target):
                    os.remove(target)

            messagebox.showinfo("Desinstalado", "El Trainer ha sido desinstalado. El juego está en su estado 100% original.")
            self.update_status()
        except Exception as e:
            messagebox.showerror("Error al desinstalar", f"No se pudo eliminar los archivos:\n{e}")

    def launch_game(self):
        try:
            webbrowser.open("steam://rungameid/648800")
        except Exception as e:
            messagebox.showerror("Error", f"No se pudo lanzar Steam:\n{e}")

    def open_game_folder(self):
        if os.path.exists(self.game_dir):
            os.startfile(self.game_dir)
        else:
            messagebox.showwarning("Aviso", "Carpeta del juego no encontrada.")

    def update_status(self):
        running = is_raft_running()
        if running:
            self.game_status_label.config(text="🎮 Juego: DETECTADO (EJECUTÁNDOSE)", fg="#a6e3a1")
        else:
            self.game_status_label.config(text="🎮 Juego: NO DETECTADO (Cerrado)", fg="#a6adc8")

        installed = is_trainer_installed(self.game_dir)
        if installed:
            self.trainer_status_label.config(text="🔧 Trainer: INSTALADO Y LISTO", fg="#a6e3a1")
            self.btn_install.config(text="✓ Trainer Instalado (Reinstalar)")
        else:
            self.trainer_status_label.config(text="🔧 Trainer: NO INSTALADO", fg="#f38ba8")
            self.btn_install.config(text="⚡ Instalar Trainer (1-Click)")

    def start_status_monitor(self):
        def monitor_loop():
            while True:
                try:
                    self.update_status()
                except Exception:
                    pass
                time.sleep(2.5)

        t = threading.Thread(target=monitor_loop, daemon=True)
        t.start()

if __name__ == "__main__":
    app = TrainerApp()
    app.mainloop()

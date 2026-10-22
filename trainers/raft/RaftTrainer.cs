using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Doorstop
{
    public class Entrypoint
    {
        private static GameObject trainerHost;

        public static void Start()
        {
            try
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            catch
            {
                AppDomain.CurrentDomain.AssemblyLoad += delegate(object s, AssemblyLoadEventArgs e)
                {
                    if (e.LoadedAssembly.GetName().Name == "Assembly-CSharp")
                    {
                        SceneManager.sceneLoaded += OnSceneLoaded;
                    }
                };
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (trainerHost == null)
            {
                trainerHost = new GameObject("RaftTrainer_Instance");
                trainerHost.AddComponent<RaftTrainer.TrainerMenu>();
                UnityEngine.Object.DontDestroyOnLoad(trainerHost);
            }
        }
    }
}

namespace RaftTrainer
{
    public class TrainerMenu : MonoBehaviour
    {
        // Visibilidad del menú
        public bool showMenu = false;
        private Rect windowRect = new Rect(50, 50, 700, 530);
        private int selectedTab = 0;
        private readonly string[] tabNames = new string[] { "👤 Jugador", "🎒 Inventario / Crafteo", "🦈 Tiburón / Mundo", "📍 Teleport" };

        // Estados de trucos - Jugador
        public bool godMode = false;
        public bool infiniteHunger = false;
        public bool infiniteThirst = false;
        public bool infiniteOxygen = false;
        public bool noFallDamage = false;
        public bool noclip = false;
        public float moveSpeedMultiplier = 1.0f;
        public float jumpMultiplier = 1.0f;
        public float noclipSpeedMultiplier = 1.0f;

        // Estados de trucos - Inventario
        public bool infiniteDurability = false;
        public bool freeCrafting = false;

        // Estados de trucos - Tiburón & Mundo
        public bool pacifyShark = false;
        public bool freezeShark = false;
        public float gameTimeScale = 1.0f;

        // Búsqueda de items
        private string itemSearchText = "Plank";
        private int itemSpawnAmount = 20;

        // Waypoints
        private Vector3 savedWaypoint = Vector3.zero;
        private bool hasSavedWaypoint = false;

        // Notificaciones temporales en pantalla
        private string toastMessage = "Presiona [F1] o [Insert] para abrir el menú del Trainer";
        private float toastTimer = 8.0f;

        // Variables de respaldo para restaurar valores originales
        private float originalNormalSpeed = 3.5f;
        private float originalSprintSpeed = 6.0f;
        private float originalSwimSpeed = 3.5f;
        private float originalJumpSpeed = 6.5f;
        private bool speedsCaptured = false;

        // Campos privados cacheados por reflexión
        private static FieldInfo sharkCanAttackField;
        private static FieldInfo sharkBiteRaftDamageField;
        private static FieldInfo sharkAttackPlayerDamageField;
        private static FieldInfo pcFallDamageMultiplierField;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            CacheReflectionFields();
        }

        private static void CacheReflectionFields()
        {
            try
            {
                sharkCanAttackField = typeof(Shark).GetField("canAttack", BindingFlags.NonPublic | BindingFlags.Instance);
                sharkBiteRaftDamageField = typeof(Shark).GetField("biteRaftDamage", BindingFlags.NonPublic | BindingFlags.Instance);
                sharkAttackPlayerDamageField = typeof(Shark).GetField("attackPlayerDamage", BindingFlags.NonPublic | BindingFlags.Instance);
                pcFallDamageMultiplierField = typeof(PersonController).GetField("fallDamageMultiplier", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            catch { }
        }

        void Start()
        {
            ShowToast("⚓ Raft Trainer v1.0 Cargado con éxito. Presiona [F1] para abrir.");
        }

        void Update()
        {
            // Toggle Menú
            if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Insert))
            {
                showMenu = !showMenu;
            }

            // Teclas rápidas globales
            if (Input.GetKeyDown(KeyCode.F2))
            {
                godMode = !godMode;
                ShowToast(godMode ? "Modo Dios: ACTIVADO" : "Modo Dios: DESACTIVADO");
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                infiniteHunger = !infiniteHunger;
                ShowToast(infiniteHunger ? "Hambre Infinita: ACTIVADA" : "Hambre Infinita: DESACTIVADA");
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                infiniteThirst = !infiniteThirst;
                ShowToast(infiniteThirst ? "Sed Infinita: ACTIVADA" : "Sed Infinita: DESACTIVADA");
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                infiniteOxygen = !infiniteOxygen;
                ShowToast(infiniteOxygen ? "Oxígeno Infinito: ACTIVADO" : "Oxígeno Infinito: DESACTIVADO");
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                noclip = !noclip;
                ShowToast(noclip ? "Modo Vuelo (Noclip): ACTIVADO" : "Modo Vuelo: DESACTIVADO");
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                KillAllSharks();
                ShowToast("🦈 Tiburones eliminados");
            }

            // Aplicar trampas
            ApplyCheats();

            // Contador toast
            if (toastTimer > 0f)
            {
                toastTimer -= Time.unscaledDeltaTime;
            }
        }

        private void ShowToast(string message)
        {
            toastMessage = message;
            toastTimer = 4.0f;
        }

        private void ApplyCheats()
        {
            Network_Player player = ComponentManager<Network_Player>.Value;
            if (player != null)
            {
                PlayerStats stats = player.Stats;
                PersonController pc = player.PersonController;

                // Capturar velocidades base una vez
                if (pc != null && !speedsCaptured && pc.normalSpeed > 0)
                {
                    originalNormalSpeed = pc.normalSpeed;
                    originalSprintSpeed = pc.sprintSpeed;
                    originalSwimSpeed = pc.swimSpeed;
                    originalJumpSpeed = pc.jumpSpeed;
                    speedsCaptured = true;
                }

                // 1. MODO DIOS
                if (godMode && stats != null)
                {
                    stats.IsInvurnerable = true;
                    if (stats.stat_health != null)
                    {
                        stats.stat_health.Value = stats.stat_health.Max;
                    }
                    if (stats.stat_BonusHealth != null)
                    {
                        stats.stat_BonusHealth.Value = stats.stat_BonusHealth.Max;
                    }
                }

                // 2. HAMBRE INFINITA
                if (infiniteHunger && stats != null && stats.stat_hunger != null)
                {
                    if (stats.stat_hunger.normalConsumable != null)
                        stats.stat_hunger.normalConsumable.Value = stats.stat_hunger.normalConsumable.Max;
                    if (stats.stat_hunger.bonusConsumable != null)
                        stats.stat_hunger.bonusConsumable.Value = stats.stat_hunger.bonusConsumable.Max;
                }

                // 3. SED INFINITA
                if (infiniteThirst && stats != null && stats.stat_thirst != null)
                {
                    if (stats.stat_thirst.normalConsumable != null)
                        stats.stat_thirst.normalConsumable.Value = stats.stat_thirst.normalConsumable.Max;
                    if (stats.stat_thirst.bonusConsumable != null)
                        stats.stat_thirst.bonusConsumable.Value = stats.stat_thirst.bonusConsumable.Max;
                }

                // 4. OXÍGENO INFINITO
                if (infiniteOxygen && stats != null && stats.stat_oxygen != null)
                {
                    stats.stat_oxygen.Value = stats.stat_oxygen.Max;
                    stats.stat_oxygen.SetOxygenLostPerSecond(0f);
                }

                // 5. CAÍDA Y VELOCIDADES
                if (pc != null)
                {
                    if (noFallDamage && pcFallDamageMultiplierField != null)
                    {
                        try { pcFallDamageMultiplierField.SetValue(pc, 0f); } catch { }
                    }

                    pc.normalSpeed = originalNormalSpeed * moveSpeedMultiplier;
                    pc.sprintSpeed = originalSprintSpeed * moveSpeedMultiplier;
                    pc.swimSpeed = originalSwimSpeed * moveSpeedMultiplier;
                    pc.jumpSpeed = originalJumpSpeed * jumpMultiplier;

                    // 6. NOCLIP / VOLAR
                    if (noclip)
                    {
                        if (pc.controller != null && pc.controller.enabled)
                        {
                            pc.controller.enabled = false;
                        }

                        Transform cam = player.CameraTransform != null ? player.CameraTransform : (Camera.main != null ? Camera.main.transform : null);
                        if (cam != null)
                        {
                            Vector3 dir = Vector3.zero;
                            if (Input.GetKey(KeyCode.W)) dir += cam.forward;
                            if (Input.GetKey(KeyCode.S)) dir -= cam.forward;
                            if (Input.GetKey(KeyCode.D)) dir += cam.right;
                            if (Input.GetKey(KeyCode.A)) dir -= cam.right;
                            if (Input.GetKey(KeyCode.Space)) dir += Vector3.up;
                            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C)) dir -= Vector3.up;

                            float speed = (Input.GetKey(KeyCode.LeftShift) ? 28f : 12f) * noclipSpeedMultiplier;
                            player.transform.position += dir * speed * Time.unscaledDeltaTime;
                        }
                    }
                    else
                    {
                        if (pc.controller != null && !pc.controller.enabled)
                        {
                            pc.controller.enabled = true;
                        }
                    }
                }

                // 7. DURABILIDAD INFINITA (Item en mano)
                if (infiniteDurability)
                {
                    PlayerInventory inv = player.Inventory;
                    if (inv != null)
                    {
                        Slot hotSlot = inv.GetSelectedHotbarSlot();
                        if (hotSlot != null && hotSlot.itemInstance != null)
                        {
                            hotSlot.itemInstance.SetUsesToMax();
                        }
                    }
                }
            }

            // 8. TIBURÓN
            if (pacifyShark || freezeShark)
            {
                Shark[] sharks = UnityEngine.Object.FindObjectsOfType<Shark>();
                for (int i = 0; i < sharks.Length; i++)
                {
                    Shark s = sharks[i];
                    if (s != null)
                    {
                        if (pacifyShark)
                        {
                            try
                            {
                                if (sharkCanAttackField != null) sharkCanAttackField.SetValue(s, false);
                                if (sharkBiteRaftDamageField != null) sharkBiteRaftDamageField.SetValue(s, 0);
                                if (sharkAttackPlayerDamageField != null) sharkAttackPlayerDamageField.SetValue(s, 0);
                            }
                            catch { }
                        }
                        if (freezeShark)
                        {
                            s.swimSpeed = 0f;
                        }
                    }
                }
            }

            // 9. TIME SCALE
            if (Time.timeScale != gameTimeScale && gameTimeScale > 0f)
            {
                Time.timeScale = gameTimeScale;
            }

            // 10. FREE CRAFTING / DEV CHEATS
            Cheat.UseCheats = true;
            if (freeCrafting)
            {
                Cheat.UnlockAllCrafting = true;
            }
        }

        void OnGUI()
        {
            // Dibujar Toast de notificación
            if (toastTimer > 0f)
            {
                GUI.color = new Color(0.1f, 0.8f, 0.3f, 0.95f);
                GUI.Box(new Rect((Screen.width - 460) / 2, 20, 460, 36), " " + toastMessage);
                GUI.color = Color.white;
            }

            // Si el menú está abierto, dibujamos la ventana
            if (showMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                windowRect = GUI.Window(7777, windowRect, DrawMainWindow, "⚓ RAFT ULTIMATE TRAINER v1.0 [F1 para Ocultar]");
            }
        }

        private void DrawMainWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, 700, 25));

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < tabNames.Length; i++)
            {
                GUI.color = (selectedTab == i) ? new Color(0.3f, 0.8f, 1.0f) : Color.white;
                if (GUILayout.Button(tabNames[i], GUILayout.Height(32)))
                {
                    selectedTab = i;
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            switch (selectedTab)
            {
                case 0:
                    DrawPlayerTab();
                    break;
                case 1:
                    DrawInventoryTab();
                    break;
                case 2:
                    DrawWorldSharkTab();
                    break;
                case 3:
                    DrawTeleportTab();
                    break;
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Raft Trainer Activo | Presiona [F1] para alternar ventana", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Cerrar [F1]", GUILayout.Width(100), GUILayout.Height(24)))
            {
                showMenu = false;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawPlayerTab()
        {
            GUILayout.Label("<b>--- ESTADÍSTICAS DEL JUGADOR ---</b>");

            GUILayout.BeginHorizontal();
            godMode = GUILayout.Toggle(godMode, " Modo Dios (Salud Infinita / Invulnerable) [F2]", GUILayout.Width(330));
            infiniteHunger = GUILayout.Toggle(infiniteHunger, " Hambre Infinita [F3]");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            infiniteThirst = GUILayout.Toggle(infiniteThirst, " Sed Infinita [F4]", GUILayout.Width(330));
            infiniteOxygen = GUILayout.Toggle(infiniteOxygen, " Oxígeno Infinito (Buceo ilimitado) [F5]");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            noFallDamage = GUILayout.Toggle(noFallDamage, " Sin Daño por Caída", GUILayout.Width(330));
            noclip = GUILayout.Toggle(noclip, " Modo Vuelo / Noclip [F6]");
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>--- MODIFICADORES DE FÍSICA ---</b>");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Velocidad de Movimiento: " + moveSpeedMultiplier.ToString("0.0") + "x", GUILayout.Width(220));
            if (GUILayout.Button("1x (Normal)")) moveSpeedMultiplier = 1.0f;
            if (GUILayout.Button("1.5x")) moveSpeedMultiplier = 1.5f;
            if (GUILayout.Button("2x")) moveSpeedMultiplier = 2.0f;
            if (GUILayout.Button("3x")) moveSpeedMultiplier = 3.0f;
            if (GUILayout.Button("5x (Flash)")) moveSpeedMultiplier = 5.0f;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuerza de Salto: " + jumpMultiplier.ToString("0.0") + "x", GUILayout.Width(220));
            if (GUILayout.Button("1x (Normal)")) jumpMultiplier = 1.0f;
            if (GUILayout.Button("1.5x")) jumpMultiplier = 1.5f;
            if (GUILayout.Button("2x (Súper)")) jumpMultiplier = 2.0f;
            if (GUILayout.Button("3x (Mega)")) jumpMultiplier = 3.0f;
            GUILayout.EndHorizontal();

            if (noclip)
            {
                GUILayout.Space(5);
                GUILayout.Label("<i>Controles de Vuelo: W/A/S/D = Moverse, Espacio = Subir, Ctrl/C = Bajar, Shift = Acelerar</i>");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Velocidad de Vuelo: " + noclipSpeedMultiplier.ToString("0.0") + "x", GUILayout.Width(220));
                if (GUILayout.Button("1x")) noclipSpeedMultiplier = 1.0f;
                if (GUILayout.Button("2x")) noclipSpeedMultiplier = 2.0f;
                if (GUILayout.Button("4x")) noclipSpeedMultiplier = 4.0f;
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("❤️ Sanar al Máximo y Restaurar Todo Ahora", GUILayout.Height(30)))
            {
                HealPlayerFull();
                ShowToast("Salud, hambre, sed y oxígeno restaurados al 100%");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawInventoryTab()
        {
            GUILayout.Label("<b>--- DURABILIDAD Y CRAFTEO ---</b>");

            GUILayout.BeginHorizontal();
            infiniteDurability = GUILayout.Toggle(infiniteDurability, " Durabilidad Infinita (Item en mano)", GUILayout.Width(340));
            freeCrafting = GUILayout.Toggle(freeCrafting, " Crafteo Gratis / Recetas Desbloqueadas");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🛠️ Reparar Todo el Inventario al 100%", GUILayout.Height(28)))
            {
                RepairEntireInventory();
                ShowToast("Todas las herramientas y objetos reparados al máximo");
            }
            if (GUILayout.Button("📦 Duplicar / Llenar Stacks a 99", GUILayout.Height(28)))
            {
                DuplicateStacks();
                ShowToast("Stacks del inventario aumentados a 99");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>--- SPAWN RÁPIDO DE RECURSOS ---</b>");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+20 Madera (Plank)")) GiveItem("Plank", 20);
            if (GUILayout.Button("+20 Plástico (Plastic)")) GiveItem("Plastic", 20);
            if (GUILayout.Button("+20 Chatarra (Scrap)")) GiveItem("Scrap", 20);
            if (GUILayout.Button("+10 Hojas (Thatch)")) GiveItem("Thatch", 10);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+10 Lingote Titanio")) GiveItem("TitaniumIngot", 10);
            if (GUILayout.Button("+10 Lingote Metal")) GiveItem("MetalIngot", 10);
            if (GUILayout.Button("+10 Lingote Cobre")) GiveItem("CopperIngot", 10);
            if (GUILayout.Button("+5 Batería Titanio")) GiveItem("Battery_Titanium", 5);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+1 Gancho Titanio")) GiveItem("Hook_Titanium", 1);
            if (GUILayout.Button("+1 Hacha Titanio")) GiveItem("Axe_Titanium", 1);
            if (GUILayout.Button("+1 Espada Titanio")) GiveItem("Sword_Titanium", 1);
            if (GUILayout.Button("+1 Arco + 30 Flechas"))
            {
                GiveItem("Bow", 1);
                GiveItem("Arrow_Titanium", 30);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+10 Carne Cocinada")) GiveItem("Cooked_Meat_Shark", 10);
            if (GUILayout.Button("+1 Machete")) GiveItem("Machete", 1);
            if (GUILayout.Button("+1 Tanque Oxígeno")) GiveItem("OxygenBottle", 1);
            if (GUILayout.Button("+1 Aletas Buceo")) GiveItem("Flippers", 1);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>--- GENERADOR PERSONALIZADO DE ITEMS ---</b>");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Nombre de Item:", GUILayout.Width(110));
            itemSearchText = GUILayout.TextField(itemSearchText, GUILayout.Width(220));
            GUILayout.Label("Cantidad:", GUILayout.Width(65));
            string amtStr = GUILayout.TextField(itemSpawnAmount.ToString(), GUILayout.Width(50));
            int parsed;
            if (int.TryParse(amtStr, out parsed)) itemSpawnAmount = parsed;

            if (GUILayout.Button("➕ Spawnear Item", GUILayout.Height(24)))
            {
                GiveItem(itemSearchText.Trim(), itemSpawnAmount);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawWorldSharkTab()
        {
            GUILayout.Label("<b>--- CONTROL DEL TIBURÓN ---</b>");

            GUILayout.BeginHorizontal();
            pacifyShark = GUILayout.Toggle(pacifyShark, " Pacificar Tiburón (No ataca balsa ni jugador)", GUILayout.Width(330));
            freezeShark = GUILayout.Toggle(freezeShark, " Congelar Tiburón (Inmóvil)");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("💀 Matar Tiburón Inmediatamente [K]", GUILayout.Height(32)))
            {
                KillAllSharks();
                ShowToast("Tiburón eliminado");
            }
            if (GUILayout.Button("🍖 Spawnear Cebo de Tiburón", GUILayout.Height(32)))
            {
                GiveItem("SharkBait", 3);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>--- VELOCIDAD DEL JUEGO / TIEMPO ---</b>");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Velocidad de Juego: " + gameTimeScale.ToString("0.0") + "x", GUILayout.Width(180));
            if (GUILayout.Button("0.5x (Lento)")) gameTimeScale = 0.5f;
            if (GUILayout.Button("1.0x (Normal)")) gameTimeScale = 1.0f;
            if (GUILayout.Button("2.0x (Rápido)")) gameTimeScale = 2.0f;
            if (GUILayout.Button("5.0x (Súper)")) gameTimeScale = 5.0f;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("<b>--- COMANDOS DE DESARROLLADOR ---</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🔓 Activar Comandos de Chat (/give, /tp, /set)", GUILayout.Height(30)))
            {
                Cheat.UseCheats = true;
                ShowToast("Comandos de chat de desarrollador activados");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawTeleportTab()
        {
            GUILayout.Label("<b>--- TELETRANSPORTE Y POSICIONAMIENTO ---</b>");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("💾 Guardar Posición Actual", GUILayout.Height(32)))
            {
                Network_Player player = ComponentManager<Network_Player>.Value;
                if (player != null)
                {
                    savedWaypoint = player.transform.position;
                    hasSavedWaypoint = true;
                    ShowToast("Posición guardada: " + savedWaypoint.ToString("F1"));
                }
            }

            GUI.enabled = hasSavedWaypoint;
            if (GUILayout.Button("⚡ Teletransportar a Posición Guardada", GUILayout.Height(32)))
            {
                Network_Player player = ComponentManager<Network_Player>.Value;
                if (player != null && hasSavedWaypoint)
                {
                    player.transform.position = savedWaypoint;
                    ShowToast("Teletransportado a punto guardado");
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("⛵ Teletransportar a la Balsa (Raft Center)", GUILayout.Height(34)))
            {
                Raft raft = ComponentManager<Raft>.Value;
                Network_Player player = ComponentManager<Network_Player>.Value;
                if (raft != null && player != null)
                {
                    player.transform.position = raft.transform.position + Vector3.up * 2.5f;
                    ShowToast("Teletransportado al centro de la balsa");
                }
                else
                {
                    ShowToast("Balsa no encontrada en la escena activa");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("⬆️ Impulso Hacia Arriba (+10m)", GUILayout.Height(28)))
            {
                Network_Player player = ComponentManager<Network_Player>.Value;
                if (player != null)
                {
                    player.transform.position += Vector3.up * 10f;
                }
            }
            GUILayout.EndHorizontal();
        }

        // Acciones auxiliares
        private void HealPlayerFull()
        {
            Network_Player player = ComponentManager<Network_Player>.Value;
            if (player != null && player.Stats != null)
            {
                PlayerStats s = player.Stats;
                if (s.stat_health != null) s.stat_health.Value = s.stat_health.Max;
                if (s.stat_BonusHealth != null) s.stat_BonusHealth.Value = s.stat_BonusHealth.Max;
                if (s.stat_hunger != null && s.stat_hunger.normalConsumable != null)
                    s.stat_hunger.normalConsumable.Value = s.stat_hunger.normalConsumable.Max;
                if (s.stat_thirst != null && s.stat_thirst.normalConsumable != null)
                    s.stat_thirst.normalConsumable.Value = s.stat_thirst.normalConsumable.Max;
                if (s.stat_oxygen != null)
                {
                    s.stat_oxygen.Value = s.stat_oxygen.Max;
                    s.stat_oxygen.SetOxygenLostPerSecond(0f);
                }
            }
        }

        private void GiveItem(string uniqueName, int count)
        {
            PlayerInventory inv = ComponentManager<PlayerInventory>.Value;
            if (inv != null)
            {
                inv.AddItem(uniqueName, count);
                ShowToast("+" + count + " " + uniqueName + " añadido");
            }
            else
            {
                ShowToast("Inventario no disponible. Entra a una partida primero.");
            }
        }

        private void RepairEntireInventory()
        {
            PlayerInventory inv = ComponentManager<PlayerInventory>.Value;
            if (inv != null)
            {
                int total = inv.GetSlotCount();
                for (int i = 0; i < total; i++)
                {
                    Slot slot = inv.GetSlot(i);
                    if (slot != null && slot.itemInstance != null)
                    {
                        slot.itemInstance.SetUsesToMax();
                        slot.RefreshComponents();
                    }
                }
            }
        }

        private void DuplicateStacks()
        {
            PlayerInventory inv = ComponentManager<PlayerInventory>.Value;
            if (inv != null)
            {
                int total = inv.GetSlotCount();
                for (int i = 0; i < total; i++)
                {
                    Slot slot = inv.GetSlot(i);
                    if (slot != null && slot.itemInstance != null)
                    {
                        slot.itemInstance.Amount = 99;
                        slot.RefreshComponents();
                    }
                }
            }
        }

        private void KillAllSharks()
        {
            Shark[] sharks = UnityEngine.Object.FindObjectsOfType<Shark>();
            for (int i = 0; i < sharks.Length; i++)
            {
                Shark s = sharks[i];
                if (s != null && !s.IsKilled)
                {
                    s.Kill();
                }
            }
        }
    }
}

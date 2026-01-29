# Showroom Tango – README

**Developer:** Julian Gomez  
**Course:** PRG - Game & Multimedia Design, SRH Hochschule Heidelberg  
**Technology:** Unity 6000.0.62f1 + FishNet 4.x Networking  
**Version:** v1.6  
**Last Updated:** 2026-01-29  

Git-Repository: Assets --> _Project
---

## Kurzbeschreibung des Spiels

Showroom Tango ist ein kooperativer 2D-Top-Down-Bullet-Hell-Shooter für 2 Spieler. In einem futuristischen Neon-Showroom kämpfen die Spieler gemeinsam gegen 3 Wellen von Gegnern (60→67→107 Enemies). Das Spiel verfügt über ein Auto-Fire-Waffensystem, einen persistenten MySQL/PHP-Highscore-Backend und Spieleranpassung in der Lobby. Ziel ist es, alle 3 Wellen zu überleben, während mindestens ein Spieler am Leben bleibt.

---

## Anleitung zum Starten von Host und Client

### Architektur (Single-Scene Pattern)

Das Spiel verwendet eine Single-Scene-Architektur:

```
Menue.unity → LoadScene("Game") → Game.unity
                                   ├── LobbyRoot (aktiv während Lobby)
                                   └── GameRoot (aktiv während Playing)
```

GameStateManager steuert die Sichtbarkeit der Root-Objekte basierend auf dem aktuellen Spielzustand.

### Standard-Testing-Setup (Build + Unity Editor)

**Host starten (Build-Version)**  

1. Standalone-Build-Datei (z.B. `Showroom_Tango.exe`) ausführen.  
2. Im Hauptmenü auf „Start Game" klicken und den Anweisungen folgen.  
3. AutoStartNetwork startet automatisch als Host mit Fallback auf Client-Modus, falls der Server-Port bereits belegt ist.

**Client starten (Unity Play Mode)**  

1. Unity Editor öffnen.  
2. Play-Modus starten.  
3. Im Hauptmenü auf „Start Game" klicken.  
4. Das Spiel verbindet sich automatisch als Client mit dem laufenden Host (gleiche Szene, gleicher Codepfad).

**Lobby**  

- Beide Spieler können Namen und Charakterfarbe anpassen (LobbyManager mit SyncDictionary-basierten PlayerLobbyData).  
- Wenn beide Spieler bereit sind (Ready), startet das Spiel automatisch nach 3-Sekunden-Countdown.  

**Steuerung im Spiel**  

- **Bewegung:** WASD-Tasten über Unity New Input System, gelesen in `PlayerController` via `moveAction = playerInput.actions["Movement"]`.  
- **Rotation:** `PlayerController` enthält `RotateTowardsMouse()`, technisch aktiv, aber für das aktuelle Gameplay nicht relevant (Auto-Targeting übernimmt das Zielen).  
- **Waffen:** Vollautomatisches Feuer – `WeaponManager` feuert ohne manuelle Eingabe auf Gegner in Reichweite, gesteuert in `Update()` nur für den Owner.  
- **Maus:** Nur für Menü- und Settings-Navigation relevant; im Gameplay ist kein manuelles Schießen gebunden.  

---

## Technischer Überblick

### Verwendete RPCs

**ServerRpc**  

- `RegisterPlayerServerRpc(string playerName, Color playerColor, NetworkConnection conn = null)` – Registriert Spieler in der Lobby mit Name und Farbe.  
- `UpdatePlayerNameServerRpc(string newName, NetworkConnection conn = null)` – Aktualisiert Spielernamen in der Lobby.  
- `UpdatePlayerColorServerRpc(Color newColor, NetworkConnection conn = null)` – Aktualisiert Spielerfarbe in der Lobby.  
- `ToggleReadyServerRpc(NetworkConnection conn = null)` – Toggles Ready-Status und triggert ggf. den Start-Countdown.  
- `SetPlayerNameServerRpc(string name)` – Setzt Spielername im `PlayerController` (Server-Authority).  
- `SetPlayerColorServerRpc(Color color)` – Setzt Spielerfarbe, SyncVar-Update plus lokales Tint-Update.  
- `TakeDamageServerRpc(int damage)` – Schaden auf Spieler anwenden, aufgerufen z.B. von `EnemyChaser` bei Kollision.  
- `RequestGameRestartServerRpc()` – Client fordert Neustart an; GameStateManager führt Reset-Sequenz aus (Score, Spieler, Gegner, Lobby-Reset).  
- `FireWeaponServerRpc(Vector3 position, Quaternion rotation, string bulletSpriteName)` – Server-seitiges Spawnen von Player-Bullets via BulletPool, inklusive Sprite-Set.  
- `AddScoreServerRpc(int points)` – Erhöht Team-Score über den Server (Generalscore-Updates).  

**ObserversRpc**  

- `NotifyWaveClearedObserversRpc(int clearedWave)` – Benachrichtigt alle Observer über eine geclearte Wave; Client-seitige Listener (z.B. UI) können darauf reagieren.  

GameState-Änderungen selbst laufen über eine SyncVar auf dem GameStateManager, nicht über explizite RPCs.

---

### Verwendete SyncVars

**PlayerController.cs**  

- `SyncVar<string> playerName` – Netzwerkweiter Spielername (Default „Player"), via ApplyName/SetPlayerNameServerRpc gesetzt.  
- `SyncVar<int> currentHP` – gespiegelt, aber eigentliche HP-Logik liegt in `PlayerHealth`.  
- `SyncVar<Color> playerColor` – Spielerfarbe mit OnChange-Callback, färbt alle SpriteRenderer-Kinder.  

**PlayerHealth.cs**  

- `SyncVar<int> currentHealth` – Aktuelle HP (maxHealth = 100 per Inspector).  
- `SyncVar<bool> isDead` – Zustandsflagge zur Steuerung von Spectator-Mode (Sprites ausblenden).  

**EnemyHealth.cs**  

- `SyncVar<int> currentHealth` – Gegner-Gesundheit (Chaser/Shooter-Werte über Prefabs).  

**EnemySpawner.cs**  

- `SyncVar<int> currentWave` – Aktuelle Welle (1 bis maxWaves, Default 3).  

**ScoreManager.cs**  

- `SyncVar<int> teamScore` – Gesamt-Team-Score.  
- `SyncVar<int> totalKills` – Gesamtzahl der Kills.  
- `SyncDictionary<int,int> playerScores` – Map von NetworkObjectID auf individuellen Score.  

**GameStateManager.cs**  

- `SyncVar<GameState> currentState` – Spielzustand (Lobby, Playing, GameOver, Victory).  

**LobbyManager.cs**  

- `SyncDictionary<int, PlayerLobbyData> playerDataDict` – Alle Lobby-Daten (Name, Farbe, Ready, Index) pro ClientId.  
- `SyncVar<bool> countdownActive` – Status, ob Countdown läuft.  
- `SyncVar<int> countdownTime` – Countdown-Zähler in Sekunden.  

---

### Bullet-Logik

**Pooling-System**  

- `BulletPool` ist ein MonoBehaviour, das FishNet's nativen Pool nutzt, keine eigene Queue-Struktur.  
- Beim Server-Start werden `prewarmCount` (Standard 200) Bullets über `ObjectPool.CacheObjects` vorgeladen.  
- `GetBullet(position, rotation)` holt ein NetworkObject aus dem Pool via `NetworkManager.GetPooledInstantiated` und spawnt es über `ServerManager.Spawn`.  
- `ReturnBullet(GameObject bullet)` despawnt über `ServerManager.Despawn(nob, DespawnType.Pool)` zurück in den Pool.  

**Auto-Fire-System (WeaponManager)**  

- `Update()` prüft nur auf dem Owner (`if (!IsOwner) return;`) und bricht ab, falls der Spieler tot ist.  
- `AutoFireAllWeapons()` iteriert über alle `equippedWeapons` (maximal 3 Slots).  
- Für jede Waffe:
  - Prüft `weaponLastFireTime[weapon] + weapon.CurrentFireRate`.  
  - Sammelt Gegner im Radius via `Physics2D.OverlapCircleAll` und LayerMask.  
  - Sortiert Gegner nach Distanz und wählt Ziel je nach Weapon-Index (0 → nächster, 1 → zweiter, 2 → dritter Gegner).  
  - Berechnet Fire-Position mit lokal transformiertem `firePointOffset` über `playerTransform.TransformDirection`.  
  - Erzeugt Rotation in Richtung Ziel inklusive `directionAngleOffset`, ruft anschließend `FireWeaponServerRpc` auf.  

**Projektilerzeugung & Bewegung (Bullet)**  

- `Initialize(BulletPool pool, GameObject player = null)` speichert Pool-Referenz, OwnerPlayer und `movementDirection = transform.up` zum Spawnzeitpunkt.  
- `FixedUpdate()`:
  - Prüft `IsServerStarted` (nur Server bewegt).  
  - Bewegt Projektil mit `transform.position += movementDirection * speed * Time.fixedDeltaTime`.  
  - Optional visuelle Rotation, getrennt von Bewegungsrichtung.  
  - Timeout nach `lifetime` (Standard 5s) oder nach Überschreiten `maxRange` (Standard 20 units); beide Fälle führen zu `ReturnToPool()`.  

**Kollision & Schaden**  

- OnTriggerEnter2D:
  - Player-Bullet (`isPlayerBullet == true`) + `collision.CompareTag("Enemy")` → `EnemyHealth.TakeDamage(damage, ownerPlayer)` und danach `ReturnToPool()`.  
  - Enemy-Bullet (`isPlayerBullet == false`) + `collision.CompareTag("Player")` → `PlayerHealth.ApplyDamage(damage)` und danach `ReturnToPool()`.  
- Score-Zuordnung erfolgt in `EnemyHealth.Die(killerPlayer)` über `ScoreManager.AddKillScore(killerPlayer)`.  

**Bekannter Bullet-Spawn-Bug**  

- Projektile erscheinen sichtbar versetzt (hinter dem Spieler). Ursache ist ein Offset/Transformationsproblem trotz Nutzung von `playerTransform.TransformDirection(firePointOffset)` in WeaponManager.  
- Root Cause ist in der GDD als offener Tier-1-Bug dokumentiert.

---

### Gegner-Logik

**Gegner-Typen**

**EnemyChaser (Melee):**  
- `moveSpeed = 3f` mit ±30% Variation zur Entsynchronisierung der Horde.  
- Sucht in regelmäßigen Abständen den nächsten lebenden Spieler (`FindNearestPlayer()` + `PlayerHealth.IsDead()` Filter).  
- Bewegt sich auf den Spieler + random Offset zu, um Stacking zu vermeiden.  
- Bei Kollision mit Spieler: ruft `PlayerController.TakeDamageServerRpc(collisionDamage)` auf.  
- Optionales Kamikaze-Verhalten: stirbt bei Kollision über `EnemyHealth.TakeDamage(int.MaxValue, attackerPlayer: null)`.  

**EnemyShooter (Ranged):**  
- Hält Distanzfenster (zu nah <4, optimal ~6, zu weit >10 Einheiten).  
- Bewegt sich je nach Distanz auf/weg vom Spieler oder patrouilliert in der Umgebung.  
- Schießt alle 2.5s ein 360° Star-Pattern mit `bulletCount = 5` Bullets, `bulletSpeed = 8f`, `bulletDamage = 10`.  
- Nutzt Enemy-BulletPool vom EnemySpawner, sofern vorhanden.  

**Spawning & Wellen-System**

- `EnemySpawner` (Server-only) verwaltet 3 Wellen mit `waveEnemyCounts = {60, 67, 107}` und `chaserSpawnWeight = 0.7f` (70% Chaser, 30% Shooter).  
- Spawnradius 33 Einheiten mit Mindestabstand 5 Einheiten zu Spielern.  
- Wellen werden über 45 Sekunden mit randomisiertem Spawnintervall (0.7–1.3x) verteilt.  
- Nach einer Wave prüft ein Polling, ob noch Gegner mit Tag „Enemy" existieren; bei 0 Gegnern wird `OnWaveCleared` Event + `NotifyWaveClearedObserversRpc` ausgelöst.  
- Victory: GameStateManager prüft im Playing-State, ob `currentWave >= 3`, keine aktiven Wellen und `GameObject.FindGameObjectsWithTag("Enemy").Length == 0` → `currentState = GameState.Victory`.  
- GameOver: wenn kein lebender Spieler mehr vorhanden ist (PlayerHealth.IsAlive() für alle Spieler ist false).  

---

## Audio System (v1.6)

Das Audio-System verwendet einen persistenten Singleton:

```
GameAudioManager (DontDestroyOnLoad)
├── Music Tracks: Menu, Lobby, Gameplay
├── SFX: Button clicks
├── States: Menu → Lobby → Playing
└── Volume: Via AudioSettings.cs (PlayerPrefs)
```

**AudioSettings.cs**  

- Statische Utility-Klasse für Volume-Persistenz mit `MasterVolume`, `MusicVolume`, `SFXVolume`.  
- Speichert Werte in PlayerPrefs, z.B. `PlayerPrefs.SetFloat("MasterVolume", value)`.  
- Finaler Musikwert wird als Produkt aus Master × Music berechnet und auf den GameAudioManager angewendet.  

**PreferencesMenu.cs**  

- Drei Sliders (Master, Music, SFX) zur Laufzeit-basierten Anpassung.  
- Uses CanvasGroup für Anzeige-/Hide-Logik.  

---

## Beschreibung der Persistenz

### PHP/SQL Backend (Primär – Pflichtanforderung)

**Setup-Schritte** (laut Dokumentation):

1. XAMPP installieren (Apache + MySQL).  
2. Datenbank `bullethell_scores` erstellen.  
3. SQL-Script `Documentation/PHP_Backend/database_setup.sql` ausführen (legt Tabelle `highscores` an).  
4. PHP-Files nach `C:/xampp/htdocs/bullethell/` kopieren.  

**PHP-Files**  

- `submit_score.php` – nimmt POST (`player_name`, `score`) entgegen und führt INSERT in die Highscore-Tabelle aus.  
- `get_highscores.php` – gibt Top-10 Scores als JSON zurück.  
- `database_setup.sql` – enthält Tabellen-Definition (z.B. id, player_name, score, created_at).  

**Unity-Integration (HighscoreManager)**  

- Nutzt UnityWebRequest POST/GET, um Scores zu submitten und Highscores abzurufen.  
- Automatisches Submit bei GameOver/Victory, basierend auf Team-Score und Spielernamen.  
- HighscoreUI zeigt Top-10 im UI an.  

**Standard-URLs**  

- Submit: `http://localhost/bullethell/submit_score.php`  
- Get: `http://localhost/bullethell/get_highscores.php`  

**Bekanntes Problem**  

- MySQL-Verbindung ist gelegentlich instabil; Fehler: „Improper privileges, crash, or shutdown by another method".  
- Führt dazu, dass Highscores sporadisch nicht gespeichert oder geladen werden.

### JSON-Fallback (Notfall)

- Optionaler Fallback über `HighscoreManager`, der bei Ausfall des Web-Backends in eine lokale JSON-Datei schreibt.  
- Pfad: `Application.persistentDataPath + "/highscores.json"`.  
- Format: `{"highscores": [{"playerName": "...", "score": 123}, ...]}`.  

**Detail-Setup** siehe `Documentation/PHP_Backend/README_BACKEND_SETUP.txt`.

---

## Übersicht der umgesetzten Bonusfeatures

### Gameplay & Networking

- **FishNet 4.x Multiplayer** mit Host/Client, SyncVars, SyncDictionary, ServerRpc und ObserversRpc.  
- **Lobby-System:** Name/Farbe/Ready-Status pro Spieler inkl. 3-Sekunden-Countdown vor Spielstart.  
- **Priority-Based Auto-Targeting:** WeaponManager verteilt bis zu 3 Waffen auf die drei nächsten Gegner; aktuell ist eine Waffe im Einsatz, weitere sind vorgesehen.  
- **Spawn-Protection-System:** Mehrere Komponenten (EnemyHealth, EnemyChaser, EnemyShooter) benutzen 0.5s Schutz direkt nach Spawn.  

### Persistenz & Backend

- **SQL/PHP Highscore-Backend** mit XAMPP, MySQL und PHP-Scripts.  
- **JSON-Fallback-Modus** für Offline/Fehlerfälle.  

### Performance & Architektur

- **FishNet Native Object-Pooling** über BulletPool mit 200 vorgewärmten Objekten.  
- **Single-Scene-Architektur** (Lobby + Game in `Game.unity`) mit klarer State-gated Visibility.  

### UI & Polish

- **Story Popup** im Menü (Lore/Story-Anzeige).  
- **Preferences-Menü** mit persistenter Lautstärke-Einstellung über PlayerPrefs.  
- **Menü-Polish:** Hover-Effekte, Audio-Feedback, Seamless Video Background im Menü.  

### Visuals

- **Neon-Glow Visual System:** Dual-Sprite-Technik mit Outline/Glow (NeonGlowController).  

---

## Bekannte Bugs oder Einschränkungen

### UI Issues

- **Button-Position im Build:** Transparente UI-Buttons sind in der Standalone-Build-Version leicht verschoben im Vergleich zur Darstellung im Unity Editor (Canvas/Resolution Scaling-Probleme).  
- **SFX-Slider ohne Funktion:** SFX-Volume-Slider vorhanden, aber nicht an tatsächliche Audio-Wiedergabe gekoppelt; nur Musik-Volume wird wirksam verändert.  

### Gameplay Issues

- **Bullet-Spawn-Offset:** Player-Projektile spawnen sichtbar hinter dem Spieler. Ursache ist eine fehlerhafte Offset-/Transformationslogik im Zusammenhang mit `firePointOffset` in WeaponManager (trotz TransformDirection-Einsatz).  

### Audio Issues

- **Musik läuft nach Spielende weiter:** Hintergrundmusik stoppt im Build nicht automatisch bei GameOver/Victory; GameAudioManager reagiert nur auf bestimmte Szenen/States, Endscreen-Handling fehlt.  

### Visual Issues

- **Spielerfarben im Build inkonsistent:**  
  - Im Editor funktionieren Farbänderungen via `SyncVar<Color> playerColor` und `ApplyColorTint` korrekt.  
  - Im Build fallen Spieler teilweise auf Standardfarben zurück, vermutlich Shader-/Material-bedingt.  
  - Shader-basierte Lösung läuft zuverlässig auf Dev-Maschine, nicht auf allen Zielsystemen.  

### Backend Issues

- **Instabile MySQL-Connection:**  
  - Sporadische Fehlermeldung „Improper privileges, crash, or shutdown by another method" im HighscoreManager.  
  - Highscores werden in diesen Fällen nicht oder nur teilweise geschrieben/gelesen.  

### Network/UI Issues

- **Wave-Transition-UI Client-Sync:**  
  - WaveTransitionUI zeigt auf Clients den Countdown zwischen Waves nicht immer korrekt an.  
  - Event/Timing-Rennen zwischen EnemySpawner.NotifyWaveClearedObserversRpc und UI-Logik.  

---

## Projekt-Struktur

```
Assets/_Project/
├── Scripts/
│   ├── Enemies/           # EnemyChaser, EnemyShooter, EnemyHealth, EnemySpawner
│   ├── Gameflow/          # GameStateManager, ScoreManager, MenuManager, GameAudioManager
│   ├── Network/           # AutoStartNetwork, NetworkUIManager, PlayerSpawner
│   ├── Persistence/       # HighscoreManager, AudioSettings
│   ├── Player/            # PlayerController, PlayerHealth, WeaponManager, CameraFollow
│   ├── Projectiles/       # Bullet, BulletPool
│   ├── UI/
│   │   ├── Root/          # HUDManager, HighscoreUI, WaveTransitionUI
│   │   ├── Lobby/         # LobbyManager, LobbyUI, PlayerSetupUI
│   │   └── Menu/          # PreferencesMenu, StoryPopup, ButtonHover, SeamlessMenuVideo
│   └── Z-Parkplatz/       # Deprecated Scripts (nicht mehr verwendet)
├── Prefabs/
└── Scenes/
    ├── Menue.unity        # Hauptmenü
    └── Game.unity         # Lobby + Spielbereich

Documentation/
├── PHP_Backend/           # submit_score.php, get_highscores.php, database_setup.sql
├── GameDesignDocument.md  # Aktuelle Architektur (v1.6)
├── OpenTasks_Prioritized.md # Bug-Tracking und Tasks
└── _Archive/              # Alte Session-Logs
```

---

## Architektur-Entscheidungen (ADRs)

- **ADR-001:** Wave-System statt Boss – einfacher zu balancieren, weniger Produktionsrisiko.  
- **ADR-002:** Object-Pooling ab Tag 1 – Bullet-Hell-Setting erfordert frühe Performance-Optimierung.  
- **ADR-003:** Single-Scene Architecture – Lobby + Game in einer Szene, um Scene-Load-Rennen und Netcode-Komplexität zu reduzieren.  
- **ADR-007:** Unity New Input System – zukunftssicher und besser für Rebinding/Multiplatform.  
- **ADR-009:** Server-Authority für alle Gameplay-Logik – Anti-Cheat und deterministische Zustände im Multiplayer.  

---

## Entwicklungs-Attribution

**Human-Authored**  

- Spielkonzept, Mechanik-Design, Wave-Struktur, Balancing.  
- Szenen-Setup, Prefabs, visuelles Design (Neon-Aesthetic).  
- Kernentscheidungen zu Single-Scene, Wave-System, Persistenz.  

**AI-Assisted**  

- Teile des Networking-Codes (FishNet-Integration, SyncVar/ServerRpc-Muster).  
- Dokumentationsstruktur (GDD, README, OpenTasks).  
- PHP/SQL Backend-Beispiele und UnityWebRequest-Muster.  
- Bug-Analyse (WaveTransitionUI, Projectile-Spawn, Highscore-Edge-Cases).  

Jedes Script enthält im Header ein detailliertes Authorship-Tracking (HUMAN-AUTHORED / AI-ASSISTED / AI-GENERATED).

---

## Lizenz & Attribution

Dieses Projekt wurde im Rahmen des PRG-Moduls an der SRH Hochschule Heidelberg entwickelt.

**Third-Party Assets & Technologien**  

- FishNet Networking (MIT License)  
- Unity Input System (Unity Companion License)  
- TextMeshPro (Unity Package)  
- Kenney Space Shooter Redux (CC0) – Sprites  
- Electronic Highway Sign – Schriftart  

---

## Kontakt

**Entwickler:** Julian Gomez  
**Repository:** https://github.com/JuliGommz/Bullet_Love  
**Hochschule:** SRH Hochschule Heidelberg  
**Kurs:** PRG - Game & Multimedia Design  

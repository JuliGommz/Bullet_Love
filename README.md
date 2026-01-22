# Showroom Tango - 2-Player Cooperative Bullet-Hell

**Developer:** Julian Gomez
**Course:** PRG - Game & Multimedia Design, SRH Hochschule Heidelberg
**Technology:** Unity 6000.0.62f1 + FishNet 4.x Networking
**Submission Date:** 23.01.2026

---

## Kurzbeschreibung

Showroom Tango ist ein kooperativer Top-Down Bullet-Hell-Shooter für 2 Spieler. Beide Spieler kämpfen gemeinsam gegen 5 Wellen von Gegnern auf einem geteilten Bildschirm. Das Spiel features automatisches Zielsystem mit bis zu 3 Waffen, Neon-Retrofuturistik-Ästhetik, und netzwerkbasiertes Multiplayer über FishNet.

---

## Anleitung zum Starten

### Host Starten:
1. Unity öffnen und Spiel-Scene laden
2. Play drücken
3. Im Netzwerk-Menü auf "Host" klicken
4. Warten bis Client verbindet
5. Spiel startet automatisch nach 3 Sekunden

### Client Verbinden:
1. Zweite Unity-Instanz öffnen (oder Build starten)
2. Play drücken
3. Im Netzwerk-Menü auf "Client" klicken
4. Verbindung wird automatisch zu localhost hergestellt
5. Spiel startet sobald Host bereit ist

**Steuerung:**
- WASD: Bewegung
- Maus: Rotation
- Waffen: Auto-Fire (automatisch auf nächste Gegner)

---

## Technischer Überblick

### Verwendete RPCs

**PlayerController.cs:**
- `SetPlayerNameServerRpc(string name)` - Setzt Spielernamen synchron
- `TakeDamageServerRpc(int damage)` - Schaden auf Server anwenden

**WeaponManager.cs:**
- `FireWeaponServerRpc(Vector3 position, Quaternion rotation, string bulletSpriteName)` - Schuss auf Server spawnen

**GameStateManager.cs:**
- `RequestRestartServerRpc()` - Spiel-Neustart anfordern

**ScoreManager.cs:**
- `AddScoreServerRpc(int points)` - Score hinzufügen (optional, primär server-seitig)

### Verwendete SyncVars

**PlayerController.cs:**
- `SyncVar<string> playerName` - Spielername
- `SyncVar<int> currentHP` - Aktuelle Lebenspunkte
- `SyncVar<Color> playerColor` - Spielerfarbe

**PlayerHealth.cs:**
- `SyncVar<int> currentHealth` - Gesundheit
- `SyncVar<bool> isDead` - Tod-Status

**EnemyHealth.cs:**
- `SyncVar<int> currentHealth` - Gegner-Gesundheit

**GameStateManager.cs:**
- `SyncVar<GameState> currentState` - Spielzustand (Lobby, Playing, GameOver, Victory)

**ScoreManager.cs:**
- `SyncVar<int> teamScore` - Team-Punktestand

### Bullet-Logik

**Pooling-System:**
- BulletPool verwaltet 1000 Bullets in Queue
- Server spawnt Bullets via ServerManager.Spawn()
- Auto-Expansion bei Pool-Erschöpfung
- Bullets werden nach 5s Lifetime oder Collision zurück in Pool
- DespawnType.Pool verhindert Destroy (Objekte bleiben erhalten)

**Schießsystem:**
- WeaponManager: Auto-Fire auf bis zu 3 nächste Gegner
- Prioritäts-Targeting: Waffe 1 → nächster Gegner, Waffe 2 → 2.-nächster, etc.
- Server-Authority: Alle Schüsse spawnen auf Server
- Fire-Rate Upgrades via WeaponConfig

**Synchronisation:**
- NetworkTransform synchronisiert Bullet-Position
- Server spawnt, alle Clients sehen identische Bullets
- Hit-Detection nur server-seitig (gegen Cheating)

### Gegner-Logik

**Enemy-Typen:**
- **EnemyChaser:** Verfolgt nächsten Spieler, 30 HP, Kamikaze-Schaden (20 HP)
- **EnemyShooter:** Hält Distanz, schießt Bullets, 20 HP

**Spawning:**
- EnemySpawner (Server-only)
- 5 Waves: 15 → 25 → 40 → 60 → 100 Gegner
- 70% Chaser, 30% Shooter
- Burst-Spawn (30%) + Trickle-Spawn (70% über 60s)
- Spawn-Position: Kreis mit 12 Units Radius

**AI:**
- Chaser: Vector2.MoveTowards zu nächstem Spieler
- Shooter: Distance-Check + Fire-Rate 1-2s (implementierung ausstehend)
- Server-seitige Bewegung, NetworkTransform synchronisiert

**Wave-System:**
- Victory-Condition: Wave 5 komplett + alle Gegner tot
- Wave-Clear-Bonus: +50 Punkte via ScoreManager
- GameStateManager prüft Conditions in Update()

---

## Persistenz

### PHP/SQL Backend (Primär - Pflichtanforderung)

**Setup:**
1. XAMPP installieren (Apache + MySQL)
2. Datenbank erstellen: `bullethell_scores`
3. SQL-Script ausführen: `Documentation/PHP_Backend/database_setup.sql`
4. PHP-Files kopieren nach: `C:/xampp/htdocs/bullethell/`

**Files:**
- `submit_score.php` - POST: player_name, score → INSERT in DB
- `get_highscores.php` - GET: Return Top 10 als JSON
- `database_setup.sql` - CREATE TABLE highscores

**Unity Integration:**
- HighscoreManager: UnityWebRequest POST/GET
- Automatisches Submit bei Game Over/Victory
- Highscore-Anzeige in UI (Top 10)

**URLs:**
- Submit: `http://localhost/bullethell/submit_score.php`
- Get: `http://localhost/bullethell/get_highscores.php`

### JSON Fallback (Notfall)

Falls PHP/SQL nicht verfügbar:
- HighscoreManager Inspector: `useJSONFallback = true`
- Speichert in: `Application.persistentDataPath/highscores.json`
- Lokale Top-10-Liste, sortiert nach Score

**Vollständige Setup-Anleitung:** `Documentation/PHP_Backend/README_BACKEND_SETUP.txt`

---

## Bonusfeatures

### Implementiert:
- **Neon-Glow Visual System:** Dual-Sprite-Technik mit Outline-Glow (NeonGlowController)
- **Multi-Weapon Auto-Fire:** Brotato-inspiriertes Waffensystem mit 3 Slots
- **Menu Polish:** Hover-Effekte, Audio-Controller, Video-Background

### Geplant (nicht implementiert):
- Power-Ups (Shield, Fire-Rate Boost)
- Komplexe Bullet-Patterns (Spiral, Ring, Homing)
- Erweiterte VFX/SFX

---

## Bekannte Bugs

### Kritisch (Beheben vor Abgabe):
- ❌ **HUD UI nicht verbunden:** HUDManager.cs erstellt, aber Unity Canvas fehlt noch
- ❌ **PHP Backend nicht deployed:** Lokales Setup erforderlich
- ❌ **GameStateManager nicht in Scene:** Prefab/GameObject fehlt

### Minor:
- ⚠️ EnemyShooter schießt noch nicht (nur Movement implementiert)
- ⚠️ WeaponManager benötigt BulletPool-Zuweisung im Inspector (Preflight-Check fehlt)
- ⚠️ Player2 HP-Bar zeigt manchmal falschen Spieler (Race-Condition bei Spawn)

### Testing-Status:
- ✅ Host-Client Verbindung funktioniert
- ✅ Player Movement synchronisiert
- ✅ Enemy Spawning funktioniert
- ⚠️ End-to-End Playthrough ausstehend (HUD fehlt)

---

## Projekt-Struktur

```
Assets/_Project/
├── Scripts/
│   ├── Enemies/          # EnemyChaser, EnemyShooter, EnemyHealth, EnemySpawner
│   ├── Gameflow/         # GameStateManager, ScoreManager (NEU)
│   ├── Network/          # PlayerSpawner, NetworkUIManager
│   ├── Player/           # PlayerController, PlayerHealth, WeaponManager, CameraFollow
│   ├── Projectiles/      # Bullet, BulletPool
│   ├── UI/               # HUDManager (NEU), MenuAudioController, ButtonHover
│   └── Persistence/      # HighscoreManager (NEU)
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   └── Projectiles/
└── Scenes/

Documentation/
├── PHP_Backend/          # submit_score.php, get_highscores.php, database_setup.sql (NEU)
├── 01-12.*.txt           # Umfassende Projekt-Dokumentation
└── GDD_*.md/txt          # Game Design Documents

```

---

## Architektur-Entscheidungen (ADRs)

- **ADR-001:** Wave-System statt Boss (Einfacher, weniger Risiko)
- **ADR-002:** Object-Pooling ab Tag 1 (Performance)
- **ADR-007:** Unity New Input System (Zukunftssicher)
- **ADR-009:** Server-Authority für alle Gameplay-Logik (Anti-Cheat)

**Vollständige ADRs:** `Documentation/04.Architecture_Desitions.txt`

---

## Entwicklungs-Attribution

**Human-Authored:**
- Spielkonzept, Mechanik-Design, Werte-Balancing
- Unity Szenen-Setup, Prefab-Erstellung
- Visual Design (Neon-Aesthetic)

**AI-Assisted:**
- Networking-Code (FishNet 4.x Integration)
- Architektur-Entscheidungen (ADRs)
- Dokumentationsstruktur
- PHP/SQL Backend-Integration
- Code-Kommentare und akademische Header

**Details:** Jedes Script enthält detailliertes Authorship-Tracking im Header

---

## Lizenz & Attribution

Dieses Projekt wurde im Rahmen des PRG-Moduls an der SRH Hochschule Heidelberg entwickelt.

**Third-Party Assets:**
- FishNet Networking (MIT License)
- Unity Input System (Unity Companion License)
- TextMeshPro (Unity Package)

---

## Kontakt

**Entwickler:** Julian Gomez
**Repository:** https://github.com/JuliGommz/Bullet_Love
**Hochschule:** SRH Hochschule Heidelberg
**Kurs:** PRG - Game & Multimedia Design

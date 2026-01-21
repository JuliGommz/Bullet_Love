# PHP/SQL BACKEND SETUP GUIDE
Project: Bullet_Love
Date: 2025-01-20

==================================================
OPTION 1: LOCAL XAMPP (Recommended for Testing)
==================================================

1. Download & Install XAMPP
   - Download from: https://www.apachefriends.org/
   - Install with Apache and MySQL

2. Start Services
   - Open XAMPP Control Panel
   - Start "Apache" and "MySQL"

3. Create Database
   - Open browser: http://localhost/phpmyadmin
   - Click "New" to create database
   - Name: bullethell_scores
   - Run: database_setup.sql (copy-paste SQL into phpMyAdmin)

4. Copy PHP Files
   - Navigate to: C:/xampp/htdocs/
   - Create folder: bullethell
   - Copy files:
     * submit_score.php
     * get_highscores.php

5. Test Backend
   - Open browser: http://localhost/bullethell/get_highscores.php
   - Should show JSON array with test data

6. Configure Unity
   - HighscoreManager Inspector:
     * submitScoreURL: http://localhost/bullethell/submit_score.php
     * getHighscoresURL: http://localhost/bullethell/get_highscores.php
     * useJSONFallback: false

==================================================
OPTION 2: JSON FALLBACK (No Backend Required)
==================================================

If PHP/SQL setup fails:

1. Open Unity Scene with HighscoreManager
2. Select HighscoreManager GameObject
3. In Inspector:
   - Check "useJSONFallback"
   - File will be saved to: Application.persistentDataPath/highscores.json

Location on Windows:
C:/Users/[USERNAME]/AppData/LocalLow/[CompanyName]/Bullet_Love/highscores.json

==================================================
TESTING
==================================================

1. Play game in Unity
2. Score some points
3. Trigger Game Over or Victory
4. Check:
   - XAMPP: http://localhost/bullethell/get_highscores.php
   - OR JSON: Check persistentDataPath folder
   - Console should show "[HighscoreManager] Score submitted: ..."

==================================================
TROUBLESHOOTING
==================================================

Error: "Database error: Connection refused"
→ XAMPP MySQL not running - start in Control Panel

Error: "Access denied for user 'root'"
→ Edit PHP files - update $username and $password

Error: "Table 'bullethell_scores.highscores' doesn't exist"
→ Run database_setup.sql in phpMyAdmin

Error: "Failed to connect to localhost"
→ Use JSON fallback or check firewall blocking port 80

==================================================
DEPLOYMENT (Optional for Production)
==================================================

For online hosting:
1. Upload PHP files to web server
2. Create MySQL database on hosting
3. Update PHP $host, $username, $password
4. Update Unity URLs to: http://yoursite.com/bullethell/...

⚠️ SECURITY NOTE for production:
- Change MySQL password from empty string
- Add SQL injection protection
- Validate input more strictly
- Use HTTPS, not HTTP

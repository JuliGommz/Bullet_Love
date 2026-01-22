<?php
/*
====================================================================
* submit_score.php - Submit Highscore to Database
====================================================================
* Project: Bullet_Love
* Developer: Julian Gomez
* Date: 2025-01-20
*
* SETUP INSTRUCTIONS:
* 1. Create MySQL database: showroomtango_scores
* 2. Create table (see database_setup.sql)
* 3. Update $host, $username, $password below
* 4. Upload to web server (or run on localhost with XAMPP)
====================================================================
*/

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *'); // Allow Unity to access

// Database configuration
$host = 'localhost';
$dbname = 'showroomtango_scores';
$username = 'root'; // Change for production
$password = '';     // Change for production

// Get POST data from Unity
$player_name = isset($_POST['player_name']) ? $_POST['player_name'] : '';
$score = isset($_POST['score']) ? intval($_POST['score']) : 0;

// Validate input
if (empty($player_name) || $score < 0) {
    echo json_encode(['success' => false, 'error' => 'Invalid input']);
    exit;
}

try {
    // Connect to database
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8", $username, $password);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Insert score
    $stmt = $pdo->prepare("INSERT INTO highscores (player_name, score, timestamp) VALUES (:name, :score, NOW())");
    $stmt->execute([
        ':name' => $player_name,
        ':score' => $score
    ]);

    echo json_encode([
        'success' => true,
        'message' => 'Score submitted successfully',
        'id' => $pdo->lastInsertId()
    ]);

} catch (PDOException $e) {
    echo json_encode([
        'success' => false,
        'error' => 'Database error: ' . $e->getMessage()
    ]);
}
?>

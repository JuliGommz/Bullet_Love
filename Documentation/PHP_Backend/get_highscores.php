<?php
/*
====================================================================
* get_highscores.php - Retrieve Top 10 Highscores
====================================================================
* Project: Bullet_Love
* Developer: Julian Gomez
* Date: 2025-01-20
====================================================================
*/

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *'); // Allow Unity to access

// Database configuration
$host = 'localhost';
$dbname = 'showroomtango_scores';
$username = 'root'; // Change for production
$password = '';     // Change for production

try {
    // Connect to database
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8", $username, $password);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Get top 10 scores
    $stmt = $pdo->query("
        SELECT player_name as playerName, score, timestamp
        FROM highscores
        ORDER BY score DESC
        LIMIT 10
    ");

    $results = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Return as JSON array
    echo json_encode($results);

} catch (PDOException $e) {
    echo json_encode([
        'error' => 'Database error: ' . $e->getMessage()
    ]);
}
?>

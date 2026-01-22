-- ====================================================================
-- Database Setup for Bullet_Love Highscores
-- ====================================================================
-- Project: Bullet_Love
-- Developer: Julian Gomez
-- Date: 2025-01-20
--
-- INSTRUCTIONS:
-- 1. Open phpMyAdmin (or MySQL command line)
-- 2. Create database: showroomtango_scores
-- 3. Run this SQL script to create the table
-- ====================================================================

CREATE DATABASE IF NOT EXISTS showroomtango_scores
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE showroomtango_scores;

CREATE TABLE IF NOT EXISTS highscores (
    id INT AUTO_INCREMENT PRIMARY KEY,
    player_name VARCHAR(50) NOT NULL,
    score INT NOT NULL DEFAULT 0,
    timestamp DATETIME NOT NULL,
    INDEX idx_score (score DESC),
    INDEX idx_timestamp (timestamp DESC)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Sample test data (optional)
INSERT INTO highscores (player_name, score, timestamp) VALUES
('TestPlayer1', 1500, NOW()),
('TestPlayer2', 1200, NOW()),
('TestPlayer3', 1000, NOW());

-- ============================================================
--  CipherGuard Security ChatBot – Part 3
--  MySQL Setup Script
--  Run this in MySQL Workbench or via command line:
--      mysql -u root -p < setup_database.sql
-- ============================================================

-- 1. Create the database
CREATE DATABASE IF NOT EXISTS cybersecurity_chatbot
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE cybersecurity_chatbot;

-- 2. Create the tasks table
CREATE TABLE IF NOT EXISTS tasks (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    title       VARCHAR(200)  NOT NULL,
    description TEXT          NOT NULL,
    reminder    VARCHAR(200)  NULL,
    status      VARCHAR(50)   NOT NULL DEFAULT 'Pending',
    created_at  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  DATETIME      ON UPDATE CURRENT_TIMESTAMP
);

-- 3. Insert sample cybersecurity tasks (optional – delete if not needed)
INSERT INTO tasks (title, description, reminder, status) VALUES
('Enable Two-Factor Authentication', 'Set up 2FA on all important accounts including email, banking, and social media.', 'Remind me in 7 days', 'Pending'),
('Review Account Privacy Settings', 'Check privacy settings on all social media platforms and disable unnecessary data sharing.', '2026-07-01', 'Pending'),
('Update All Passwords', 'Change weak or reused passwords to strong unique ones. Use a password manager.', 'Remind me in 3 days', 'Pending');

-- 4. Verify
SELECT * FROM tasks;

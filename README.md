#  Cyra - Cybersecurity Chatbot (WPF + C#)

Cyra is an interactive desktop chatbot built using **C# WPF** that helps users learn cybersecurity concepts, manage tasks, and test their knowledge through quizzes. It also logs user activity and stores tasks in a MySQL database.

---

##  Features

###  AI Chatbot (Cyra)
- Keyword-based NLP response system
- Cybersecurity-focused knowledge base:
  - Phishing awareness
  - Password safety
  - Malware & ransomware
  - VPN & Wi-Fi security
  - 2FA / MFA explanation
- Simple conversational interface

### Quiz System
- 12 cybersecurity multiple-choice questions
- Instant feedback with explanations
- Score tracking and final results summary
- Progress tracking per question

###  Task Manager (MySQL Database)
- Add cybersecurity-related tasks
- Store tasks in a MySQL database
- Mark tasks as completed
- Delete tasks
- Auto table creation if missing

###  Activity Log
- Tracks user actions:
  - Chat queries
  - Quiz progress
  - Task operations
- Displays last 10 activities in UI
- Chat command: `"show activity log"`

###  Sound Feature
- Plays a greeting sound (`greetings.wav`) on startup

---

## Technologies Used

- C# (.NET WPF)
- XAML (UI Design)
- MySQL (Database)
- Regex-based NLP logic
- File I/O (chat history logging)

---

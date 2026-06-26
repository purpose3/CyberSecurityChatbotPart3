using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace WpfApp2
{
   
    public class TaskItem
    {
        public int    Id          { get; set; }
        public string Title       { get; set; } = "";
        public string Description { get; set; } = "";
        public string Reminder    { get; set; } = "";
        public string Status      { get; set; } = "Pending";
    }

    public class QuizQuestion
    {
        public string         Question { get; set; } = "";
        public List<string>   Options  { get; set; } = new();
        public int            CorrectIndex { get; set; }
        public string         Explanation  { get; set; } = "";
        public bool           IsTrueFalse  { get; set; } = false;
    }

    public partial class MainWindow : Window
    {
      
        private readonly Dictionary<string, string> responses = new()
        {
            { "phishing",     "Phishing is a cyberattack where someone pretends to be trusted to steal your sensitive info. Look for urgent language, suspicious links, and misspellings." },
            { "warning signs","Warning signs include: urgent language, requests for OTPs, suspicious links, or misspellings." },
            { "hello",        "Hello there! I'm Cyra, your cybersecurity assistant. How can I help?" },
            { "hi",           "Hi! Ask me about phishing, passwords, Wi-Fi safety, malware, or type 'quiz' to test your knowledge." },
            { "wi-fi",        "Avoid logging into sensitive accounts on public Wi-Fi. Use a VPN on public networks." },
            { "home wi-fi",   "Change default passwords on your home router and use WPA3 encryption if available." },
            { "password",     "Use long, unique passwords (12+ chars). Mix uppercase, lowercase, numbers and symbols. Use a password manager!" },
            { "malware",      "Malware is malicious software. Keep your OS and antivirus updated, and avoid downloading files from untrusted sources." },
            { "ransomware",   "Ransomware encrypts your files and demands payment. Back up data regularly and never open suspicious email attachments." },
            { "2fa",          "Two-Factor Authentication (2FA) adds an extra security layer. Enable it on all important accounts." },
            { "vpn",          "A VPN encrypts your internet traffic, protecting your privacy—especially on public networks." },
            { "social engineering", "Social engineering manipulates people into revealing info. Always verify identities before sharing sensitive data." },
            { "firewall",     "A firewall monitors incoming/outgoing network traffic. Keep it enabled on all devices." },
            { "sql",          "SQL (Structured Query Language) is used to manage databases. SQL Injection is a common web attack—always sanitise inputs." },
            { "add task",     "Sure! Switch to the Tasks tab to add a cybersecurity task with a reminder." },
            { "quiz",         "Great! Switch to the Quiz tab to test your cybersecurity knowledge." },
            { "activity log", "Switch to the Activity Log tab to view recent chatbot actions." },
            { "help",         "I can help with: phishing, passwords, Wi-Fi safety, malware, 2FA, VPN, firewalls. Or type 'quiz' / 'add task'." },
        };

        private readonly string historyFile = "History/chat_history.txt";

        
        private readonly List<string> activityLog = new();

        
        private readonly List<QuizQuestion> quizQuestions = new()
        {
            new QuizQuestion {
                Question = "What should you do if you receive an email asking for your password?",
                Options = new() { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing", "D) Ignore it" },
                CorrectIndex = 2,
                Explanation = "Reporting phishing emails helps prevent scams and protects others."
            },
            new QuizQuestion {
                Question = "True or False: Using the same password for multiple accounts is safe.",
                Options = new() { "True", "False" },
                CorrectIndex = 1,
                Explanation = "False! If one account is breached, attackers can access all others using the same password.",
                IsTrueFalse = true
            },
            new QuizQuestion {
                Question = "What does '2FA' stand for?",
                Options = new() { "A) Two-Factor Authentication", "B) Two-File Access", "C) Transfer File Authority", "D) Twin Firewall Activation" },
                CorrectIndex = 0,
                Explanation = "Two-Factor Authentication adds a second verification step, greatly improving account security."
            },
            new QuizQuestion {
                Question = "True or False: Public Wi-Fi is always safe to use for online banking.",
                Options = new() { "True", "False" },
                CorrectIndex = 1,
                Explanation = "False! Public Wi-Fi can be monitored by attackers. Always use a VPN or mobile data for banking.",
                IsTrueFalse = true
            },
            new QuizQuestion {
                Question = "Which of the following is a sign of a phishing email?",
                Options = new() { "A) Personalised greeting with your full name", "B) Urgent language demanding immediate action", "C) Sent from a known company domain", "D) Contains no links" },
                CorrectIndex = 1,
                Explanation = "Phishing emails create urgency to pressure victims into acting without thinking."
            },
            new QuizQuestion {
                Question = "What is ransomware?",
                Options = new() { "A) Software that speeds up your PC", "B) A virus that displays ads", "C) Malware that encrypts your files and demands payment", "D) A firewall tool" },
                CorrectIndex = 2,
                Explanation = "Ransomware locks your data until you pay a ransom. Regular backups are the best defence."
            },
            new QuizQuestion {
                Question = "True or False: A VPN hides your IP address and encrypts your traffic.",
                Options = new() { "True", "False" },
                CorrectIndex = 0,
                Explanation = "True! A VPN creates an encrypted tunnel, masking your real IP from websites and attackers.",
                IsTrueFalse = true
            },
            new QuizQuestion {
                Question = "What is social engineering?",
                Options = new() { "A) Building social networks", "B) Manipulating people into revealing confidential info", "C) Engineering social media platforms", "D) A type of firewall" },
                CorrectIndex = 1,
                Explanation = "Social engineering exploits human psychology rather than technical vulnerabilities."
            },
            new QuizQuestion {
                Question = "Which password is the strongest?",
                Options = new() { "A) password123", "B) MyBirthday1990", "C) T@9kL!pQ#mX2", "D) qwerty" },
                CorrectIndex = 2,
                Explanation = "Strong passwords use a mix of uppercase, lowercase, numbers and symbols, and are at least 12 characters long."
            },
            new QuizQuestion {
                Question = "True or False: You should keep your operating system and software up to date.",
                Options = new() { "True", "False" },
                CorrectIndex = 0,
                Explanation = "True! Updates patch security vulnerabilities that attackers actively exploit.",
                IsTrueFalse = true
            },
            new QuizQuestion {
                Question = "What does a firewall do?",
                Options = new() { "A) Blocks physical access to servers", "B) Monitors and filters network traffic", "C) Encrypts your files", "D) Scans emails for grammar" },
                CorrectIndex = 1,
                Explanation = "A firewall acts as a gatekeeper, allowing or blocking traffic based on security rules."
            },
            new QuizQuestion {
                Question = "Which of these is an example of a safe browsing habit?",
                Options = new() { "A) Clicking all links in emails", "B) Downloading software from unknown sites", "C) Checking for HTTPS before entering personal info", "D) Using the same browser tab for banking and social media" },
                CorrectIndex = 2,
                Explanation = "HTTPS indicates the connection is encrypted. Always verify it before entering sensitive data."
            },
        };

        private int  currentQuestionIndex = 0;
        private int  quizScore            = 0;
        private bool quizAnswered         = false;
        private bool quizActive           = false;

        
        public MainWindow()
        {
            InitializeComponent();
            PlayGreeting();
            BotMessage("Hello! I'm Cyra, your cybersecurity chatbot. How may I help you today?\n\nTry asking about: phishing • passwords • malware • 2FA • VPN\nOr type: 'quiz', 'add task', 'show activity log'");
        }

        
        private void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greetings.wav");
                if (File.Exists(path))
                {
                    var player = new SoundPlayer(path);
                    player.Load();
                    player.Play();
                }
            }
            catch { /* silent fail if wav missing */ }
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendButton_Click(sender, e);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            AddUserMessage(userMessage);
            SaveMessage("USER", userMessage);

            string response = ProcessNLP(userMessage);
            BotMessage(response);
            SaveMessage("BOT", response);

            UserInput.Clear();
            ChatScroll.ScrollToBottom();
        }

        
        private string ProcessNLP(string input)
        {
            string lower = input.ToLower();

            // Activity log commands
            if (Regex.IsMatch(lower, @"(show|view|display|what have you done|activity|log)"))
            {
                LogActivity("NLP: User requested activity log via chat");
                return ShowActivityLogSummary();
            }

            // Task-related intent
            if (Regex.IsMatch(lower, @"(add|create|new|set).*(task|reminder|to-?do)") ||
                Regex.IsMatch(lower, @"remind me (to|about)") ||
                lower.Contains("remind me"))
            {
                LogActivity("NLP: Detected task/reminder intent in chat input");
                return "I'll help you set that up! Switch to the ✅ Tasks tab and fill in the form. You can add a title, description, and reminder date.";
            }

            // Quiz intent
            if (Regex.IsMatch(lower, @"(quiz|test|question|game|play|challenge)"))
            {
                LogActivity("NLP: Detected quiz intent in chat input");
                return "Switch to the 🎮 Quiz tab to start the cybersecurity quiz! You'll face 12 questions on phishing, passwords, malware, and more.";
            }

            // 2FA / two-factor
            if (Regex.IsMatch(lower, @"(2fa|two.?factor|two factor|mfa|multi.?factor)"))
            {
                LogActivity("NLP: User asked about 2FA/MFA");
                return responses["2fa"];
            }

            // Search dictionary responses with flexible keyword matching
            foreach (var kvp in responses)
            {
                if (lower.Contains(kvp.Key.ToLower()))
                {
                    LogActivity($"NLP: Matched keyword '{kvp.Key}' in user input");
                    return kvp.Value;
                }
            }

            
            LogActivity("NLP: No keyword matched – returned default response");
            return "I didn't quite understand that. Could you rephrase? You can ask about phishing, passwords, malware, 2FA, VPN, or type 'quiz' / 'add task'.";
        }

        
        public void BotMessage(string message)
        {
            var stack = new StackPanel();

            var time = new TextBlock
            {
                Text       = DateTime.Now.ToString("HH:mm"),
                Foreground = Brushes.DarkGray,
                FontSize   = 11,
                Margin     = new Thickness(4, 0, 0, 2)
            };

            var border = new Border
            {
                Background          = new SolidColorBrush(Color.FromRgb(22, 33, 62)),
                CornerRadius        = new CornerRadius(0, 12, 12, 12),
                Padding             = new Thickness(12, 10, 12, 10),
                Margin              = new Thickness(4, 0, 60, 6),
                MaxWidth            = 500,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var text = new TextBlock
            {
                Text        = " Cyra: " + message,
                Foreground  = Brushes.White,
                FontSize    = 14,
                TextWrapping = TextWrapping.Wrap,
                LineHeight  = 20
            };

            border.Child = text;
            stack.Children.Add(time);
            stack.Children.Add(border);
            ChatPanel.Children.Add(stack);
        }

        private void AddUserMessage(string message)
        {
            var stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };

            var time = new TextBlock
            {
                Text                = DateTime.Now.ToString("HH:mm"),
                Foreground          = Brushes.Gray,
                FontSize            = 11,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin              = new Thickness(0, 0, 6, 2)
            };

            var border = new Border
            {
                Background          = new SolidColorBrush(Color.FromRgb(233, 69, 96)),
                CornerRadius        = new CornerRadius(12, 0, 12, 12),
                Padding             = new Thickness(12, 10, 12, 10),
                Margin              = new Thickness(60, 0, 4, 6),
                MaxWidth            = 500,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var text = new TextBlock
            {
                Text         = "You: " + message,
                Foreground   = Brushes.White,
                FontSize     = 14,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = text;
            stack.Children.Add(time);
            stack.Children.Add(border);
            ChatPanel.Children.Add(stack);
        }

        private void SaveMessage(string user, string message)
        {
            try
            {
                Directory.CreateDirectory("History");
                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{user}] {message}";
                File.AppendAllText(historyFile, line + Environment.NewLine);
            }
            catch { /* ignore file errors */ }
        }

       
        private string GetConnectionString()
        {
            return ConnStringBox.Text.Trim();
        }

        private void EnsureTableExists()
        {
            using var conn = new MySqlConnection(GetConnectionString());
            conn.Open();
            string sql = @"
                CREATE TABLE IF NOT EXISTS tasks (
                    id          INT AUTO_INCREMENT PRIMARY KEY,
                    title       VARCHAR(200) NOT NULL,
                    description TEXT,
                    reminder    VARCHAR(200),
                    status      VARCHAR(50) DEFAULT 'Pending',
                    created_at  DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private void SaveTask_Click(object sender, RoutedEventArgs e)
        {
            string title       = TaskTitle.Text.Trim();
            string description = TaskDescription.Text.Trim();
            string reminder    = TaskReminder.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                TaskStatusLabel.Text = " Title and Description are required.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.Orange);
                return;
            }

            try
            {
                EnsureTableExists();
                using var conn = new MySqlConnection(GetConnectionString());
                conn.Open();

                string sql = "INSERT INTO tasks (title, description, reminder) VALUES (@t, @d, @r)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@d", description);
                cmd.Parameters.AddWithValue("@r", reminder);
                cmd.ExecuteNonQuery();

                LogActivity($"Task added: '{title}'" + (reminder != "" ? $" (Reminder: {reminder})" : ""));
                TaskStatusLabel.Text = $"✔ Task '{title}' saved successfully!";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.LightGreen);

                TaskTitle.Clear();
                TaskDescription.Clear();
                TaskReminder.Clear();

                LoadTasks();
            }
            catch (Exception ex)
            {
                TaskStatusLabel.Text = $"❌ DB Error: {ex.Message}";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.OrangeRed);
            }
        }

        private void RefreshTasks_Click(object sender, RoutedEventArgs e) => LoadTasks();

        private void LoadTasks()
        {
            try
            {
                EnsureTableExists();
                using var conn = new MySqlConnection(GetConnectionString());
                conn.Open();

                string sql = "SELECT id, title, description, reminder, status FROM tasks ORDER BY id DESC";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                var tasks = new List<TaskItem>();
                while (reader.Read())
                {
                    tasks.Add(new TaskItem
                    {
                        Id          = reader.GetInt32(0),
                        Title       = reader.GetString(1),
                        Description = reader.GetString(2),
                        Reminder    = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Status      = reader.GetString(4)
                    });
                }

                TaskListView.ItemsSource = tasks;
                TaskStatusLabel.Text = $" Loaded {tasks.Count} task(s).";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.LightGreen);
            }
            catch (Exception ex)
            {
                TaskStatusLabel.Text = $" DB Error: {ex.Message}";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.OrangeRed);
            }
        }

        private void MarkComplete_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is not TaskItem selected) return;
            try
            {
                using var conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                string sql = "UPDATE tasks SET status='Completed' WHERE id=@id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", selected.Id);
                cmd.ExecuteNonQuery();

                LogActivity($"Task marked complete: '{selected.Title}'");
                TaskStatusLabel.Text = $" Task '{selected.Title}' marked as completed.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.LightGreen);
                LoadTasks();
            }
            catch (Exception ex)
            {
                TaskStatusLabel.Text = $" DB Error: {ex.Message}";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.OrangeRed);
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is not TaskItem selected) return;

            var confirm = MessageBox.Show($"Delete task '{selected.Title}'?", "Confirm Delete",
                          MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                using var conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                string sql = "DELETE FROM tasks WHERE id=@id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", selected.Id);
                cmd.ExecuteNonQuery();

                LogActivity($"Task deleted: '{selected.Title}'");
                TaskStatusLabel.Text = $" Task '{selected.Title}' deleted.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.Orange);
                LoadTasks();
            }
            catch (Exception ex)
            {
                TaskStatusLabel.Text = $" DB Error: {ex.Message}";
                TaskStatusLabel.Foreground = new SolidColorBrush(Colors.OrangeRed);
            }
        }

        
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex = 0;
            quizScore            = 0;
            quizActive           = true;
            FeedbackBorder.Visibility = Visibility.Collapsed;
            NextQuestionBtn.Visibility = Visibility.Visible;
            LogActivity("Quiz started");
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            quizAnswered = false;
            var q = quizQuestions[currentQuestionIndex];

            QuizProgressLabel.Text = $"Question {currentQuestionIndex + 1} of {quizQuestions.Count}  |  Score: {quizScore}";
            QuizProgress.Value = currentQuestionIndex;
            QuizQuestion.Text  = q.Question;

            FeedbackBorder.Visibility  = Visibility.Collapsed;
            NextQuestionBtn.Visibility = Visibility.Collapsed;
            AnswerPanel.Children.Clear();

            for (int i = 0; i < q.Options.Count; i++)
            {
                int idx = i; 
                var btn = new Button
                {
                    Content         = q.Options[i],
                    Height          = 44,
                    Margin          = new Thickness(0, 0, 0, 8),
                    FontSize        = 14,
                    Background      = new SolidColorBrush(Color.FromRgb(15, 52, 96)),
                    Foreground      = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor          = Cursors.Hand,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Padding         = new Thickness(16, 0, 0, 0)
                };
                btn.Click += (s, e) => HandleAnswer(idx);
                AnswerPanel.Children.Add(btn);
            }
        }

        private void HandleAnswer(int selectedIndex)
        {
            if (quizAnswered) return;
            quizAnswered = true;

            var q = quizQuestions[currentQuestionIndex];
            bool correct = selectedIndex == q.CorrectIndex;

            if (correct) quizScore++;

            // Colour the buttons
            for (int i = 0; i < AnswerPanel.Children.Count; i++)
            {
                if (AnswerPanel.Children[i] is Button btn)
                {
                    if (i == q.CorrectIndex)
                        btn.Background = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    else if (i == selectedIndex && !correct)
                        btn.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                    btn.IsEnabled = false;
                }
            }

            // Show feedback
            FeedbackBorder.Visibility = Visibility.Visible;
            FeedbackText.Text = correct
                ? $" Correct! {q.Explanation}"
                : $" Incorrect. The correct answer was: {q.Options[q.CorrectIndex]}\n{q.Explanation}";
            FeedbackText.Foreground = correct
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                : new SolidColorBrush(Color.FromRgb(244, 67, 54));

            NextQuestionBtn.Visibility = Visibility.Visible;
            LogActivity($"Quiz Q{currentQuestionIndex + 1}: {(correct ? "Correct" : "Incorrect")}");
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;
            ShowQuestion();
        }

        private void EndQuiz()
        {
            quizActive = false;
            int total  = quizQuestions.Count;
            int pct    = (int)Math.Round((double)quizScore / total * 100);

            string verdict = pct >= 80
                ? " Great job! You're a cybersecurity pro!"
                : pct >= 50
                    ? " Good effort! Keep learning to stay safe online."
                    : " Keep learning to stay safe online!";

            QuizProgressLabel.Text     = $"Quiz Complete! Score: {quizScore}/{total} ({pct}%)";
            QuizProgress.Value         = total;
            QuizQuestion.Text          = $"Quiz finished!\n\nYour score: {quizScore} / {total}  ({pct}%)\n\n{verdict}";
            AnswerPanel.Children.Clear();
            FeedbackBorder.Visibility  = Visibility.Collapsed;
            NextQuestionBtn.Visibility = Visibility.Collapsed;

            LogActivity($"Quiz completed – Score: {quizScore}/{total} ({pct}%)");
        }

        
        private void LogActivity(string description)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {description}";
            activityLog.Add(entry);
            RefreshActivityLogUI();
        }

        private void RefreshActivityLogUI()
        {
            ActivityLogPanel.Children.Clear();

            // Show only last 10
            var recentEntries = activityLog.TakeLast(10).Reverse().ToList();

            if (recentEntries.Count == 0)
            {
                ActivityLogPanel.Children.Add(new TextBlock
                {
                    Text       = "No activity recorded yet.",
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize   = 14,
                    Margin     = new Thickness(8)
                });
                return;
            }

            int index = recentEntries.Count;
            foreach (var entry in recentEntries)
            {
                var border = new Border
                {
                    Background    = new SolidColorBrush(Color.FromRgb(26, 26, 46)),
                    CornerRadius  = new CornerRadius(6),
                    Padding       = new Thickness(12, 8, 12, 8),
                    Margin        = new Thickness(0, 0, 0, 6),
                    BorderBrush   = new SolidColorBrush(Color.FromRgb(50, 50, 80)),
                    BorderThickness = new Thickness(1)
                };

                var tb = new TextBlock
                {
                    Text         = $"{index}. {entry}",
                    Foreground   = Brushes.LightGray,
                    FontSize     = 13,
                    TextWrapping = TextWrapping.Wrap
                };

                border.Child = tb;
                ActivityLogPanel.Children.Add(border);
                index--;
            }

            if (activityLog.Count > 10)
            {
                ActivityLogPanel.Children.Add(new TextBlock
                {
                    Text       = $"… and {activityLog.Count - 10} older entries (cleared from view).",
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize   = 12,
                    Margin     = new Thickness(8, 4, 0, 0)
                });
            }
        }

        private string ShowActivityLogSummary()
        {
            if (activityLog.Count == 0)
                return "No activity recorded yet.";

            var recent = activityLog.TakeLast(5).ToList();
            string summary = "Here's a summary of recent actions:\n";
            for (int i = 0; i < recent.Count; i++)
                summary += $"  {i + 1}. {recent[i]}\n";

            return summary.TrimEnd();
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            activityLog.Clear();
            RefreshActivityLogUI();
        }
    }
}

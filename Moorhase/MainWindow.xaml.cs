using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Moorhase
{
    public partial class MainWindow : Window
    {
        private List<Button> gridButtons = new List<Button>();
        private Random random = new Random();
        private DispatcherTimer? appearTimer;
        private DispatcherTimer? hideTimer;
        private DispatcherTimer? difficultyTimer;
        private int currentRabbitButton = -1;
        private int score = 0;
        private int rabbitsCount = 0;
        private double currentInterval = 2000;
        private GameMode gameMode;
        private GameState currentGameState;
        private HighScoreManager highScoreManager;
        private string currentPlayerName = "";
        private bool isRabbitVisible = false;
        private bool isHase = false; // true = Hase, false = Jäger
        
        // Image brushes
        private Brush? grassBrush;
        private Brush? hasenBrush;
        private Brush? jaegerBrush;
        
        // Window state for fullscreen
        private WindowStyle previousWindowStyle;
        private WindowState previousWindowState;
        private double previousHeight;
        private double previousWidth;
        private double previousTop;
        private double previousLeft;
        private bool isFullscreen = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadImages();
            highScoreManager = new HighScoreManager();
            currentGameState = GameState.Menu;
        }

        private void LoadImages()
        {
            try
            {
                string assetsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
                
                string grassPath = System.IO.Path.Combine(assetsPath, "Grass.png");
                string hasenPath = System.IO.Path.Combine(assetsPath, "Hase.png");
                string jaegerPath = System.IO.Path.Combine(assetsPath, "Jäger.png");
                
                if (System.IO.File.Exists(grassPath))
                {
                    var grassBitmap = new BitmapImage();
                    grassBitmap.BeginInit();
                    grassBitmap.UriSource = new Uri(grassPath, UriKind.Absolute);
                    grassBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    grassBitmap.EndInit();
                    grassBitmap.Freeze();
                    grassBrush = new ImageBrush(grassBitmap) { Stretch = Stretch.UniformToFill };
                }
                else
                {
                    grassBrush = new SolidColorBrush(Colors.Green);
                }
                
                if (System.IO.File.Exists(hasenPath))
                {
                    var hasenBitmap = new BitmapImage();
                    hasenBitmap.BeginInit();
                    hasenBitmap.UriSource = new Uri(hasenPath, UriKind.Absolute);
                    hasenBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    hasenBitmap.EndInit();
                    hasenBitmap.Freeze();
                    hasenBrush = new ImageBrush(hasenBitmap) { Stretch = Stretch.UniformToFill };
                }
                else
                {
                    hasenBrush = new SolidColorBrush(Colors.Brown);
                }
                
                if (System.IO.File.Exists(jaegerPath))
                {
                    var jaegerBitmap = new BitmapImage();
                    jaegerBitmap.BeginInit();
                    jaegerBitmap.UriSource = new Uri(jaegerPath, UriKind.Absolute);
                    jaegerBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    jaegerBitmap.EndInit();
                    jaegerBitmap.Freeze();
                    jaegerBrush = new ImageBrush(jaegerBitmap) { Stretch = Stretch.UniformToFill };
                }
                else
                {
                    jaegerBrush = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Bilder: {ex.Message}");
                grassBrush = new SolidColorBrush(Colors.Green);
                hasenBrush = new SolidColorBrush(Colors.Brown);
                jaegerBrush = null;
            }
        }

        private void TaskModeButton_Click(object sender, RoutedEventArgs e)
        {
            gameMode = GameMode.Task;
            ModeIntroLabel.Text = "AUFGABEN-MODUS  - Finde den Hasen!";
            ShowNameInputScreen();
        }

        private void ExpertModeButton_Click(object sender, RoutedEventArgs e)
        {
            gameMode = GameMode.Expert;
            ModeIntroLabel.Text = "EXPERT-MODUS  - Schnell und treffsicher!";
            ShowNameInputScreen();
        }

        private void ShowNameInputScreen()
        {
            MenuScreen.Visibility = Visibility.Collapsed;
            NameInputScreen.Visibility = Visibility.Visible;
            PlayerNameTextBox.Text = "";
            PlayerNameTextBox.Focus();
            currentGameState = GameState.NameInput;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            currentPlayerName = PlayerNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(currentPlayerName))
            {
                MessageBox.Show("Bitte geben Sie einen Namen ein!", "Fehler");
                return;
            }

            InitializeGame();
            ShowGameScreen();
        }

        private void ShowGameScreen()
        {
            NameInputScreen.Visibility = Visibility.Collapsed;
            GameScreen.Visibility = Visibility.Visible;
            currentGameState = GameState.Playing;
            ScoreLabel.Content = "0";
            RabbitsLabel.Content = "x0";
            score = 0;
            rabbitsCount = 0;
            GameGrid.Focus();
        }

        private void InitializeGame()
        {
            GameGrid.Children.Clear();
            gridButtons.Clear();

            for (int i = 0; i < 64; i++)
            {
                Button btn = new Button();
                btn.Background = grassBrush ?? new SolidColorBrush(Colors.Green);
                btn.Cursor = Cursors.Hand;
                btn.Tag = i;
                btn.Click += GridButton_Click;
                btn.Focusable = false;

                gridButtons.Add(btn);
                GameGrid.Children.Add(btn);
            }

            currentInterval = 2000;
            
            appearTimer = new DispatcherTimer();
            appearTimer.Interval = TimeSpan.FromMilliseconds(currentInterval);
            appearTimer.Tick += AppearTimer_Tick;
            appearTimer.Start();

            if (gameMode == GameMode.Expert)
            {
                difficultyTimer = new DispatcherTimer();
                difficultyTimer.Interval = TimeSpan.FromSeconds(5);
                difficultyTimer.Tick += DifficultyTimer_Tick;
                difficultyTimer.Start();
            }
        }

        private void AppearTimer_Tick(object? sender, EventArgs e)
        {
            appearTimer?.Stop();

            if (currentRabbitButton >= 0 && currentRabbitButton < gridButtons.Count)
            {
                gridButtons[currentRabbitButton].Background = grassBrush ?? new SolidColorBrush(Colors.Green);
            }

            currentRabbitButton = random.Next(0, gridButtons.Count);
            
            // 20% Chance für Jäger, sonst Hase (in beiden Modi)
            if (jaegerBrush != null)
            {
                isHase = random.Next(0, 100) >= 20; // 80% Hase, 20% Jäger
            }
            else
            {
                isHase = true;
            }
            
            gridButtons[currentRabbitButton].Background = isHase ? 
                (hasenBrush ?? new SolidColorBrush(Colors.Brown)) : 
                jaegerBrush;
            isRabbitVisible = true;

            hideTimer = new DispatcherTimer();
            hideTimer.Interval = TimeSpan.FromSeconds(1);
            hideTimer.Tick += HideTimer_Tick;
            hideTimer.Start();
        }

        private void HideTimer_Tick(object? sender, EventArgs e)
        {
            hideTimer?.Stop();

            if (isRabbitVisible && gameMode == GameMode.Expert)
            {
                EndGame("Du warst nicht schnell genug!");
                return;
            }

            if (currentRabbitButton >= 0 && currentRabbitButton < gridButtons.Count)
            {
                gridButtons[currentRabbitButton].Background = grassBrush ?? new SolidColorBrush(Colors.Green);
            }

            isRabbitVisible = false;
            currentRabbitButton = -1;

            if (appearTimer != null && currentGameState == GameState.Playing)
            {
                appearTimer.Interval = TimeSpan.FromMilliseconds(currentInterval);
                appearTimer.Start();
            }
        }

        private void GridButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentGameState != GameState.Playing) return;

            Button? clickedButton = sender as Button;
            if (clickedButton?.Tag is not int buttonIndex) return;

            if (buttonIndex == currentRabbitButton && isRabbitVisible)
            {
                // Jäger getroffen!
                if (!isHase)
                {
                    EndGame("Zwei Jäger treffen sich");
                    return;
                }
                
                // Hase getroffen
                HandleRabbitHit();
            }
            else if (gameMode == GameMode.Expert)
            {
                EndGame("Du hast die falsche Kachel angeklickt!");
            }
        }

        private void HandleRabbitHit()
        {
            isRabbitVisible = false;

            appearTimer?.Stop();
            hideTimer?.Stop();

            gridButtons[currentRabbitButton].Background = grassBrush ?? new SolidColorBrush(Colors.Green);

            score += 10;
            rabbitsCount += 1;
            ScoreLabel.Content = score.ToString();
            RabbitsLabel.Content = "x" + rabbitsCount.ToString();

            if (gameMode == GameMode.Task)
            {
                currentInterval = (int)(currentInterval * 0.9);
                if (currentInterval < 500) currentInterval = 500;
            }
            else if (gameMode == GameMode.Expert)
            {
                currentInterval = (int)(currentInterval * 0.85);
                if (currentInterval < 300) currentInterval = 300;
            }

            currentRabbitButton = -1;
            UpdateTimeDisplay();

            if (appearTimer != null && currentGameState == GameState.Playing)
            {
                appearTimer.Interval = TimeSpan.FromMilliseconds(currentInterval);
                appearTimer.Start();
            }
        }

        private void DifficultyTimer_Tick(object? sender, EventArgs e)
        {
            if (currentGameState == GameState.Playing && gameMode == GameMode.Expert)
            {
                currentInterval = (int)(currentInterval * 0.95);
                if (currentInterval < 300) currentInterval = 300;
                UpdateTimeDisplay();
            }
        }

        private void UpdateTimeDisplay()
        {
            TimeLabel.Content = (currentInterval / 1000.0).ToString("F1") + "s";
        }

        private void EndGame(string reason)
        {
            currentGameState = GameState.GameOver;

            appearTimer?.Stop();
            hideTimer?.Stop();
            difficultyTimer?.Stop();

            highScoreManager.AddHighScore(currentPlayerName, score, gameMode.ToString());

            FinalScoreLabel.Text = $"Punkte: {score} | 🐰 {rabbitsCount}";
            GameOverReasonLabel.Text = reason;
            GameScreen.Visibility = Visibility.Collapsed;
            GameOverScreen.Visibility = Visibility.Visible;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            GameOverScreen.Visibility = Visibility.Collapsed;
            MenuScreen.Visibility = Visibility.Visible;
            currentGameState = GameState.Menu;
        }

        private void ShowHighScoresButton_Click(object sender, RoutedEventArgs e)
        {
            MenuScreen.Visibility = Visibility.Collapsed;
            HighScoresScreen.Visibility = Visibility.Visible;
            DisplayHighScores();
        }

        private void DisplayHighScores()
        {
            HighScoresListBox.Items.Clear();
            
            var taskScores = highScoreManager.GetTopScores("Task", 10);
            var expertScores = highScoreManager.GetTopScores("Expert", 10);

            if (taskScores.Count > 0 || expertScores.Count > 0)
            {
                if (taskScores.Count > 0)
                {
                    HighScoresListBox.Items.Add("=== AUFGABEN-MODUS ===");
                    int rank = 1;
                    foreach (var score in taskScores)
                    {
                        HighScoresListBox.Items.Add($"{rank}. {score.PlayerName}: {score.Points} Punkte ({score.Date:dd.MM.yyyy})");
                        rank++;
                    }
                    HighScoresListBox.Items.Add("");
                }

                if (expertScores.Count > 0)
                {
                    HighScoresListBox.Items.Add("=== EXPERT-MODUS ===");
                    int rank = 1;
                    foreach (var score in expertScores)
                    {
                        HighScoresListBox.Items.Add($"{rank}. {score.PlayerName}: {score.Points} Punkte ({score.Date:dd.MM.yyyy})");
                        rank++;
                    }
                }
            }
            else
            {
                HighScoresListBox.Items.Add("Keine Highscores vorhanden");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            HighScoresScreen.Visibility = Visibility.Collapsed;
            MenuScreen.Visibility = Visibility.Visible;
        }

        private void ClearHighScoresButton_Click(object sender, RoutedEventArgs e)
        {
            // Create password dialog
            var passwordWindow = new Window
            {
                Title = "Passwort eingeben",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Background = new SolidColorBrush(Colors.DarkOliveGreen),
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            var label = new TextBlock
            {
                Text = "Passwort zum Löschen:",
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var passwordBox = new PasswordBox
            {
                Width = 200,
                Height = 30,
                Padding = new Thickness(5),
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 70,
                Height = 30,
                Background = new SolidColorBrush(Colors.YellowGreen),
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),
                IsDefault = true
            };

            var cancelButton = new Button
            {
                Content = "Abbrechen",
                Width = 80,
                Height = 30,
                Background = new SolidColorBrush(Colors.OrangeRed),
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),
                IsCancel = true
            };

            okButton.Click += (s, args) =>
            {
                if (passwordBox.Password == "Hase")
                {
                    highScoreManager.ClearHighScores();
                    DisplayHighScores();
                    MessageBox.Show("Highscores wurden gelöscht!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    passwordWindow.Close();
                }
                else
                {
                    MessageBox.Show("Falsches Passwort!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    passwordBox.Clear();
                    passwordBox.Focus();
                }
            };

            cancelButton.Click += (s, args) => passwordWindow.Close();

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(passwordBox);
            stackPanel.Children.Add(buttonPanel);

            passwordWindow.Content = stackPanel;
            passwordBox.Focus();
            passwordWindow.ShowDialog();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (isFullscreen)
            {
                WindowStyle = previousWindowStyle;
                WindowState = previousWindowState;
                Height = previousHeight;
                Width = previousWidth;
                Top = previousTop;
                Left = previousLeft;
                isFullscreen = false;
            }
            else
            {
                previousWindowStyle = WindowStyle;
                previousWindowState = WindowState;
                previousHeight = Height;
                previousWidth = Width;
                previousTop = Top;
                previousLeft = Left;

                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                isFullscreen = true;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            Height = 600;
            Width = 800;
            Left = (SystemParameters.PrimaryScreenWidth - 800) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - 600) / 2;
            isFullscreen = false;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void QuitGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentGameState == GameState.Playing)
            {
                EndGame("Spiel abgebrochen");
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (currentGameState == GameState.NameInput)
                {
                    NameInputScreen.Visibility = Visibility.Collapsed;
                    MenuScreen.Visibility = Visibility.Visible;
                    currentGameState = GameState.Menu;
                    e.Handled = true;
                }
                else if (currentGameState == GameState.Playing)
                {
                    EndGame("Spiel abgebrochen");
                    e.Handled = true;
                }
            }
        }
    }
}

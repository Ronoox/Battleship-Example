using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Navmaxia
{
    public partial class Form1 : Form
    {
        // Connection to the database
        string connectionString = "Data source=scores.db; Version=3";
        SQLiteConnection connection;

        private const int gridSize = 10;
        private Button[,] playerGrid = new Button[gridSize, gridSize];     // Player grid buttons
        private Button[,] computerGrid = new Button[gridSize, gridSize];   // Computer grid buttons
        private Random random = new Random();

        // Arrays for storing ship positions
        private int[,] computerShips = new int[gridSize, gridSize];  // Computer ship positions
        private int[,] playerShips = new int[gridSize, gridSize];    // Player ship positions

        // Counters
        private int playerHits = 0;
        private int computerHits = 0;
        private int totalShipParts = 14;
        private int playerClicks = 0;
        private int timerTicks = 0;

        // Stats
        private int wins = 0;
        private int losses = 0;
        private int restarts = 0;

        // Dictionary with ship names and their sizes
        private Dictionary<int, string> shipNames = new Dictionary<int, string>
        {
            { 5, "Aircraft Carrier" },
            { 4, "Destroyer" },
            { 3, "Battleship" },
            { 2, "Submarine" }
        };

        private string playerName;

        public Form1()
        {
            InitializeComponent();
            Logo.Image = Image.FromFile("Resources\\Logo.png");
            // Stretch the image
            Logo.SizeMode = PictureBoxSizeMode.StretchImage;

            // Center horizontally
            Logo.Left = 611;

            // Distance from top
            Logo.Top = 20;

            InitializeGrids();          // Create grids
            PlacePlayerShips();         // Place player ships
            PlaceComputerShips();       // Place computer ships
            timer1.Start();             // Start timer
            playerClicks = 0;
            timerTicks = 0;
            this.FormClosing += Form1_FormClosing;
        }

        private void InitializeGrids()
        {
            CreateGrid(playerGrid, 50, 100, false);    // Player grid - not clickable
            CreateGrid(computerGrid, 400, 100, true);   // Computer grid - clickable
        }

        private void CreateGrid(Button[,] grid, int startX, int startY, bool clickable)
        {
            for (int i = 0; i < gridSize; i++) // Grid rows
            {
                for (int j = 0; j < gridSize; j++) // Grid columns
                {
                    Button btn = new Button // Button for ship cell
                    {
                        Size = new Size(30, 30), // Size: width & height
                        Location = new Point(startX + j * 30, startY + i * 30),
                        BackColor = Color.LightGray,
                        Tag = new Point(i, j) // Store coordinates to know which was clicked
                    };

                    if (clickable)
                        btn.Click += new EventHandler(Attack_Check);

                    this.Controls.Add(btn);
                    grid[i, j] = btn; // Save in the correct position in the array
                }
            }
        }

        private void PlaceComputerShips()
        {
            // Place each ship with its corresponding size
            PlaceShip(computerShips, playerShips, 5);  // Aircraft Carrier
            PlaceShip(computerShips, playerShips, 4);  // Destroyer
            PlaceShip(computerShips, playerShips, 3);  // Battleship
            PlaceShip(computerShips, playerShips, 2);  // Submarine
        }

        private void PlacePlayerShips()
        {
            PlaceShip(playerShips, computerShips, 5);
            PlaceShip(playerShips, computerShips, 4);
            PlaceShip(playerShips, computerShips, 3);
            PlaceShip(playerShips, computerShips, 2);

            // Color the player's ships in blue
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (playerShips[i, j] != 0)
                    {
                        playerGrid[i, j].BackColor = Color.Blue;
                    }
                }
            }
        }

        // Check if a ship can be placed at a specific position
        private bool CanPlaceShip(int[,] shipGrid, int[,] otherGrid, int row, int col, int size, bool horizontal)
        {
            if (horizontal)
            {
                if (col + size > gridSize) return false;  // Check grid bounds
                for (int i = 0; i < size; i++)
                {
                    // Check for collision with other ships
                    if (shipGrid[row, col + i] != 0 || otherGrid[row, col + i] != 0)
                        return false;
                }
            }
            else
            {
                if (row + size > gridSize) return false;  // Check grid bounds
                for (int i = 0; i < size; i++)
                {
                    // Check for collision with other ships
                    if (shipGrid[row + i, col] != 0 || otherGrid[row + i, col] != 0)
                        return false;
                }
            }
            return true;
        }

        private void PlaceShip(int[,] shipGrid, int[,] otherGrid, int size)
        {
            bool placed = false;
            while (!placed)
            {
                int row = random.Next(gridSize); // Random row
                int col = random.Next(gridSize); // Random column
                bool horizontal = random.Next(2) == 0;  // Random orientation: horizontal or vertical

                if (CanPlaceShip(shipGrid, otherGrid, row, col, size, horizontal))
                {
                    // Place the ship cell by cell
                    for (int i = 0; i < size; i++)
                    {
                        if (horizontal)
                            shipGrid[row, col + i] = size;
                        else
                            shipGrid[row + i, col] = size;
                    }
                    placed = true;
                }
            }
        }

        private void ComputerAttack()
        {
            bool attacked = false;

            while (!attacked)
            {
                // Random attack position
                int row = random.Next(gridSize);
                int col = random.Next(gridSize);
                Button btn = playerGrid[row, col];

                if (btn.Enabled) // If button is active
                {
                    if (playerShips[row, col] > 0)  // Successful hit
                    {
                        btn.BackColor = Color.Red;
                        btn.Text = "X";
                        computerHits++;

                        playerShips[row, col] = -playerShips[row, col];  // Mark hit section (negative)

                        // Check if ship is sunk
                        if (IsShipSunk(playerShips, Math.Abs(playerShips[row, col]))) // Absolute value to know which ship
                        {
                            MessageBox.Show($"Your {shipNames[Math.Abs(playerShips[row, col])]} was sunk!");
                        }

                        // Check for computer win
                        if (computerHits == totalShipParts)
                        {
                            losses++;
                            string message = $"The computer sank all your ships!\nAttempts: {playerClicks}\nTime (seconds): {timerTicks}";
                            MessageBox.Show(message);
                            SaveGameResult("Computer", timerTicks);
                            DialogResult result = MessageBox.Show("New game?", "Title", MessageBoxButtons.YesNo);

                            if (result == DialogResult.Yes)
                            {
                                button2.PerformClick();
                            }
                            else
                            {
                                button3.PerformClick();
                            }
                        }
                    }
                    else  // Missed shot
                    {
                        btn.BackColor = Color.Green;
                        btn.Text = "-";
                    }

                    btn.Enabled = false;
                    attacked = true;
                }
            }
        }

        private void Attack_Check(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag is Point point)
            {
                playerClicks++;
                int row = point.X;
                int col = point.Y;

                if (computerShips[row, col] > 0)  // Successful hit
                {
                    btn.BackColor = Color.Red;
                    btn.Text = "X";
                    playerHits++;

                    computerShips[row, col] = -computerShips[row, col];  // Mark hit section

                    // Check if ship is sunk
                    int shipSize = Math.Abs(computerShips[row, col]); // Absolute value to know which ship
                    if (IsShipSunk(computerShips, shipSize))
                    {
                        MessageBox.Show($"You sank the opponent's {shipNames[shipSize]}!");
                    }

                    // Check for player win
                    if (playerHits == totalShipParts)
                    {
                        wins++;
                        string message = $"Congratulations! You sank all the ships!\nAttempts: {playerClicks}\nTime (seconds): {timerTicks}";
                        MessageBox.Show(message);
                        SaveGameResult(playerName, timerTicks);
                        DialogResult result = MessageBox.Show("New game?", "Title", MessageBoxButtons.YesNo);

                        if (result == DialogResult.Yes)
                        {
                            button2.PerformClick();
                        }
                        else
                        {
                            button3.PerformClick();
                        }
                    }
                }
                else  // Missed shot
                {
                    btn.BackColor = Color.Green;
                    btn.Text = "-";
                }

                btn.Enabled = false;
                ComputerAttack();  // Computer's turn to attack
            }
        }

        private bool IsShipSunk(int[,] shipGrid, int shipSize)
        {
            int count = 0;
            for (int i = 0; i < gridSize; i++) // Rows
            {
                for (int j = 0; j < gridSize; j++) // Columns
                {
                    if (Math.Abs(shipGrid[i, j]) == shipSize)
                    {
                        if (shipGrid[i, j] < 0)  // Hit section
                            count++;
                        else
                            return false;
                    }
                }
            }
            return count == shipSize;  // All parts are hit
        }

        // Restart button
        private void button2_Click_1(object sender, EventArgs e)
        {
            restarts++;

            // Show stats before restarting
            string statsMessage = $"Wins: {wins}\nLosses: {losses}";
            MessageBox.Show(statsMessage, "Game Stats");

            // Create a new game keeping the stats
            Form1 newGame = new Form1();
            newGame.wins = this.wins;
            newGame.losses = this.losses;
            newGame.restarts = this.restarts;
            newGame.playerName = this.playerName;

            this.Hide();
            newGame.Show();
        }

        // Exit button
        private void button3_Click_1(object sender, EventArgs e)
        {
            // Show final stats if at least one restart occurred
            if (restarts >= 1)
            {
                string message = $"Wins: {wins}\nLosses: {losses}";
                MessageBox.Show(message, "Stats", MessageBoxButtons.OK);
            }

            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AlignButtons();

            // Connect to the database
            connection = new SQLiteConnection(connectionString);

            InitializeDatabase();  // Initialize the database

            // Set window position and size
            this.StartPosition = FormStartPosition.CenterScreen;

            int width = 50 + gridSize * 30 + 300 + 50;
            int height = 100 + gridSize * 30 + 50;
            this.Size = new Size(width, height);

            // Center the grids in the window
            int centerX = (this.ClientSize.Width - width) / 2;
            int centerY = (this.ClientSize.Height - height) / 2;

            // Place player grid buttons
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    playerGrid[i, j].Location = new Point(50 + j * 30 + centerX, 100 + i * 30 + centerY);
                }
            }

            // Place computer grid buttons
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    computerGrid[i, j].Location = new Point(400 + j * 30 + centerX, 100 + i * 30 + centerY);
                }
            }

            // Handle player name
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = Interaction.InputBox("Enter your name:", "Name");
                if (!string.IsNullOrEmpty(playerName))
                {
                    MessageBox.Show("Name: " + playerName);
                }
                else
                {
                    MessageBox.Show("You must enter a name! Closing application...");
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("Name: " + playerName);
            }
        }

        // Initialize the database and create the table
        private void InitializeDatabase()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "CREATE TABLE IF NOT EXISTS scores (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, winner TEXT NOT NULL, time INTEGER NOT NULL)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing database: " + ex.Message);
            }
        }

        // Save the game result to the database
        private void SaveGameResult(string winner, int timeInSeconds)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO scores (name, winner, time) VALUES (@name, @winner, @time)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", playerName);
                        cmd.Parameters.AddWithValue("@winner", winner);
                        cmd.Parameters.AddWithValue("@time", timeInSeconds);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving game result: " + ex.Message);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timerTicks++;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                DialogResult result = MessageBox.Show("Do you want to close the application?", "Exit", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void AlignButtons()
        {
            Alignment aligner = new Alignment();
            aligner.AlignToRight(this, button6, button4);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RulesButton redirect = new RulesButton();
            redirect.Redirect("https://www.officialgamerules.org/board-games/battleship");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HelpButton message = new HelpButton();
            message.ShowMessage("Better not play.");
        }
    }
}

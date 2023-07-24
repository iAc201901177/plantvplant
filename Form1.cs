using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.Data.OleDb;

namespace finals_group6_cselec13
{
    public partial class Form1 : Form
    {

        //string connectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=E:\Visual Studio\Works\finals-group6-cselec13\Database1.accdb";
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source= Database1.accdb";

        WindowsMediaPlayer gameMedia;
        WindowsMediaPlayer shootgMedia;
        WindowsMediaPlayer explosion;

        PictureBox[] EnemyAmmo;
        int EnemyAmmoSpeed;

        PictureBox[] stars;
        int backgroundspeed;
        int playerSpeed;

        PictureBox[] Ammo;
        int AmmoSpeed;

        PictureBox[] Enemy;
        int EnemySpeed;

        Random rand;

        int score;
        int level;
        int difficulty;
        bool pause;
        bool gameIsOver;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            pause = false;
            gameIsOver = false;
            score = 0;
            level = 1;
            difficulty = 9;

            backgroundspeed = 4;
            playerSpeed = 4;
            EnemySpeed = 4;
            AmmoSpeed = 20;
            EnemyAmmoSpeed = 4;

            Ammo = new PictureBox[3];

            Image munition = Image.FromFile(@"asserts\munition.png");

            Image enemy = Image.FromFile("asserts\\enemy.png");
            Image enemyAmmo = Image.FromFile("asserts\\munition - Copy.png");
            
            Enemy = new PictureBox[10];

            for (int i = 0; i < Enemy.Length; i++)
            {
                Enemy[i] = new PictureBox();
                Enemy[i].Image = enemy;
                Enemy[i].Size = new Size(40, 40);
                Enemy[i].SizeMode = PictureBoxSizeMode.Zoom;
                Enemy[i].BorderStyle = BorderStyle.None;
                Enemy[i].Visible = false;
                this.Controls.Add(Enemy[i]);
                Enemy[i].Location = new Point((i + 1) * 50, -50);
            }


            for (int i = 0; i < Ammo.Length; i++)
            {
                Ammo[i] = new PictureBox();
                Ammo[i].Size = new Size(20, 20);
                Ammo[i].Image = munition;
                Ammo[i].SizeMode = PictureBoxSizeMode.Zoom;
                Ammo[i].BorderStyle = BorderStyle.None;
                this.Controls.Add(Ammo[i]);
            }


            gameMedia = new WindowsMediaPlayer(); 
            shootgMedia = new WindowsMediaPlayer();
            explosion = new WindowsMediaPlayer();

            gameMedia.URL = "songs\\GameSong.mp3";
            shootgMedia.URL = "songs\\shoot.mp3";
            explosion.URL = "songs\\boom.mp3";

            gameMedia.settings.setMode("loop", true);
            gameMedia.settings.volume = 5;
            shootgMedia.settings.volume = 1;
            explosion.settings.volume = 6;

            stars = new PictureBox[15];

            rand = new Random();
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle = BorderStyle.None;
                stars[i].Location = new Point(rand.Next(20, 580), rand.Next(-10, 400));

                if (i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }

                this.Controls.Add(stars[i]);
            }

            EnemyAmmo = new PictureBox[10];

            for(int i = 0; i < EnemyAmmo.Length; i++)
            {
                EnemyAmmo[i] = new PictureBox();
                EnemyAmmo[i].Size = new Size(20, 20);
                EnemyAmmo[i].Image = enemyAmmo;
                EnemyAmmo[i].SizeMode = PictureBoxSizeMode.Zoom;
                EnemyAmmo[i].BorderStyle = BorderStyle.None;
                EnemyAmmo[i].Visible = false;
                int x = rand.Next(0, 10);
                EnemyAmmo[i].Location = new Point(Enemy[x].Location.X, Enemy[x].Location.Y - 20);
                this.Controls.Add(EnemyAmmo[i]);
            }

            gameMedia.controls.play();
        }

        private void MoveBgTimer_Tick(object sender, EventArgs e)
        {
            for(int i = 0; i<stars.Length/2; i++) 
            {
                stars[i].Top += backgroundspeed; 

                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }

            for(int i = stars.Length / 2; i < stars.Length; i++)
            {
                stars[i].Top += backgroundspeed - 2;

                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
            {
                Player.Left -= playerSpeed;
            }
        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Right < 580)
            {
                Player.Left += playerSpeed;
            }
        }

        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < 400)
            {
                Player.Top += playerSpeed;
            }
        }

        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
            {
                Player.Top -= playerSpeed;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pause)
            {
                if (e.KeyCode == Keys.Right)
                {
                    RightMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Left)
                {
                    LeftMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Down)
                {
                    DownMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Up)
                {
                    UpMoveTimer.Start();
                }
            }
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            RightMoveTimer.Stop();
            LeftMoveTimer.Stop();
            DownMoveTimer.Stop();
            UpMoveTimer.Stop();

            if (e.KeyCode == Keys.Space)
            {
                if (!gameIsOver)
                {
                    if(pause)
                    {
                        StartTimers();
                        label1.Visible = false;
                        gameMedia.controls.play();
                        pause = false;
                    }
                    else
                    {
                        label1.Location = new Point(this.Width/2 - 120, 150);
                        label1.Text = "PAUSED";
                        label1.Visible = true;
                        gameMedia.controls.pause();
                        StopTimers();
                        pause = true;
                    }
                }
            }
        }

        private void MoveMunitionTimer_Tick(object sender, EventArgs e)
        {
            shootgMedia.controls.play();
            for (int i = 0; i < Ammo.Length; i++)
            {
                if (Ammo[i].Top > 0)
                {
                    Ammo[i].Visible = true;
                    Ammo[i].Top -= AmmoSpeed;

                    Collision();
                }
                else
                {
                    Ammo[i].Visible = false;
                    Ammo[i].Location = new Point(Player.Location.X + 35, Player.Location.Y - i * 30);
                }
            }
        }

        private void MoveEnemiesTimer_Tick(object sender, EventArgs e)
        {
            MoveEnemies(Enemy, EnemySpeed);
        }

        private void MoveEnemies(PictureBox[] array, int speed)
        {
            for(int i = 0;i < array.Length; i++)
            {
                array[i].Visible = true;
                array[i].Top += speed;

                if (array[i].Top > this.Height)
                {
                    array[i].Location = new Point((i + 1) * 50, -200);
                }
            }
        }

        private void Collision()
        {
            for (int i = 0; i < Enemy.Length; i++)
            {
                if (Ammo[0].Bounds.IntersectsWith(Enemy[i].Bounds) || Ammo[1].Bounds.IntersectsWith(Enemy[i].Bounds) ||
                        Ammo[2].Bounds.IntersectsWith(Enemy[i].Bounds))
                {
                    explosion.controls.play();

                    score += 1;
                    label2.Text = (score < 10) ? "0" + score.ToString() : score.ToString();
                    
                    if(score % 30 == 0)
                    {
                        if (level != 10)
                        {
                            level += 1;
                            label3.Text = (level < 10) ? "0" + level.ToString() : level.ToString(); 

                            if(EnemySpeed <= 10 && EnemyAmmoSpeed <= 10 && difficulty >= 0)
                            {
                                difficulty--;
                                EnemySpeed++;
                                EnemyAmmoSpeed++;

                            }
                        }

                    }

                    Enemy[i].Location = new Point((i + 1) * 50, -100);
                }

                if (Player.Bounds.IntersectsWith(Enemy[i].Bounds))
                {
                    explosion.settings.volume = 10;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("");
                }
            }
        }

        private void GameOver(string str)
        {
            label1.Text = str;
            label1.Visible = true;
            groupBox1.Visible = true;

            gameMedia.controls.stop();
            StopTimers();
        }
        private void StopTimers()
        {
            MoveBgTimer.Stop();
            MoveEnemiesTimer.Stop();
            MoveMunitionTimer.Stop();
            EnemiesMunationTimer.Stop();
        }

        private void StartTimers()
        {
            MoveBgTimer.Start();
            MoveEnemiesTimer.Start();
            MoveMunitionTimer.Start();
            EnemiesMunationTimer.Start();
        }

        private void EnemiesMunationTimer_Tick(object sender, EventArgs e)
        {
            for(int i = 0;i < EnemyAmmo.Length - difficulty; i++)
            {
                if (EnemyAmmo[i].Top < this.Height)
                {
                    EnemyAmmo[i].Visible = true;
                    EnemyAmmo[i].Top += EnemyAmmoSpeed;

                    CollisionWithEnemisMunation();
                }
                else
                {
                    EnemyAmmo[i].Visible = false;
                    int x = rand.Next(0, 10);
                    EnemyAmmo[i].Location = new Point(Enemy[x].Location.X + 20, Enemy[x].Location.Y + 30);
                }
            }
        }

        private void CollisionWithEnemisMunation()
        {
            for(int i = 0; i < EnemyAmmo.Length; i++)
            {
                if (EnemyAmmo[i].Bounds.IntersectsWith(Player.Bounds))
                {
                    EnemyAmmo[i].Visible = false;
                    explosion.settings.volume = 10;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeComponent();
            Form1_Load(e, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string playerName = textBox1.Text;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                string query = "INSERT INTO Leaderboard (Name, Score) VALUES (@Name, @Score)";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", playerName);
                    command.Parameters.AddWithValue("@Score", score);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            groupBox1.Visible = false;

            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            label7.Visible = true;
            button5.Visible = true;

            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                string query = "SELECT TOP 10 Name, Score FROM Leaderboard ORDER BY Score DESC";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    connection.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            label7.Visible = false;
            button5.Visible = false;

            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
        }
    }
}

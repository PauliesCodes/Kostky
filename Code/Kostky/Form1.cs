using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kostky
{
    public partial class Kostky : Form
    {
        private readonly HttpClient client = new HttpClient();
        private string apiUrl = "https://paulas-michal.mzf.cz/Kostky/positions.php"; // Změň na svůj server
        private int playerNumber = 1; // 1 nebo 2
        private PictureBox player1, player2;

        public Kostky()
        {
            InitializeComponent();
            InitializeGame();
            StartPositionUpdater();
        }

        private void InitializeGame()
        {

            //this.Width = 500;
            //this.Height = 300;

            player1 = new PictureBox
            {
                Size = new Size(50, 50),
                BackColor = Color.Blue,
                Location = new Point(100, 100)
            };

            player2 = new PictureBox
            {
                Size = new Size(50, 50),
                BackColor = Color.Red,
                Location = new Point(300, 100)
            };

            this.Controls.Add(player1);
            this.Controls.Add(player2);
            this.KeyDown += new KeyEventHandler(OnKeyDown);
        }

        private async void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (playerNumber == 1)
            {
                if (e.KeyCode == Keys.Left && player1.Left > 0) player1.Left -= 10;
                if (e.KeyCode == Keys.Right && player1.Right < this.ClientSize.Width) player1.Left += 10;
                await UpdatePosition("player1", player1.Left);
            }
            else if (playerNumber == 2)
            {
                if (e.KeyCode == Keys.Left && player2.Left > 0) player2.Left -= 10;
                if (e.KeyCode == Keys.Right && player2.Right < this.ClientSize.Width) player2.Left += 10;
                await UpdatePosition("player2", player2.Left);
            }
        }

        private async Task UpdatePosition(string player, int position)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { player, position }), Encoding.UTF8, "application/json");
            await client.PostAsync(apiUrl, content);
        }

        private async void StartPositionUpdater()
        {
            while (true)
            {
                await Task.Delay(100); // Počkej 100ms
                var response = await client.GetStringAsync(apiUrl);
                var positions = JsonSerializer.Deserialize<PositionData>(response);

                if (positions != null)
                {
                    player1.Left = positions.player1;
                    player2.Left = positions.player2;
                }
            }
        }
    }

    public class PositionData
    {
        public int player1 { get; set; }
        public int player2 { get; set; }
    }

}

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsFormsApp2
{
    public partial class Form3 : MaterialForm
    {
        private string _isim;
        private string _soyisim;
        private int _kullaniciId;
        private string _cinsiyet;
        private Timer otocikisTimer;
        private int remainingTime; 

        public Form3(string isim, string soyisim, int kullaniciId, string cinsiyet)
        {
            InitializeComponent();

            _isim = isim;
            _soyisim = soyisim;
            _kullaniciId = kullaniciId;
            _cinsiyet = cinsiyet;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal900, Primary.Teal500, Accent.Teal200, TextShade.WHITE);
            this.FormClosed += Form3_FormClosed;

            
            otocikisTimer = new Timer();
            otocikisTimer.Interval = 1000; 
            otocikisTimer.Tick += OtocikisTimer_Tick;

            remainingTime = 600; 
            otocikisTimer.Start(); 
        }

        private void Form3_Load(object sender, EventArgs e) //karşılama mesajı
        {
            if (_cinsiyet.ToLower() == "erkek")
            {
                materialLabel1.Text = $"Hoşgeldiniz, {_isim} Bey! Ne yapmak istiyorsunuz?";
            }
            else if (_cinsiyet.ToLower() == "kadın")
            {
                materialLabel1.Text = $"Hoşgeldiniz, {_isim} Hanım! Ne yapmak istiyorsunuz?";
            }
            else
            {
                materialLabel1.Text = $"Hoşgeldiniz, {_isim} {_soyisim}! Ne yapmak istiyorsunuz?";
            }
        }

        
        private void OtocikisTimer_Tick(object sender, EventArgs e) //10 dakikalık otomatik çıkış timerı
        {
            remainingTime--; 

            int dakika = remainingTime / 60;
            int saniye = remainingTime % 60;

            
            Label timeLabel = (Label)this.Controls["timeLabel"];
            timeLabel.Text = $"Kalan Süre: {dakika:D2}:{saniye:D2}";

            if (remainingTime <= 0)
            {
                otocikisTimer.Stop(); 

                this.Close();

                Nakemu girisFormu = new Nakemu();
                girisFormu.Show();
            }
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            otocikisTimer.Stop();
        }

        private void materialButton1_Click(object sender, EventArgs e) //odunc menusune kullanıcı datası yollama ve açma
        {
            Form7 odunc = new Form7(_isim, _soyisim, _kullaniciId, _cinsiyet);
            odunc.Show();
            this.Hide();
        }

        private void materialButton2_Click(object sender, EventArgs e) //iade menusune kullanıcı datası yollama ve açma
        {
            Form8 iade = new Form8(_isim, _soyisim, _kullaniciId, _cinsiyet);
            iade.Show();
            this.Hide();
        }

        private void materialButton3_Click(object sender, EventArgs e) //geri tuşu
        {
            this.Close();

            Nakemu girisFormu = new Nakemu();
            girisFormu.Show();
        }
    }
}

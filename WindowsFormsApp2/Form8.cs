using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsFormsApp2
{
    public partial class Form8 : MaterialForm
    {
        private string _isim;
        private string _soyisim;
        private int _kullaniciId;
        private string _cinsiyet;
        private string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";

        public Form8(string isim, string soyisim, int kullaniciId, string cinsiyet)
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
        }

        private void Form8_Load(object sender, EventArgs e) //flowlayoutpanel kaydırma ayarı
        {
            odunckitapyükle();
            flowLayoutPanel1.AutoScroll = true; 
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight; 
            flowLayoutPanel1.WrapContents = false; 
        }

        private void odunckitapyükle()//alınmış kitapları yükleme
        {
            string query = "SELECT o.IslemID, k.KitapAdi, k.resim, o.IadeTarihi FROM odunc o INNER JOIN kitaplar1 k ON o.KitapID = k.KitapID WHERE o.KullaniciID = @KullaniciID AND o.Durum = 'Alındı'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@KullaniciID", _kullaniciId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int islemId = (int)reader["IslemID"];
                            string kitapAdi = reader["KitapAdi"].ToString();
                            DateTime iadeTarihi = (DateTime)reader["IadeTarihi"];
                            TimeSpan kalanSure = iadeTarihi - DateTime.Now;
                            byte[] resimVeri = reader["resim"] as byte[];

                            Panel kitapPanel = ödüncpaneli(kitapAdi, kalanSure, islemId, resimVeri);
                            flowLayoutPanel1.Controls.Add(kitapPanel);
                        }
                    }
                }
            }
        }

        private Panel ödüncpaneli(string kitapAdi, TimeSpan kalanSure, int islemId, byte[] resimVeri) //ödünc alınan kitap kartlarını olusturma
        {
            Panel panel = new Panel
            {
                Width = 400,
                Height = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = islemId
            };

            PictureBox pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 200,
                Height = 220
            };

            if (resimVeri != null)
            {
                using (MemoryStream ms = new MemoryStream(resimVeri))
                {
                    pictureBox.Image = Image.FromStream(ms);
                }
            }

            Label lblKitapAdi = new Label
            {
                Text = $"Kitap: {kitapAdi}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Dock = DockStyle.Bottom
            };

            Label lblKalanSure = new Label
            {
                Text = $"Kalan Süre: {(kalanSure.TotalDays > 0 ? $"{Math.Ceiling(kalanSure.TotalDays)} gün" : "Süresi dolmuş!")} ",
                Font = new Font("Arial", 10),
                Dock = DockStyle.Bottom
            };

            Button btnTeslimEt = new Button
            {
                Text = "Teslim Et",
                Dock = DockStyle.Bottom,
                Height = 30
            };

            btnTeslimEt.Click += (s, e) => TeslimEt(islemId);

            panel.Controls.Add(pictureBox); 
            panel.Controls.Add(lblKitapAdi); 
            panel.Controls.Add(lblKalanSure); 
            panel.Controls.Add(btnTeslimEt); 

            return panel;
        }

        private void TeslimEt(int islemId) //teslim etme tuşu 
        {
            string query = "UPDATE odunc SET Durum = 'Teslim Edildi' WHERE IslemID = @IslemID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IslemID", islemId);
                    cmd.ExecuteNonQuery();
                }
            }

            MaterialMessageBox.Show("Kitap başarıyla teslim edildi.", "Teslim Alındı!", MessageBoxButtons.OK, true);
            flowLayoutPanel1.Controls.Clear();
            odunckitapyükle();
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            Form3 iade = new Form3(_isim, _soyisim, _kullaniciId,_cinsiyet);
            iade.Show();
            this.Hide();
        }

        private void materialButton1_Click_1(object sender, EventArgs e)
        {
            Form3 iade = new Form3(_isim, _soyisim, _kullaniciId,_cinsiyet);
            iade.Show();
            this.Hide();
        }
    }
}

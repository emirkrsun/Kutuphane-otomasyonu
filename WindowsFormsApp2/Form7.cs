using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Collections.Generic;

namespace WindowsFormsApp2
{
    public partial class Form7 : MaterialForm
    {
        private string _isim;
        private string _soyisim;
        private int _kullaniciId;
        private string _cinsiyet;
        private string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";
        private HashSet<int> eklenenKitaplar; 

        public Form7(string isim, string soyisim, int kullaniciId, string cinsiyet)
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

            eklenenKitaplar = new HashSet<int>(); 
        }

        private void Form7_Load(object sender, EventArgs e)//flow layout panel kaydırma ayarı
        {
            kitapyükleme();
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = false;
        }

        private void kitapyükleme() //alınmamıs kitapların datasını sqlden cekme
        {
            string query = @"
    SELECT DISTINCT k.KitapID, k.KitapAdi, k.Yazar, k.resim
    FROM kitaplar1 k
    LEFT JOIN odunc o ON k.KitapID = o.KitapID
    WHERE k.KitapID NOT IN (
        SELECT KitapID 
        FROM odunc 
        WHERE Durum = 'Alındı'
    )";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int kitapId = (int)reader["KitapID"];

                            if (eklenenKitaplar.Contains(kitapId))
                            {
                                continue;
                            }

                            string kitapAdi = reader["KitapAdi"].ToString();
                            string yazar = reader["Yazar"].ToString();
                            byte[] resimVerisi = reader["resim"] as byte[];

                            Panel kitapKart = kitapkartıolusturma(kitapAdi, yazar, resimVerisi, kitapId);
                            flowLayoutPanel1.Controls.Add(kitapKart);

                            eklenenKitaplar.Add(kitapId);
                        }
                    }
                }
            }
        }


        private Panel kitapkartıolusturma(string kitapAdi, string yazar, byte[] resimVerisi, int kitapId) //kitap kartlarını oluşturma
        {
            Panel panel = new Panel
            {
                Width = 200,
                Height = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = kitapId
            };

            PictureBox pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 200,
                Height = 220
            };

            if (resimVerisi != null)
            {
                using (MemoryStream ms = new MemoryStream(resimVerisi))
                {
                    pictureBox.Image = Image.FromStream(ms);
                }
            }

            Label lblKitapAdi = new Label
            {
                Text = kitapAdi,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom
            };

            Label lblYazar = new Label
            {
                Text = yazar,
                Font = new Font("Arial", 10),
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom
            };

            Button btnOduncAl = new Button
            {
                Text = "Ödünç Al",
                Dock = DockStyle.Bottom,
                Height = 30
            };

            btnOduncAl.Click += (s, e) => OduncAl(panel, kitapId);

            panel.Controls.Add(lblYazar);
            panel.Controls.Add(lblKitapAdi);
            panel.Controls.Add(pictureBox);
            panel.Controls.Add(btnOduncAl);

            return panel;
        }

        private void OduncAl(Panel kitapKart, int kitapId)//ödünc alınan kitapları sqle kaydetme
        {
            string query = @"
            SELECT COUNT(*)
            FROM odunc
            WHERE KitapID = @KitapID
            AND Durum = 'Alındı'
            AND IadeTarihi IS NULL";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@KitapID", kitapId);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Bu kitap şu anda başka bir kullanıcı tarafından ödünç alınmış. Lütfen iade edilmesini bekleyin.");
                    }
                    else
                    {
                        string insertQuery = @"
                        INSERT INTO odunc (KullaniciID, KitapID, AlisTarihi, IadeTarihi, Durum)
                        VALUES (@KullaniciID, @KitapID, @AlisTarihi, @IadeTarihi, @Durum)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@KullaniciID", _kullaniciId);
                            insertCmd.Parameters.AddWithValue("@KitapID", kitapId);
                            insertCmd.Parameters.AddWithValue("@AlisTarihi", DateTime.Now);
                            insertCmd.Parameters.AddWithValue("@IadeTarihi", DateTime.Now.AddDays(14));
                            insertCmd.Parameters.AddWithValue("@Durum", "Alındı");

                            insertCmd.ExecuteNonQuery();
                        }

                        flowLayoutPanel1.Controls.Remove(kitapKart);

                        MaterialMessageBox.Show("Kitap başarıyla ödünç alındı.", "Ödünç Alındı!", MessageBoxButtons.OK, true);
                    }
                }
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            Form3 kullanıcı = new Form3(_isim, _soyisim, _kullaniciId, _cinsiyet);
            kullanıcı.Show();
            this.Hide();
        }

        private void materialButton1_Click_1(object sender, EventArgs e)
        {
            Form3 kullanıcı = new Form3(_isim, _soyisim, _kullaniciId,_cinsiyet);
            kullanıcı.Show();
            this.Hide();
        }
    }
}

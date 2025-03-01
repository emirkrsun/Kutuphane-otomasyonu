using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsFormsApp2
{
    public partial class Form5 : MaterialForm
    {
        public Form5()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal900, Primary.Teal500, Accent.Teal200, TextShade.WHITE);
        }

        private void materialButton2_Click(object sender, EventArgs e)//SQL' e kitap ekleme
        {
            string kitapadi = materialTextBox1.Text;
            string yazaradi = materialTextBox2.Text;
            string sayfasayısıStr = materialTextBox3.Text;

            if (!int.TryParse(sayfasayısıStr, out int sayfaSayisi))
            {
                MaterialMessageBox.Show("Geçersiz sayfa sayısı.", "Hata", MessageBoxButtons.OK, true);
                return;
            }

            string dosyaYolu = materialTextBox4.Text; 
            if (!File.Exists(dosyaYolu))
            {
                MaterialMessageBox.Show("Geçerli bir resim dosyası seçin.", "Hata", MessageBoxButtons.OK, true);
                return;
            }

            byte[] resim = File.ReadAllBytes(dosyaYolu);

            string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO kitaplar1 (KitapAdi, Yazar, SayfaSayisi, resim) " +
                               "VALUES (@KitapAdi, @Yazar, @SayfaSayisi, @resim)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KitapAdi", kitapadi);
                    command.Parameters.AddWithValue("@Yazar", yazaradi);
                    command.Parameters.AddWithValue("@SayfaSayisi", sayfaSayisi);
                    command.Parameters.AddWithValue("@Resim", resim);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            MaterialMessageBox.Show("Kitap başarıyla kaydedildi.", "Kaydedildi!", MessageBoxButtons.OK, true);

            Form4 admin = new Form4();
            admin.Show();
            this.Close(); 
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            Form4 admin = new Form4();
            admin.Show();
            this.Close();
        }

        private void materialButton1_Click_1(object sender, EventArgs e) //gözat butonu
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog1.Title = "Bir Resim Dosyası Seçin";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string dosyaYolu = openFileDialog1.FileName;
                        pictureBox1.Image = new Bitmap(dosyaYolu);
                        materialTextBox4.Text = dosyaYolu;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Resim yüklenirken bir hata oluştu: " + ex.Message);
                    }
                }
            }
        }

        private void materialButton3_Click_1(object sender, EventArgs e)
        {
            Form4 admin = new Form4();
            admin.Show();
            this.Close(); 
        }
    }
}
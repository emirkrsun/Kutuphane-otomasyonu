using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsFormsApp2
{
    public partial class Form6 : MaterialForm
    {
        private string kitapID;
        private string resimYolu;

        public Form6(string id, string kitapAdi, string yazar, string sayfaSayisi, string resimYolu)
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal900, Primary.Teal500, Accent.Teal200, TextShade.WHITE);

            this.kitapID = id;
            this.resimYolu = resimYolu;

            materialTextBox1.Text = kitapAdi;
            materialTextBox2.Text = yazar;
            materialTextBox3.Text = sayfaSayisi;
        }

        private void Form6_Load(object sender, EventArgs e) //form 5 in gonderdigi kitapIDden resim çekme 
        {
            string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Resim FROM kitaplar1 WHERE KitapID = @KitapID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KitapID", kitapID);

                    try
                    {
                        connection.Open();

                        byte[] resimData = command.ExecuteScalar() as byte[];

                        if (resimData != null && resimData.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(resimData))
                            {
                                Image img = Image.FromStream(ms);
                                pictureBox1.Image = img;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Resim verisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void materialButton2_Click(object sender, EventArgs e)//SQL kitap güncelleme
        {
            string kitapAdi = materialTextBox1.Text;
            string yazar = materialTextBox2.Text;
            string sayfaSayisi = materialTextBox3.Text;

            if (string.IsNullOrWhiteSpace(kitapAdi) || string.IsNullOrWhiteSpace(yazar) || string.IsNullOrWhiteSpace(sayfaSayisi))
            {
                MessageBox.Show("Tüm alanları doldurduğunuzdan emin olun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(sayfaSayisi, out int sayfaSayisiInt))
            {
                MessageBox.Show("Sayfa sayısı geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] resimByteArray = null;
            if (!string.IsNullOrWhiteSpace(resimYolu) && File.Exists(resimYolu))
            {
                resimByteArray = File.ReadAllBytes(resimYolu);
            }

            string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query;

                    if (resimByteArray != null)
                    {
                        query = "UPDATE kitaplar1 SET KitapAdi = @KitapAdi, Yazar = @Yazar, SayfaSayisi = @SayfaSayisi, Resim = @Resim WHERE KitapID = @KitapID";
                    }
                    else
                    {
                        query = "UPDATE kitaplar1 SET KitapAdi = @KitapAdi, Yazar = @Yazar, SayfaSayisi = @SayfaSayisi WHERE KitapID = @KitapID";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KitapID", kitapID);
                        command.Parameters.AddWithValue("@KitapAdi", kitapAdi);
                        command.Parameters.AddWithValue("@Yazar", yazar);
                        command.Parameters.AddWithValue("@SayfaSayisi", sayfaSayisiInt);

                        if (resimByteArray != null)
                        {
                            command.Parameters.AddWithValue("@Resim", resimByteArray);
                        }

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MaterialMessageBox.Show("Kitap başarıyla güncellendi.", "Başarılı!", MessageBoxButtons.OK, true);
                Form4 form4 = new Form4();
                form4.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void materialButton3_Click_1(object sender, EventArgs e)
        {
            Form4 geriDön = new Form4();
            geriDön.Show();
            this.Hide();
        }

        private void materialButton1_Click_1(object sender, EventArgs e) //gözat butonu
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                resimYolu = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(resimYolu);
            }
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            Form4 geriDön = new Form4();
            geriDön.Show();
            this.Hide();
        }
    }
}

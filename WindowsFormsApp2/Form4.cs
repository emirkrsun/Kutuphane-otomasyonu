using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsFormsApp2
{
    public partial class Form4 : MaterialForm
    {
        public Form4()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal900, Primary.Teal500, Accent.Teal200, TextShade.WHITE);
            this.FormClosed += Form4_FormClosed;
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            Nakemu girisFormu = new Nakemu();
            girisFormu.Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            kitapyükle();
        }

        private void kitapyükle() //SQL den kitap datası çekme ve listboxa sıralama
        {
            materialListBox1.Items.Clear();
            string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT k.KitapID, k.KitapAdi, k.Yazar, k.SayfaSayisi, k.Resim, 
                           u.İsim, u.Soyisim, o.IadeTarihi 
                    FROM kitaplar1 k
                    LEFT JOIN odunc o ON k.KitapID = o.KitapID AND o.Durum = 'Alındı'
                    LEFT JOIN Kullanıcı u ON o.KullaniciID = u.ID";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string kitapAdi = reader["KitapAdi"].ToString();
                    string yazarAdi = reader["Yazar"].ToString();
                    string sayfaSayisi = reader["SayfaSayisi"].ToString();
                    string kitapID = reader["KitapID"].ToString();
                    string resimYolu = reader["Resim"].ToString();
                    string isim = reader["İsim"].ToString();
                    string soyisim = reader["Soyisim"].ToString();
                    DateTime? iadeTarihi = reader["IadeTarihi"] as DateTime?;

                    string kalanSure = iadeTarihi.HasValue ? $"Kalan Süre: {Math.Ceiling((iadeTarihi.Value - DateTime.Now).TotalDays)} gün" : "Teslim Edildi";

                    string listItem = $"{kitapAdi} - {yazarAdi} ({sayfaSayisi} sayfa) " +
                                      (string.IsNullOrEmpty(isim) ? "" : $"- Ödünç Alan: {isim} {soyisim} - {kalanSure}");

                    MaterialListBoxItem item = new MaterialListBoxItem(listItem, kitapID);
                    item.Tag = kitapID;

                    materialListBox1.Items.Add(item);
                }
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            Form5 ekleform = new Form5();
            ekleform.Show();
            this.Hide();
        }

        private void materialButton2_Click(object sender, EventArgs e) //SQL den kitap silme
        {
            if (materialListBox1.SelectedItem != null)
            {
                MaterialListBoxItem selectedItem = (MaterialListBoxItem)materialListBox1.SelectedItem;
                string kitapID = selectedItem.Tag.ToString();

                string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string deleteOduncQuery = "DELETE FROM odunc WHERE KitapID = @KitapID";
                        using (SqlCommand deleteOduncCommand = new SqlCommand(deleteOduncQuery, connection))
                        {
                            deleteOduncCommand.Parameters.AddWithValue("@KitapID", kitapID);
                            deleteOduncCommand.ExecuteNonQuery();
                        }

                        string deleteKitapQuery = "DELETE FROM kitaplar1 WHERE KitapID = @KitapID";
                        using (SqlCommand deleteKitapCommand = new SqlCommand(deleteKitapQuery, connection))
                        {
                            deleteKitapCommand.Parameters.AddWithValue("@KitapID", kitapID);
                            deleteKitapCommand.ExecuteNonQuery();
                        }
                    }

                    MaterialMessageBox.Show("Kitap başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, true);
                    kitapyükle();
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, true);
                }
            }
            else
            {
                MaterialMessageBox.Show("Silmek için bir kitap seçin.", "Hata", MessageBoxButtons.OK, true);
            }
        }


        private void materialButton3_Click(object sender, EventArgs e)//kitap güncelleme formunu açma ve kitap datasını yollama
        {
            if (materialListBox1.SelectedItem != null)
            {
                MaterialListBoxItem selectedItem = (MaterialListBoxItem)materialListBox1.SelectedItem;
                string kitapID = selectedItem.Tag.ToString();
                string kitapAdi = selectedItem.Text.Split('-')[0].Trim();
                string yazarAdi = selectedItem.Text.Split('-')[1].Split('(')[0].Trim();
                string sayfaSayisi = selectedItem.Text.Split('(')[1].Split(' ')[0].Trim();
                string resimYolu = "";

                Form6 guncelle = new Form6(kitapID, kitapAdi, yazarAdi, sayfaSayisi, resimYolu);
                guncelle.Show();
                this.Hide();
            }
            else
            {
                MaterialMessageBox.Show("Güncellemek için bir kitap seçin.", "Hata", MessageBoxButtons.OK, true);
            }
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            this.Close();
            Nakemu girisFormu = new Nakemu();
            girisFormu.Show();
        }

        private void materialListBox1_SelectedIndexChanged(object sender, MaterialListBoxItem selectedItem) //kitap seçili değilse sil ve güncelle butonunu kapatma
        {
            bool secilenkitap = materialListBox1.SelectedItem != null;
            materialButton2.Enabled = secilenkitap;
            materialButton3.Enabled = secilenkitap;
        }

       
    }
}

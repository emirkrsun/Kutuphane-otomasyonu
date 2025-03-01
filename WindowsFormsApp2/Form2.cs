using System;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsFormsApp2
{
    public partial class Form2 : MaterialForm
    {
        public Form2()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal900, Primary.Teal500, Accent.Teal200, TextShade.WHITE);
        }

        private bool KullaniciAdiKontrol(string kullaniciAdi, string connectionString) //kullanıcı adı kullanılıyor mu?
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Kullanıcı WHERE nickname = @nickname";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nickname", kullaniciAdi);
                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true; 
                    }
                }
            }
        }

        private bool epostaKontrol(string eposta, string connectionString) //eposta kullanılıyor mu?
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Kullanıcı WHERE email = @email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", eposta);
                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                }
            }
        }

        private bool telnoKontrol(string telno, string connectionString) //tel no kullanılıyor mu?
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Kullanıcı WHERE Telefon = @Telefon";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Telefon", telno);
                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                }
            }
        }

        private bool SifreDogrula(string sifre) //şifre büyük kücük harf dogrulaması
        {
            if (sifre.Length < 8)
                return false;

            bool buyukHarf = false;
            bool kucukHarf = false;
            bool rakam = false;
            bool ozelKarakter = false;

            foreach (char c in sifre)
            {
                if (char.IsUpper(c))
                    buyukHarf = true;
                else if (char.IsLower(c))
                    kucukHarf = true;
                else if (char.IsDigit(c))
                    rakam = true;
                else
                    ozelKarakter = true;
            }

            return buyukHarf && kucukHarf && rakam && ozelKarakter;
        }

        private void materialButton1_Click(object sender, EventArgs e)//boş alan kontrolu
        {
            string kullaniciAdi = kullanıcıadi.Text;
            string sifre = parola.Text;
            string sifreDogrulama = resifre.Text;
            string ad = isim.Text;
            string soyisim = soyad.Text;
            string eposta = email.Text;
            string telno = telefon.Text;

            string cinsiyet = "";
            if (erkekradio.Checked)
            {
                cinsiyet = "Erkek";
            }
            else if (kadınradio.Checked)
            {
                cinsiyet = "Kadın";
            }

            if (string.IsNullOrEmpty(cinsiyet))
            {
                MaterialMessageBox.Show(
                 "Lütfen bir cinsiyet seçiniz.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (string.IsNullOrEmpty(ad))
            {
                MaterialMessageBox.Show(
                 "Lütfen isminizi giriniz.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (string.IsNullOrEmpty(soyisim))
            {
                MaterialMessageBox.Show(
                 "Lütfen soyisminizi giriniz.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (string.IsNullOrEmpty(eposta))
            {
                MaterialMessageBox.Show(
                 "Lütfen epostanızı giriniz.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (string.IsNullOrEmpty(telno))
            {
                MaterialMessageBox.Show(
                 "Lütfen telefon numaranızı giriniz.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                MaterialMessageBox.Show(
                 "Lütfen bir kullanıcı adı giriniz.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                MaterialMessageBox.Show(
                 "Lütfen bir kullanıcı adı giriniz.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (sifre != sifreDogrulama)
            {
                MaterialMessageBox.Show(
                 "Şifre eşleşmiyor.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            if (!SifreDogrula(sifre))
            {
                MaterialMessageBox.Show(
                 "Şifre en az 8 karakter olmalı ve büyük harf, küçük harf, rakam ve özel karakter içermelidir.",
                 "UYARI!",
                 MessageBoxButtons.OK,
                 true
                 );
                return;
            }

            string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";

            if (KullaniciAdiKontrol(kullaniciAdi, connectionString))
            {
                MaterialMessageBox.Show("Bu kullanıcı adı zaten kullanılıyor. Lütfen başka bir kullanıcı adı seçin.", "UYARI!", MessageBoxButtons.OK, true);
                return;
            }

            if (epostaKontrol(eposta, connectionString))
            {
                MaterialMessageBox.Show("Bu e-posta zaten kullanılıyor. Lütfen başka bir e-posta seçin.", "UYARI!", MessageBoxButtons.OK, true);
                return;
            }

            if (telnoKontrol(telno, connectionString))
            {
                MaterialMessageBox.Show("Bu telefon numarası zaten kullanılıyor. Lütfen başka bir telefon numarası seçin.", "UYARI!", MessageBoxButtons.OK, true);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString)) //SQL hesap kaydı
            {
                string query = "INSERT INTO Kullanıcı (nickname, şifre, İsim, Soyisim, email, Telefon, Cinsiyet) " +
                               "VALUES (@nickname, @şifre, @İsim, @Soyisim, @email, @Telefon, @Cinsiyet)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nickname", kullaniciAdi);
                    command.Parameters.AddWithValue("@şifre", sifre);
                    command.Parameters.AddWithValue("@İsim", ad);
                    command.Parameters.AddWithValue("@Soyisim", soyisim);
                    command.Parameters.AddWithValue("@email", eposta);
                    command.Parameters.AddWithValue("@Telefon", telno);
                    command.Parameters.AddWithValue("@Cinsiyet", cinsiyet);

                    try
                    {
                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MaterialMessageBox.Show(
                             "Kayıt başarılı.",
                             "Başarılı",
                             MessageBoxButtons.OK,
                             true
                             );
                            Nakemu girisFormu = new Nakemu();
                            girisFormu.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Kayıt başarısız.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veritabanı hatası: " + ex.Message);
                    }
                }
            }
        }


        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e) //şifreyi göster checkboxu
        {
            bool showPassword = materialCheckbox1.Checked;

            parola.Password = !showPassword;
            parola.Refresh(); 

            resifre.Password = !showPassword;
            resifre.Refresh(); 
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Nakemu girisFormu = new Nakemu();
            girisFormu.Show();
            girisFormu.Invalidate(); 
            girisFormu.Refresh();    
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            Nakemu girisFormu = new Nakemu();
            girisFormu.Show();
            girisFormu.Invalidate();
            girisFormu.Refresh();
            this.Close();
        }
    }
}       
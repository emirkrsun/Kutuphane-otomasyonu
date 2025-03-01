using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsFormsApp2
{
    public partial class Nakemu : MaterialForm
    {
        public Nakemu()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal900, Primary.Teal500, Accent.Teal200, TextShade.WHITE);
        }

        private void materialButton1_Click(object sender, EventArgs e) //sqlden giriş doğrulama
        {
            string nickname = gelenad.Text;
            string sifre = gelensifre.Text;

            string connectionString = "Data Source=SYLVIA\\SQLEXPRESS;Initial Catalog=Nakemu Kütüphanesi;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT İsim, Soyisim, ID, Cinsiyet FROM Kullanıcı WHERE nickname = @nickname AND şifre = @sifre";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nickname", nickname);
                    command.Parameters.AddWithValue("@sifre", sifre);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string isim = reader["İsim"].ToString();
                                string soyisim = reader["Soyisim"].ToString();
                                int ID = Convert.ToInt32(reader["ID"]);
                                string cinsiyet = reader["Cinsiyet"].ToString();


                                if (nickname.ToLower() == "admin")
                                {
                                    MaterialMessageBox.Show("Admin girişi başarılı!", "Başarılı!", MessageBoxButtons.OK, true);
                                    Form4 adminForm = new Form4();
                                    adminForm.Show();
                                }
                                else
                                {
                                    MaterialMessageBox.Show("Giriş başarılı!", "Başarılı!", MessageBoxButtons.OK, true);
                                    Form3 userForm = new Form3(isim, soyisim,ID,cinsiyet);
                                    userForm.Show();
                                }

                                this.Hide();
                            }
                            else
                            {
                                MaterialMessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Hata!", MessageBoxButtons.OK, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veritabanı hatası: " + ex.Message);
                    }
                }
            }
        }

        private void materialButton3_Click(object sender, EventArgs e) //kayıt formu
        {
            Form2 yeniForm = new Form2();
            yeniForm.FormClosed += Form2_FormClosed;
            yeniForm.Show();
            this.Hide();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }

        private void Nakemu_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e) //şifreyi göster butonu
        {
            bool showPassword = materialCheckbox1.Checked;

            gelensifre.Password = !showPassword;
            gelensifre.Refresh();
        }
    }
}

using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StokSayim
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtKullaniciAdi.EditValue != null && txtSifre.EditValue != null)
            {
                login();
            }
            else
                XtraMessageBox.Show("Lütfen Kullanıcı Adı ve Şifre Giriniz.",
                         "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void login()
        {
            MainMenu frm = new MainMenu();
            this.Hide();
            frm.ShowDialog();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtKullaniciAdi.Text = "admin";
            txtSifre.Text = "admin";
        }
    }
}

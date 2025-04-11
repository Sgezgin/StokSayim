using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using StokSayim.Models;
using StokSayim.Data.Repositories;

namespace StokSayim.Moduller.Personel
{
    public partial class PersonelEkleDuzenle : DevExpress.XtraEditors.XtraUserControl
    {
        private Models.Personel _personel;
        private PersonelRepository _personelRepository;

        public PersonelEkleDuzenle(Models.Personel pers, PersonelRepository personelRepository)
        {
            InitializeComponent();
            _personel = pers;
            _personelRepository = personelRepository;
        }

    

        private void PersonelEkleDuzenle_Load(object sender, EventArgs e)
        {
            if (_personel != null)
            {
                // Düzenleme modu
                txtAdi.Text = _personel.Adi;
                txtSoyadi.Text = _personel.Soyadi;
                txtTckn.Text = _personel.TcNo;
                radioPersTip.EditValue = _personel.Tip;
                labelControl.Text = "Personel Bilgi Düzenle";
            }
            else
            {
                // Ekleme modu
                radioPersTip.SelectedIndex = 0;
                labelControl.Text = "Yeni Personel Ekle";
                _personel = new Models.Personel();
            }
        }

   

        private void windowsUIButtonPanel_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var caption = e.Button.Properties.Caption;     
            if (caption == "Kaydet") Kaydet();
            else if (caption == "Sil") Sil();
            else if (caption == "Kapat") (this.Parent as Form).DialogResult = DialogResult.No;
        }

        private void Kaydet()
        {
            try
            {

                _personel.Adi = txtAdi.Text.Trim();
                _personel.Soyadi = txtSoyadi.Text.Trim();
                _personel.Tip = Convert.ToInt32(radioPersTip.EditValue);

                if (!string.IsNullOrEmpty(_personel.TcNo))
                {
                    _personel.Duzenleyen = "admin";
                    _personel.EklemeTarih = DateTime.Now;
                    _personelRepository.Update(_personel);
                    MessageBox.Show("Personel Bilgisi Güncellendi.", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    _personel.TcNo = txtTckn.Text.Trim();
                    _personel.Ekleyen = "admin";
                    _personel.EklemeTarih = DateTime.Now;
                    _personelRepository.Add(_personel);
                    MessageBox.Show("Yeni Personel Eklendi.", "Bilgi",
                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

              
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void Sil()
        {

        }
    }
}

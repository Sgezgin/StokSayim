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
using StokSayim.Data.Repositories;
using StokSayim.Models;

namespace StokSayim.Moduller.Marka
{
    public partial class MarkaEkleDuzenle : DevExpress.XtraEditors.XtraUserControl
    {
        private Brand _brand;
        private BrandRepository _brandRepository;

        public MarkaEkleDuzenle(Brand brand, BrandRepository brandRepository)
        {
            InitializeComponent();
            _brand = brand;
            _brandRepository = brandRepository; // Repository'yi burada ata

        }

        private void windowsUIButtonPanel_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var caption = e.Button.Properties.Caption;
            if (caption == "Çıkış") (this.Parent as Form).DialogResult = DialogResult.No;
            else if (caption == "Kaydet") Kaydet();
        }

     

        private void MarkaEkleDuzenle_Load(object sender, EventArgs e)
        {
            if (_brand != null)
            {
                // Düzenleme modu
                txtmarkaKodu.Text = _brand.BrandCode;
                txtMarkaAdi.Text = _brand.BrandName;

                labelControl.Text = "Marka Düzenle";
            }
            else
            {
                // Ekleme modu
                labelControl.Text = "Yeni Marka Ekle";
                _brand = new Brand();
            }
        }

        private void Kaydet()
        {
            if (string.IsNullOrWhiteSpace(txtmarkaKodu.Text) ||
        string.IsNullOrWhiteSpace(txtMarkaAdi.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Form değerlerini modele aktar
                _brand.BrandCode = txtmarkaKodu.Text;
                _brand.BrandName = txtMarkaAdi.Text;

                if (_brand.BrandID == 0)
                {
                    // Yeni marka
                    _brandRepository.Add(_brand);        

                }
                else
                {
                    // Mevcut marka
                    //_brand.UpdatedDate = DateTime.Now;
                    _brandRepository.Update(_brand);
                }

       
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme işlemi sırasında hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

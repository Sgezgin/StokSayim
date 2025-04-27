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
using System.Data.Entity.Validation;

namespace StokSayim.Moduller.Musteriler
{
    public partial class MusteriEkleDuzenle : DevExpress.XtraEditors.XtraUserControl
    {
        private Models.Store _musteri;
        private StoreRepository _storeRepository;
        private BrandRepository _brandRepository;
        private BaseRepository<IlTanim> _ilRepository;

        private List<Brand> brandList = new List<Brand>();

        public MusteriEkleDuzenle(Models.Store musteri, StoreRepository storeRepository)
        {
            InitializeComponent();
            _musteri = musteri;
            _storeRepository = storeRepository;
            _brandRepository = Global.BrandRepository;
            _ilRepository = new BaseRepository<IlTanim>(Global.DbContext);
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
                if(lueBrandID.EditValue == null)
                {
                    MessageBox.Show("Marka Seçiniz.", "Uyarı",
               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(lueil.EditValue == null)
                {
                    MessageBox.Show("İl Seçiniz.", "Uyarı",
             MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _musteri.Adres = memAdres.Text.Trim();
                _musteri.BrandID = Convert.ToInt32(lueBrandID.EditValue);
                _musteri.Telefon = txtTelefon.Text;
                _musteri.StoreName = txtStoreName.Text;
                _musteri.StoreCode = "";
                _musteri.OzelKod3 = txtOzelKod3.Text;
                _musteri.OzelKod2 = txtOzelKod2.Text;
                _musteri.OzelKod1 = txtOzelKod1.Text;
                _musteri.IlId = Convert.ToInt32(lueil.EditValue);
                _musteri.IlceAdi = txtIlce.Text;
                _musteri.Gsm = txtGsm.Text;
                _musteri.Email = txtEmail.Text;
                _musteri.IlgiliKisi = txtIlgiliKisi.Text;
               

                if (_musteri.StoreID > 0)
                {         
                    _musteri.GuncellemeTarih = DateTime.Now;
                    _storeRepository.Update(_musteri);
                    MessageBox.Show("Müşteri Bilgisi Güncellendi.", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {                
                  
                    _musteri.EklemeTarih = DateTime.Now;
                    _storeRepository.Add(_musteri);
                    MessageBox.Show("Yeni Müşteri Eklendi.", "Bilgi",
                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


            }
            catch (DbEntityValidationException dbEx)
            {
                // Validation hatalarını yakala
                StringBuilder errorMessages = new StringBuilder();

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessages.AppendLine($"Özellik: {validationError.PropertyName} - Hata: {validationError.ErrorMessage}");
                    }
                }

                MessageBox.Show($"Doğrulama hatası oluştu:\n{errorMessages}", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Hatayı loglama
                Console.WriteLine(errorMessages.ToString());
            }
            catch (Exception ex)
            {
                // Diğer tüm hatalar için
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

                // İç hataları da kontrol et
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"İç hata detayı: {ex.InnerException.Message}", "İç Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Sil()
        {

        }

        private void MusteriEkleDuzenle_Load(object sender, EventArgs e)
        {
            brandList = _brandRepository.GetAll().ToList();

           
            lueBrandID.Properties.DataSource = brandList;
            lueil.Properties.DataSource = _ilRepository.GetAll().ToList();

            if(_musteri.StoreID > 0)
            {
                txtStoreName.Text = _musteri.StoreName;
                txtEmail.Text = _musteri.Email;
                txtIlgiliKisi.Text = _musteri.IlgiliKisi;
                txtTelefon.Text = _musteri.Telefon;
                txtGsm.Text = _musteri.Gsm;
                txtOzelKod1.Text = _musteri.OzelKod1;
                txtOzelKod2.Text = _musteri.OzelKod2;
                txtOzelKod3.Text = _musteri.OzelKod3;
                txtIlce.Text = _musteri.IlceAdi;
                memAdres.Text = _musteri.Adres;
                if(_musteri.IlId > 0)
                {
                    lueil.EditValue = Convert.ToInt32(_musteri.IlId);
                }
                if(_musteri.BrandID > 0)
                {
                    lueBrandID.EditValue = Convert.ToInt32(_musteri.BrandID);
                }
            }
            else
            {

            }

            //if (_musteri != null)
            //{
            //    // Düzenleme modu
            //    txtAdi.Text = _personel.Adi;
            //    txtSoyadi.Text = _personel.Soyadi;
            //    txtStoreName.Text = _personel.TcNo;
            //    radioPersTip.EditValue = _personel.Tip;
            //    labelControl.Text = "Personel Bilgi Düzenle";
            //}
            //else
            //{
            //    // Ekleme modu
            //    radioPersTip.SelectedIndex = 0;
            //    labelControl.Text = "Yeni Personel Ekle";
            //    _personel = new Models.Personel();
            //}
        }
    }
}

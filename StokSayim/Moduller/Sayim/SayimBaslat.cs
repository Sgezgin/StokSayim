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

namespace StokSayim.Moduller.Sayim
{
    public partial class SayimBaslat : DevExpress.XtraEditors.XtraUserControl
    {
        private Models.Sayim _sayim;
        private Models.Store _store;
        private PersonelRepository _personelRepository;
        private StoreRepository _storeRepository;
        private BrandRepository _brandRepository;
        private BaseRepository<AlanTipTanim> _alanTipTanimRepo;
        private BaseRepository<SayimPersonel> _sayimPersonelRepository;
        private SayimLokasyonRepository _sayimLokasyonRepository;
        private SayimLokasyonDetayRepository _sayimLokasyonDetayRepository;
        private SayimRepository _sayimRepository;


        private List<SayimPersonel> sayimPersonelList = new List<SayimPersonel>();
        private List<Brand> brandList = new List<Brand>();
        private List<Store> storeList = new List<Store>();
        private List<AlanTipTanim> alanTipTanimList = new List<AlanTipTanim>();


        public SayimBaslat(Models.Sayim sayim,Models.Store store) //Models.Personel pers, PersonelRepository personelRepository
        {
            InitializeComponent();
            _sayim = sayim;
            _store = store;
            _storeRepository = Global.StoreRepository;
            _brandRepository = Global.BrandRepository;
            _alanTipTanimRepo = new BaseRepository<AlanTipTanim>(Global.DbContext);
            _sayimPersonelRepository = new BaseRepository<SayimPersonel>(Global.DbContext);
            _sayimLokasyonRepository = Global.SayimLokasyonRepository;
            _sayimLokasyonDetayRepository = Global.SayimLokasyonDetayRepository;
            _sayimRepository = Global.SayimRepository;
            _personelRepository = Global.PersonelRepository;
          

        }

        private void SayimKontrol()
        {
            if(_sayim.Id > 0)
            {             
                lytLokasyonlar.Visible = true;
                lytAyarlar.Visible = true;
                lytPersoneller.Visible = true;
                lytLokasyonLabel.Control.Visible = false;
            }
            else
            {
                lytLokasyonlar.Visible = false;
                lytAyarlar.Visible = false;
                lytPersoneller.Visible = false;
                //lytLokasyonLabel.Control.Visible = true;
                lblLokasyonUyarı.Text = "Genel Bilgiler Sekmesinden Kayıt Yapınız.";
            }
        }

        private void SayimPersonelListele()
        {
            try
            {
                if(_sayim.Id > 0)
                {
                    sayimPersonelList = _sayimPersonelRepository.Find(x => x.SayimId == _sayim.Id).ToList();
                    gridControlPers.DataSource = sayimPersonelList;
                }
          
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Hata SayimPersonelList : " + ex.Message);
            }
        }


        private void SayimBaslat_Load(object sender, EventArgs e)
        {
   

            alanTipTanimList = _alanTipTanimRepo.GetAll().ToList();
            lueAlanTipi.Properties.DataSource = alanTipTanimList;

            brandList = _brandRepository.GetAll().ToList();
            storeList = _storeRepository.GetAll().ToList();

            lueMarka.Properties.DataSource = brandList;
            lueMagaza.Properties.DataSource = storeList;
            if (_store.StoreID > 0)
            {
                lueMagaza.EditValue = _store.StoreID;
                lueMarka.EditValue = _store.BrandID;

                lueMarka.Properties.ReadOnly = true;
                lueMagaza.Properties.ReadOnly = true;
            }

            if(_sayim.Id > 0)
            {
                lueMagaza.EditValue = _sayim.StoreID;
                lueMarka.EditValue = _sayim.BrandID;

                lueMarka.Properties.ReadOnly = true;
                lueMagaza.Properties.ReadOnly = true;

                dtBastarih.EditValue = _sayim.BaslangicTarihi;
                dtBittarih.EditValue = _sayim.BitisTarihi;

                gridSayimLokasyon.DataSource = _sayimLokasyonRepository.GetAll();

                windowsUIButtonPanel.Buttons["Kaydet"].Properties.Caption = "Güncelle";
            }

            luePersonelSec.Properties.DataSource = _personelRepository.GetAll();

            SayimPersonelListele();
            SayimKontrol();



        }



        private void windowsUIButtonPanel_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var caption = e.Button.Properties.Caption;     
            if (caption == "Kaydet") Kaydet(); 
            else if (caption == "Kapat") (this.Parent as Form).DialogResult = DialogResult.No;
        }

        private void Kaydet()
        {
            if (KontrolleriDogrula())
            {
                try
                {
                    if (_sayim.Id == 0)
                    {
                        Models.Sayim yeniSayim = new Models.Sayim();
                        yeniSayim.OlusturmaTarihi = DateTime.Now;
                        yeniSayim.SayimDurumu = 0;
                        yeniSayim.SayimKodu = "STK-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                        yeniSayim.StoreID = Convert.ToInt32(lueMagaza.EditValue);
                        yeniSayim.BrandID = Convert.ToInt32(lueMarka.EditValue);
                        yeniSayim.BaslangicTarihi = Convert.ToDateTime(dtBastarih.EditValue);
                        yeniSayim.BitisTarihi = Convert.ToDateTime(dtBittarih.EditValue);
                        yeniSayim.Aciklama = "";

                        _sayimRepository.Add(yeniSayim);
                        _sayim = yeniSayim;
                        MessageBox.Show("Yeni Sayım Oluşturuldu.", "Bilgi",
              MessageBoxButtons.OK, MessageBoxIcon.Information);


                        windowsUIButtonPanel.Buttons["Kaydet"].Properties.Caption = "Güncelle";
                       
                    }
                    else
                    {

                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Validation hatalarını detaylı göster
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("Property: {0} Error: {1}",
                                            validationError.PropertyName,
                                            validationError.ErrorMessage);
                            MessageBox.Show(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
               
            }
        }
        private void Sil()
        {

        }

        private void btnLokasyonEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if(_sayim.Id == 0)
                    XtraMessageBox.Show("Sayım Bilgisini Kaydedin", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (string.IsNullOrEmpty(txtMiktar.EditValue.ToString()))
                    XtraMessageBox.Show("Miktar Giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    int miktar = Convert.ToInt32(txtMiktar.EditValue);
                    string alanKodu = txtAlanKodu.Text;

                    SayimLokasyon sayimLokasyon = new SayimLokasyon();
                    sayimLokasyon.Aciklama = txtAcikalam.Text;
                    sayimLokasyon.Adi = lueAlanTipi.Text;
                    sayimLokasyon.Aktif = 1;
                    sayimLokasyon.AlanKod = alanKodu;
                    sayimLokasyon.Miktar = miktar;
                    sayimLokasyon.SayimId =_sayim.Id;
                    _sayimLokasyonRepository.Add(sayimLokasyon);

                    List<SayimLokasyonDetay> lokasyonDetayList = new List<SayimLokasyonDetay>(); 

                    for (int i = 1; i <= miktar; i++)
                    {
                        // Alan kodu uzunluğu hesaplanır
                        int alanKodUzunluk = alanKodu.Length;

                        // Kalan kısım için sıfır sayısını hesapla (toplam 6 karakter olacak şekilde)
                        int sifirSayisi = 6 - alanKodUzunluk - i.ToString().Length;

                        // Sıfırları oluştur
                        string sifirlar = new string('0', sifirSayisi > 0 ? sifirSayisi : 0);

                        // LokasyonKod oluştur
                        string lokasyonKod = $"{alanKodu}{sifirlar}{i}";

                        lokasyonDetayList.Add(new SayimLokasyonDetay
                        {
                            SayimId = sayimLokasyon.SayimId,
                            AlanKod = alanKodu,
                            LokasyonKod = lokasyonKod,
                            SayimLokasyonId = sayimLokasyon.Id,
                           IptalAciklama="",
                           Aktif =1
                        });
                    }

                    _sayimLokasyonDetayRepository.AddRange(lokasyonDetayList);

                    XtraMessageBox.Show("Lokasyonlar Eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    gridSayimLokasyon.DataSource = _sayimLokasyonRepository.GetAll();
                    Application.DoEvents();



                }
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lueAlanTipi_EditValueChanged(object sender, EventArgs e)
        {
            if(lueAlanTipi.EditValue != null)
            {
                int secilenId = Convert.ToInt32(lueAlanTipi.EditValue);

                AlanTipTanim secilenTipTanim = alanTipTanimList.FirstOrDefault(x => x.Id == secilenId);
                if(secilenTipTanim != null)
                {
                    string alankodu = secilenTipTanim.AlanKod;
                    string aciklama = secilenTipTanim.Aciklama;

                    txtAlanKodu.Text = alankodu;
                    txtAcikalam.Text = aciklama;
                }
            }
        }




        private bool KontrolleriDogrula()
        {
            // LookUpEdit kontrolleri
            if (lueMarka.EditValue == null || string.IsNullOrEmpty(lueMarka.EditValue.ToString()))
            {
                MessageBox.Show("Lütfen marka seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (lueMagaza.EditValue == null || string.IsNullOrEmpty(lueMagaza.EditValue.ToString()))
            {
                MessageBox.Show("Lütfen mağaza seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // DateEdit kontrolleri
            if (dtBastarih.EditValue == null)
            {
                MessageBox.Show("Lütfen başlangıç tarihini giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtBittarih.EditValue == null)
            {
                MessageBox.Show("Lütfen bitiş tarihini giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void gridView2_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            //SayimLokasyondan Sayımlokasyondetay lsiteleme

            var selectedRow = gridView2.GetFocusedRow();
            if (selectedRow != null)
            {
                int lokasyonId = Convert.ToInt32(gridView2.GetFocusedRowCellValue("Id"));

                List<SayimLokasyonDetay> lokasyonDetay = _sayimLokasyonDetayRepository.Find(x => x.SayimLokasyonId == lokasyonId).ToList();

                gridSayimLokasyonDetay.DataSource = lokasyonDetay;

            }
        }

        private void btnPersonelEkle_Click(object sender, EventArgs e)
        {
            var selectedTcNo = luePersonelSec.EditValue?.ToString();

            if (!string.IsNullOrEmpty(selectedTcNo))
            {
                var selectedPersonel = _personelRepository.FirstOrDefault(p => p.TcNo == selectedTcNo);

                if (selectedPersonel != null)
                {
                    int tipDegeri = selectedPersonel.Tip;
                    string tckn = selectedPersonel.TcNo;
                    if(sayimPersonelList.FirstOrDefault(x=> x.TcNo == tckn) == null)
                    {
                        SayimPersonel yeniPersEkle = new SayimPersonel();
                        yeniPersEkle.TcNo = tckn;
                        yeniPersEkle.Tip = tipDegeri;
                        yeniPersEkle.Soyadi = selectedPersonel.Soyadi;
                        yeniPersEkle.SayimId = _sayim.Id;
                        yeniPersEkle.Adi = selectedPersonel.Adi;
                        _sayimPersonelRepository.Add(yeniPersEkle);

                        SayimPersonelListele();
                    }
               
                }
            }
        }

        private void gridViewPers_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Tip")
            {
                if (e.Value is int tipValue)
                {
                    if (tipValue == 1)
                        e.DisplayText = "Operatör";
                    else if (tipValue == 2)
                        e.DisplayText = "Yönetici";
                    else if (tipValue == 3)
                        e.DisplayText = "Bölge Sorumlusu";
                    else
                        e.DisplayText = "Bilinmiyor"; // Diğer durumlar için
                }
            }
        }

        private void btnGridPersSil_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // Seçilen satırın indexini al
            int rowIndex = gridViewPers.FocusedRowHandle;

            // Örneğin satırdan TcNo değerini alalım
            var id = gridViewPers.GetRowCellValue(rowIndex, "Id")?.ToString();

            if (!string.IsNullOrEmpty(id))
            {
                long silinecekPersId = Convert.ToInt64(id);
                var silinecek = _sayimPersonelRepository.FirstOrDefault(p => p.Id == silinecekPersId);
                if (silinecek != null)
                {
                    _sayimPersonelRepository.Delete(silinecek);
                    SayimPersonelListele();
                }
            }
        }
    }
}

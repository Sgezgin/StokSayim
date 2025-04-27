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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Customization;

namespace StokSayim.Moduller.Musteriler
{
    public partial class MusteriListesi : DevExpress.XtraEditors.XtraUserControl
    {
        private StoreRepository _storeRepository;
        private BaseRepository<IlTanim> _ilRepository;
        private BrandRepository _brandRepository;

        public MusteriListesi()
        {
            InitializeComponent();
            _storeRepository = Global.StoreRepository;
            _ilRepository = new BaseRepository<IlTanim>(Global.DbContext);
            _brandRepository = Global.BrandRepository;
        }

        private void MusteriListesi_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            try
            {
                // Tüm illeri bir kez yükle (performans için)
                var illerDict = _ilRepository.GetAll().ToDictionary(il => il.Id, il => il.Adi);
                var markalarDict = _brandRepository.GetAll().ToDictionary(b => b.BrandID, b => b.BrandName);

                // Mağazaları ve ilişkili verileri al
                var stores = _storeRepository.GetAll();

                // Gösterilecek veriyi oluştur (DTO)
                var storeDisplayList = stores.Select(store => new
                {
                    store.StoreID,
                    store.StoreCode,
                    store.StoreName,
                    store.BrandID,
                    BrandName = markalarDict.ContainsKey(store.BrandID) ? markalarDict[store.BrandID] : "Bilinmiyor",
                    store.IlId,
                    IlAdi = store.IlId.HasValue ? (illerDict.ContainsKey(store.IlId.Value) ? illerDict[store.IlId.Value] : "") : "",
                    store.IlceAdi,
                    store.Adres,
                    store.Telefon,
                    store.Gsm,
                    store.Email,
                    store.IlgiliKisi,
                    store.OzelKod1,
                    store.OzelKod2,
                    store.OzelKod3,
                    store.EklemeTarih,
                    store.GuncellemeTarih
                }).ToList();

                // Grid'e veriyi bağla
                gridControl1.DataSource = storeDisplayList;

                // Grid'i özelleştir
                GridiOzelleştir();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler listelenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void GridiOzelleştir()
        {
            // GridView referansını al
            GridView view = gridControl1.MainView as GridView;
            if (view == null) return;

            // Gizlenmesi gereken sütunlar
            string[] gizlenecekSutunlar = new[] {
                "StoreID",
                "BrandID",
                "IlId",
                "OzelKod1",
                "OzelKod2",
                "OzelKod3",
                "GuncellemeTarih"
            };

            // Sütun başlıklarını değiştirme
            Dictionary<string, string> sutunBasliklari = new Dictionary<string, string>
            {
                { "StoreName", "Müşteri Adı" },
                { "StoreCode", "Müşteri Kodu" },
                { "BrandName", "Marka" },
                { "IlAdi", "İl" },
                { "IlceAdi", "İlçe" },
                { "Adres", "Adres" },
                { "Telefon", "Telefon" },
                { "Gsm", "GSM" },
                { "Email", "E-posta" },
                { "IlgiliKisi", "İlgili Kişi" },
                { "EklemeTarih", "Eklenme Tarihi" }
            };

            // Sütun genişlikleri (isteğe bağlı)
            Dictionary<string, int> sutunGenislikleri = new Dictionary<string, int>
            {
                { "StoreName", 150 },
                { "BrandName", 100 },
                { "IlAdi", 80 },
                { "IlceAdi", 80 },
                { "Telefon", 100 },
                { "IlgiliKisi", 120 }
            };

            // Sütunları gizle
            foreach (string sutunAdi in gizlenecekSutunlar)
            {
                if (view.Columns[sutunAdi] != null)
                {
                    view.Columns[sutunAdi].Visible = false;
                }
            }

            // Sütun başlıklarını değiştir
            foreach (var baslik in sutunBasliklari)
            {
                if (view.Columns[baslik.Key] != null)
                {
                    view.Columns[baslik.Key].Caption = baslik.Value;
                }
            }

            // Sütun genişliklerini ayarla (isteğe bağlı)
            foreach (var genislik in sutunGenislikleri)
            {
                if (view.Columns[genislik.Key] != null)
                {
                    view.Columns[genislik.Key].Width = genislik.Value;
                }
            }

            // Sütun sırasını değiştirme (isteğe bağlı)
            if (view.Columns["StoreCode"] != null && view.Columns["StoreName"] != null)
            {
                view.Columns["StoreCode"].VisibleIndex = 0;
                view.Columns["StoreName"].VisibleIndex = 1;
            }

            // Diğer grid ayarları
            view.OptionsBehavior.Editable = false; // Düzenlemeyi engelle
            view.OptionsSelection.EnableAppearanceFocusedCell = false; // Hücre odağını gizle
            view.OptionsView.ShowIndicator = false; // Sol taraftaki göstergeyi gizle

            // Alternatif satır renklerini etkinleştir
            view.OptionsView.EnableAppearanceEvenRow = true;

            // Best fit uygula (sütunları içeriğe göre boyutlandır)
            view.BestFitColumns();
        }

        private void barSayimBaslat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var selectedRow = gridView1.GetFocusedRow();
            if (selectedRow != null)
            {
                int storeId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("StoreID"));
                var selectStore = _storeRepository.GetById(storeId);

                if (selectStore != null)
                {
                    Models.Sayim newSayim = new Models.Sayim();
                    using (var frm = new Moduller.Sayim.SayimBaslat(newSayim, selectStore)) //selectStore, _storeRepository
                    {
                        FlyoutProperties properties = new FlyoutProperties();
                        properties.Style = FlyoutStyle.Popup;
                        var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                        Listele();

                    }
                }

            }
            else
            {
                MessageBox.Show("Lütfen Sayım Başlatmak için bir müşteri seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void barDetay_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
          
        }

        private void barDuzenle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var selectedRow = gridView1.GetFocusedRow();
            if (selectedRow != null)
            {
                int storeId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("StoreID"));
                var selectStore = _storeRepository.GetById(storeId);

                if(selectStore != null)
                {
                    using (var frm = new Moduller.Musteriler.MusteriEkleDuzenle(selectStore, _storeRepository))
                    {
                        FlyoutProperties properties = new FlyoutProperties();
                        properties.Style = FlyoutStyle.Popup;
                        var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                        Listele();

                    }
                }
               
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek için bir müşteeri seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu1.ShowPopup(Control.MousePosition);
            }
        }
    }
}

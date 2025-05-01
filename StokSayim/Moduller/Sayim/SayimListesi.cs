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
using DevExpress.XtraBars.FluentDesignSystem;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Customization;

namespace StokSayim.Moduller.Sayim
{
    public partial class SayimListesi : DevExpress.XtraEditors.XtraUserControl
    {
        private SayimRepository _sayimRepository;
    
        FlyoutProperties properties = new FlyoutProperties();
        FluentDesignFormContainer fluentContainer = new FluentDesignFormContainer();
        public SayimListesi(FluentDesignFormContainer flContainer)
        {
            InitializeComponent();
            _sayimRepository = Global.SayimRepository;  
            fluentContainer = flContainer;
            properties.Style = FlyoutStyle.Popup;
        }

      

        private void SayimListesi_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            // İlişkili verileri içeren sayım listesini al
            var sayimlarWithRelations = _sayimRepository.GetAllWithRelations();

            // Grid için anonim tip oluşturarak verileri hazırla
            var gridData = sayimlarWithRelations.Select(s => new {
                Id = s.Id,
                SayimKodu = s.SayimKodu,
                BaslangicTarihi = s.BaslangicTarihi,
                BitisTarihi = s.BitisTarihi,
                Marka = s.Brand?.BrandName ?? "Belirtilmemiş",
                Magaza = s.Store?.StoreName ?? "Belirtilmemiş",
                SayimDurumu = s.SayimDurumu == 0 ? "Başladı" : (s.SayimDurumu == 1 ? "Tamamlandı" : "İptal Edildi"),
                Aciklama = s.Aciklama,
                OlusturmaTarihi = s.OlusturmaTarihi,
                GuncellemeTarihi = s.GuncellemeTarihi
            }).ToList();

            // Veri kaynağını ayarla
            gridControl1.DataSource = gridData;
        }
        private void btnListele_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu1.ShowPopup(Control.MousePosition);
            }
        }

        private void barDashboardAktar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            var selectedRow = gridView1.GetFocusedRow();
            if (selectedRow != null)
            {
                // Seçili satırdan ID al
                int sayimId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("Id"));

                // ID ile sayımı ve ilişkili verileri getir
                var selectSayim = _sayimRepository.GetByIdWithRelations(sayimId);

                if (selectSayim != null)
                {
                    fluentContainer.Controls.Clear();
                    fluentContainer.Controls.Add(new Moduller.Dashboard.DashboardMain() { Dock = DockStyle.Fill });
                }
                else
                {
                    MessageBox.Show("Sayım bilgisi alınamadı.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek için bir sayım seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void barDuzenle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var selectedRow = gridView1.GetFocusedRow();
            if (selectedRow != null)
            {
                // Anonim tipten Id değerini al
                int sayimId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("Id"));

                // ID ile sayımı ve ilişkili verileri getir
                var selectSayim = _sayimRepository.GetByIdWithRelations(sayimId);

                if (selectSayim != null)
                {
                    // Store bilgisini sayımdan al
                    Models.Store store = selectSayim.Store;

                    using (var frm = new Moduller.Sayim.SayimBaslat(selectSayim, store))
                    {
                        FlyoutProperties properties = new FlyoutProperties();
                        properties.Style = FlyoutStyle.Popup;
                        var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek için bir sayım seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

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
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Customization;
using StokSayim.Models;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using StokSayim.Data.Services;
using System.Configuration;

namespace StokSayim.Moduller.Marka
{
    public partial class MarkaList : DevExpress.XtraEditors.XtraUserControl
    {

        FlyoutProperties properties = new FlyoutProperties();
        TabbedView tabbedView;

        private BrandRepository _brandRepository;

        public MarkaList(TabbedView tabbedView1)
        {
            InitializeComponent();
            properties.Style = FlyoutStyle.Popup;
            tabbedView = tabbedView1;
            _brandRepository = Global.BrandRepository;
        }

        private void MarkaTanim_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            var brands = _brandRepository.GetAll();
            gridControl1.DataSource = brands;

        }

        private void btnYeniMarkaEkle_Click(object sender, EventArgs e)
        {

            using (var frm = new Moduller.Marka.MarkaEkleDuzenle(null, _brandRepository))
            {
                FlyoutProperties properties = new FlyoutProperties();
                properties.Style = FlyoutStyle.Popup;
                var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                Listele();
                
            }
          
        }

        private void barDuzenle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var selectedBrand = gridView1.GetFocusedRow() as Brand;
            if (selectedBrand != null)
            {
                using (var frm = new Moduller.Marka.MarkaEkleDuzenle(selectedBrand, _brandRepository))
                {
                    FlyoutProperties properties = new FlyoutProperties();
                    properties.Style = FlyoutStyle.Popup;
                    var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                    Listele();

                }
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek için bir marka seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void barKatalog_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MarkaKatalog();
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu1.ShowPopup(Control.MousePosition);
            }
        }


        private void MarkaKatalog()
        {

            var selectedBrand = gridView1.GetFocusedRow() as Brand;
            if (selectedBrand != null)
            {          

                Moduller.Katalog.KatalogList ctr = new Katalog.KatalogList(
                    Global.CatalogRepository,
                    Global.BulkImportService,
                    selectedBrand.BrandID, 
                    selectedBrand.BrandName
                    );
                BaseDocument doc = tabbedView.AddDocument(ctr);
                ActiveDocument(ctr);
                doc.Caption = "Marka Listesi";
            }
            else
            {
                MessageBox.Show("Lütfen Katalog için bir marka seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

         
        }

        public void ActiveDocument(Control ctr)
        {
            tabbedView.ActivateDocument(ctr);
        }
    }
}

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
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using StokSayim.Data.Repositories;
using DevExpress.XtraBars.Docking2010.Customization;
using StokSayim.Moduller.Personel;
using DevExpress.XtraBars.Docking2010.Views;

namespace StokSayim.Moduller.Musteriler
{
    public partial class MuslerilerMain : DevExpress.XtraEditors.XtraUserControl
    {
        FlyoutProperties properties = new FlyoutProperties();
        private StoreRepository _storeRepository;
        public MuslerilerMain()
        {
            InitializeComponent();
            _storeRepository = Global.StoreRepository;
            properties.Style = FlyoutStyle.Popup;
        }


        void ActiveDocument(Control ctr)
        {
            tabbedView1.ActivateDocument(ctr);
        }

        private void barMusteriList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MusteriList();
        }

     


        private void barMusteriEkle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Models.Store musteri = new Models.Store();
            using (var frm = new Moduller.Musteriler.MusteriEkleDuzenle(musteri, _storeRepository))
            {
                FlyoutProperties properties = new FlyoutProperties();
                properties.Style = FlyoutStyle.Popup;
                var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                MusteriList();

            }
        }

        private void MusteriList()
        {
            MusteriListesi ctr = new MusteriListesi();
            BaseDocument doc = tabbedView1.AddDocument(ctr);
            ActiveDocument(ctr);
            doc.Caption = "Müşteri Listesi";
        }

        private void Musleriler_Load(object sender, EventArgs e)
        {

        }
    }
}

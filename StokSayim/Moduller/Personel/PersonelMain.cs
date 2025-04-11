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
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Customization;
using StokSayim.Data.Repositories;

namespace StokSayim.Moduller.Personel
{
    public partial class PersonelMain : DevExpress.XtraEditors.XtraUserControl
    {
        FlyoutProperties properties = new FlyoutProperties();

        private PersonelRepository _personelRepository;
        public PersonelMain()
        {
            InitializeComponent();
            _personelRepository = Global.PersonelRepository;
            properties.Style = FlyoutStyle.Popup;
        }

        void ActiveDocument(Control ctr)
        {
            tabbedView1.ActivateDocument(ctr);
        }

        private void barPersonelListesi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PersonelList();
        }

        private void PersonelList()
        {
            PersonelListesi ctr = new PersonelListesi();
            BaseDocument doc = tabbedView1.AddDocument(ctr);
            ActiveDocument(ctr);
            doc.Caption = "Personel Listesi";
        }

    

        private void barPersonelEkle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Models.Personel pers = new Models.Personel();
            using (var frm = new Moduller.Personel.PersonelEkleDuzenle(pers, _personelRepository))
            {
                FlyoutProperties properties = new FlyoutProperties();
                properties.Style = FlyoutStyle.Popup;
                var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                PersonelList();

            }
        }

        private void Listele()
        {

        }
    }
}

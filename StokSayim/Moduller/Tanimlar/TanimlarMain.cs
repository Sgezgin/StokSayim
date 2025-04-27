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
using StokSayim.Moduller.Personel;

namespace StokSayim.Moduller.Tanimlar
{
    public partial class TanimlarMain : DevExpress.XtraEditors.XtraUserControl
    {
        public TanimlarMain()
        {
            InitializeComponent();
        }

        void ActiveDocument(Control ctr)
        {
            tabbedView1.ActivateDocument(ctr);
        }


        private void barAlanTipTanim_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AlanTipTanimlar();
        }

        private void AlanTipTanimlar()
        {
            Moduller.Tanimlar.AlanTipTanimList ctr = new AlanTipTanimList();
            BaseDocument doc = tabbedView1.AddDocument(ctr);
            ActiveDocument(ctr);
            doc.Caption = "Personel Listesi";
        }

    }
}

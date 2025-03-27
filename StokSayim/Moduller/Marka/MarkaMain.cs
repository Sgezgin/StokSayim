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
using StokSayim.Data.Repositories;

namespace StokSayim.Moduller.Marka
{
    public partial class MarkaMain : System.Windows.Forms.UserControl
    {
        private BrandRepository _brandRepository;


        public MarkaMain()
        {
            InitializeComponent();
            _brandRepository = Global.BrandRepository;
        }

        private void MarkaMain_Load(object sender, EventArgs e)
        {
            MarkaList();
        }

        private void MarkaList()
        {
            Moduller.Marka.MarkaList ctr = new MarkaList(tabbedView1);
            BaseDocument doc = tabbedView1.AddDocument(ctr);
            ActiveDocument(ctr);
            doc.Caption = "Marka Listesi";
        }

        public void ActiveDocument(Control ctr)
        {
            tabbedView1.ActivateDocument(ctr);
        }
    }
}

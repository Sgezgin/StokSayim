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

namespace StokSayim.Moduller.Personel
{
    public partial class PersonelEkleDuzenle : DevExpress.XtraEditors.XtraUserControl
    {
        public PersonelEkleDuzenle()
        {
            InitializeComponent();
        }

    

        private void PersonelEkleDuzenle_Load(object sender, EventArgs e)
        {

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

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void Sil()
        {

        }
    }
}

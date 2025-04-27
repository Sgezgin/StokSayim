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

        public SayimBaslat(Models.Sayim sayim,Models.Store store) //Models.Personel pers, PersonelRepository personelRepository
        {
            InitializeComponent();
            _sayim = sayim;
            _store = store;
            _storeRepository = Global.StoreRepository;
            _brandRepository = Global.BrandRepository;
        }


        private void SayimBaslat_Load(object sender, EventArgs e)
        {
            lueMarka.Properties.DataSource = _brandRepository.GetAll();
            lueMagaza.Properties.DataSource = _storeRepository.GetAll();
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
         
        }
        private void Sil()
        {

        }

    }
}

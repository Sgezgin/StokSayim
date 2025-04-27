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

         
        }



        private void windowsUIButtonPanel_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var caption = e.Button.Properties.Caption;     
            if (caption == "Kaydet") Kaydet(); 
            else if (caption == "Kapat") (this.Parent as Form).DialogResult = DialogResult.No;
        }

        private void Kaydet()
        {
         
        }
        private void Sil()
        {

        }

        private void btnLokasyonEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMiktar.EditValue.ToString()))
                    XtraMessageBox.Show("Miktar Giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {

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
    }
}

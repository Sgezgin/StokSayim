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

namespace StokSayim.Moduller.Tanimlar
{
    public partial class AlanTipEkleDuzenle : DevExpress.XtraEditors.XtraUserControl
    {
        private Models.AlanTipTanim _alanTipTanim;
        private BaseRepository<AlanTipTanim> _alanTanimRepository;

        public AlanTipEkleDuzenle(Models.AlanTipTanim alanTipTanim, BaseRepository<AlanTipTanim> alanTipTanimRepo)
        {
            InitializeComponent();
            _alanTipTanim = alanTipTanim;
            _alanTanimRepository = alanTipTanimRepo;
        }


        private void AlanTipEkleDuzenle_Load(object sender, EventArgs e)
        {
            if(_alanTipTanim.Id > 0)
            {
                txtAlanAdi.Text = _alanTipTanim.Adi;
                txtAlanKodu.Text = _alanTipTanim.AlanKod;
                memAciklama.Text = _alanTipTanim.Aciklama;
                radioAktifPasif.EditValue = _alanTipTanim.Aktif;
                labelControl.Text = "Alan Bilgisi Düzenle";

            }
            else
            {
                labelControl.Text = "Yeni Alan Bilgisi Ekle";
            }
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

                _alanTipTanim.Adi = txtAlanAdi.Text.Trim();
                _alanTipTanim.AlanKod = txtAlanKodu.Text.Trim();
                _alanTipTanim.Aciklama = memAciklama.Text;
                _alanTipTanim.Aktif = Convert.ToInt32(radioAktifPasif.EditValue);

                if (_alanTipTanim.Id > 0)
                {         
                    _alanTanimRepository.Update(_alanTipTanim);
                    MessageBox.Show("Alan Bilgisi Güncellendi.", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {              
                    _alanTanimRepository.Add(_alanTipTanim);
                    MessageBox.Show("Yeni Alan Eklendi.", "Bilgi",
                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


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

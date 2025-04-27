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
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Customization;

namespace StokSayim.Moduller.Tanimlar
{
    public partial class AlanTipTanimList : DevExpress.XtraEditors.XtraUserControl
    {
        private BaseRepository<AlanTipTanim> _alanTipTanimRepo;

        public AlanTipTanimList()
        {
            InitializeComponent();
            _alanTipTanimRepo = new BaseRepository<AlanTipTanim>(Global.DbContext);
        }

        private void AlanTipTanimList_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void btnYeniAlanTipEkle_Click(object sender, EventArgs e)
        {
            AlanTipTanim newAlan = new AlanTipTanim();
            using (var frm = new Moduller.Tanimlar.AlanTipEkleDuzenle(newAlan, _alanTipTanimRepo))
            {
                FlyoutProperties properties = new FlyoutProperties();
                properties.Style = FlyoutStyle.Popup;
                var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                Listele();

            }
        }

        private void Listele()
        {
            try
            {
                gridControl1.DataSource = _alanTipTanimRepo.GetAll();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void barDuzenle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var selectAlanTip = gridView1.GetFocusedRow() as Models.AlanTipTanim;
            if (selectAlanTip != null)
            {
                using (var frm = new Moduller.Tanimlar.AlanTipEkleDuzenle(selectAlanTip, _alanTipTanimRepo))
                {
                    FlyoutProperties properties = new FlyoutProperties();
                    properties.Style = FlyoutStyle.Popup;
                    var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                    Listele();

                }
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek için bir alan tipi seçin.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu1.ShowPopup(Control.MousePosition);
            }
        }
    }
}

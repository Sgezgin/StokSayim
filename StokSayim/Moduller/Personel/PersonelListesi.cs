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

namespace StokSayim.Moduller.Personel
{
    public partial class PersonelListesi : DevExpress.XtraEditors.XtraUserControl
    {
        private PersonelRepository _personelRepository;

        public PersonelListesi()
        {
            InitializeComponent();
            _personelRepository = Global.PersonelRepository;
        }

        private void PersonelListesi_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            var persList = _personelRepository.GetAll();
            gridControl1.DataSource = persList;

            tileBarItem1.Elements[1].Text =persList.Where(x=> x.Tip == 1).Count().ToString();
            tileBarItem2.Elements[1].Text =persList.Where(x=> x.Tip == 3).Count().ToString();
            tileBarItem3.Elements[1].Text =persList.Where(x=> x.Tip == 2).Count().ToString();
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Tip")
            {
                if (e.Value is int tipValue)
                {
                    if (tipValue == 1)
                        e.DisplayText = "Operatör";    
                    else if (tipValue == 2)
                        e.DisplayText = "Yönetici";
                    else if (tipValue == 3)
                        e.DisplayText = "Bölge Sorumlusu";
                    else
                        e.DisplayText = "Bilinmiyor"; // Diğer durumlar için
                }
            }
        }

        private void barBtnPersDuzenle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var selectPers = gridView1.GetFocusedRow() as Models.Personel;
            if (selectPers != null)
            {
                using (var frm = new Moduller.Personel.PersonelEkleDuzenle(selectPers, _personelRepository))
                {
                    FlyoutProperties properties = new FlyoutProperties();
                    properties.Style = FlyoutStyle.Popup;
                    var result = FlyoutDialog.Show(MainMenu.ActiveForm, frm, properties);
                    Listele();

                }
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek için bir personel seçin.", "Uyarı",
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

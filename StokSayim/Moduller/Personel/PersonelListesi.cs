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

namespace StokSayim.Moduller.Personel
{
    public partial class PersonelListesi : DevExpress.XtraEditors.XtraUserControl
    {
        private PersonelRepository personelRepository;

        public PersonelListesi()
        {
            InitializeComponent();
            personelRepository = Global.PersonelRepository;
        }

        private void PersonelListesi_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            var persList = personelRepository.GetAll();
            gridControl1.DataSource = persList;
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Tip")
            {
                if (e.Value is int tipValue)
                {
                    if (tipValue == 0)
                        e.DisplayText = "Admin";    // 0 ise Admin yazacak
                    else if (tipValue == 1)
                        e.DisplayText = "Kullanıcı"; // 1 ise Kullanıcı yazacak
                    else
                        e.DisplayText = "Bilinmiyor"; // Diğer durumlar için
                }
            }
        }
    }
}

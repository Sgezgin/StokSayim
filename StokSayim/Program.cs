using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using StokSayim.Data.Repositories;
using StokSayim.Data;
using StokSayim.Data.Services;
using System.Configuration;

namespace StokSayim
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Global servisleri başlat
            try
            {
                Global.Initialize();

                // Ana formu başlat
                Application.Run(new MainMenu());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Uygulama başlatılırken bir hata oluştu: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
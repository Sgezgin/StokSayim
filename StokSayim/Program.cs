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
using System.Threading.Tasks;
using StokSayim.Service;

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

         

            string exportFolder = @"C:\Temp\StokSayimExport"; // İstediğiniz bir klasör yolu

            // API Sunucusunu ayrı bir thread'de başlat
            Task.Run(() => StartApiServer(exportFolder));

            // Global servisleri başlat
            try
            {
                Global.Initialize();

                // Ana formu başlat
                Application.Run(new Login());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Uygulama başlatılırken bir hata oluştu: " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        static async Task StartApiServer(string exportFolder)
        {
            try
            {
                // MobileApiServer sınıfını oluştur
                var apiServer = new MobileApiServer(exportFolder);

                // Konsol penceresini göster (opsiyonel)
                AllocConsole();

                Console.WriteLine("API Sunucusu başlatılıyor...");

                // API Sunucusunu başlat
                await apiServer.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"API Sunucusu hatası: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Konsol penceresi açmak için Windows API çağrısı
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AllocConsole();
    }
}
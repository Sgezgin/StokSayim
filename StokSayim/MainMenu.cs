﻿using DevExpress.XtraBars;
using StokSayim.Data.Repositories;
using StokSayim.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StokSayim
{
    public partial class MainMenu : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public static int AktifSayimId =0;
        private UserControl _activeControl;


        public MainMenu()
        {
            InitializeComponent();
        }

        private void aceKullanicilar_Click(object sender, EventArgs e)
        {

        }

        private void aceFirmalar_Click(object sender, EventArgs e)
        {

        }

        private void aceMarka_Click(object sender, EventArgs e)
        {
            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Marka.MarkaMain() { Dock = DockStyle.Fill });
        }

        private void accordionControlElement165_Click(object sender, EventArgs e)
        {
           
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }

        private void acePersonelslemleri_Click(object sender, EventArgs e)
        {
            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Personel.PersonelMain() { Dock = DockStyle.Fill });

        }

        private void aceMarkaTanimlari_Click(object sender, EventArgs e)
        {
            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Marka.MarkaMain() { Dock = DockStyle.Fill });
        }

        private void aceMusteriler_Click(object sender, EventArgs e)
        {
            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Musteriler.MuslerilerMain() { Dock = DockStyle.Fill });
        }

        private void aceTanimlar_Click(object sender, EventArgs e)
        {

            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Tanimlar.TanimlarMain() { Dock = DockStyle.Fill });
        }

        private void barSayimislemleri_Click(object sender, EventArgs e)
        {
            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Sayim.SayimListesi(flContainer) { Dock = DockStyle.Fill });
        }

        private void aceDashboard_Click(object sender, EventArgs e)
        {
            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Dashboard.DashboardMain() { Dock = DockStyle.Fill });
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.EntryUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = ThemeExtension.GetStyleAbsoluteUri("Theme.xaml") });
            base.OnStartup(e);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.Com.Options
{
    public static class GlobalParam
    {
        public static WindowState WindowState { get; set; } = WindowState.Normal;
        public static double MAXWidth {  get; set; }
        public static double MAXHeight { get; set; }
        public static double NavHeight {  get; set; }
        public static double NavWidth {  get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hg.DoomHistory.Utilities
{
    public class FormHelper
    {
        public static IEnumerable<T> FindControls<T>(Control control) where T : Control
        {
            var controls = control.Controls.Cast<Control>().ToList();
            return controls.SelectMany(FindControls<T>).Concat(controls).Where(c => c.GetType() == typeof(T)).Cast<T>();
        }
    }
}

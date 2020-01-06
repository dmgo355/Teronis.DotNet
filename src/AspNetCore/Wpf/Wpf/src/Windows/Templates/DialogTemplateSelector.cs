﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Teronis.ViewModels.Wpf;

namespace Teronis.Windows.Templates
{
    public class DialogTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeaderFooterTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DialogFooterYesNoViewModel)
                return HeaderFooterTemplate;
            else
                return base.SelectTemplate(item, container);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GestionePosizioni.ViewModels;
using Models;
using QueryManager.Repositories;

namespace GestionePosizioni.CustomControls
{
    /// <summary>
    /// Interaction logic for CompanyDetails.xaml
    /// </summary>
    public partial class CompanyDetails : UserControl
    {
        public CompanyDetails()
        {
            InitializeComponent();
        }

        public CompanyDetails(ICompanyDetailsViewModel  model)
            : this()
        {
            DataContext = model;
            var companiesCombo = new Binding("Companies");
            CompanyName.SetBinding(ItemsControl.ItemsSourceProperty, companiesCombo);
            var myBinding = new Binding("Address");
            Address.SetBinding(TextBox.TextProperty, myBinding);
        }
    }
}

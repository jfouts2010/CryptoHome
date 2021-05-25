using System;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace CryptoHome
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LineGraph : UserControl
    {
        public ChartValues<double> Values1 { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public LineGraph()
        {
            InitializeComponent();
            Values1 = new ChartValues<double> { 3, 4, 6, 3, 2, 6 };
            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter = value => value.ToString("C");
            DataContext = this;
        }

        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {

        }
    }
}

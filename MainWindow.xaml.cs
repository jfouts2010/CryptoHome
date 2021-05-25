using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CryptoHome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*LineSegment ls1 = new LineSegment(new Point(0, 0), true);
            LineSegment ls2 = new LineSegment(new Point(1, 1), true);
            PathSegmentCollection psc = new PathSegmentCollection();
            psc.Add(ls1); psc.Add(ls2);
            ((((((Crypto0.Content as Image).Source as DrawingImage).Drawing as DrawingGroup).Children[2] as GeometryDrawing).Geometry as PathGeometry).Figures)[0].Segments = psc;*/
            RefreshData();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }
        private void RefreshData()
        {
            List<string> Labels = new List<string>();
            for (DateTime dt = DateTime.Now.AddMonths(-1); dt <= DateTime.Now; dt = dt.AddDays(1))
            {
                Labels.Add(dt.ToString("MMM d"));
            }
            //market data
            var client = new RestClient("https://data.messari.io/api/v2/assets");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JObject Assets = JObject.Parse(response.Content);
            int count = 0;
            foreach (var asset in Assets["data"].Take(6))
            {
                string symbol = asset["symbol"].ToString();
                if (symbol.ToLower() == "usdt")
                    continue;
                var client2 = new RestClient($"https://data.messari.io/api/v1/markets/binance-{symbol}-usdt/metrics/price/time-series?start={DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}&interval=1d");
                var request2 = new RestRequest(Method.GET);
                IRestResponse response2 = client2.Execute(request2);
                JObject marketData = JObject.Parse(response2.Content);
                List<double> values = marketData["data"]["values"].Select(p => (double)p[1]).ToList();
                ChartValues<double> cv = new ChartValues<double>();
                cv.AddRange(values);
                double SevenDays = cv[cv.Count - 1] / cv[cv.Count - 8] - 1;
                double OneDays = cv[cv.Count - 1] / cv[cv.Count - 2] - 1;
                (this.FindName("Crypto" + count) as LineGraph).Values1 = cv;
                (this.FindName("Crypto" + count + "Name") as TextBlock).Text = asset["name"].ToString();
                (this.FindName("Crypto" + count) as LineGraph).Labels = Labels.ToArray();
                //% info
                (this.FindName("Crypto" + count + "7") as TextBlock).Text = $"{(SevenDays >= 0 ? "▲" : "▼")} {String.Format("{0:P2}.", SevenDays)}";
                (this.FindName("Crypto" + count + "1") as TextBlock).Text = $"{(OneDays >= 0 ? "▲" : "▼")} {String.Format("{0:P2}.", OneDays)}";
                (this.FindName("Crypto" + count + "7") as TextBlock).Foreground = (SevenDays >= 0 ? Brushes.Green : Brushes.Red);
                (this.FindName("Crypto" + count + "1") as TextBlock).Foreground = (OneDays >= 0 ? Brushes.Green : Brushes.Red);
                count++;
            }
            for (int i = 0; i < 100; i++)
            {
                TextBlock tweetTitle = new TextBlock()
                {
                    Text = "@elonmusk Tweeted",
                    Style = this.Resources["ApplicationHeader"] as Style,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(2, 2, 2, 2),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                TextBlock tweet = new TextBlock()
                {
                    Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam. #Loremipsum",
                    Style = this.Resources["ApplicationHeader"] as Style,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(2, 2, 2, 2),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                StackPanel sp = new StackPanel()
                {
                    Margin = new Thickness(10, 10, 10, 10)
                };
                sp.Children.Add(tweetTitle);
                sp.Children.Add(tweet);
                Tweets.Children.Add(sp);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}

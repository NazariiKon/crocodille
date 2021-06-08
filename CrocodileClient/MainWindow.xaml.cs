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

namespace CrocodileClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Color color = ((SolidColorBrush)(sender as Button).Background).Color;
            this.inkCanvas.DefaultDrawingAttributes.Color = color;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.inkCanvas.DefaultDrawingAttributes.Height = (sender as Button).Height-3;
            this.inkCanvas.DefaultDrawingAttributes.Width = (sender as Button).Width-3;
        }

        private void sendMessageBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

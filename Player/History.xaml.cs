using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для History.xaml
    /// </summary>
    public partial class History : Window
    {
        private IEnumerable history;
        public List<string> his = new List<string>();
        public string choose;

        public History()
        {
            InitializeComponent();
            history_list.ItemsSource = history;
        }

        private void history_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            choose = (string)history_list.SelectedItem;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

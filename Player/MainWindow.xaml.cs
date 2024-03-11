using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> list_music = new List<string>();
        public List<string> history = new List<string>();
        public bool repeat = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Add_music_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string[] files = Directory.GetFiles(dialog.FileName);
                foreach (string file in files.Where(item => item.EndsWith(".mp3") || item.EndsWith(".m4a") || item.EndsWith(".wav"))) 
                {
                    list_music.Add(file.ToString());
                }
                ListMusic.ItemsSource = list_music;
                if (list_music.Count == 0)
                    return;
            }
            player.Source = new Uri(list_music[0]);
            player.Play();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChooseTrack((string)ListMusic.SelectedItem);
        }

        private void pause_but_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void play_but_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
        }

        private void forward_but_Click(object sender, RoutedEventArgs e)
        {
            NextTrack();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = valuechanger.Value;
        }

        void NextTrack()
        {
            string active = player.Source.ToString().Replace("file:///", "").Replace("/", "\\");
            if (active != list_music.Last())
                player.Source = new Uri(list_music[list_music.IndexOf(active) + 1]);
            else
                player.Source = new Uri(list_music.First());
        }

        void PreviosTrack()
        {
            string active = player.Source.ToString().Replace("file:///", "").Replace("/", "\\");
            if (active != list_music.First())
                player.Source = new Uri(list_music[list_music.IndexOf(active) - 1]);
            else
                player.Source = new Uri(list_music.First());
        }

        void ChooseTrack(string item)
        {
            player.Source = new Uri(item);
            time_slider.Value = 0;
            player.Play();
        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            time_slider.Maximum = player.NaturalDuration.TimeSpan.Ticks;
            history.Add(player.Source.ToString());
            Thread slider_moov = new Thread(_ =>
            {
                while (true)
                { 
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        time_slider.Value = player.Position.Ticks;
                        if (player.NaturalDuration.HasTimeSpan) // присрал как чувствовал, переодически скипает трек, ХЗ почему
                        {
                            if (player.Position == player.NaturalDuration.TimeSpan && repeat == false)
                            {
                                NextTrack();
                            }
                            else if (player.Position == player.NaturalDuration.TimeSpan && repeat == true)
                            {
                                NextTrack();
                                PreviosTrack();
                            }
                        }
                    });
                    Thread.Sleep(1000);
                }
            });
            slider_moov.Start();
            
        }

        private void time_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Position = TimeSpan.FromTicks((long)time_slider.Value);
            //позиция музыки = позиция слайдера
        }

        private void back_but_Click(object sender, RoutedEventArgs e)
        {
            PreviosTrack();
        }

        private void repeat_but_Click(object sender, RoutedEventArgs e)
        {
            repeat = repeat ? false : true;
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            History h = new History();
            h.history_list.ItemsSource = history;
            if ((bool)h.ShowDialog()) 
                ChooseTrack(h.choose);
            
        }
    }
}

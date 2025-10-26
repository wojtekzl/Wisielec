using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace wisielec
{
    public partial class DodajSlowoWindow : Window
    {
        private const string SciezkaPliku = "slowa.txt";

        public DodajSlowoWindow()
        {
            InitializeComponent();
            PokazSlowa();
        }

        private void DodajSlowo_Click(object sender, RoutedEventArgs e)
        {
            string nowe = NoweSlowoInput.Text?.Trim().ToLower();
            NoweSlowoInput.Clear();

            if (string.IsNullOrWhiteSpace(nowe) || !nowe.All(char.IsLetter))
            {
                InfoText.Text = "Wpisz poprawne słowo (tylko litery).";
                InfoText.Foreground = Brushes.Red;
                return;
            }

            var istniejace = File.Exists(SciezkaPliku) ? File.ReadAllLines(SciezkaPliku) : Array.Empty<string>();

            if (istniejace.Contains(nowe))
            {
                InfoText.Text = "To słowo już istnieje.";
                InfoText.Foreground = Brushes.Orange;
                return;
            }

            try
            {
                File.AppendAllLines(SciezkaPliku, new[] { nowe });
                InfoText.Text = $"Dodano słowo: {nowe}";
                InfoText.Foreground = Brushes.Green;
                PokazSlowa();
            }
            catch (Exception ex)
            {
                InfoText.Text = $"Błąd zapisu: {ex.Message}";
                InfoText.Foreground = Brushes.Red;
            }
        }

        private void PokazSlowa_Click(object sender, RoutedEventArgs e)
        {
            PokazSlowa();
        }

        private void PokazSlowa()
        {
            ListaSlow.Items.Clear();

            if (File.Exists(SciezkaPliku))
            {
                var slowa = File.ReadAllLines(SciezkaPliku)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .OrderBy(s => s);

                foreach (var slowo in slowa)
                    ListaSlow.Items.Add(slowo);
            }
        }

        private void Zamknij_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}

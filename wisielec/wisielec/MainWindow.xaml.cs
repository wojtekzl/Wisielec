using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace wisielec
{
    public partial class MainWindow : Window
    {
        private string[] slowa;
        private const string sciezka = "slowa.txt";

        private string haslo;
        private HashSet<char> zgadniete = new HashSet<char>();

        private const int maxProby = 11;
        private int pozostaleProby;
        private bool graTrwa = true;

        public MainWindow()
        {
            InitializeComponent();
            ZaladujSlowa();
            RozpocznijGre();
        }

        private void ZaladujSlowa()
        {
            if (!File.Exists(sciezka))
            {
                File.WriteAllLines(sciezka, new[] { "komputer", "programowanie", "internet", "aplikacja", "serwer" });
            }

            slowa = File.ReadAllLines(sciezka)
                .Select(s => s.Trim().ToLower())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToArray();
        }

        private void RozpocznijGre()
        {
            ZaladujSlowa();
            Random rand = new Random();
            haslo = slowa[rand.Next(slowa.Length)];
            zgadniete.Clear();
            pozostaleProby = maxProby;
            graTrwa = true;
            WiadomoscText.Text = "";
            LiteraInput.Text = "";
            ZgadnijButton.Content = "Zgadnij";
            ZgadnijButton.IsEnabled = true;
            AktualizujWidok();
            UstawObrazWisielca();
        }

        private void Zgadnij_Click(object sender, RoutedEventArgs e)
        {
            if (!graTrwa)
            {
                RozpocznijGre();
                return;
            }

            if (string.IsNullOrWhiteSpace(LiteraInput.Text))
                return;

            char litera = char.ToLower(LiteraInput.Text[0]);
            LiteraInput.Text = "";

            if (!char.IsLetter(litera))
            {
                WiadomoscText.Foreground = Brushes.Red;
                WiadomoscText.Text = "Proszę wpisać pojedynczą literę (a-z).";
                return;
            }

            // Każda próba zmniejsza licznik prób
            pozostaleProby--;

            if (zgadniete.Contains(litera))
            {
                WiadomoscText.Foreground = Brushes.Red;
                WiadomoscText.Text = "Już próbowałeś tej litery.";
                AktualizujWidok();
                UstawObrazWisielca();

                if (pozostaleProby <= 0)
                {
                    graTrwa = false;
                    HasloText.Text = haslo;
                    WiadomoscText.Foreground = Brushes.Red;
                    WiadomoscText.Text = "Przegrałeś! Hasło to: " + haslo;
                    ZgadnijButton.Content = "Nowa gra";
                }

                return;
            }

            // Dodanie nowej litery
            zgadniete.Add(litera);

            AktualizujWidok();
            UstawObrazWisielca();

            // Sprawdzenie wygranej
            if (CzyWygrana())
            {
                graTrwa = false;
                HasloText.Text = haslo;
                WiadomoscText.Foreground = Brushes.Green;
                WiadomoscText.Text = "Gratulacje! Wygrałeś!";
                ZgadnijButton.Content = "Nowa gra";
                return;
            }

            // Sprawdzenie przegranej
            if (pozostaleProby <= 0)
            {
                graTrwa = false;
                HasloText.Text = haslo;
                WiadomoscText.Foreground = Brushes.Red;
                WiadomoscText.Text = "Przegrałeś! Hasło to: " + haslo;
                ZgadnijButton.Content = "Nowa gra";
                return;
            }

            WiadomoscText.Text = "";
        }





        private void AktualizujWidok()
        {
            HasloText.Text = UkryjHaslo();
            ProbyText.Text = pozostaleProby.ToString();
        }

        private string UkryjHaslo()
        {
            string wynik = "";
            foreach (char c in haslo)
            {
                wynik += zgadniete.Contains(c) ? c + " " : "_ ";
            }
            return wynik.Trim();
        }

        private bool CzyWygrana()
        {
            foreach (char c in haslo)
            {
                if (!zgadniete.Contains(c))
                    return false;
            }
            return true;
        }

        private void UstawObrazWisielca()
        {
            try
            {
                int maxEtap = 11;
                int etap = maxEtap - pozostaleProby;
                etap = Math.Clamp(etap, 0, maxEtap);
                string nazwaPliku = etap.ToString().PadLeft(2, '0') + ".png";
                string sciezkaObrazka = $"pack://application:,,,/Images/{nazwaPliku}";
                WisielecImage.Source = new BitmapImage(new Uri(sciezkaObrazka));
            }
            catch (Exception ex)
            {
                WiadomoscText.Text = "Błąd ładowania obrazu: " + ex.Message;
            }
        }

        private void OtworzDodajSlowoWindow_Click(object sender, RoutedEventArgs e)
        {
            var okno = new DodajSlowoWindow();
            okno.Owner = this;
            bool? wynik = okno.ShowDialog();

            if (wynik == true)
            {
                RozpocznijGre(); 
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            string zasady =
               "Zasady gry w Wisielca:\n" +
                "Gra w wisielca polega na tym, że jeden gracz wymyśla słowo i zaznacza jego długość, a drugi próbuje odgadnąć je, podając litery. Za każdą błędną literę rysowany jest kolejny element wisielca.Gra kończy się wygraną, jeśli słowo zostanie odgadnięte przed narysowaniem całej postaci.";

            MessageBox.Show(zasady, "Zasady gry", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

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
using System.Windows.Shapes;
using System.Windows.Forms;

namespace LR3
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public double LineThickness { get; private set; }
        public Color LineColor { get; private set; }
        public Color BackgroundColor { get; private set; }

        public SettingsWindow(double lineThickness, Color lineColor, Color backgroundColor)
        {
            InitializeComponent();

            LineThickness = lineThickness;
            LineColor = lineColor;
            BackgroundColor = backgroundColor;

            lineThicknessBox.Text = LineThickness.ToString();
        }

        private void LineColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LineColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            }
        }

        private void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BackgroundColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(lineThicknessBox.Text, out double thickness))
            {
                LineThickness = thickness;
                DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid line thickness value.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

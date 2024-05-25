using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicsEditor
{
    public partial class SettingsWindow : Window
    {
        public Brush LineColor { get; private set; }
        public Brush FillColor { get; private set; }
        public double LineThickness { get; private set; }

        public SettingsWindow(Brush lineColor, Brush fillColor, double lineThickness)
        {
            InitializeComponent();

            LineColor = lineColor;
            FillColor = fillColor;
            LineThickness = lineThickness;

            lineColorComboBox.SelectedValue = LineColor.ToString();
            fillColorComboBox.SelectedValue = FillColor.ToString();
            lineThicknessTextBox.Text = LineThickness.ToString();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            LineColor = (Brush)new BrushConverter().ConvertFromString((string)((ComboBoxItem)lineColorComboBox.SelectedItem).Tag);
            FillColor = (Brush)new BrushConverter().ConvertFromString((string)((ComboBoxItem)fillColorComboBox.SelectedItem).Tag);
            LineThickness = double.Parse(lineThicknessTextBox.Text);
            DialogResult = true;
        }
    }
}

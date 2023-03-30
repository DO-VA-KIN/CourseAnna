
using System.Windows.Controls;
using System.Windows.Media;


namespace CurseAnna.Controls
{
    /// <summary>
    /// Логика взаимодействия для PBRoad.xaml
    /// </summary>
    public partial class PBRoad : UserControl
    {
        public PBRoad()
        {
            InitializeComponent();
        }

        public void SetValue(int value)
        { 
            PBProgress.Value = value;
            this.ToolTip = value + "/" + PBProgress.Maximum;
        }

        public int GetValue()
        { return (int)PBProgress.Value; }

        public int GetMaximum()
        { return (int)PBProgress.Maximum; }

        public int GetStageCount()
        { return 10; }

        public void SetColor(Brush br1)
        {
            PBProgress.Foreground = br1;
        }
        public void SetColor(Brush br1, Brush br2)
        {
            PBProgress.Foreground = br1;
            PBProgress.Background = br2;
        }
        public void SetColor(Brush br1, Brush br2, Brush br3)
        {
            PBProgress.Foreground = br1;
            PBProgress.Background = br2;

            B1.Background = br3;
            B2.Background = br3;
            B3.Background = br3;
            B4.Background = br3;
            B5.Background = br3;
            B6.Background = br3;
            B7.Background = br3;
            B8.Background = br3;
            B9.Background = br3;
        }
    }
}

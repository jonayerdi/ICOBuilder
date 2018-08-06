using System.IO;
using System.Windows;
using ICO;

namespace ICOBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICOFile ico;
        private string icoLoadedPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void RefreshAll()
        {

        }

        public void NewICO(ICOType type)
        {
            ico = new ICOFile(type);
            icoLoadedPath = null;
            RefreshAll();
        }

        public void LoadICO(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            ico = new ICOFile(data);
            icoLoadedPath = path;
            RefreshAll();
        }
    }
}

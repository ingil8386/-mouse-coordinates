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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MouseMacro
{
    /// <summary>
    /// HelpPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HelpPage : Page
    {
        private MainWindow _mainWindow;

        public HelpPage()
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.Loaded += HelpPage_Loaded;
        }

        private void HelpPage_Loaded(object sender, RoutedEventArgs e)
        {
            // 애니메이션을 가져옵니다.
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            // 페이지의 Opacity를 애니메이션의 대상로 설정합니다.
            Storyboard.SetTarget(storyboard, this);
            storyboard.Begin();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow != null)
            {
                var storyboard = (Storyboard)this.Resources["FadeOutStoryboard"];
                // 페이지의 Opacity를 애니메이션의 대상로 설정합니다.
                Storyboard.SetTarget(storyboard, this);
                storyboard.Begin();
            }

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.HelpFrame.Visibility = Visibility.Collapsed;
                mainWindow.HelpFrame.Navigate(null); // 페이지를 비웁니다.
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace MouseMacro
{
    /// <summary>
    /// Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Setting : Window
    {
        private MainWindow mainWindow ;
        public Setting(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow; // mainWindow를 초기화합니다.
            // 윈도우가 열릴 때, 현재 설정된 interval을 표시
               this.mainWindow.PropertyChanged += MainWindow_PropertyChanged; 
            UpdateCurrentIntervalLabel();
        }

        private void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindow.Interval))
            {
                // Interval 값이 변경되었을 때 Label을 업데이트
                UpdateCurrentIntervalLabel();
            }
        }

        private void UpdateCurrentIntervalLabel()
        {
            CurrentIntervalLabel.Content = CurrentIntervalLabel.Content = $"CurrentInterval : {mainWindow.Interval} ms";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // TextBox에서 시간 간격(밀리초)을 읽어옵니다.
            if (int.TryParse(TimeIntervalTextBox.Text, out int interval))
            {
                mainWindow.Interval = interval; // 설정된 시간 간격을 MainWindow에 전달
                UpdateCurrentIntervalLabel(); // Label 업데이트

                MessageBox.Show("설정이 적용되었습니다.");
            }
            else
            {
                MessageBox.Show("올바른 시간을 입력하세요.");
            }
        }
    }
}

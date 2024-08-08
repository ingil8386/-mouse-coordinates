using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace MouseMacro
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    /// 챌린지 들어가자마자 생기는 확인버튼        
    /// 챌린지 결과버튼 두번클릭   다시 시작 최소 세번? 
    // 1번 파티 클릭 - 스테미나/스트레스 재충전 용도
    // 2번 파티 클릭 - 스테미나/스트레스 재충전 용도
    // 3번 파티 클릭 - 스테미나/스트레스 재충전 용도
    // 스테미나/스트레스 소모완료 > 경고 문구창 클릭
    public partial class MainWindow : Window
    {
   
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000; // 핫키 식별자

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        private bool isMessageBoxShowing = false;

        private bool isRunning = false;
        private List<POINT> points = new List<POINT>(); // 여러 좌표를 저장할 리스트
        private int currentStep = 0; // 현재 진행중인 단계


        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);




        // Windows API 함수 선언
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        public struct POINT
        {
            public int X;
            public int Y;

            public override string ToString() => $"X: {X}, Y: {Y}";
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            RegisterGlobalHotKey();
            // 커맨드 초기화
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            UnregisterGlobalHotKey();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterGlobalHotKey();
        }


        protected override void OnClosed(EventArgs e)
        {
            UnregisterGlobalHotKey();
            base.OnClosed(e);
        }
          private HwndSource _source;
        private void RegisterGlobalHotKey()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;

            if (handle == IntPtr.Zero)
            {
               // MessageBox.Show("Window handle is not valid.");
                return;
            }

            _source = HwndSource.FromHwnd(handle);
            _source.AddHook(HwndHook);

            // Register Alt+X as global hotkey
            RegisterHotKey(handle, HOTKEY_ID, (uint)ModifierKeys.Alt, (uint)KeyInterop.VirtualKeyFromKey(Key.X));
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                StopMacro();
                //MessageBox.Show("Macro stopped by Alt+X shortcut!");
                return IntPtr.Zero;
            }
            return IntPtr.Zero;
        }

        private void UnregisterGlobalHotKey()
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            UnregisterHotKey(handle, HOTKEY_ID);

            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source = null;
            }
        }


        private void StopMacro()
        {
            if (isMessageBoxShowing)
            {
                // 메시지 박스가 이미 표시되고 있는 경우, 아무 것도 하지 않음
                return;
            }

            // 반복이 이미 중지된 상태일 때
            if (!isRunning)
            {
                isMessageBoxShowing = true;

                // 현재 창을 최상위로 설정
                this.Topmost = true;

                // 메시지 박스 표시 후 작업
                MessageBox.Show(this, "실행 중이지 않습니다", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                // 메시지 박스가 닫히면 표시 상태를 리셋하고 창을 원래 상태로 복원
                isMessageBoxShowing = false;
                this.Topmost = false;

                return; // 메소드 종료
            }

            // 반복 중지를 처리하는 코드
            isRunning = false;
            isMessageBoxShowing = true;

            // 현재 창을 최상위로 설정
            this.Topmost = true;

            // 메시지 박스를 표시하여 반복 횟수를 출력
            MessageBox.Show(this, $"반복을 중지합니다. \n  총 {repeatCount}번 반복했습니다.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

            // 메시지 박스가 닫히면 표시 상태를 리셋하고 창을 원래 상태로 복원
            isMessageBoxShowing = false;
            this.Topmost = false;

            // 반복 카운트 초기화 (필요한 경우)
            repeatCount = 0;
        }



        private void CoordinateBtn_Click(object sender, RoutedEventArgs e)
        {
            var explain = MessageBox.Show("원하는 좌표에 마우스를 올려두고 Enter를 누르세요. \n \n 지금부터 키보드만을 사용하세요. \n 다음 창이 떠도 키보드 Enter로 진행합니다 ", "좌표 저장 확인", MessageBoxButton.OK, MessageBoxImage.Question);

            // 사용자가 좌표를 저장할지 확인하는 대화상자 표시
            var result = MessageBox.Show("마우스를 이동하지 마시고 키보드의 Enter를 누르세요. \n 현재 좌표를 저장하시겠습니까? \n", "좌표 저장 확인", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 현재 마우스 좌표를 리스트에 저장
                POINT point;
                GetCursorPos(out point);
                points.Add(point);

                // ListBox에 좌표를 추가하여 표시
                CoordinatesListBox.Items.Add(point);
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            // 매크로 실행 시작
            if (points.Count == 0)
            {
                MessageBox.Show("좌표를 지정해주세요.");
                return;
            }

            isRunning = true;
            Thread thread = new Thread(RunMacro);
            thread.Start();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            StopMacro();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // 프로그램 종료
            CloseApplication();
        }

        private void CloseApplication()
        {
            isRunning = false;
            Application.Current.Shutdown();
        }
        private int repeatCount = 0; // 반복 횟수를 추적하는 변수
        private void RunMacro()
        {
            repeatCount = 0; // 반복 카운트 초기화
            while (isRunning)
            {
                var point = points[currentStep]; // 현재 단계의 좌표를 가져옴
                SetCursorPos(point.X, point.Y);
                Thread.Sleep(100); // 잠시 대기
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

                // 다음 단계로 진행, 마지막 단계면 처음으로 되돌아감
                currentStep = (currentStep + 1) % points.Count;

                // 첫 번째 단계로 돌아왔을 때 반복 횟수를 증가시키고 Label 업데이트
                if (currentStep == 0)
                {
                    repeatCount++;
                    UpdateRepeatCountLabel(); // Label 업데이트 메서드 호출
                }

                // 작업 사이의 대기 시간 (필요에 따라 조정)
                Thread.Sleep(3000);
            }
            currentStep = 0; // 중지될 때 초기화
            repeatCount = 0; // 반복 카운트도 초기화
            UpdateRepeatCountLabel(); // 중지 후 Label 초기화
        }

        private void UpdateRepeatCountLabel()
        {
            // Dispatcher를 사용하여 UI 스레드에서 Label을 업데이트
            Dispatcher.Invoke(() =>
            {
                RepeatCountLabel.Content = $"반복 횟수: {repeatCount}"; // Label의 Content 속성 업데이트
            });
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // 선택된 항목이 있는지 확인
            if (CoordinatesListBox.SelectedItem != null)
            {
                // 선택된 항목의 인덱스 가져오기
                int index = CoordinatesListBox.SelectedIndex;

                // 선택된 항목을 가져옴
                var selectedPoint = (POINT)CoordinatesListBox.SelectedItem;

                // 좌표와 몇 번째 항목인지를 표시하고 삭제 여부를 묻는 메시지 박스 출력
                string message = $"선택된 좌표 (X : {selectedPoint.X},  Y : {selectedPoint.Y})는 리스트에서 {index + 1}번째 항목입니다.\n삭제하시겠습니까?";
                var result = MessageBox.Show(message, "좌표 삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // 사용자가 "Yes"를 선택했을 때만 삭제 수행
                if (result == MessageBoxResult.Yes)
                {
                    // ListBox에서 선택된 항목을 제거
                    CoordinatesListBox.Items.RemoveAt(index);

                    // points 리스트에서 해당 좌표를 제거
                    points.RemoveAt(index);
                }
            }
            else
            {
                // 선택된 항목이 없을 경우 경고 메시지 출력
                MessageBox.Show("삭제할 항목을 선택하세요.", "항목 선택", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
           // var explain = MessageBox.Show("원하는 좌표에 마우스를 올려두고 Enter를 누르세요. \n \n 지금부터 키보드만을 사용하세요. \n 다음 창이 떠도 키보드 Enter로 진행합니다 ", "좌표 저장 확인", MessageBoxButton.OK, MessageBoxImage.Question);

            // 사용자가 좌표를 저장할지 확인하는 대화상자 표시
            var result = MessageBox.Show("마우스를 이동하지 마시고 키보드의 Enter를 누르세요. \n 해당좌표는 선택된 리스트 아래에 삽입됩니다 \n 현재 좌표를 저장하시겠습니까? \n", "좌표 저장 확인", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 현재 마우스 좌표를 가져옴
                POINT point;
                GetCursorPos(out point);

                // ListBox에서 선택된 항목의 인덱스를 가져옴
                int selectedIndex = CoordinatesListBox.SelectedIndex;

                if (selectedIndex >= 0 && selectedIndex < CoordinatesListBox.Items.Count)
                {
                    // 선택된 항목 사이에 새로운 좌표를 추가
                    points.Insert(selectedIndex + 1, point);  // 리스트에 좌표 추가
                    CoordinatesListBox.Items.Insert(selectedIndex + 1, point);  // ListBox에 좌표 추가
                }
                else
                {
                    // 선택된 항목이 없거나 리스트가 비어 있을 경우, 리스트 끝에 추가
                    points.Add(point);
                    CoordinatesListBox.Items.Add(point);
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            // 삭제 여부를 묻는 메시지 박스 출력
            var result = MessageBox.Show("정말 초기화 하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            // 사용자가 "Yes"를 선택했을 때만 리스트와 ListBox를 비움
            if (result == MessageBoxResult.Yes)
            {
                // ListBox의 항목을 모두 제거
                CoordinatesListBox.Items.Clear();

                // 좌표 리스트를 비워서 데이터도 제거
                points.Clear();
            }
        }
    }
}

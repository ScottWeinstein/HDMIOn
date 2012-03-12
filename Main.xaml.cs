using System;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace HDMIOn
{
    public partial class Main : Window
    {
        #region interop

        private const int MONITOR_OFF = 2;
        private const int MONITOR_ON = -1;
        private const int WM_SYSCOMMAND = 274;
        private const int SC_MONITORPOWER = 61808;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr GetForegroundWindow();
        #endregion

        #region Dependency Props
        public static readonly DependencyProperty RefreshTimeLeftProperty = DependencyProperty.Register("RefreshTimeLeft", typeof(int), typeof(Main), new UIPropertyMetadata(0));

        public int RefreshTimeLeft
        {
            get
            {
                return (int)GetValue(RefreshTimeLeftProperty);
            }
            set
            {
                SetValue(RefreshTimeLeftProperty, value);
            }
        }

        public static readonly DependencyProperty RefreshRateProperty = DependencyProperty.Register("RefreshRate", typeof(int), typeof(Main), new UIPropertyMetadata(30));

        public int RefreshRate
        {
            get
            {
                return (int)GetValue(RefreshRateProperty);
            }
            set
            {
                SetValue(RefreshRateProperty, value);
            }
        }
        #endregion
        

        public Main()
        {
            InitializeComponent();
            DataContext = this;
            var _tmr = new System.Windows.Threading.DispatcherTimer();

            var _tmrLeft = new System.Windows.Threading.DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            int running = 0;
            _tmrLeft.Tick += (sender, e) =>
            {
                RefreshTimeLeft = RefreshRate - (++running % RefreshRate);
            };
            _tmrLeft.Start();

            _tmr.Interval = new TimeSpan(0, 0, RefreshRate);
            _tmr.Tick += _tmr_Tick;
            _tmr.Start();
        }


        void _tmr_Tick(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
            SendMessage(helper.Handle, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_ON);
        }
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            _tmr_Tick(this, null);
        }

        private void _keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Space)
            {
                Close();
            }
        }
    }
}


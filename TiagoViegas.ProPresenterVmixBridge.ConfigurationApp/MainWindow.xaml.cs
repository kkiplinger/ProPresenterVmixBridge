using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TiagoViegas.ProPresenterVmixBridge.Business.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.ConfigurationApp.IoC;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;

namespace TiagoViegas.ProPresenterVmixBridge.ConfigurationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly IConfigManager _configManager;
        public readonly Container _container;

        public MainWindow()
        {
            InitializeComponent();

            _container = IoCManager.CreateContainer();
            _configManager = _container.GetInstance<IConfigManager>();

            FillTextBoxes();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleEnable(false);

            _configManager.EditConfig(ConfigKeys.ProPresenterIp, ProPresenterIp.Text);
            _configManager.EditConfig(ConfigKeys.ProPresenterPort, ProPresenterPort.Text);
            _configManager.EditConfig(ConfigKeys.ProPresenterPassword, ProPresenterPassword.Text);
            _configManager.EditConfig(ConfigKeys.VmixIp, VmixIp.Text);
            _configManager.EditConfig(ConfigKeys.VmixPort, VmixPort.Text);
            _configManager.EditConfig(ConfigKeys.VmixInputNumber, VmixInput.Text);

            try
            {
                _configManager.SaveConfig();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Access is denied!\nPlease run this application as administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ToggleEnable(true);
                return;
            }
            

            Message.Content = "Checking configurations...";

            var proPresenterDA = _container.GetInstance<IProPresenterDataAgent>();
            var vmixDA = _container.GetInstance<IVmixDataAgent>();

            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.CancelAfter(5000);

            await proPresenterDA.Connect(cancellationTokenSource.Token);

            var vmixReachable = await vmixDA.CheckConfig();

            Message.Content = "";

            if (!proPresenterDA.Connected || !vmixReachable)
            {
                var message = new StringBuilder();

                if (!proPresenterDA.Connected)
                {
                    message.AppendLine("Could not connect to ProPresenter.");
                }

                if (!vmixReachable)
                {
                    message.AppendLine("Could not connect to Vmix.");
                }

                MessageBox.Show(message.ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                RestartService();
            }

            ToggleEnable(true);
        }

        private void ToggleEnable(bool isEnabled)
        {
            ApplyButton.IsEnabled = isEnabled;
            ProPresenterIp.IsEnabled = isEnabled;
            ProPresenterPort.IsEnabled = isEnabled;
            ProPresenterPassword.IsEnabled = isEnabled;
            VmixIp.IsEnabled = isEnabled;
            VmixPort.IsEnabled = isEnabled;
            VmixInput.IsEnabled = isEnabled;
        }

        private void RestartService()
        {
            ServiceController service = new ServiceController("ProPresenterVmixBridgeService");

            try
            {
                var displayName = service.DisplayName;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("ProPresenter Vmix Bridge service is not installed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Message.Content = "Service is not installed!";
                return;
            }

            try
            {
                if(service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Access is denied!\nPlease run this application as administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillTextBoxes()
        {
            try
            {
                _configManager.LoadConfig();

                ProPresenterIp.Text = _configManager.GetConfig(ConfigKeys.ProPresenterIp);
                ProPresenterPort.Text = _configManager.GetConfig(ConfigKeys.ProPresenterPort);
                ProPresenterPassword.Text = _configManager.GetConfig(ConfigKeys.ProPresenterPassword);

                VmixIp.Text = _configManager.GetConfig(ConfigKeys.VmixIp);
                VmixPort.Text = _configManager.GetConfig(ConfigKeys.VmixPort);
                VmixInput.Text = _configManager.GetConfig(ConfigKeys.VmixInputNumber);
            }
            catch (Exception e)
            {
                _configManager.EditConfig(ConfigKeys.ProPresenterIp, "127.0.0.1");
                _configManager.EditConfig(ConfigKeys.ProPresenterPort, "50001");
                _configManager.EditConfig(ConfigKeys.ProPresenterPassword, "");
                _configManager.EditConfig(ConfigKeys.VmixIp, "127.0.0.1");
                _configManager.EditConfig(ConfigKeys.VmixPort, "8088");
                _configManager.EditConfig(ConfigKeys.VmixInputNumber, "1");


                ProPresenterIp.Text = _configManager.GetConfig(ConfigKeys.ProPresenterIp);
                ProPresenterPort.Text = _configManager.GetConfig(ConfigKeys.ProPresenterPort);
                ProPresenterPassword.Text = _configManager.GetConfig(ConfigKeys.ProPresenterPassword);

                VmixIp.Text = _configManager.GetConfig(ConfigKeys.VmixIp);
                VmixPort.Text = _configManager.GetConfig(ConfigKeys.VmixPort);
                VmixInput.Text = _configManager.GetConfig(ConfigKeys.VmixInputNumber);
            }
            
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            RestartService();
        }
    }
}

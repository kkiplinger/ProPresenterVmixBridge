using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using TiagoViegas.ProPresenterVmixBridge.ConfigurationApp.Annotations;
using TiagoViegas.ProPresenterVmixBridge.ConfigurationApp.IoC;
using TiagoViegas.ProPresenterVmixBridge.Data.Interfaces;
using TiagoViegas.ProPresenterVmixBridge.Entities;
using Container = SimpleInjector.Container;

namespace TiagoViegas.ProPresenterVmixBridge.ConfigurationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public readonly IConfigManager _configManager;
        public readonly IProPresenterDataAgent _proPresenterDataAgent;
        public readonly Container _container;
        public ProPresenterInstance CurrentInstance { get; set; }


        private ObservableCollection<ProPresenterInstance> _proPresenterInstances = new ObservableCollection<ProPresenterInstance>();

        public ObservableCollection<ProPresenterInstance> ProPresenterInstances
        {
            get => _proPresenterInstances;
            set
            {
                _proPresenterInstances = value;
                RaisePropertyChanged("ProPresenterInstances");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            _container = IoCManager.CreateContainer();
            _configManager = _container.GetInstance<IConfigManager>();
            _proPresenterDataAgent = _container.GetInstance<IProPresenterDataAgent>();
            _proPresenterDataAgent.OnProPresenterInstancesChanged += UpdateComboBox;
            _proPresenterDataAgent.LookForProPresenter();

            FillTextBoxes();

            ProPresenterInstances = new ObservableCollection<ProPresenterInstance>();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleEnable(false);

            CurrentInstance = (ProPresenterInstance) NetworkNames.SelectionBoxItem;

            _configManager.EditConfig(ConfigKeys.ProPresenterName,
                CurrentInstance.Name);
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

            var vmixDA = _container.GetInstance<IVmixDataAgent>();

            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.CancelAfter(5000);

            await _proPresenterDataAgent.Connect(CurrentInstance.Name, cancellationTokenSource.Token);

            var vmixReachable = await vmixDA.CheckConfig();

            Message.Content = "";

            if (!_proPresenterDataAgent.Connected || !vmixReachable)
            {
                var message = new StringBuilder();

                if (!_proPresenterDataAgent.Connected)
                {
                    message.AppendLine("Could not connect to ProPresenter.");
                }
                else
                {
                    await _proPresenterDataAgent.Close();
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
            NetworkNames.IsEnabled = isEnabled;
            ApplyButton.IsEnabled = isEnabled;
            ProPresenterPassword.IsEnabled = isEnabled;
            VmixIp.IsEnabled = isEnabled;
            VmixPort.IsEnabled = isEnabled;
            VmixInput.IsEnabled = isEnabled;
        }

        private void UpdateComboBox(object sender, ProPresenterInstancesChangedEventArgs args)
        {
            var networkName = _configManager.GetConfig(ConfigKeys.ProPresenterName);

            ProPresenterInstances = new ObservableCollection<ProPresenterInstance>(args.Instances);

            if (args.Instances.Any(x => x.Name == networkName))
            {
                NetworkNames.SelectedItem = ProPresenterInstances.First(x => x.Name == networkName);
                RaisePropertyChanged("InstanceName");
            }
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

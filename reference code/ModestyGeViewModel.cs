using MicroMvvm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace ModestyGE
{
   public class BoundingBox
   {
      #region Declarations

      private double _top;

      public double Top
      {
         get { return _top; }
         set { _top = value; }
      }

      private double _bottom;

      public double Bottom
      {
         get { return _bottom; }
         set { _bottom = value; }
      }

      private double _left;

      public double Left
      {
         get { return _left; }
         set { _left = value; }
      }

      private double _right;

      public double Right
      {
         get { return _right; }
         set { _right = value; }
      }
      
      public BoundingBox()
      {
         _top = 0;
         _bottom = 0;
         _left = 0;
         _right = 0;           
      }

      public BoundingBox(double top, double bottom, double left, double right)
      {
         _top = top;
         _bottom = bottom;
         _left = left;
         _right = right;
      }

      #endregion
   }




   public class ModestyGeViewModel : ViewModelBase
   {
      #region Types

      public enum AppState
      {
         editing,
         preview,
         simRunning,
         recordingAnalysis
      }

      #endregion

      #region Declarations

      private AppState _mainState;

      private string _defaultLibraryFile;

      private string _simulationFile;

      private SimulationTop _simTop;
      private ModesLibrary _libTop;

      private TreeViewViewModel _treeTopViewModel = new TreeViewViewModel();
      private ModesLibraryTreeViewViewModel _libTopViewModel = new ModesLibraryTreeViewViewModel();
      private SignalModesTreeViewViewModel _signalModeViewModel = new SignalModesTreeViewViewModel();
      private ScanModesTreeViewViewModel _scanModeViewModel = new ScanModesTreeViewViewModel();
      private SimulationControlTreeViewViewModel _simCntrlVm;

      // Fields for handling right-click on map
      private PendingRightClickWaypoint _pendingRightClickWp;
      private PendingRightClickPosRot _pendingRightClickPr;
      private bool _pendingRightClick;

      ObservableCollection<PositionRotationTreeViewViewModel> _platforms;

      ObservableCollection<MapImage> _mapImages = new ObservableCollection<MapImage>();
      private int _pathIndex;
      private DispatcherTimer _previewTickTimer;

      private MainWindow _mainWindow;

      private bool _simIsDirty;
      private bool _libIsDirty;


      private AsynchSocketServer _server;
      private Process _client;
      private string _statusMessage;
      private DispatcherTimer _acceptTimer;
      private DispatcherTimer _readTimer;
      private bool _interruptByIntent;
      private bool _simIsRunning;

      private BackgroundWorker _flightPathWorker = new BackgroundWorker();

      private Random _rnd = new Random();

      #endregion

      #region Constructor

      public ModestyGeViewModel()
      {
         _pendingRightClickWp = null;
         _pendingRightClickPr = null;
         _pendingRightClick = false;

         _simTop = new SimulationTop();
         _platforms = new ObservableCollection<PositionRotationTreeViewViewModel>();

         _previewTickTimer = new DispatcherTimer();
         _previewTickTimer.Tick += new EventHandler(disaptchTimer_Tick);
         _previewTickTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);

         _simIsDirty = false;

         _libIsDirty = false;

         MapImage mw = new MapImage();

         mw.Source = "World.png";
         mw.MapCoord = new Point(-180, 90);
         mw.MapSize = new Size(360, 180);

         MapImages.Add(mw);

         MapImage mi = new MapImage();

         mi.Source = "Stockholm.png";
         mi.MapCoord = new Point(17.454078, 59.506259);
         mi.MapSize = new Size(18.807497 - 17.454078, 59.506259 - 59.140181);

         MapImages.Add(mi);

         _defaultLibraryFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LibraryData\\simulationsLibrary.xml");

         ReadLibrary();
         BuildTreeViewViewModel();

         MainState = AppState.editing;

         _client = null;
         _server = null;
         _acceptTimer = new DispatcherTimer();
         _acceptTimer.Tick += new EventHandler(HandleSocketAcceptTimeout);
         _acceptTimer.Interval = new TimeSpan(0, 0, 0, 0, 2000);

         _readTimer = new DispatcherTimer();
         _readTimer.Tick += new EventHandler(HandleMessageTimeout);
         _readTimer.Interval = new TimeSpan(0, 0, 0, 0, 2000);

         StartTheServer();

         StatusMessage = "Welcome to Modesty GE";

         _flightPathWorker.WorkerReportsProgress = true;
         _flightPathWorker.DoWork += _flightPathWorker_FligthPathWork;
         _flightPathWorker.RunWorkerCompleted += _flightPathWorker_FlightPathComplete;
         _flightPathWorker.ProgressChanged += _flightPathWorker_ProgressReporter;

      }

      #endregion

      #region Properties

      public bool PendingRightClick
      {
         get { return _pendingRightClick; }
         set
         {
            _pendingRightClick = value;

            if (value)
            {
               StatusMessage = "Right Click on Map to set point";
            }

            OnPropertyChanged("PendingRightClick");
         }
      }

      public SimulationControlTreeViewViewModel SimControlViewModel
      {
         get { return _simCntrlVm; }
         set
         {
            _simCntrlVm = value;
            OnPropertyChanged("SimControlViewModel");
         }
      }

      public AppState MainState
      {
         get { return _mainState; }
         set
         {
            // Turn off play if previous state was preview
            if ((_mainState == AppState.preview) && (value != AppState.preview))
            {
               _previewTickTimer.Stop();
            }

            _mainState = value;

            OnPropertyChanged("MainState");
         }
      }

      public string SimulationFile
      {
         get { return _simulationFile; }
         set
         {
            _simulationFile = value;
            OnPropertyChanged("SimulationFile");
         }
      }

      public TreeViewViewModel TreeTopViewModel
      {
         get { return _treeTopViewModel; }
         set
         {
            _treeTopViewModel = value;
            OnPropertyChanged("TreeTopViewModel");
         }
      }

      public SimulationTop SimTop
      {
         get { return _simTop; }
         set
         {
            _simTop = value;
            OnPropertyChanged("SimTop");
         }
      }

      public ModesLibrary LibTop
      {
         get { return _libTop; }
         set
         {
            _libTop = value;
            OnPropertyChanged("LibTop");
         }
      }

      public ModesLibraryTreeViewViewModel LibTopViewModel
      {
         get { return _libTopViewModel; }
         set
         {
            _libTopViewModel = value;
            OnPropertyChanged("LibTopViewModel");
         }
      }

      public SignalModesTreeViewViewModel SignalModeViewModel
      {
         get { return _signalModeViewModel; }
         set
         {
            _signalModeViewModel = value;
            OnPropertyChanged("SignalModeViewModel");
         }
      }

      public ScanModesTreeViewViewModel ScanModeViewModel
      {
         get { return _scanModeViewModel; }
         set
         {
            _scanModeViewModel = value;
            OnPropertyChanged("ScanModeViewModel");
         }
      }
      
      public ObservableCollection<PositionRotationTreeViewViewModel> Platforms
      {
         get { return _platforms; }
         set { _platforms = value; }
      }

      public int PathIndex
      {
         get { return _pathIndex; }
         set
         {
            _pathIndex = value;

            foreach (PositionRotationTreeViewViewModel tvvm in Platforms)
            {
               tvvm.Platform.PathIndex = value;
            }

            OnPropertyChanged("PathIndex");
         }
      }

      public ObservableCollection<MapImage> MapImages
      {
         get { return _mapImages; }
         set { _mapImages = value; }
      }

      public MainWindow MainWindow
      {
         get { return _mainWindow; }
         set { 
            _mainWindow = value;

            if (_mainWindow != null)
            {
               _mainWindow.Closing += this.OnClosing;
            }

         }
      }

      public bool SimIsDirty
      {
         get { return _simIsDirty; }
         set
         {
            _simIsDirty  = value;
         }
      }

      public bool LibIsDirty
      {
         get { return _libIsDirty; }
         set
         {
            _libIsDirty = value;
         }
      }

      public string StatusMessage
      {
         get { return _statusMessage; }
         set
         {
            _statusMessage = value;
            OnPropertyChanged("StatusMessage");
         }
      }

      public bool SimIsRunning
      {
         get { return _simIsRunning; }
         set
         {
            _simIsRunning = value;
            OnPropertyChanged("SimIsRunning");
         }
      }
      

      #endregion

      #region Commands

      void SetMainStateEditingExecute(Object parameter)
      {
         MainState = AppState.editing;
      }

      bool CanSetMainStateEditingExecute(Object parameter)
      {
         return ((MainState != AppState.editing) && (!SimIsRunning));
      }

      public ICommand SetMainStateEditing
      {
         get
         {
            return new RelayCommand<Object>(parameter => SetMainStateEditingExecute(parameter), parameter => CanSetMainStateEditingExecute(parameter));
         }
      }

      void CancelRightClickExecute(Object parameter)
      {
         _pendingRightClickPr = null;
         _pendingRightClickWp = null;
         PendingRightClick = false;
         StatusMessage = "";
      }

      bool CanCancelRightClickExecute(Object parameter)
      {
         return PendingRightClick;
      }

      public ICommand CancelRightClick
      {
         get
         {
            return new RelayCommand<Object>(parameter => CancelRightClickExecute(parameter), parameter => CanCancelRightClickExecute(parameter));
         }
      }



      private void _flightPathWorker_FligthPathWork(object sender, DoWorkEventArgs e)
      {
         System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
         customCulture.NumberFormat.NumberDecimalSeparator = ".";

         System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

         int i = 0;

         foreach (PositionRotationTreeViewViewModel vm in Platforms)
         {
            vm.Platform.UpdateFlightPath();

            i++;

            _flightPathWorker.ReportProgress(100 * i / Platforms.Count);
         }

      }

      private void _flightPathWorker_ProgressReporter(object sender, ProgressChangedEventArgs e)
      {
         StatusMessage = "Generating Flight Paths. " + e.ProgressPercentage.ToString() + "% complete";
      }

      private void _flightPathWorker_FlightPathComplete(object sender, RunWorkerCompletedEventArgs e)
      {
         StatusMessage = "Flight Paths generated.";

         MainState = AppState.preview;

         // Needed to update Can-Execute of commands, Don't know why, possibly something
         // to do with this being an event handler for an event from another thread
         CommandManager.InvalidateRequerySuggested();
      }

      void SetMainStatePreviewExecute(Object parameter)
      {

         StatusMessage = "Generating Flight Paths...";

         _flightPathWorker.RunWorkerAsync();
      }

      bool CanSetMainStatePreviewExecute(Object parameter)
      {
         return ((MainState != AppState.preview) && (Platforms.Count > 0) && (!SimIsRunning));
      }

      public ICommand SetMainStatePreview
      {
         get
         {
            return new RelayCommand<Object>(parameter => SetMainStatePreviewExecute(parameter), parameter => CanSetMainStatePreviewExecute(parameter));
         }
      }

      void SetMainStateRunningExecute(Object parameter)
      {
         if (SimIsDirty)
         {
            MessageBoxResult res = MessageBox.Show("Simulation is not saved, do you want to continue and run the simulation with previously saved data?", "Unsaved Simulation", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (res == MessageBoxResult.OK)
            {
               MainState = AppState.simRunning;

               RunSimulation();
            }
         }
         else
         {
            MainState = AppState.simRunning;

            RunSimulation();
         }

     }

      bool CanSetMainStateRunningExecute(Object parameter)
      {
         return (MainState == AppState.preview);
      }

      public ICommand SetMainStateRunning
      {
         get
         {
            return new RelayCommand<Object>(parameter => SetMainStateRunningExecute(parameter), parameter => CanSetMainStateRunningExecute(parameter));
         }
      }

      void SetMainStateAnalysingExecute(Object parameter)
      {
         MainState = AppState.recordingAnalysis;
      }

      bool CanSetMainStateAnalysingExecute(Object parameter)
      {
         return false;
      }

      public ICommand SetMainStateAnalysing
      {
         get
         {
            return new RelayCommand<Object>(parameter => SetMainStateAnalysingExecute(parameter), parameter => CanSetMainStateAnalysingExecute(parameter));
         }
      }

      void TerminateSimulationExecute(Object parameter)
      {
         _interruptByIntent = true;
         InterruptSimulation();
      }

      bool CanTerminateSimulationExecute(Object parameter)
      {
         return SimIsRunning;
      }

      public ICommand TerminateSimulation
      {
         get
         {
            return new RelayCommand<Object>(parameter => TerminateSimulationExecute(parameter), parameter => CanTerminateSimulationExecute(parameter));
         }
      }

      void RerunSimulationExecute(Object parameter)
      {
         RunSimulation();
      }

      bool CanRerunSimulationExecute(Object parameter)
      {
         return !SimIsRunning;
      }

      public ICommand RerunSimulation
      {
         get
         {
            return new RelayCommand<Object>(parameter => RerunSimulationExecute(parameter), parameter => CanRerunSimulationExecute(parameter));
         }
      }

      void ReloadLibraryExecute(Object parameter)
      {
         ReadLibrary();
      }

      bool CanReloadLibraryExecute(Object parameter)
      {
         return (MainState == AppState.editing);
      }

      public ICommand ReloadLibrary
      {
         get
         {
            return new RelayCommand<Object>(parameter => ReloadLibraryExecute(parameter), parameter => CanReloadLibraryExecute(parameter));
         }
      }

      void SaveLibraryExecute(Object parameter)
      {
         WriteLibrary(_defaultLibraryFile);
      }

      bool CanSaveLibraryExecute(Object parameter)
      {
         return LibIsDirty;
      }

      public ICommand SaveLibrary
      {
         get
         {
            return new RelayCommand<Object>(parameter => SaveLibraryExecute(parameter), parameter => CanSaveLibraryExecute(parameter));
         }
      }

      void ExportLibraryExecute(Object parameter)
      {
         var fileDialog = new SaveFileDialog();

         fileDialog.CheckFileExists = false;

         fileDialog.Filter = "Modes Library File (*.xml)|*.xml";

         var result = fileDialog.ShowDialog();

         if (result == true)
         {
            WriteLibrary(fileDialog.FileName);
         }
      }

      bool CanExportLibraryExecute(Object parameter)
      {
         return true;
      }

      public ICommand ExportLibrary
      {
         get
         {
            return new RelayCommand<Object>(parameter => ExportLibraryExecute(parameter), parameter => CanExportLibraryExecute(parameter));
         }
      }

      void FitViewExecute(Object parameter)
      {
         SetBoundingBox();
      }

      bool CanFitViewExecute(Object parameter)
      {
         return (Platforms.Count > 0);
      }

      public ICommand FitView
      {
         get
         {
            return new RelayCommand<Object>(parameter => FitViewExecute(parameter), parameter => CanFitViewExecute(parameter));
         }
      }

      void FitMapExecute(Object parameter)
      {
         MainWindow.zoomAndPanControl.ScaleToFit();

      }

      bool CanFitMapExecute(Object parameter)
      {
         return true;
      }

      public ICommand FitMap
      {
         get
         {
            return new RelayCommand<Object>(parameter => FitMapExecute(parameter), parameter => CanFitMapExecute(parameter));
         }
      }


      void ZoomInExecute(Object parameter)
      {
         MainWindow.ZoomIn_Executed(null, null);
      }

      bool CanZoomInExecute(Object parameter)
      {
         return true;
      }

      public ICommand ZoomIn
      {
         get
         {
            return new RelayCommand<Object>(parameter => ZoomInExecute(parameter), parameter => CanZoomInExecute(parameter));
         }
      }

      void ZoomOutExecute(Object parameter)
      {
         MainWindow.ZoomOut_Executed(null, null);
      }

      bool CanZoomOutExecute(Object parameter)
      {
         return true;
      }

      public ICommand ZoomOut
      {
         get
         {
            return new RelayCommand<Object>(parameter => ZoomOutExecute(parameter), parameter => CanZoomOutExecute(parameter));
         }
      }

      void PlayExecute(Object parameter)
      {
         _previewTickTimer.Start();
      }

      bool CanPlayExecute(Object parameter)
      {
         return ((MainState == AppState.preview) && (Platforms.Count > 0));
      }

      public ICommand Play
      {
         get
         {
            return new RelayCommand<Object>(parameter => PlayExecute(parameter), parameter => CanPlayExecute(parameter));
         }
      }

      void PauseExecute(Object parameter)
      {
         _previewTickTimer.Stop();
      }

      bool CanPauseExecute(Object parameter)
      {
         return ((MainState == AppState.preview) && (Platforms.Count > 0));
      }

      public ICommand Pause
      {
         get
         {
            return new RelayCommand<Object>(parameter => PauseExecute(parameter), parameter => CanPauseExecute(parameter));
         }
      }

      void RewindExecute(Object parameter)
      {
         PathIndex = 0;
      }

      bool CanRewindExecute(Object parameter)
      {
         return ((MainState == AppState.preview) && (Platforms.Count > 0));
      }

      public ICommand Rewind
      {
         get
         {
            return new RelayCommand<Object>(parameter => RewindExecute(parameter), parameter => CanRewindExecute(parameter));
         }
      }


      void NewSimulationExecute(Object parameter)
      {
         // TODO: Check if SimIsDirty and if so, prompt user to save it.

         // This is Create of Simulation, not the total top since library emitters are in the total top
         NewSimulationViewModel nsvm = new NewSimulationViewModel();

         // Prompt user for Simulation Name, directory, and configuration
         // - Dialog with name, path (defaults to working dir) and configuration

         nsvm.SimAbsoluteFile = Directory.GetCurrentDirectory() + "\\newSimulationName.xml";
         nsvm.ConfType = SimulationConfiguration.SimConfigType.rwr;
         nsvm.HwConfiguration = 0; // Sweden configuration

         NewSimDlg nsdlg = new NewSimDlg();
         nsdlg.NS = nsvm;
         nsdlg.DataContext = nsvm;

         nsdlg.ShowDialog();

         if (nsdlg.DialogResult == true)
         {
            SimTop.Create(nsvm.SimAbsoluteFile, nsvm.ConfType);

            Platforms.Clear();

            if (nsvm.HwConfiguration == 0)
            {
               SimTop.ConfigLib.SetCpPreset(1);
            }
            else if (nsvm.HwConfiguration == 1)
            {
               SimTop.ConfigLib.SetCpPreset(1);
            }

            if (nsvm.ConfType == SimulationConfiguration.SimConfigType.rxOnly)
            {
               SimTop.ConfigLib.SetldmPreset(1);
            }
            else if (nsvm.ConfType == SimulationConfiguration.SimConfigType.rwr)
            {
               SimTop.ConfigLib.SetldmPreset(2);
            }
            else
            {
               SimTop.ConfigLib.SetldmPreset(3);
            }

            BuildSimulationTreeViewViewModel();

            SimIsDirty = true;
         }
      }

      bool CanNewSimulationExecute(Object parameter)
      {
         return (MainState == AppState.editing);
      }

      public ICommand NewSimulation
      {
         get
         {
            return new RelayCommand<Object>(parameter => NewSimulationExecute(parameter), parameter => CanNewSimulationExecute(parameter));
         }
      }


      void OpenSimulationExecute(Object parameter)
      {
         //TODO: Check if current sim is dirty and if so, prompt for saving it.

         OpenFileDialog fileDialog = new OpenFileDialog();

         fileDialog.CheckFileExists = true;

         fileDialog.Filter = "Modesty Simulation Control File (*.xml)|*.xml";

         var result = fileDialog.ShowDialog();

         if (result == true)
         {
            SimulationFile = fileDialog.FileName;

            SimulationTop st = new SimulationTop();

            if (st.ReadSimulation(SimulationFile))
            {
               SimTop = st;
               BuildSimulationTreeViewViewModel();
               SimIsDirty = false;
               SetBoundingBox();
            }
            else
            {
               SimTop = null;
               SimIsDirty = false;
            }
         }
      }

      bool CanOpenSimulationExecute(Object parameter)
      {
         return (MainState == AppState.editing);
      }

      public ICommand OpenSimulation
      {
         get
         {
            return new RelayCommand<Object>(parameter => OpenSimulationExecute(parameter), parameter => CanOpenSimulationExecute(parameter));
         }
      }

      void SaveSimulationExecute(Object parameter)
      {
         WriteSimulation(SimTop.AbsoluteSimFile);
         SimIsDirty = false;

      }

      bool CanSaveSimulationExecute(Object parameter)
      {
         return SimIsDirty;
      }

      public ICommand SaveSimulation
      {
         get
         {
            return new RelayCommand<Object>(parameter => SaveSimulationExecute(parameter), parameter => CanSaveSimulationExecute(parameter));
         }
      }


      void EditItemExecute(Object parameter)
      {
         if (parameter is WaypointTreeViewViewModel)
         {
            EditWaypoint(parameter as WaypointTreeViewViewModel);
         }

         if (parameter is SignalModeTreeViewViewModel)
         {
            EditSignalMode(parameter as SignalModeTreeViewViewModel);
         }

         if (parameter is RfEmitterTreeViewViewModel)
         {
            EditRfEmitter(parameter as RfEmitterTreeViewViewModel);
         }

         if (parameter is PositionRotationTreeViewViewModel)
         {
            EditPositionRotation(parameter as PositionRotationTreeViewViewModel);
         }

         if (parameter is SimulationControlTreeViewViewModel)
         {
            EditSimulationControl(parameter as SimulationControlTreeViewViewModel);
         }

         if (parameter is EmitterModeTreeViewViewModel)
         {
            EditEmitterMode(parameter as EmitterModeTreeViewViewModel);
         }

         if (parameter is RadarEmitterModeTreeViewViewModel)
         {
            EditRadarEmitterMode(parameter as RadarEmitterModeTreeViewViewModel);
         }

         if (parameter is AntennaScanModeTreeViewViewModel)
         {
            EditScanMode(parameter as AntennaScanModeTreeViewViewModel);
         }

         if (parameter is LibrarySetTreeViewViewModel)
         {
            EditLibrarySet(parameter as LibrarySetTreeViewViewModel);
         }

         if (parameter is ConfigLibraryTreeViewViewModel)
         {
            EditConfigurationLibrary(parameter as ConfigLibraryTreeViewViewModel);
         }

         if (parameter is LmhbrxLibraryTreeViewViewModel)
         {
            EditLmhbrxLibrary(parameter as LmhbrxLibraryTreeViewViewModel);
         }

         if (parameter is OwnRadarTreeViewViewModel)
         {
            EditOwnRadar(parameter as OwnRadarTreeViewViewModel);
         }

         if (parameter is RecLibraryTreeViewViewModel)
         {
            EditRecordingLibrary(parameter as RecLibraryTreeViewViewModel);
         }

         if (parameter is SignalModeGroupTreeViewViewModel)
         {
            EditSignalModeGroup(parameter as SignalModeGroupTreeViewViewModel);
         }

         if (parameter is ScanModeGroupTreeViewViewModel)
         {
            EditScanModeGroup(parameter as ScanModeGroupTreeViewViewModel);
         }
      }

      bool CanEditItemExecute(Object parameter)
      {
         if (MainState != AppState.editing)
         {
            return false;
         }

         if (parameter is WaypointTreeViewViewModel)
         {
            return true;
         }

         if (parameter is SignalModeTreeViewViewModel)
         {
            return true;
         }

         if (parameter is RfEmitterTreeViewViewModel)
         {
            return true;
         }

         if (parameter is PositionRotationTreeViewViewModel)
         {
            return true;
         }

         if (parameter is SimulationControlTreeViewViewModel)
         {
            return true;
         }

         if (parameter is EmitterModeTreeViewViewModel)
         {
            return true;
         }

         if (parameter is RadarEmitterModeTreeViewViewModel)
         {
            return true;
         }

         if (parameter is AntennaScanModeTreeViewViewModel)
         {
            return true;
         }

         if (parameter is LibrarySetTreeViewViewModel)
         {
            return true;
         }

         if (parameter is ConfigLibraryTreeViewViewModel)
         {
            return true;
         }

         if (parameter is LmhbrxLibraryTreeViewViewModel)
         {
            return true;
         }

         if (parameter is OwnRadarTreeViewViewModel)
         {
            return true;
         }

         if (parameter is RecLibraryTreeViewViewModel)
         {
            return true;
         }

         if (parameter is SignalModeGroupTreeViewViewModel)
         {
            return true;
         }

         if (parameter is ScanModeGroupTreeViewViewModel)
         {
            return true;
         }

         return false;
      }

      public ICommand EditItem
      {
         get
         {
            return new RelayCommand<Object>(parameter => EditItemExecute(parameter), parameter => CanEditItemExecute(parameter));
         }
      }

      void RemoveItemExecute(Object parameter)
      {
         if (parameter is WaypointTreeViewViewModel)
         {
            RemoveWp(parameter as WaypointTreeViewViewModel);
            SimIsDirty = true;
            PositionRotationTreeViewViewModel parent = (PositionRotationTreeViewViewModel)((TreeViewViewModel)parameter).Parent;
            parent.GeometryIsDirty = true;
         }
         
         if (parameter is RadarEmitterModeTreeViewViewModel)
         {
            RemoveRadarEmitterMode(parameter as RadarEmitterModeTreeViewViewModel);
            SimIsDirty = true;
         }
         
         if (parameter is EmitterModeTreeViewViewModel)
         {
            RemoveRfEmitterMode(parameter as EmitterModeTreeViewViewModel);
            SimIsDirty = true;
         }
         
         if (parameter is RfEmitterTreeViewViewModel)
         {
            RemoveRfEmitter(parameter as RfEmitterTreeViewViewModel);
            SimIsDirty = true;
         }

         if (parameter is SignalModeGroupTreeViewViewModel)
         {
            RemoveSignalModeGroup(parameter as SignalModeGroupTreeViewViewModel);
            LibIsDirty = true;
         }

         if (parameter is ScanModeGroupTreeViewViewModel)
         {
            RemoveScanModeGroup(parameter as ScanModeGroupTreeViewViewModel);
            LibIsDirty = true;
         }

         if (parameter is SignalModeTreeViewViewModel)
         {
            SignalModeTreeViewViewModel tvvm = parameter as SignalModeTreeViewViewModel;

            if (tvvm.Parent is SignalModeGroupTreeViewViewModel)
            {
               RemoveSignalMode(tvvm);
               LibIsDirty = true;
            }
         }

         if (parameter is AntennaScanModeTreeViewViewModel)
         {
            AntennaScanModeTreeViewViewModel tvvm = parameter as AntennaScanModeTreeViewViewModel;

            if (tvvm.Parent is ScanModeGroupTreeViewViewModel)
            {
               RemoveScanMode(tvvm);
               LibIsDirty = true;
            }
         }

      }

      bool CanRemoveItemExecute(Object parameter)
      {
         if (MainState != AppState.editing)
         {
            return false;
         }

         if (parameter is WaypointTreeViewViewModel)
         {
            return true;
         }

         if (parameter is RadarEmitterModeTreeViewViewModel)
         {
            return true;
         }

         if (parameter is EmitterModeTreeViewViewModel)
         {
            return true;
         }

         if (parameter is RfEmitterTreeViewViewModel)
         {
            return true;
         }

         if (parameter is SignalModeGroupTreeViewViewModel)
         {
            return true;
         }

         if (parameter is ScanModeGroupTreeViewViewModel)
         {
            return true;
         }

         if (parameter is SignalModeTreeViewViewModel)
         {
            SignalModeTreeViewViewModel tvvm = parameter as SignalModeTreeViewViewModel;

            if (tvvm.Parent is SignalModeGroupTreeViewViewModel)
            {
               return true;
            }
         }

         if (parameter is AntennaScanModeTreeViewViewModel)
         {
            AntennaScanModeTreeViewViewModel tvvm = parameter as AntennaScanModeTreeViewViewModel;

            if (tvvm.Parent is ScanModeGroupTreeViewViewModel)
            {
               return true;
            }
         }

         return false;
      }

      public ICommand RemoveItem
      {
         get
         {
            return new RelayCommand<Object>(parameter => RemoveItemExecute(parameter), parameter => CanRemoveItemExecute(parameter));
         }
      }

      void AddItemExecute(Object parameter)
      {
         if (parameter is PositionRotationTreeViewViewModel)
         {
            AddWp(parameter as PositionRotationTreeViewViewModel);
         }

         if (parameter is RfEmitterTreeViewViewModel)
         {
            AddRfEmitterMode(parameter as RfEmitterTreeViewViewModel);
         }

         if (parameter is OwnRadarTreeViewViewModel)
         {
            AddRadarEmitterMode(parameter as OwnRadarTreeViewViewModel);
         }

         if (parameter is EmitterListTreeViewViewModel)
         {
            AddRfEmitter(parameter as EmitterListTreeViewViewModel);
         }

         if (parameter is SignalModesTreeViewViewModel)
         {
            AddSignalModeGroup(parameter as SignalModesTreeViewViewModel);
         }

         if (parameter is ScanModesTreeViewViewModel)
         {
            AddScanModeGroup(parameter as ScanModesTreeViewViewModel);
         }

         if (parameter is SignalModeGroupTreeViewViewModel)
         {
            AddSignalMode(parameter as SignalModeGroupTreeViewViewModel);
         }

         if (parameter is ScanModeGroupTreeViewViewModel)
         {
            AddScanMode(parameter as ScanModeGroupTreeViewViewModel);
         }

      
      }

      bool CanAddItemExecute(Object parameter)
      {
         if (MainState != AppState.editing)
         {
            return false;
         }

         if (parameter is PositionRotationTreeViewViewModel)
         {
            return true;
         }

         if (parameter is RfEmitterTreeViewViewModel)
         {
            return true;
         }

         if (parameter is OwnRadarTreeViewViewModel)
         {
            return true;
         }

         if (parameter is EmitterListTreeViewViewModel)
         {
            return true;
         }

         if (parameter is SignalModesTreeViewViewModel)
         {
            return true;
         }

         if (parameter is ScanModesTreeViewViewModel)
         {
            return true;
         }

         if (parameter is SignalModeGroupTreeViewViewModel)
         {
            return true;
         }

         if (parameter is ScanModeGroupTreeViewViewModel)
         {
            return true;
         }

         return false;
      }

      public ICommand AddItem
      {
         get
         {
            return new RelayCommand<Object>(parameter => AddItemExecute(parameter), parameter => CanAddItemExecute(parameter));
         }
      }


      void MoveItemUpExecute(Object parameter)
      {
         TreeViewViewModel sel = MainWindow.TheTreeView.SelectedItem as TreeViewViewModel;

         // We trust "Can...Execute" here
         sel.MoveUp();
         SimIsDirty = true;

         if (sel is WaypointTreeViewViewModel)
         {
            WaypointTreeViewViewModel wp = sel as WaypointTreeViewViewModel;
            BuildWaypointsTreeViewViewModel(wp.Parent as PositionRotationTreeViewViewModel) ;
         }
      }

      bool CanMoveItemUpExecute(Object parameter)
      {
         if (MainState != AppState.editing)
         {
            return false;
         }

         if (MainWindow == null)
         {
            return false;
         }

         if (MainWindow.TheTreeView == null)
         {
            return false;
         }

         TreeViewViewModel sel = MainWindow.TheTreeView.SelectedItem as TreeViewViewModel;

         if (sel == null)
         {
            return false;
         }

         // Can only move up items in list that have an list index higher than 0
         if (sel is WaypointTreeViewViewModel)
         {
            return sel.CanMoveUp();
         }

         if (sel is RadarEmitterModeTreeViewViewModel)
         {
            return sel.CanMoveUp();
         }

         if (sel is EmitterModeTreeViewViewModel)
         {
            return sel.CanMoveUp();
         }

         if (sel is RfEmitterTreeViewViewModel)
         {
            return sel.CanMoveUp();
         }

         return false;
      }

      public ICommand MoveItemUp
      {
         get
         {
            return new RelayCommand<Object>(parameter => MoveItemUpExecute(parameter), parameter => CanMoveItemUpExecute(parameter));
         }
      }

      void MoveItemDownExecute(Object parameter)
      {
         TreeViewViewModel sel = MainWindow.TheTreeView.SelectedItem as TreeViewViewModel;

         // We trust "Can...Execute" here
         sel.MoveDown();
         SimIsDirty = true;

         if (sel is WaypointTreeViewViewModel)
         {
            WaypointTreeViewViewModel wp = sel as WaypointTreeViewViewModel;
            BuildWaypointsTreeViewViewModel(wp.Parent as PositionRotationTreeViewViewModel);
         }
      }

      bool CanMoveItemDownExecute(Object parameter)
      {
         if (MainState != AppState.editing)
         {
            return false;
         }

         if (MainWindow == null)
         {
            return false;
         }

         if (MainWindow.TheTreeView == null)
         {
            return false;
         }

         TreeViewViewModel sel = MainWindow.TheTreeView.SelectedItem as TreeViewViewModel;

         if (sel == null)
         {
            return false;
         }

         // Can only move up items in list that have an list index higher than 0
         if (sel is WaypointTreeViewViewModel)
         {
            return sel.CanMoveDown();
         }

         if (sel is RadarEmitterModeTreeViewViewModel)
         {
            return sel.CanMoveDown();
         }

         if (sel is EmitterModeTreeViewViewModel)
         {
            return sel.CanMoveDown();
         }

         if (sel is RfEmitterTreeViewViewModel)
         {
            return sel.CanMoveDown();
         }

         return false;
      }

      public ICommand MoveItemDown
      {
         get
         {
            return new RelayCommand<Object>(parameter => MoveItemDownExecute(parameter), parameter => CanMoveItemDownExecute(parameter));
         }
      }


      #endregion

      #region Eventhandler

      private void disaptchTimer_Tick(object sender, EventArgs e)
      {
         if (PathIndex >= SimTop.SimControl.SimulationTime.SimTime * 20)
         {
            PathIndex = 0;
         }
         else
         {
            PathIndex++;
         }
      }

      private void HandleSocketAcceptTimeout(object sender, EventArgs e)
      {
         if (!CheckClientAlive())
         {
            HandleUnexpectedTermination("ERROR: Simulation has terminated during wait for socket accept.");
         }
      }

      private void HandleMessageTimeout(object sender, EventArgs e)
      {
         if (!CheckClientAlive())
         {
            HandleUnexpectedTermination("ERROR: Simulation has terminated during wait for simulation progress.");
         }
      }

      private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (SimIsRunning)
         {
            TerminateSimulationExecute(null);
         }

         if (SimIsDirty)
         {
            MessageBoxResult result = MessageBox.Show("Save changes to current Simulation?", "Unsaved Simulation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
               e.Cancel = true;

               return;
            }
            else
            {
               if (result == MessageBoxResult.Yes)
               {
                  WriteSimulation(SimTop.AbsoluteSimFile);
               }
            }
         }

         if (LibIsDirty)
         {
            MessageBoxResult libRresult = MessageBox.Show("Save changes to current Library?", "Unsaved Library", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (libRresult == MessageBoxResult.Cancel)
            {
               e.Cancel = true;

               return;
            }
            else
            {
               if (libRresult == MessageBoxResult.Yes)
               {
                  WriteLibrary(_defaultLibraryFile);
               }
            }
         }

         e.Cancel = false;
      } 

      #endregion

      #region Private Methods

      private SignalModeTreeViewViewModel CreateSignalMode(SignalModeGroupTreeViewViewModel parentTvvm)
      {
         // Create new Signal Mode model
         SignalMode model = new SignalMode();

         SignalModeTreeViewViewModel tvvm = new SignalModeTreeViewViewModel(model.Name, model, parentTvvm, this);

         parentTvvm.LocalObject.SignalModes.Add(model);

         parentTvvm.Children.Add(tvvm);

         return tvvm;
      }

      private void RemoveSignalMode(SignalModeTreeViewViewModel tvvm)
      {
         // Signal modes are only allowed to be removed if they are part of a Signal Mode Group
         // so we check for it here
         if (!(tvvm.Parent is SignalModeGroupTreeViewViewModel))
         {
            return;
         }

         SignalModeGroupTreeViewViewModel parentTvvm = tvvm.Parent as SignalModeGroupTreeViewViewModel;

         parentTvvm.Children.Remove(tvvm);

         parentTvvm.LocalObject.SignalModes.Remove(tvvm.LocalObject);
      }

      private bool EditSignalMode(SignalModeTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of sm to use as backup if user clicks cancel
         SignalMode backup = MemoryUtils.DeepCopy<SignalMode>(tvvm.LocalObject, true);

         EmitterSignalModeDlg editDlg = new EmitterSignalModeDlg();
         editDlg.SM = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            // Ok was clicked
            if (tvvm.VmDirty)
            {
               if (tvvm.Parent is EmitterModeTreeViewViewModel)
               {
                  SimIsDirty = true;
               }
               else if (tvvm.Parent is SignalModeGroupTreeViewViewModel)
               {
                  LibIsDirty = true;
               }
            }

            return true;
         }

         // Cancel was clicked, restore to backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<SignalMode>(backup, true);

         return false;
      }

      private void AddSignalMode(SignalModeGroupTreeViewViewModel parentTvvm)
      {
         SignalModeTreeViewViewModel tvvm = CreateSignalMode(parentTvvm);

         if (!EditSignalMode(tvvm))
         {
            RemoveSignalMode(tvvm);
         }
      }

      private bool EditPositionRotation(PositionRotationTreeViewViewModel tvvm, bool adding = false)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of prm to use as backup if user clicks cancel
         PositionRotation backup = MemoryUtils.DeepCopy<PositionRotation>(tvvm.LocalObject, true);

         // Find absolute path to scenario file since this 
         // is used to find relative path to animation file
         string scenAbsFile = _simTop.SimControl.GetScenarioAbsolutePath();

         tvvm.AbsScenFile = scenAbsFile;

         PosRotDlg editDlg = new PosRotDlg();
         editDlg.PM = tvvm;
         editDlg.DataContext = tvvm;

         editDlg.ShowDialog();

         if (editDlg.RightClick)
         {
            _pendingRightClickPr = new PendingRightClickPosRot(tvvm, backup, adding);
            PendingRightClick = true;
         }
         else
         {
            if (editDlg.DialogResult == true)
            {
               if (tvvm.VmDirty)
               {
                  SimIsDirty = true;

                  tvvm.GeometryIsDirty = true;
               }

               return true;

            }

            // Cancel was clicked, restore backup
            tvvm.LocalObject = MemoryUtils.DeepCopy<PositionRotation>(backup, true);

            return false;

         }

         return true;
      }

      private bool EditSimulationControl(SimulationControlTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of scm to use as backup if user clicks cancel
         SimulationControl backup = MemoryUtils.DeepCopy<SimulationControl>(tvvm.LocalObject, true);

         // Find absolute path to Simulation Control file since this 
         // is used to find relative path to animation file
         string simCntrlAbs = SimTop.AbsoluteSimFile;

         tvvm.AbsSimControlFile = simCntrlAbs;

         SimControlDlg editDlg = new SimControlDlg();
         editDlg.SC = tvvm;
         editDlg.DataContext = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;


            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<SimulationControl>(backup, true);

         return false;
      }

      private AntennaScanModeTreeViewViewModel CreateScanMode(ScanModeGroupTreeViewViewModel parentTvvm)
      {
         // Create new Scan Mode model
         AntennaScanMode model = new AntennaScanMode();

         AntennaScanModeTreeViewViewModel tvvm = new AntennaScanModeTreeViewViewModel(model.Name, model, parentTvvm, this);

         parentTvvm.LocalObject.ScanModes.Add(model);

         parentTvvm.Children.Add(tvvm);

         return tvvm;
      }

      private void RemoveScanMode(AntennaScanModeTreeViewViewModel tvvm)
      {
         // Scan modes are only allowed to be removed if they are part of a Scan Mode Group
         // so we check for it here
         if (!(tvvm.Parent is ScanModeGroupTreeViewViewModel))
         {
            return;
         }

         ScanModeGroupTreeViewViewModel parentTvvm = tvvm.Parent as ScanModeGroupTreeViewViewModel;

         parentTvvm.Children.Remove(tvvm);

         parentTvvm.LocalObject.ScanModes.Remove(tvvm.LocalObject);
      }

      private bool EditScanMode(AntennaScanModeTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of sm to use as backup if user clicks cancel
         AntennaScanMode backup = MemoryUtils.DeepCopy<AntennaScanMode>(tvvm.LocalObject, true);

         ScanModeDlg editDlg = new ScanModeDlg();
         editDlg.SM = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               if (tvvm.Parent is EmitterModeTreeViewViewModel)
               {
                  SimIsDirty = true;
               }
               else if (tvvm.Parent is ScanModeGroupTreeViewViewModel)
               {
                  LibIsDirty = true;
               }
            }

            return true;
         }
         
         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<AntennaScanMode>(backup, true);

         return false;
      }

      private void AddScanMode(ScanModeGroupTreeViewViewModel parentTvvm)
      {
         AntennaScanModeTreeViewViewModel tvvm = CreateScanMode(parentTvvm);

         if (!EditScanMode(tvvm))
         {
            RemoveScanMode(tvvm);
         }
      }

      private bool EditLibrarySet(LibrarySetTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of sm to use as backup if user clicks cancel
         LibrarySet backup = MemoryUtils.DeepCopy<LibrarySet>(tvvm.LocalObject, true);

         // Find absolute path to Library Set file since this 
         // is used to find relative path to other files
         tvvm.AbsLibSetFile = SimTop.GetAbsoluteLibSetFile();

         LibrarySetDlg editDlg = new LibrarySetDlg();
         editDlg.LS = tvvm;
         editDlg.DataContext = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<LibrarySet>(backup, true);

         return false;
      }

      private bool EditConfigurationLibrary(ConfigLibraryTreeViewViewModel tvvm)
      {
         tvvm.PreSynch();

         tvvm.VmDirty = false;

         ConfigLibDlg editDlg = new ConfigLibDlg();
         editDlg.CL = tvvm;
         editDlg.DataContext = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            tvvm.PostSynch();

            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }
            
            return true;
         }

         return false;
      }

      private bool EditLmhbrxLibrary(LmhbrxLibraryTreeViewViewModel tvvm)
      {
         LmhbrxLibrary backup = MemoryUtils.DeepCopy<LmhbrxLibrary>(tvvm.LocalObject, true);

         tvvm.PreSynch();

         tvvm.VmDirty = false;

         LmhbrxLibDlg editDlg = new LmhbrxLibDlg();
         editDlg.LL = tvvm;
         editDlg.DataContext = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            tvvm.PostSynch();

            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }
            
            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<LmhbrxLibrary>(backup, true);

         return false;
      }

      private bool EditOwnRadar(OwnRadarTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of sm to use as backup if user clicks cancel
         OwnRadar backup = MemoryUtils.DeepCopy<OwnRadar>(tvvm.LocalObject, true);

         OwnRadarDlg editDlg = new OwnRadarDlg();
         editDlg.EM = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<OwnRadar>(backup, true);

         return false;
      }

      private bool EditRecordingLibrary(RecLibraryTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of sm to use as backup if user clicks cancel
         RecordingLibrary backup = MemoryUtils.DeepCopy<RecordingLibrary>(tvvm.LocalObject, true);

         RecordingLibDlg editDlg = new RecordingLibDlg();
         editDlg.RL = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<RecordingLibrary>(backup, true);

         return false;
      }

      private WaypointTreeViewViewModel CreateWp(PositionRotationTreeViewViewModel parentTvvm)
      {
         // Create new Waypoint model
         Waypoint wp = new Waypoint();

         // Create WaypointTVMM linking it to the the Waypoint model
         WaypointTreeViewViewModel tvvm = new WaypointTreeViewViewModel(wp.Name, wp, parentTvvm, this);

         // Add Waypoint model to the SyntPos member of the PositionRotation model
         parentTvvm.LocalObject.SyntPos.Waypoints.Add(wp);

         // Add WaypointTVVM to the PositionRotationTVVM
         parentTvvm.Children.Add(tvvm);

         return tvvm;
      }

      private void RemoveWp(WaypointTreeViewViewModel tvvm )
      {
         PositionRotationTreeViewViewModel parentTvvm = tvvm.Parent as PositionRotationTreeViewViewModel;

         // Remove WaypointTVVM from the PositionRotationTVVM 
         parentTvvm.Children.Remove(tvvm);

         // Also remove Waypoint model from the PositionRotation model
         parentTvvm.LocalObject.SyntPos.Waypoints.Remove(tvvm.LocalObject);
      }

      private bool EditWaypoint(WaypointTreeViewViewModel tvvm, bool adding = false)
      {
         tvvm.VmDirty = false;

         Waypoint backup = MemoryUtils.DeepCopy<Waypoint>(tvvm.LocalObject, true);

         WaypointDlg editDlg = new WaypointDlg();
         editDlg.WP = tvvm;

         editDlg.ShowDialog();

         if (editDlg.RightClick)
         {
            _pendingRightClickWp = new PendingRightClickWaypoint(tvvm, backup, adding);
            PendingRightClick = true;
            return true;
         }

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;

               PositionRotationTreeViewViewModel parent = tvvm.Parent as PositionRotationTreeViewViewModel;

               parent.GeometryIsDirty = true;
            }

            return true;
         }

         tvvm.LocalObject = MemoryUtils.DeepCopy<Waypoint>(backup, true);

         return false;
      }

      private void AddWp(PositionRotationTreeViewViewModel parentTvvm)
      {
         WaypointTreeViewViewModel tvvm = CreateWp(parentTvvm);

         if (!EditWaypoint(tvvm, true))
         {
            RemoveWp(tvvm);
         }
         else
         {
            tvvm.IsExpanded = true;
            tvvm.IsSelected = true;
         }
      }

      private EmitterModeTreeViewViewModel CreateRfEmitterMode(RfEmitterTreeViewViewModel parentTvvm)
      {
         // Adds a Emitter Mode to the spcified RfEmitterTreeViewViewModel object
         // and the corresponding model object. Also adds the Signal mode and scan pattern
         // objects to the mode

         EmitterMode em = new EmitterMode();

         EmitterModeTreeViewViewModel tvvm = new EmitterModeTreeViewViewModel(em.Name, em, parentTvvm, this);

         tvvm.Children.Add(new SignalModeTreeViewViewModel(em.SigMode.Name, em.SigMode, tvvm, this));

         tvvm.Children.Add(new AntennaScanModeTreeViewViewModel(em.ScanMode.Name, em.ScanMode, tvvm, this));

         parentTvvm.LocalObject.Modes.Add(em);

         parentTvvm.Children.Add(tvvm);
         
         return tvvm;
      }

      private void RemoveRfEmitterMode(EmitterModeTreeViewViewModel tvvm)
      {
         RfEmitterTreeViewViewModel parentTvvm = tvvm.Parent as RfEmitterTreeViewViewModel;

         parentTvvm.Children.Remove(tvvm);

         parentTvvm.LocalObject.Modes.Remove(tvvm.LocalObject);
      }

      private bool EditEmitterMode(EmitterModeTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of model to use as backup if user clicks cancel
         EmitterMode backup = MemoryUtils.DeepCopy<EmitterMode>(tvvm.LocalObject, true);

         EmitterModeDlg editDlg = new EmitterModeDlg();

         editDlg.EM = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<EmitterMode>(backup, true);

         return false;
      }

      private void AddRfEmitterMode(RfEmitterTreeViewViewModel parentTvvm)
      {
         EmitterModeTreeViewViewModel tvvm = CreateRfEmitterMode(parentTvvm);

         if (!EditEmitterMode(tvvm))
         {
            // User clicked cancel, remove the object
            RemoveRfEmitterMode(tvvm);
         }
         else
         {
            tvvm.IsExpanded = true;
            tvvm.IsSelected = true;
         }
      }

      private SignalModeGroupTreeViewViewModel CreateSignalModeGroup(SignalModesTreeViewViewModel parentTvvm)
      {
         SignalModeGroup model = new SignalModeGroup();

         SignalModeGroupTreeViewViewModel tvvm = new SignalModeGroupTreeViewViewModel(model.Name, model, parentTvvm, this);


         parentTvvm.LocalObject.Groups.Add(model);

         parentTvvm.Children.Add(tvvm);

         return tvvm;
      }

      private void RemoveSignalModeGroup(SignalModeGroupTreeViewViewModel tvvm)
      {
         SignalModesTreeViewViewModel parentTvvm = tvvm.Parent as SignalModesTreeViewViewModel;

         parentTvvm.Children.Remove(tvvm);

         parentTvvm.LocalObject.Groups.Remove(tvvm.LocalObject);
      }

      private bool EditSignalModeGroup(SignalModeGroupTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of model to use as backup if user clicks cancel
         SignalModeGroup backup = MemoryUtils.DeepCopy<SignalModeGroup>(tvvm.LocalObject, true);

         EditNameDlg editDlg = new EditNameDlg();
         editDlg.EdName = tvvm.GroupName;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            tvvm.GroupName = editDlg.EdName;

            if (tvvm.VmDirty)
            {
               LibIsDirty = true;
            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<SignalModeGroup>(backup, true);

         return false;
      }


      private void AddSignalModeGroup(SignalModesTreeViewViewModel parentTvvm)
      {
         SignalModeGroupTreeViewViewModel tvvm = CreateSignalModeGroup(parentTvvm);

         if (!EditSignalModeGroup(tvvm))
         {
            // User clicked cancel, remove the object
            RemoveSignalModeGroup(tvvm);
         }
         else
         {
            tvvm.IsExpanded = true;
            tvvm.IsSelected = true;
         }
      }

      private ScanModeGroupTreeViewViewModel CreateScanModeGroup(ScanModesTreeViewViewModel parentTvvm)
      {
         ScanModeGroup smg = new ScanModeGroup();

         ScanModeGroupTreeViewViewModel tvvm = new ScanModeGroupTreeViewViewModel(smg.Name, smg, parentTvvm, this);

         parentTvvm.LocalObject.Groups.Add(smg);

         parentTvvm.Children.Add(tvvm);

         return tvvm;
      }

      private void RemoveScanModeGroup(ScanModeGroupTreeViewViewModel tvvm)
      {
         ScanModesTreeViewViewModel parentTvvm = tvvm.Parent as ScanModesTreeViewViewModel;

         parentTvvm.Children.Remove(tvvm);

         parentTvvm.LocalObject.Groups.Remove(tvvm.LocalObject);
      }

      private bool EditScanModeGroup(ScanModeGroupTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of model to use as backup if user clicks cancel
         ScanModeGroup backup = MemoryUtils.DeepCopy<ScanModeGroup>(tvvm.LocalObject, true);

         EditNameDlg editDlg = new EditNameDlg();
         editDlg.EdName = tvvm.GroupName;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            tvvm.GroupName = editDlg.EdName;

            if (tvvm.VmDirty)
            {
               if (tvvm.VmDirty)
               {
                  LibIsDirty = true;
               }

            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<ScanModeGroup>(backup, true);

         return false;
      }


      private void AddScanModeGroup(ScanModesTreeViewViewModel parentTvvm)
      {
         ScanModeGroupTreeViewViewModel tvvm = CreateScanModeGroup(parentTvvm);

         if (!EditScanModeGroup(tvvm))
         {
            // User clicked cancel, remove the object
            RemoveScanModeGroup(tvvm);
         }
         else
         {
            tvvm.IsExpanded = true;
            tvvm.IsSelected = true;
         }
      }

      private RadarEmitterModeTreeViewViewModel CreateRadarEmitterMode(OwnRadarTreeViewViewModel parentTvvm)
      {
         RadarEmitterMode em = new RadarEmitterMode();

         RadarEmitterModeTreeViewViewModel tvvm = new RadarEmitterModeTreeViewViewModel(em.Name, em, parentTvvm, this);

         tvvm.Children.Add(new SignalModeTreeViewViewModel(em.SigMode.Name, em.SigMode, tvvm, this));

         parentTvvm.LocalObject.Modes.Add(em);

         parentTvvm.Children.Add(tvvm);

         return tvvm;
      }

      private void RemoveRadarEmitterMode(RadarEmitterModeTreeViewViewModel tvvm)
      {
         OwnRadarTreeViewViewModel parentTvvm = tvvm.Parent as OwnRadarTreeViewViewModel;

         parentTvvm.Children.Remove(tvvm);

         parentTvvm.LocalObject.Modes.Remove(tvvm.LocalObject);

      }

      private bool EditRadarEmitterMode(RadarEmitterModeTreeViewViewModel tvvm)
      {
         tvvm.VmDirty = false;

         // Do a deep copy of model to use as backup if user clicks cancel
         RadarEmitterMode backup = MemoryUtils.DeepCopy<RadarEmitterMode>(tvvm.LocalObject, true);

         RadarEmitterModeDlg editDlg = new RadarEmitterModeDlg();
         editDlg.EM = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<RadarEmitterMode>(backup, true);

         return false;
      }

      private void AddRadarEmitterMode(OwnRadarTreeViewViewModel parentTvvm)
      {
         RadarEmitterModeTreeViewViewModel tvvm = CreateRadarEmitterMode(parentTvvm);

         if (!EditRadarEmitterMode(tvvm))
         {
            RemoveRadarEmitterMode(tvvm);
         }
         else
         {
            tvvm.IsExpanded = true;
            tvvm.IsSelected = true;
         }
      }

      private RfEmitterTreeViewViewModel CreateRfEmitter(EmitterListTreeViewViewModel parentTvvm)
      // Adds an Emitter to the specified EmitterListTreeViewViewModel object
      // and the corresponding model object. Also adds a PlatformViewModel to
      // the Plattforms collection
      {
         // Create model object for the emitter
         // This object automatically creates a PositionRotation object and all
         // child object incl a Fix WayPoint
         RfEmitter em = new RfEmitter();

         // Create RfEmitterTVVM to match the new RfEmitter linking them together
         RfEmitterTreeViewViewModel emtvvm = new RfEmitterTreeViewViewModel(em.Name, em, parentTvvm, this);

         // The PositionRotation object of the RfEmitter model is now linked to a new PositionRotationTVVM
         PositionRotationTreeViewViewModel prtvvm = new PositionRotationTreeViewViewModel("Position", em.PosRot, emtvvm, this, false);

         // Since we want to reuse the EditRfEmitter dialog here it is required to push all data into the
         // correct objects of the TreeView and other ViewModels. If the user then clicks cancel we remove
         // them

         // Add PositionRotationTVVM to RfEmitterTVVM
         emtvvm.Children.Add(prtvvm);

         // Add RfEmitterTVVM to the supplied EmitterListTVVM
         parentTvvm.Children.Add(emtvvm);

         // Add emitter model to the EmitterList model (found in the supplied EmitterListTVVM)
         parentTvvm.LocalObject.Emitters.Add(em);
         
         // Add prtvvm to Platform View Model collection
         Platforms.Add(prtvvm);

         return emtvvm;
      }

      private void RemoveRfEmitter(RfEmitterTreeViewViewModel tvvm)
      {
         EmitterListTreeViewViewModel parentTvvm = tvvm.Parent as EmitterListTreeViewViewModel;

         // Remove the RfEmitterTVVM from the EmitterListTVVM
         // this will eventually kill the PositionRotationTVVM also.
         parentTvvm.Children.Remove(tvvm);

         // The RfEmitter model needs to be removed from the EmitterList model:
         // this will eventually kill the RfEmitter model also.
         parentTvvm.LocalObject.Emitters.Remove(tvvm.LocalObject);

         // Remove PlatformVm from collection of PlatformVms
         // PlatformVm can be fond in the PositionRotationTVVM which is one of the
         // children of the RfEmitterTVVM (tvvm). We don't know which
         // one, though. Lets find it.
         Platforms.Remove(tvvm.FindPosRotChild());
      }

      private bool EditRfEmitter(RfEmitterTreeViewViewModel tvvm)
      // Returns true if Ok was clicked, false otherwise
      {
         tvvm.VmDirty = false;

         // Do a deep copy of model to use as backup if user clicks cancel
         RfEmitter backup = MemoryUtils.DeepCopy<RfEmitter>(tvvm.LocalObject, true);

         RfEmitterEditDlg editDlg = new RfEmitterEditDlg();
         editDlg.EM = tvvm;

         editDlg.ShowDialog();

         if (editDlg.DialogResult == true)
         {
            if (tvvm.VmDirty)
            {
               SimIsDirty = true;
            }

            return true;
         }

         // Cancel was clicked, restore backup
         tvvm.LocalObject = MemoryUtils.DeepCopy<RfEmitter>(backup, true);

         return false;
      }

      private void AddRfEmitter(EmitterListTreeViewViewModel parentTvvm)
      {
         RfEmitterTreeViewViewModel tvvm = CreateRfEmitter(parentTvvm);

         if (!EditRfEmitter(tvvm))
         {
            RemoveRfEmitter(tvvm);
         }
         else
         {
            tvvm.IsExpanded = true;
            tvvm.IsSelected = true;
         }
      }

      private void WriteSimulation(string simulationFile)
      {
         SimTop.WriteSimulation(simulationFile);

      }

      private void SetBoundingBox()
      {
         BoundingBox boundBox = new BoundingBox(-90, 90, 180, -180);

         foreach (PositionRotationTreeViewViewModel tvvm in Platforms)
         {
            boundBox = tvvm.Platform.ExpandBoundingBox(boundBox);
         }

         // Determine the bounding box of the display

         // reverse order on Y since negative latitude go up
         double dt = Nav2DispConv.ConvertY(boundBox.Top);
         double db = Nav2DispConv.ConvertY(boundBox.Bottom);
         double dl = Nav2DispConv.ConvertX(boundBox.Left);
         double dr = Nav2DispConv.ConvertX(boundBox.Right);

         MainWindow.zoomAndPanControl.ZoomTo(new Rect(dl, dt, dr - dl, db - dt));

      }

      private void BuildSimulationTreeViewViewModel()
      {
         TreeTopViewModel.Children.Remove(GetSimulationTreeViewViewModel());

         TreeViewViewModel sim = new TreeViewViewModel("Simulation", SimTop, TreeTopViewModel, this);

         TreeTopViewModel.Children.Add(sim);

         TreeViewViewModel simCntrl = new SimulationControlTreeViewViewModel("Simulation Control", SimTop.SimControl, sim, this);

         SimControlViewModel = simCntrl as SimulationControlTreeViewViewModel;

         sim.Children.Add(simCntrl);

         BuildLibrarySetTreeViewViewModel();

         BuildConfLibTreeViewViewModel();

         BuildLmhbrxLibTreeViewViewModel();

         BuildRecLibTreeViewViewModel();

         BuildScenarioTreeViewViewModel();
      }

      private void BuildLibraryTreeViewViewModel()
      {
         LibTopViewModel = new ModesLibraryTreeViewViewModel("Library", LibTop, TreeTopViewModel, this);

         TreeTopViewModel.Children.Add(LibTopViewModel);

         SignalModeViewModel = new SignalModesTreeViewViewModel("Emitter Signal Modes", LibTop.SignalModes, LibTopViewModel, this);

         LibTopViewModel.Children.Add(SignalModeViewModel);

         foreach (SignalModeGroup smg in LibTop.SignalModes.Groups)
         {
            TreeViewViewModel signalGroup = new SignalModeGroupTreeViewViewModel(smg.Name, smg, SignalModeViewModel, this);
            SignalModeViewModel.Children.Add(signalGroup);

            foreach (SignalMode sm in smg.SignalModes)
            {
               signalGroup.Children.Add(new SignalModeTreeViewViewModel(sm.Name, sm, signalGroup, this));
            }
         }

         ScanModeViewModel = new ScanModesTreeViewViewModel("Antenna Scan Modes", LibTop.ScanModes, LibTopViewModel, this);

         LibTopViewModel.Children.Add(ScanModeViewModel);

         foreach (ScanModeGroup smg in LibTop.ScanModes.Groups)
         {
            TreeViewViewModel scanGroup = new ScanModeGroupTreeViewViewModel(smg.Name, smg, ScanModeViewModel, this);
            ScanModeViewModel.Children.Add(scanGroup);

            foreach (AntennaScanMode sm in smg.ScanModes)
            {
               scanGroup.Children.Add(new AntennaScanModeTreeViewViewModel(sm.Name, sm, scanGroup, this));
            }
         }
      }

      private void BuildTreeViewViewModel()
      {

         TreeTopViewModel = new TreeViewViewModel("top", null, null, this);

         BuildLibraryTreeViewViewModel();
      }

      private void BuildWaypointsTreeViewViewModel(PositionRotationTreeViewViewModel tvvm)
      {
         tvvm.Children.Clear();

         foreach (Waypoint wp in tvvm.LocalObject.SyntPos.Waypoints)
         {
            tvvm.Children.Add(new WaypointTreeViewViewModel(wp.Name, wp, tvvm, this));
         }
      }

      private TreeViewViewModel BuildScenarioTreeViewViewModel()
         // This method builds the scenario parts of the tree view.
         // It requires that the parts of the tree "above it" is already built
      {
         Platforms.Clear();

         TreeViewViewModel sim = GetSimulationTreeViewViewModel();
         TreeViewViewModel scen = GetScenarioTreeViewViewModel();

         sim.Children.Remove(scen);

         scen = new TreeViewViewModel("Scenario", SimTop.Scen, sim, this);
         sim.Children.Add(scen);

         TreeViewViewModel ac = new TreeViewViewModel("Aircraft", SimTop.Scen.AC, scen, this);
         scen.Children.Add(ac);

         TreeViewViewModel acPos = new PositionRotationTreeViewViewModel("Position", SimTop.Scen.AC.PosRot, ac, this, true);
         ac.Children.Add(acPos);

         Platforms.Add(acPos as PositionRotationTreeViewViewModel);

         BuildWaypointsTreeViewViewModel(acPos as PositionRotationTreeViewViewModel);

         TreeViewViewModel radar = new OwnRadarTreeViewViewModel(SimTop.Scen.Radar.Name, SimTop.Scen.Radar, scen, this);
         scen.Children.Add(radar);

         foreach (RadarEmitterMode pm in SimTop.Scen.Radar.Modes)
         {
            TreeViewViewModel md = new RadarEmitterModeTreeViewViewModel(pm.Name, pm, radar, this);

            radar.Children.Add(md);

            md.Children.Add(new SignalModeTreeViewViewModel(pm.SigMode.Name, pm.SigMode, md, this));

         }

         // The RF Emitter is really a sub-set view of the scenario. It's local object is also
         // scenario, the same as the scen TreeViewViewModel.
         EmitterListTreeViewViewModel emitters = new EmitterListTreeViewViewModel("RF Emitters", SimTop.Scen, scen, this);
         scen.Children.Add(emitters);

         foreach (RfEmitter em in SimTop.Scen.Emitters)
         {
            TreeViewViewModel emtr = new RfEmitterTreeViewViewModel(em.Name, em, emitters, this);
            emitters.Children.Add(emtr);

            TreeViewViewModel emPos = new PositionRotationTreeViewViewModel("Position", em.PosRot, emtr, this, false);
            emtr.Children.Add(emPos);

            Platforms.Add(emPos as PositionRotationTreeViewViewModel);

            BuildWaypointsTreeViewViewModel(emPos as PositionRotationTreeViewViewModel);

            foreach (EmitterMode mode in em.Modes)
            {
               TreeViewViewModel md = new EmitterModeTreeViewViewModel(mode.Name, mode, emtr, this);

               emtr.Children.Add(md);

               md.Children.Add(new SignalModeTreeViewViewModel(mode.SigMode.Name, mode.SigMode, md, this));
               md.Children.Add(new AntennaScanModeTreeViewViewModel(mode.ScanMode.Name, mode.ScanMode, md, this));
            }
         }

         return scen;
      }

      private TreeViewViewModel BuildLibrarySetTreeViewViewModel()
      // This method builds the Library Set parts of the tree view.
      // It requires that the parts of the tree "above it" is already built
      {
         TreeViewViewModel simCntrl = GetSimulationControlTreeViewViewModel();
         TreeViewViewModel libSet = GetLibSetTreeViewViewModel();

         simCntrl.Children.Remove(libSet);

         libSet = new LibrarySetTreeViewViewModel("Library Set", SimTop.LibSet, simCntrl, this);

         simCntrl.Children.Add(libSet);

         return libSet;
      }

      private void BuildConfLibTreeViewViewModel()
      // This method builds the configuration library parts of the tree view.
      // It requires that the parts of the tree "above it" is already built
      {
         TreeViewViewModel libSet = GetLibSetTreeViewViewModel();
         TreeViewViewModel configLib = GetConfLibTreeViewViewModel();

         libSet.Children.Remove(configLib);

         libSet.Children.Add(new ConfigLibraryTreeViewViewModel("Configuration Library", SimTop.ConfigLib, libSet, this));                  
      }

      private void BuildLmhbrxLibTreeViewViewModel()
      // This method builds the lmhbrx library parts of the tree view.
      // It requires that the parts of the tree "above it" is already built
      {
         TreeViewViewModel libSet = GetLibSetTreeViewViewModel();
         TreeViewViewModel lmhbrxLib = GetLmhbrxTreeViewViewModel();

         libSet.Children.Remove(lmhbrxLib);

         libSet.Children.Add(new LmhbrxLibraryTreeViewViewModel("LMHBRx Library", SimTop.LmhbrxLib, libSet, this));
      }

      private void BuildRecLibTreeViewViewModel()
      // This method builds the Recording library parts of the tree view.
      // It requires that the parts of the tree "above it" is already built
      {
         TreeViewViewModel libSet = GetLibSetTreeViewViewModel();
         TreeViewViewModel recLib = GetRecLibTreeViewViewModel();

         libSet.Children.Remove(recLib);

         libSet.Children.Add(new RecLibraryTreeViewViewModel("Recording Library", SimTop.RecLib, libSet, this));
      }

      private TreeViewViewModel GetSimulationTreeViewViewModel()
      {
         foreach (TreeViewViewModel vm in TreeTopViewModel.Children)
         {
            if (vm.Name == "Simulation") return vm;
         }

         return null;
      }

      private SimulationControlTreeViewViewModel GetSimulationControlTreeViewViewModel()
      {
         TreeViewViewModel tvvm = GetSimulationTreeViewViewModel();

         if (tvvm == null)
         {
            return null;
         }

         foreach (TreeViewViewModel vm in tvvm.Children)
         {
            if (vm.Name == "Simulation Control")
            {
               return vm as SimulationControlTreeViewViewModel;
            }
         }

         return null;
      }

      private TreeViewViewModel GetScenarioTreeViewViewModel()
      {
         foreach (TreeViewViewModel vm in GetSimulationTreeViewViewModel().Children)
         {
            if (vm.Name == "Scenario") return vm;
         }

         return null;
      }


      private TreeViewViewModel GetLibSetTreeViewViewModel()
      {
         foreach (TreeViewViewModel vm in GetSimulationControlTreeViewViewModel().Children)
         {
            if (vm.Name == "Library Set") return vm;
         }

         return null;
      }

      private TreeViewViewModel GetConfLibTreeViewViewModel()
      {

         foreach (TreeViewViewModel vm in GetLibSetTreeViewViewModel().Children)
         {
            if (vm.Name == "Configuration Library") return vm;
         }

         return null;
      }

      private TreeViewViewModel GetLmhbrxTreeViewViewModel()
      {

         foreach (TreeViewViewModel vm in GetLibSetTreeViewViewModel().Children)
         {
            if (vm.Name == "LMHBRx Library") return vm;
         }

         return null;
      }

      private TreeViewViewModel GetRecLibTreeViewViewModel()
      {

         foreach (TreeViewViewModel vm in GetLibSetTreeViewViewModel().Children)
         {
            if (vm.Name == "Recording Library") return vm;
         }

         return null;
      }


      private bool ReadLibrary()
      {
         // ----------------------------------------------------------------------
         // Deserialize ModesLibrary 

         try
         {
            XmlSerializer serializer = new XmlSerializer(typeof(ModesLibrary));

            FileStream fs = new FileStream(_defaultLibraryFile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            LibTop = (ModesLibrary)serializer.Deserialize(reader);
            fs.Close();

         }
         catch
         {

// Want to get rid of updates in XAML Designer that reloaded library (and failed)
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
               return false;
            }
#endif

            MessageBoxResult mbres = MessageBox.Show("The file: " + _defaultLibraryFile + " contains errors. The library could not be loaded.",
               "XML File Reading Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Create empty library here
            LibTop = new ModesLibrary();

            return false;

         }

         return true;
      }

      private void WriteLibrary(string file)
      {
         XmlSerializerNamespaces xns = new XmlSerializerNamespaces();
         xns.Add(string.Empty, string.Empty);

         XmlWriterSettings settings = new XmlWriterSettings();
         settings.Encoding = Encoding.UTF8;
         settings.Indent = true;
         settings.IndentChars = "   ";

         // ----------------------------------------------------------------------
         // Serialize ModesLibrary 

         settings.NewLineOnAttributes = false;

         XmlSerializer writer = new XmlSerializer(typeof(ModesLibrary));

         using (FileStream fileStream = new FileStream(file, FileMode.Create))
         using (StreamWriter sw = new StreamWriter(fileStream))
         using (XmlWriter xmlWriter = XmlWriter.Create(sw, settings))
         {
            writer.Serialize(xmlWriter, LibTop, xns);
         }

      }

      #endregion

      #region Public Methods

      public bool ImportScenario(string file)
      {
         if (SimTop.ReadScenario(file))
         {
            BuildScenarioTreeViewViewModel();
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool ImportLibrarySet(string file)
      {
         if (SimTop.ReadLibrarySet(file))
         {
            BuildLibrarySetTreeViewViewModel();
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool ImportConfigurationLibrary(string file)
      {
         if (SimTop.ReadConfigurationLibrary(file))
         {
            BuildConfLibTreeViewViewModel();
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool ImportLmhbrxLibrary(string file)
      {
         if (SimTop.ReadLmhbrxLibrary(file))
         {
            BuildLmhbrxLibTreeViewViewModel();
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool ImportRecordingLibrary(string file)
      {
         if (SimTop.ReadRecordingLibrary(file))
         {
            BuildRecLibTreeViewViewModel();
            return true;
         }
         else
         {
            return false;
         }
      }

      public string BrowseNonEditableFile(string filter, string absBaseFile)
      {
         OpenFileDialog fileDialog = new OpenFileDialog();

         fileDialog.CheckFileExists = true;

         fileDialog.Filter = filter;

         var result = fileDialog.ShowDialog();

         if (result == true)
         {
            //Uri fullPath = new Uri(fileDialog.FileName, UriKind.Absolute);
            //Uri relRoot = new Uri(System.IO.Path.GetDirectoryName(absBaseFile), UriKind.Absolute);
            //string relPath = relRoot.MakeRelativeUri(fullPath).ToString();

            string relPath = RelativePath.GetRelativePath(System.IO.Path.GetDirectoryName(absBaseFile), fileDialog.FileName);

            return relPath;
         }

         return "";
      }

      public string BrowseEditableFile(string filter, string absBaseFile, string importType)
      {
         OpenFileDialog fileDialog = new OpenFileDialog();

         fileDialog.CheckFileExists = false;

         fileDialog.Filter = filter;

         var result = fileDialog.ShowDialog();

         if (result == true)
         {
            // Check if file exists
            if (System.IO.File.Exists(fileDialog.FileName))
            {
               // If so, prompt user for either overwrite the selected file with data in 
               // the database of this simulation

               MessageBoxResult mbres = MessageBox.Show("The selected file already exists, do you want to overwrite it with data from the current simulation? If so, click Yes. The file is not overwritten until you save this simulation. If you click No, the data of the selected file is loaded in to the current simulation overwriting any current data.",
                  "Overwrite selected file?", MessageBoxButton.YesNo, MessageBoxImage.Question);

               if (mbres == MessageBoxResult.Yes)
               {

               }
               else
               {
                  bool importResult = false;

                  if (importType == "scenario")
                  {
                     importResult = ImportScenario(fileDialog.FileName);
                  }
                  else if (importType == "librarySet")
                  {
                     importResult = ImportLibrarySet(fileDialog.FileName);
                  }
                  else if (importType == "configLib")
                  {
                     importResult = ImportConfigurationLibrary(fileDialog.FileName);
                  }
                  else if (importType == "lmhbrxLib")
                  {
                     importResult = ImportLmhbrxLibrary(fileDialog.FileName);
                  }
                  else if (importType == "recLib")
                  {
                     importResult = ImportRecordingLibrary(fileDialog.FileName);
                  }

                  if (!importResult)
                  {
                     return "";
                  }
               }
            }

            // If file does not exists, select the first option

            // Either way, always do this:
            string relPath = RelativePath.GetRelativePath(System.IO.Path.GetDirectoryName(absBaseFile), fileDialog.FileName);

            return relPath;
         }

         return "";
      }

      public void RightClickMap(Point p)
      {
         if (_pendingRightClickWp != null)
         {
            // Update coordinates of WP here
            _pendingRightClickWp.TreeViewModel.Longitude = Nav2DispConv.ReciprConvertX(p.X);
            _pendingRightClickWp.TreeViewModel.Latitude = Nav2DispConv.ReciprConvertY(p.Y);

            WaypointDlg wpdlg = new WaypointDlg();
            wpdlg.WP = _pendingRightClickWp.TreeViewModel;

            wpdlg.ShowDialog();

            if (!wpdlg.RightClick)
            {
               if (wpdlg.DialogResult == true)
               {
                  if (_pendingRightClickWp.TreeViewModel.IsDirty)
                  {
                     SimIsDirty = true;

                     PositionRotationTreeViewViewModel parent = _pendingRightClickWp.TreeViewModel.Parent as PositionRotationTreeViewViewModel;

                     parent.GeometryIsDirty = true;
                  }
               }
               else
               {
                  // We clicked cancel, was we adding a new or editing an old?
                  if (_pendingRightClickWp.PendingAdd)
                  {
                     // If we click cancel and the WP was just added (i.e. if the Edit Way Point was called
                     // by the AddWaypoint method, we should now remove the object 
                     RemoveWp(_pendingRightClickWp.TreeViewModel);
                  }
                  else
                  {
                     // If we are not adding a new waypoint but editing an old, we should now revert to the backup object 
                     _pendingRightClickWp.TreeViewModel.LocalObject = MemoryUtils.DeepCopy<Waypoint>(_pendingRightClickWp.CancelBackup, true);
                  }

               }

               _pendingRightClickWp = null;
               PendingRightClick = false;
            }
         }

         if (_pendingRightClickPr != null)
         {
            // Update coordinates of WP here
            _pendingRightClickPr.TreeViewModel.FixLongitude = Nav2DispConv.ReciprConvertX(p.X);
            _pendingRightClickPr.TreeViewModel.FixLatitude = Nav2DispConv.ReciprConvertY(p.Y);

            PosRotDlg emdlg = new PosRotDlg();
            emdlg.PM = _pendingRightClickPr.TreeViewModel;

            emdlg.ShowDialog();

            if (!emdlg.RightClick)
            {
               if (emdlg.DialogResult == true)
               {
                  if (_pendingRightClickPr.TreeViewModel.IsDirty)
                  {
                     SimIsDirty = true;
                     _pendingRightClickPr.TreeViewModel.GeometryIsDirty = true;

                  }
               }
               else
               {
                  _pendingRightClickPr.TreeViewModel.LocalObject = MemoryUtils.DeepCopy<PositionRotation>(_pendingRightClickPr.CancelBackup, true);
               }

               _pendingRightClickPr = null;
               PendingRightClick = false;
            }
         }

      }

      #endregion

      #region SimulationExecution

   //   private void RunSimulation()
   //   {
   //      // Start Winsocket Server
   //      _server = new AsynchSocketServer(this);

   //      _server.StartListening();

   //      // Start Socket client (i.e. Modesty simulation)
   //      //_client = new Process();
   //      //_client.StartInfo.FileName = "SystemCSocketTest.exe";
   //      //_client.StartInfo.Arguments = _server.IpAddress.ToString() + " " + _server.Port.ToString();
   //      //_client.StartInfo.UseShellExecute = false;
   //      //_client.StartInfo.CreateNoWindow = true;
   //      //_client.StartInfo.WorkingDirectory = "";
   //      //_client.Exited += new EventHandler(myProcess_Exited);
   //      //_client.Start();

   //      PathIndex = 0;
      
   //      _socketConnectionTimer.Start();

   //      StatusMessage = "Simulation is running...";

   //      SimIsRunning = true;
   //   }

   //   public void HandleSimulationMessage(string msg)
   //   {
   //      SimulationCommXml comm = new SimulationCommXml();

   //      try
   //      {
   //         XmlSerializer serializer = new XmlSerializer(typeof(SimulationCommXml));

   //         using (TextReader reader = new StringReader(msg))
   //         {
   //            comm = (SimulationCommXml)serializer.Deserialize(reader);
   //         }
   //      }
   //      catch(Exception ex)
   //      {
   //         Console.WriteLine("Error in reading XML: " + ex.InnerException);

   //      }

   //      if (comm.Status == "finished")
   //      {
   //         SimulationFinished();

   //         return;
   //      }

   //      if (comm.Status == "error")
   //      {
   //         SimulationErrorReport(comm.StatusMessage);

   //         return;
   //      }

   //      if (comm.Status == "running")
   //      {
   //         PathIndex = (int)(comm.SimTime / 0.05);
   //      }
   //   }

   //   public void SimulationFinished()
   //   {
   //      SimulationGracefullStop();

   //      StatusMessage = "Simulation finished";
   //   }

   //   public void SimulationErrorReport(string message)
   //   {
   //      SimulationGracefullStop();

   //      StatusMessage = "Simulation Error: " + message;
   //   }

   //   public void SimulationGracefullStop()
   //   {
   //      _socketConnectionTimer.Stop();

   //      // Remove event handler so the message is not overwritten
   //      _client.Exited -= new EventHandler(myProcess_Exited);

   //      // Wait for simulation/client process to terminate
   //      if (!_client.HasExited)
   //      {
   //         _client.WaitForExit();
   //      }

   //      _server.Shutdown();

   //      SimIsRunning = false;
   //   }

   //   private void InterruptSimulation()
   //   {
   //      _socketConnectionTimer.Stop();

   //      // Remove event handler so the message is not overwritten
   //      _client.Exited -= new EventHandler(myProcess_Exited);

   //      if (!_client.HasExited)
   //      {
   //         _client.Kill();
   //      }

   //      _server.Shutdown();

   //      StatusMessage = "Simulation interupted";

   //      SimIsRunning = false;
   //   }

   //   private void myProcess_Exited(object sender, System.EventArgs e)
   //   {
   //      HandleUnexpectedTermination("Simulation Process terminated");
   ////eventHandled = true;
   //      //Console.WriteLine("Exit time:    {0}\r\n" +
   //      //    "Exit code:    {1}\r\nElapsed time: {2}", myProcess.ExitTime, myProcess.ExitCode, elapsedTime);
   //   }

   //   public void HandleUnexpectedTermination(string exMsg)
   //   {
   //      _socketConnectionTimer.Stop();

   //      StatusMessage = exMsg;

   //      //if (!_client.HasExited)
   //      //{
   //      //   _client.Kill();
   //      //}

   //      _server.Shutdown();

   //      SimIsRunning = false;
   //   }

   //   private void checkSocketConnection(object sender, EventArgs e)
   //   {
   //      //if (!_server.IsConnected())
   //      //{
   //      //   StatusMessage = "Lost communication with simulation, terminating";

   //      //   // Remove event handler so the error message is not overwritten
   //      //   _client.Exited -= new EventHandler(myProcess_Exited);

   //      //   if (!_client.HasExited)
   //      //   {
   //      //      _client.Kill();
   //      //   }

   //      //   _server.Shutdown();

   //      //   _socketConnectionTimer.Stop();

   //      //   SimIsRunning = false;
   //      //}

   //   }
      
      private void StartTheServer()
      {
         // Start Winsocket Server
         _server = new AsynchSocketServer();
         _server.Status += _server_Status;
         _server.Data += _server_Data;

         _server.ConnectServer();
      }

      void _server_Status(AsynchSocketServer server, AsynchSocketServerState e)
      {
         if (e.State == AsynchSocketServerState.AssStateType.ServerConnected)
         {
            StatusMessage = "Server is connected";
            return;
         }

         if (e.State == AsynchSocketServerState.AssStateType.ConnectionAccepted)
         {
            HandleSimulatorStarted();
            return;
         }
 
         if (e.State == AsynchSocketServerState.AssStateType.AcceptCallbackException) 
         {
            HandleSocketTerminationWhileAccept(e.Message);
            return;
         }

         if (e.State == AsynchSocketServerState.AssStateType.SocketTerminationWhileReadException)
         {
            HandleSocketTerminationWhileRead(e.Message);
            return;
         }

         StatusMessage = "Server Error: " + e.Message;
      }

      void _server_Data(AsynchSocketServer server, string data)
      {
         HandleSimulationMessage(data);
      }

      private void RunSimulation()
      {
         _server.StartListening();

         // Start Socket client (i.e. Modesty simulation)
         _client = new Process();
         _client.StartInfo.FileName = "SystemCSocketTest.exe";

//         _client.StartInfo.Arguments = _server.IpAddress.ToString() + " " + _server.Port.ToString() + " " + _rnd.Next(0, 6).ToString();
         _client.StartInfo.Arguments = _server.IpAddress.ToString() + " " + _server.Port.ToString() + " 0";
         _client.StartInfo.UseShellExecute = false;
         _client.StartInfo.CreateNoWindow = true;
         _client.StartInfo.WorkingDirectory = "";
         _client.Start();

         _interruptByIntent = false;

         _acceptTimer.Start();

         SimIsRunning = true;

         StatusMessage = "Waiting for simulation to start...";

         // TODO: Start an acceptTimeout timer that fires after while
      }

      public void HandleSimulatorStarted()
      {
         // Remove acceptTimeout timer 
         _acceptTimer.Stop();

         StatusMessage = "Simulation is running...";

         // Start an readTimeout timer that fires after while
         _readTimer.Start();
      }
      
      public void HandleSimulationMessage(string msg)
      {
         // Restart readTimeout timer
         _readTimer.Stop();
         _readTimer.Start();

         SimulationCommXml comm = new SimulationCommXml();

         try
         {
            XmlSerializer serializer = new XmlSerializer(typeof(SimulationCommXml));

            using (TextReader reader = new StringReader(msg))
            {
               comm = (SimulationCommXml)serializer.Deserialize(reader);
            }
         }
         catch(Exception ex)
         {
            Console.WriteLine("Error in reading XML: " + ex.InnerException);

         }

         if (comm.Status == "finished")
         {
            SimulationFinished();

            return;
         }

         if (comm.Status == "error")
         {
            SimulationErrorReport(comm.StatusMessage);

            return;
         }

         if (comm.Status == "running")
         {
            PathIndex = (int)(comm.SimTime / 0.05);
         }
      }

      public void SimulationFinished()
      {
         SimulationGracefullStop();

         StatusMessage = "Simulation finished";
      }

      public void SimulationErrorReport(string message)
      {
         SimulationGracefullStop();

         StatusMessage = "Simulation Error: " + message;
      }

      public void SimulationGracefullStop()
      {
         _readTimer.Stop();

         if (_client != null)
         {
            // Wait for simulation/client process to terminate
            if (!_client.HasExited)
            {
               _client.WaitForExit();
            }
         }

         SimIsRunning = false;
      }

      private void InterruptSimulation()
      {
         _readTimer.Stop();

         if (!_client.HasExited)
         {
            _client.Kill();
         }

         StatusMessage = "Simulation interupted by user";

         SimIsRunning = false;
      }

      public void HandleSocketTerminationWhileRead(string exMsg)
      {
         if (!_interruptByIntent)
         {
            HandleUnexpectedTermination(exMsg);
         }
         else
         {
            _interruptByIntent = false;
         }
      }

      public void HandleSocketTerminationWhileAccept(string exMsg)
      {
         HandleUnexpectedTermination(exMsg);
      }


      public void HandleUnexpectedTermination(string exMsg)
      {
         _readTimer.Stop();         
         StatusMessage = exMsg;

         SimIsRunning = false;
      }

      private void CheckSocketConnection()
      {
         if (!_server.IsConnected())
         {
            StatusMessage = "Server lost connection.";

            if (_client != null)
            {
               if (!_client.HasExited)
               {
                  _client.Kill();
               }
            }

            SimIsRunning = false;
         }
         else
         {
            StatusMessage = "Connection is ok.";
         }
      }

      private bool CheckClientAlive()
      {
         if (_client != null)
         {
            if (_client.HasExited)
            {
               return false;
            }

            return true;
         }

         return false;     
      
      }

      #endregion

   }
}

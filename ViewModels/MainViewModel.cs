using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   class MainViewModel : MicroMvvm.ViewModelBase
   {

      #region Declarations

      private CompoundObjectRef _myCpRef = new CompoundObjectRef();
      private ObjectRefStateProperties _myStateProp = new ObjectRefStateProperties();
      private CompoundObject _myCP;
      private CompoundObjectViewModel _myCpVm;

      private CompoundObjectViewModel _selectedCompoundObject;
      private ObservableCollection<IShapeInterface> _selectedShapes;
      private ObservableCollection<DragablePointViewModel> _selectedPoints;

      #endregion

      #region Constructor

      public MainViewModel()
      {
         MyCP = CompoundObject.ReadFromFile("landing_scene.xml");
         MyStateProp.CompObj = MyCP;
         MyCpRef.StateProperties.Add(MyStateProp);

         MyCpVm = new CompoundObjectViewModel(this, null, MyCpRef);
         MyCpVm.BuildViewModel(MyCpRef);
      }

      #endregion

      #region Properties

      public CompoundObjectRef MyCpRef
      {
         get { return _myCpRef; }
         set { _myCpRef = value; }
      }

      public ObjectRefStateProperties MyStateProp
      {
         get { return _myStateProp; }
         set { _myStateProp = value; }
      }

      public CompoundObject MyCP
      {
         get { return _myCP; }
         set { _myCP = value; }
      }

      public CompoundObjectViewModel MyCpVm
      {
         get { return _myCpVm; }
         set { _myCpVm = value; }
      }

      #endregion

      #region Commands

      void ReloadExecute(Object parameter)
      {
         MyCP = CompoundObject.ReadFromFile("landing_scene.xml");
         MyStateProp.CompObj = MyCP;
         MyCpRef.StateProperties.Add(MyStateProp);

         MyCpVm.BuildViewModel(MyCpRef);

      }

      bool CanReloadExecute(Object parameter)
      {
         return true;
      }

      public ICommand Reload
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => ReloadExecute(parameter), parameter => CanReloadExecute(parameter));
         }
      }

      void SaveExecute(Object parameter)
      {
         MyCpVm.ModelObject.WriteToFile();
      }

      bool CanSaveExecute(Object parameter)
      {
         return true;
      }

      public ICommand Save
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => SaveExecute(parameter), parameter => CanSaveExecute(parameter));
         }
      }

      void StateExecute(Object parameter)
      {
         CompoundObjectViewModel covm = MyCpVm.ChildObjects[0];

         if (covm.SelectedStateIndex == 0)
         {
            covm.SelectedStateIndex = 1;
         }
         else
         {
            covm.SelectedStateIndex = 0;
         }
      }

      bool CanStateExecute(Object parameter)
      {
         return true;
      }

      public ICommand State
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => StateExecute(parameter), parameter => CanStateExecute(parameter));
         }
      }


      #endregion

      #region Public Methods

      // NOTE: FrameworkElement is used to communicate from the View. It can 
      // be used to obtain the ViewModel object from it's datacontext.
      // In some cases we need the type of FrameworkElement (to see if a 
      // Line or Rectangle).

      // All mouse handling function return true if the the data was used for some action
      // (if original event was handled)

      public bool MouseDown(FrameworkElement target, Point clickPoint, bool shift, bool ctrl, bool alt)
      {
         // Decode target ViewModel and view oobject that was clicked
         if (target.DataContext is CompoundObjectViewModel)
         {
            // Mouse down on rectangle around CompoundObject
            CompoundObjectViewModel covm = (CompoundObjectViewModel)target.DataContext;

            Debug.WriteLine("Clicked rectangle around CompoundObject");

            return true;
         }
         else if((target is Rectangle) && (target.DataContext is DragablePointViewModel))
         {
            // Mouse down on rectangle of DragablePoint
            DragablePointViewModel dpvm = (DragablePointViewModel)target.DataContext;

            Debug.WriteLine("Clicked rectangle of DragablePoint");

            return true;
         }
         else if ((target is Line) && (target.DataContext is DragablePointViewModel))
         {
            // Mouse down on Line between DragablePoints 
            DragablePointViewModel dpvm = (DragablePointViewModel)target.DataContext;

            Debug.WriteLine("Clicked line between DragablePoint");

            return true;
         }
         else if (target.DataContext is IShapeInterface)
         {
            // Mouse down on rectangle around Shape
            IShapeInterface shvm = (IShapeInterface)target.DataContext;

            Debug.WriteLine("Clicked on Shape");

            return true;
         }
         else if ((target is Rectangle) && (target.DataContext is IPositionInterface))
         {
            // Mouse down on rectangle around Shape
            IPositionInterface posvm = (IPositionInterface)target.DataContext;

            Debug.WriteLine("Clicked rectangle around something that can be dragged");

            return true;
         }

         return false;
      }

      public bool MouseMove(FrameworkElement target, Vector dragVector, bool shift, bool ctrl, bool alt)
      {

         return false;
      }

      // clickPoint will be in coordinates of the parent CompoundObject
      public bool MouseUp(FrameworkElement targe, Point clickPoint, bool shift, bool ctrl, bool alt)
      {

         return false;
      }

      #endregion
   }
}

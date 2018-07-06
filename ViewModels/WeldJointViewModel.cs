using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LeapfrogEditor
{
   class WeldJointViewModel : MicroMvvm.ViewModelBase, IMainVmInterface
   {
      #region Declarations

      private MainViewModel _mainVm;
      private WeldJoint _modelObject;
      private CompoundObjectViewModel _parent;
      protected LfShapeViewModel _aVm;
      protected LfShapeViewModel _bVm;
      private bool _isSelected;

      #endregion

      #region Constructors

      public WeldJointViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, WeldJoint modelObject)
      {
         MainVm = mainVm;
         Parent = parent;
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

      public WeldJoint ModelObject
      {
         get
         {
            if (_modelObject == null) return null;
            return _modelObject;
         }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public CompoundObjectViewModel Parent
      {
         get { return _parent; }
         set
         {
            _parent = value;
            OnPropertyChanged("");
         }
      }

      public string Name
      {
         get
         {
            if (_modelObject == null) return "";

            return _modelObject.Name;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.Name = value;
            OnPropertyChanged("Name");
         }
      }

      public string AName
      {
         get
         {
            if (_modelObject == null) return "";

            return _modelObject.AName;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.AName = value;
            OnPropertyChanged("AName");
         }
      }

      public string BName
      {
         get
         {
            if (_modelObject == null) return "";

            return _modelObject.BName;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.BName = value;
            OnPropertyChanged("BName");
         }
      }

      public LfShapeViewModel AShapeObject
      {
         get
         {
            return _aVm;
         }
      }

      public LfShapeViewModel BShapeObject
      {
         get
         {
            return _bVm;
         }
      }

      public double AAnchorX
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.AAnchorX;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.AAnchorX = value;
            OnPropertyChanged("AAnchorX");
         }
      }

      public double AAnchorY
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.AAnchorY;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.AAnchorY = value;
            OnPropertyChanged("AAnchorY");
         }
      }

      public double BAnchorX
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.BAnchorX;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.BAnchorX = value;
            OnPropertyChanged("BAnchorX");
         }
      }

      public double BAnchorY
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.BAnchorY;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.BAnchorY = value;
            OnPropertyChanged("BAnchorY");
         }
      }

      public bool IsSelected
      {
         get { return _isSelected; }
         set
         {
            _isSelected = value;
            OnPropertyChanged("IsSelected");
         }
      }

      #endregion

      #region public Methods

      public void ConnectToShapes(CompositeCollection shapes)
      {
         _aVm = Parent.FindShape(ModelObject.AName, shapes);
         if (_aVm == null)
         {
            MessageBox.Show("The shape A pointed to by " + ModelObject.Name + " does not exists in CO " + Parent.Name, "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Error);
         }

         _bVm = Parent.FindShape(ModelObject.BName, shapes);
         if (_bVm == null)
         {
            MessageBox.Show("The shape B pointed to by " + ModelObject.Name + " does not exists in CO " + Parent.Name, "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      #endregion
   }
}

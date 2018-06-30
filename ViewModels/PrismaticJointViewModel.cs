using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LeapfrogEditor
{
   class PrismaticJointViewModel : WeldJointViewModel
   {
      #region Declarations

      private PrismaticJoint _modelObject;

      // The upper and lower limit points are dummypoints used to
      // drag the limits and angle. The model objects recide here
      // TODO: These points need to be more custom. Fortsätt här...
      private LfDragablePoint _uprLimitP;
      private LfDragablePoint _lwrLimitP;

      private LfDragablePointViewModel _uprLimitPvm;
      private LfDragablePointViewModel _lwrLimitPvm;

      #endregion

      #region Constructors

      public PrismaticJointViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, PrismaticJoint modelObject) 
         : base(mainVm, parent, modelObject)
      {
         MainVm = mainVm;
         Parent = parent;
         ModelObject = modelObject;

         _aVm = Parent.FindShape(ModelObject.AName);
         if (_aVm == null)
         {
            MessageBox.Show("The shape A pointed to by " + ModelObject.Name + " does not exists in CO " + Parent.Name, "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Error);
         }

         _bVm = Parent.FindShape(ModelObject.BName);
         if (_bVm == null)
         {
            MessageBox.Show("The shape B pointed to by " + ModelObject.Name + " does not exists in CO " + Parent.Name, "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Error);
         }

         _uprLimitP = new LfDragablePoint();
         _lwrLimitP = new LfDragablePoint();

         _uprLimitPvm = new LfDragablePointViewModel(mainVm, parent, _uprLimitP);
      }

      #endregion

      #region Properties

      public new PrismaticJoint ModelObject
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

      public double AAxisX
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.AAxisX;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.AAxisX = value;
            OnPropertyChanged("AAxisX");
         }
      }

      public double AAxisY
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.AAxisY;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.AAxisY = value;
            OnPropertyChanged("AAxisY");
         }
      }

      public double RefAngle
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.RefAngle;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.RefAngle = value;
            OnPropertyChanged("RefAngle");
         }
      }

      public double LowerLimit
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.LowerLimit;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.LowerLimit = value;
            OnPropertyChanged("LowerLimit");
         }
      }

      public double UpperLimit
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.UpperLimit;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.UpperLimit = value;
            OnPropertyChanged("UpperLimit");
         }
      }


      #endregion

      #region public Methods

      #endregion
   }
}

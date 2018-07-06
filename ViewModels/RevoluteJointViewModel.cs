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
   class RevoluteJointViewModel : WeldJointViewModel, IMainVmInterface
   {
      #region Declarations

      private RevoluteJoint _modelObject;

      #endregion

      #region Constructors

      public RevoluteJointViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, RevoluteJoint modelObject) 
         : base(mainVm, parent, modelObject)
      {
         MainVm = mainVm;
         Parent = parent;
         ModelObject = modelObject;

         //_aVm = Parent.FindShape(ModelObject.AName);
         //if (_aVm == null)
         //{
         //   MessageBox.Show("The shape A pointed to by " + ModelObject.Name + " does not exists in CO " + Parent.Name, "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Error);
         //}

         //_bVm = Parent.FindShape(ModelObject.BName);
         //if (_bVm == null)
         //{
         //   MessageBox.Show("The shape B pointed to by " + ModelObject.Name + " does not exists in CO " + Parent.Name, "Error parsing file", MessageBoxButton.OK, MessageBoxImage.Error);
         //}
      }

      #endregion

      #region Properties

      public new RevoluteJoint ModelObject
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


      #endregion

      #region public Methods

      #endregion
   }
}

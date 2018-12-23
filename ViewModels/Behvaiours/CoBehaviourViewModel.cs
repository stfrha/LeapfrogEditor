using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LeapfrogEditor
{
   class CoBehaviourViewModel : MicroMvvm.ViewModelBase
   {
      #region Declarations

      private CoBehaviour ModelObject;

      #endregion

      #region Constructors

      public CoBehaviourViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, CoBehaviour modelObject) 
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public CoBehaviour LocalModelObject
      {
         get { return (CoBehaviour)ModelObject; }
      }

      public string Type
      {
         get { return LocalModelObject.Type; }
         set
         {
            LocalModelObject.Type = value;
            OnPropertyChanged("Type");
         }
      }

      public BreakableObjectProperties BreakableObjProps
      {
         get { return LocalModelObject.BreakableObjProps; }
         set
         {
            LocalModelObject.BreakableObjProps = value;
            OnPropertyChanged("BreakableObjProps");
         }
      }

      public SteerableObjectProperties SteerableObjProps
      {
         get { return LocalModelObject.SteerableObjProps; }
         set
         {
            LocalModelObject.SteerableObjProps = value;
            OnPropertyChanged("SteerableObjProps");
         }
      }

      #endregion

      #region private Methods

      #endregion

      #region protected Methods

      #endregion

      #region public Methods

      #endregion
   }
}

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
   class CoSystemViewModel : TreeViewViewModel
   {
      #region Declarations

      private CoSystem _modelObject;

      // Only one object is in use per instance of this class. The object
      // in use is defined by the type string of the system.
      private ObjectFactoryPropertiesViewModel _objFactStatesProps;
      private FlameEmitterPropertiesViewModel _flameEmitterStatesProps;
      private GunPropertiesViewModel _gunStatesProps;
      private ShieldPropertiesViewModel _shieldStatesProps;

      #endregion

      #region Constructors

      public CoSystemViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, CoSystem modelObject) 
      {
         _modelObject = modelObject;
      }

      #endregion

      #region Properties

      public CoSystem LocalModelObject
      {
         get { return _modelObject; }
      }

      public string Name
      {
         get { return LocalModelObject.Name; }
         set
         {
            LocalModelObject.Name = value;
            OnPropertyChanged("Name");
         }
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

      public SystemViewModelBase Properties
      {
         get
         {
            if (Type == "objectFactory")
            {
               return _objFactStatesProps;
            }
            else if (Type == "flameEmitter")
            {
               return _flameEmitterStatesProps;
            }
            else if (Type == "gun")
            {
               return _gunStatesProps;
            }
            else if (Type == "shield")
            {
               return _shieldStatesProps;
            }

            return null;
         }
         set
         {
            if (Type == "objectFactory")
            {
               _objFactStatesProps = (ObjectFactoryPropertiesViewModel)value;
            }
            else if (Type == "flameEmitter")
            {
               _flameEmitterStatesProps = (FlameEmitterPropertiesViewModel)value;
            }
            else if (Type == "gun")
            {
               _gunStatesProps = (GunPropertiesViewModel)value;
            }
            else if (Type == "shield")
            {
               _shieldStatesProps = (ShieldPropertiesViewModel)value;
            }
         }
      }


      #endregion

      #region private Methods

      #endregion

      #region protected Methods

      #endregion

      #region public Methods

      virtual public void BuildTreeViewModel()
      {
         TreeName = Name;
      }

      #endregion
   }
}

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
   class BreakableObjectPropertiesViewModel : BehaviourViewModelBase
   {
      #region Declarations

      private BreakableObjectProperties ModelObject;

      #endregion

      #region Constructors

      public BreakableObjectPropertiesViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, BreakableObjectProperties modelObject) 
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public BreakableObjectProperties LocalModelObject
      {
         get { return ModelObject; }
      }

      public int BreakBulletDamage
      {
         get { return LocalModelObject.BreakBulletDamage; }
         set
         {
            LocalModelObject.BreakBulletDamage = value;
            OnPropertyChanged("BreakBulletDamage");
         }
      }

      public int NumberOfSpawns
      {
         get { return LocalModelObject.NumberOfSpawns; }
         set
         {
            LocalModelObject.NumberOfSpawns = value;
            OnPropertyChanged("NumberOfSpawns");
         }
      }

      public ObservableCollection<SpawnObject> SpawnObjects
      {
         get { return LocalModelObject.SpawnObjects; }
         set
         {
            LocalModelObject.SpawnObjects = value;
            OnPropertyChanged("SpawnObjects");
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

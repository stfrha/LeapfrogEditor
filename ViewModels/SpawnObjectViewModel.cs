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
   class SpawnObjectViewModel : MicroMvvm.ViewModelBase
   {
      #region Declarations

      private SpawnObject ModelObject;

      #endregion

      #region Constructors

      public SpawnObjectViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, SpawnObject modelObject) 
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public SpawnObject LocalModelObject
      {
         get { return ModelObject; }
      }

      public double ProbabilityFactor
      {
         get { return LocalModelObject.ProbabilityFactor; }
         set
         {
            LocalModelObject.ProbabilityFactor = value;
            OnPropertyChanged("ProbabilityFactor");
         }
      }

      public ChildObject MyChildObject
      {
         get { return LocalModelObject.MyChildObject; }
         set
         {
            LocalModelObject.MyChildObject = value;
            OnPropertyChanged("MyChildObject");
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

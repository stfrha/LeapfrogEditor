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
   public class SpawnObjectViewModel : TreeViewViewModel
   {
      #region Declarations

      private SpawnObject ModelObject;

      // The _spawnCompoundObject field is a collection because it must be added to the itemssource
      private ObservableCollection<CompoundObjectViewModel> _spawnCompoundObject = new ObservableCollection<CompoundObjectViewModel>();

      #endregion

      #region Constructors

      public SpawnObjectViewModel(
         TreeViewViewModel treeParent, 
         CompoundObjectViewModel parentVm, 
         MainViewModel mainVm, 
         SpawnObject modelObject) :
         base(treeParent, parentVm, mainVm)
      {
         ModelObject = modelObject;

         // Iterate all state properties of this ChildObject and process it
         foreach (TStateProperties<ChildObjectStateProperties> sp in ModelObject.MyChildObject.StateProperties)
         {
            CompoundObjectViewModel covm = new CompoundObjectViewModel(this, parentVm, mainVm, sp.Properties.CompObj);
            covm.BuildViewModel();
            SpawnCompoundObject.Add(covm);
         }
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
            OnPropertyChanged("Headline");
         }
      }

      public ObservableCollection<CompoundObjectViewModel> SpawnCompoundObject
      {
         get { return _spawnCompoundObject; }
         set { _spawnCompoundObject = value; }
      }

      public string Headline
      {
         get { return LocalModelObject.MyChildObject.Name + "(" + LocalModelObject.ProbabilityFactor.ToString() + ")";  }
      }

      public ChildObject MyChildObject
      {
         get { return LocalModelObject.MyChildObject; }
         set
         {
            LocalModelObject.MyChildObject = value;
            OnPropertyChanged("MyChildObject");
            OnPropertyChanged("Headline");
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

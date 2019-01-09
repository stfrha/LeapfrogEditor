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
   public class ScenePropertiesViewModel : BehaviourViewModelBase
   {
      #region Declarations

      private SceneProperties _modelObject;
      private ObservableCollection<StateViewModel> _states = new ObservableCollection<StateViewModel>();

      private int _selectedStateIndex = 0;


      #endregion

      #region Constructors

      public ScenePropertiesViewModel(
         TreeViewViewModel treeParent,
         CompoundObjectViewModel parentVm,
         MainViewModel mainVm,
         SceneProperties modelObject) :
         base(treeParent, parentVm, mainVm)
      {
         ModelObject = modelObject;

         foreach (string s in ModelObject.States)
         {
            StateViewModel svm = new StateViewModel(this, parentVm, mainVm, this, s);
            States.Add(svm);
         }
      }

      #endregion

      #region Properties

      public SceneProperties ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("ModelObject");
         }
      }

      public string SceneType
      {
         get { return ModelObject.SceneType; }
         set
         {
            ModelObject.SceneType = value;
            OnPropertyChanged("SceneType");
         }
      }

      public ObservableCollection<StateViewModel> States
      {
         get { return _states; }
         set { _states = value; }
      }


      public int SelectedStateIndex
      {
         get
         {
            return _selectedStateIndex;
         }
         set
         {
            if (value == -1)
            {
               _selectedStateIndex = 0;
            }
            else
            {
               _selectedStateIndex = value;
            }

            DeselectAllChildren();
            OnPropertyChanged("");
            ParentVm.BuildTreeViewCollection();

            CompoundObjectViewModel p = ParentVm;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.ParentVm;
            }
         }
      }

      #endregion

      #region private Methods

      #endregion

      #region protected Methods

      #endregion

      #region public Methods

      public bool IsOnDisplay(StateViewModel svm)
      {
         return (States.IndexOf(svm) == SelectedStateIndex);
      }

      public void SetStateOnDisplay(StateViewModel svm)
      {
         int i = States.IndexOf(svm);

         if (i >= 0)
         {
            SelectedStateIndex = i;
         }
         else
         {
            SelectedStateIndex = 0;
         }
      }

      #endregion
   }
}

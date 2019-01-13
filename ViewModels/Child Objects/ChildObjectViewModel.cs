using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


/*
 * The ChildObjectViewModel encompass the CompoundObject (incl its states, and behaviour) 
 * as well as all the ChildObjects of all states
 */


namespace LeapfrogEditor
{
   public class ChildObjectViewModel : ConditionalSelectTreeViewViewModel
   {
      #region Declarations

      private ChildObject _modelObject;
      private ObservableCollection<ChildObjectStatePropertiesViewModel> _stateProperties = new ObservableCollection<ChildObjectStatePropertiesViewModel>();
      
      #endregion

      #region Constructors

      public ChildObjectViewModel(
         TreeViewViewModel treeParent,
         CompoundObjectViewModel parentVm,
         MainViewModel mainVm, 
         ChildObject modelObject ) :
         base(treeParent, parentVm, mainVm)
      {
         ModelObject = modelObject;

         foreach (TStateProperties<ChildObjectStateProperties> cosp in ModelObject.StateProperties)
         {
            ChildObjectStatePropertiesViewModel cospvm = new ChildObjectStatePropertiesViewModel(this, parentVm, mainVm, cosp);
            StateProperties.Add(cospvm);
         }
      }

      #endregion

      #region Properties

      public ChildObject ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public string Name
      {
         get { return _modelObject.Name; }
         set
         {
            _modelObject.Name = value;
            OnPropertyChanged("Name");
            OnPropertyChanged("RefName");
            if (TreeParent != null)
            {
               TreeParent.OnPropertyChanged("");
            }
         }
      }

      public ObservableCollection<ChildObjectStatePropertiesViewModel> StateProperties
      {
         get { return _stateProperties; }
         set { _stateProperties = value; }
      }

      #endregion

      #region Private Methods


      #endregion

      #region Public Methods


      public void DeselectAllChildren()
      {
         foreach (ChildObjectStatePropertiesViewModel spvm in StateProperties)
         {
            spvm.CompoundObjectChild.DeselectAllChildren();
            spvm.CompoundObjectChild.IsSelected = false;
            spvm.IsSelected = false;
         }
      }

      #endregion

   }
}

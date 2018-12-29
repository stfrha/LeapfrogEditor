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
   public class StateCollectionViewModelBase : TreeViewViewModel
   {
      #region Declarations

      private CompoundObjectViewModel _parent;

      #endregion

      #region Constructors

      public StateCollectionViewModelBase(CompoundObjectViewModel parent)
      {
         Parent = parent;
      }

      #endregion

      #region Properties

      public CompoundObjectViewModel Parent
      {
         get { return _parent; }
         set
         {
            _parent = value;
            OnPropertyChanged("");
         }
      }

      public new bool IsSelected
      {
         get { return _isSelected; }
         set
         {
            _isSelected = value;

            if (!_isSelected)
            {
               DeselectAllChildren();
            }


            OnPropertyChanged("IsSelected");
         }
      }

      #endregion

      #region protected Methods

      protected virtual void DeselectAllChildren()
      {
         // Only polygon shapes has children (so far), se we do nothing here
      }

      #endregion

      #region public Methods

      //public virtual void InvalidateAll()
      //{
      //   OnPropertyChanged("");
      //}

      #endregion
   }
}

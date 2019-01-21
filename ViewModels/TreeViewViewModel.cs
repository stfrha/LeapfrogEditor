using MicroMvvm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LeapfrogEditor
{
   [Serializable]
   public class TreeViewViewModel : MicroMvvm.ViewModelBase, IMainVmInterface, IParentInterface
   {
      private CompoundObjectViewModel _parentVm;
      protected TreeViewViewModel _treeParent;
      protected bool _isSelected;
      protected bool _isExpanded;
      private bool _isEnabled;

      private MainViewModel _mainVm;

      public TreeViewViewModel(
         TreeViewViewModel treeParent, 
         CompoundObjectViewModel parentVm, 
         MainViewModel mainVm,
         bool enabled = true)
      {
         _treeParent = treeParent;
         _parentVm = parentVm;
         _mainVm = mainVm;
         _isSelected = false;
         _isExpanded = false;
         _isEnabled = enabled;
      }

      public TreeViewViewModel TreeParent
      {
         get { return _treeParent; }
         set { _treeParent = value; }
      }

      public CompoundObjectViewModel ParentVm
      {
         get { return _parentVm; }
         set
         {
            _parentVm = value;
            OnPropertyChanged("");
         }
      }

      public bool IsSelected
      {
         get { return _isSelected; }
         set
         {
            _isSelected = value;
            OnPropertyChanged("IsSelected");

            if (_isSelected == true)
            {
               if (_treeParent != null)
               {
                  _treeParent.IsExpanded = true;
               }
            }
         }
      }

      public bool IsExpanded
      {
         get { return _isExpanded; }
         set
         {
            _isExpanded = value;
            OnPropertyChanged("IsExpanded");

            if (_isExpanded && _treeParent != null)
            {
               _treeParent.IsExpanded = true;
            }
         }
      }

      public bool IsEnabled
      {
         get { return _isEnabled; }
         set
         {
            if (value != _isEnabled)
            {
               _isEnabled = value;
               OnPropertyChanged("IsEnabled");
            }
         }
      }

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }
   }
}



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
   public class TreeViewViewModel : MicroMvvm.ViewModelBase, IMainVmInterface
   {
      protected TreeViewViewModel _treeParent;
      protected bool _isSelected;
      protected bool _isExpanded;
      private MainViewModel _mainVm;

      public TreeViewViewModel()
      {
         _treeParent = null;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = null;
      }

      public TreeViewViewModel(MainViewModel mainVm)
      {
         _treeParent = null;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = mainVm;
      }

      public TreeViewViewModel(string name, TreeViewViewModel parent, MainViewModel mainVm)
      {
         _treeParent = parent;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = mainVm;
      }

      public TreeViewViewModel TreeParent
      {
         get { return _treeParent; }
         set { _treeParent = value; }
      }


      public bool IsSelected
      {
         get { return _isSelected; }
         set
         {
            _isSelected = value;
            OnPropertyChanged("IsSelected");
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

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }
   }
}



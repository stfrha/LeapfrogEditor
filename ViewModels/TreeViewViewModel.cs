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
      protected string _treeName;
      protected TreeViewViewModel _treeParent;
      protected ObservableCollection<TreeViewViewModel> _treeChildren;
      protected bool _isSelected;
      protected bool _isExpanded;
      private MainViewModel _mainVm;

      public TreeViewViewModel()
      {
         _treeName = "undefined";
         _treeChildren = new ObservableCollection<TreeViewViewModel>();
         _treeParent = null;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = null;
      }

      public TreeViewViewModel(MainViewModel mainVm)
      {
         _treeName = "undefined";
         _treeChildren = new ObservableCollection<TreeViewViewModel>();
         _treeParent = null;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = mainVm;
      }

      public TreeViewViewModel(string name, Object obj, TreeViewViewModel parent, MainViewModel mainVm)
      {
         _treeChildren = new ObservableCollection<TreeViewViewModel>();
         _treeName = name;
         _treeParent = parent;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = mainVm;
      }

      public string TreeName
      {
         get { return _treeName; }
         set
         {
            _treeName = value;
            OnPropertyChanged("TreeName");
         }
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

      public ObservableCollection<TreeViewViewModel> TreeChildren
      {
         get { 
            return _treeChildren; 
         }
         set { 
            _treeChildren = value; 
         }
      }

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

      public bool CanMoveUp()
      {
         return (_treeParent.TreeChildren.IndexOf(this) > 0);
      }

      public bool CanMoveDown()
      {
         return (_treeParent.TreeChildren.IndexOf(this) < _treeParent.TreeChildren.Count - 1);
      }

      public virtual void MoveUp()
      {
         int oldIndex = _treeParent.TreeChildren.IndexOf(this);
         
         if (oldIndex < 1)
         {
            return;
         }

         _treeParent.TreeChildren.Move(oldIndex, oldIndex - 1);
      }

      public virtual void MoveDown()
      {
         int oldIndex = _treeParent.TreeChildren.IndexOf(this);

         if (oldIndex > _treeParent.TreeChildren.Count - 1)
         {
            return;
         }

         _treeParent.TreeChildren.Move(oldIndex, oldIndex + 1);
      }

   }
}



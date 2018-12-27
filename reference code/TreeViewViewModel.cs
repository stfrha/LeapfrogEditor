using MicroMvvm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ModestyGE
{
   [Serializable]
   public class TreeViewViewModel : ViewModelBase
   {
      protected string _name;
      protected TreeViewViewModel _parent;
      protected ObservableCollection<TreeViewViewModel> _children;
      protected bool _isSelected;
      protected bool _isExpanded;
      private ModestyGeViewModel _mainVm;

      public TreeViewViewModel()
      {
         _name = "undefined";
         _children = new ObservableCollection<TreeViewViewModel>();
         _parent = null;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = null;
      }

      public TreeViewViewModel(ModestyGeViewModel mainVm)
      {
         _name = "undefined";
         _children = new ObservableCollection<TreeViewViewModel>();
         _parent = null;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = mainVm;
      }

      public TreeViewViewModel(string name, Object obj, TreeViewViewModel parent, ModestyGeViewModel mainVm)
      {
         _children = new ObservableCollection<TreeViewViewModel>();
         _name = name;
         _parent = parent;
         _isSelected = false;
         _isExpanded = false;
         _mainVm = mainVm;
      }

      public string Name
      {
         get { return _name; }
         set
         {
            _name = value;
            OnPropertyChanged("Name");
         }
      }

      public TreeViewViewModel Parent
      {
         get { return _parent; }
         set { _parent = value; }
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

            if (_isExpanded && _parent != null)
            {
               _parent.IsExpanded = true;
            }
         }
      }

      public ObservableCollection<TreeViewViewModel> Children
      {
         get { 
            return _children; 
         }
         set { 
            _children = value; 
         }
      }

      public ModestyGeViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

      public bool CanMoveUp()
      {
         return (_parent.Children.IndexOf(this) > 0);
      }

      public bool CanMoveDown()
      {
         return (_parent.Children.IndexOf(this) < _parent.Children.Count - 1);
      }

      public virtual void MoveUp()
      {
         int oldIndex = _parent.Children.IndexOf(this);
         
         if (oldIndex < 1)
         {
            return;
         }

         _parent.Children.Move(oldIndex, oldIndex - 1);
      }

      public virtual void MoveDown()
      {
         int oldIndex = _parent.Children.IndexOf(this);

         if (oldIndex > _parent.Children.Count - 1)
         {
            return;
         }

         _parent.Children.Move(oldIndex, oldIndex + 1);
      }

   }


   public class EmitterListTreeViewViewModel : TreeViewViewModel
   {
      // Lets all specialised model objects have the same, generic, name
      private Scenario _localObject;

      public Scenario LocalObject
      {
         get { return _localObject; }
         set
         {
            _localObject = value;
            OnPropertyChanged("LocalObject");
         }
      }

      public EmitterListTreeViewViewModel()
         : base()
      {
         _localObject = null;
      }

      public EmitterListTreeViewViewModel(string name, Scenario obj, TreeViewViewModel parent, ModestyGeViewModel mainVm)
         : base(name, obj, parent, mainVm)
      {
         _localObject = obj;
      }

   }
}



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
   public class ConditionalSelectTreeViewViewModel : TreeViewViewModel
   {

      public ConditionalSelectTreeViewViewModel(TreeViewViewModel treeParent, CompoundObjectViewModel parentVm, MainViewModel mainVm) :
         base(treeParent, parentVm, mainVm)
      {
      }

      override public bool IsSelected
      {
         get { return _isSelected; }
         set
         {
            if (MainVm.AmISelectable(this))
            {
               _isSelected = value;

               OnPropertyChanged("IsSelected");

               if (_isSelected == true)
               {
                  IsExpanded = true;
               }
            }
         }
      }
   }
}



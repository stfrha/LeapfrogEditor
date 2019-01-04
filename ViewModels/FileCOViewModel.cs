﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LeapfrogEditor
{
   public class FileCOViewModel : CompoundObjectViewModel
   {
      #region Declarations

      // Even though this is a collection, it should only contain one object, the top level 
      // CompoundObject of the file.
      private ObservableCollection<CompoundObjectViewModel> _topObject = new ObservableCollection<CompoundObjectViewModel>();

      private string _fileName;

      #endregion

      #region Constructors

      public FileCOViewModel(
         MainViewModel mainVm,
         CompoundObject modelObject,
         ChildObjectStateProperties modelObjectProperties,
         CompoundObjectViewModel parentVm,
         ChildObject childObjectOfParent) :
         base(mainVm, modelObject, modelObjectProperties, parentVm, childObjectOfParent)
      {
         _fileName = modelObjectProperties.File;
      }

      #endregion

      #region Properties

      public string FileName
      {
         get { return _fileName; }
         set
         {
            _fileName = value;
            OnPropertyChanged("FileName");
         }
      }

      // Even though this is a collection, it should only contain one object, the top level 
      // CompoundObject of the file.
      public ObservableCollection<CompoundObjectViewModel> TopObject
      {
         get { return _topObject; }
         set { _topObject = value; }
      }

      #endregion

      #region Private Methods


      #endregion

      #region Public Methods


      #endregion

   }
}

﻿using System;
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
   public class CoBehaviourViewModel : MicroMvvm.ViewModelBase
   {
      #region Declarations

      private CoBehaviour _modelObject;
      private SteerableObjectPropertiesViewModel _steerableObjProperties;
      private BreakableObjectPropertiesViewModel _breakableObjProperties;
      private ScenePropertiesViewModel _sceneProperties;

      #endregion

      #region Constructors

      public CoBehaviourViewModel(
         TreeViewViewModel treeParent,
         CompoundObjectViewModel parentVm,
         MainViewModel mainVm,
         CoBehaviour modelObject) 
      {
         _modelObject = modelObject;

         _sceneProperties = new ScenePropertiesViewModel(treeParent, parentVm, mainVm, ModelObject.SceneProperties);
         _steerableObjProperties = new SteerableObjectPropertiesViewModel(treeParent, parentVm, mainVm, ModelObject.SteerableObjProps);
         _breakableObjProperties = new BreakableObjectPropertiesViewModel(treeParent, parentVm, mainVm, ModelObject.BreakableObjProps);
      }

      #endregion

      #region Properties

      public CoBehaviour ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("ModelObject");
         }
      }

      public string Type
      {
         get { return ModelObject.Type; }
         set
         {
            ModelObject.Type = value;
            OnPropertyChanged("Type");
            OnPropertyChanged("BehaviourProperties");
         }
      }

      // To be general, all behaviour need to provide the SelectedStateIndex
      // property even though it is only scenes that actually has some
      // meaningfull content. For all other behaviour types, index 0 is returned
      // and the property can not be set.

      public int SelectedStateIndex
      {
         get
         {
            if (Type == "scene")
            {
               return _sceneProperties.SelectedStateIndex;
            }

            return 0;
         }
         set
         {
            if (Type == "scene")
            {
               _sceneProperties.SelectedStateIndex = value;

               OnPropertyChanged("");
            }
         }
      }

      public ObservableCollection<StateViewModel> States
      {
         get
         {
            if (Type == "scene")
            {
               return _sceneProperties.States;
            }

            ObservableCollection<StateViewModel> svms = new ObservableCollection<StateViewModel>();

            StateViewModel svm = new StateViewModel(null, null, null, null, "default");
            svms.Add(svm);

            return svms;
         }
         set { }
      }

      public BehaviourViewModelBase BehaviourProperties
      {
         get
         {
            if (Type == "scene")
            {
               return _sceneProperties;
            }
            else if (Type == "steerableObject")
            {
               return _steerableObjProperties;
            }
            else if (Type == "breakableObject")
            {
               return _breakableObjProperties;
            }

            return null;
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

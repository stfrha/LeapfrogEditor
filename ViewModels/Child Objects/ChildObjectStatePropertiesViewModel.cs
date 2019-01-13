﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


/*
 * The ChildObjectStatePropertiesViewModel encompass the CompoundObject (incl its states, and behaviour) 
 * as well as all the ChildObjects of all states
 */


namespace LeapfrogEditor
{
   public class ChildObjectStatePropertiesViewModel : TreeViewViewModel, IPositionInterface
   {
      #region Declarations

      private TStateProperties<ChildObjectStateProperties> _modelObject;
      private ObservableCollection<CompoundObjectViewModel> _compoundObjectChild = new ObservableCollection<CompoundObjectViewModel>();

      private int _selectedStateIndex = 0;


      #endregion

      #region Constructors

      public ChildObjectStatePropertiesViewModel(
         TreeViewViewModel treeParent,
         CompoundObjectViewModel parentVm,
         MainViewModel mainVm,
         TStateProperties<ChildObjectStateProperties> modelObject) :
         base(treeParent, parentVm, mainVm)
      {
         ModelObject = modelObject;

         CompoundObjectViewModel covm = new CompoundObjectViewModel(this, parentVm, mainVm, modelObject.Properties.CompObj);

         SingleObjectCollectionChild.Add(covm);
         OnPropertyChanged("CompoundObjectChild");

         SingleObjectCollectionChild.CollectionChanged += this.OnCollectionChanged;

         CompoundObjectChild.BuildViewModel();

         int i = ParentVm.Behaviour.States.IndexOf(ParentVm.Behaviour.FindStateVM(ModelObject.State));

         if (i < 0)
         {
            i = 0;
         }

         SelectedStateIndex = i;

      }

      #endregion

      #region Properties

      public TStateProperties<ChildObjectStateProperties> ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public string State
      {
         get { return _modelObject.State; }
      }

      public int SelectedStateIndex
      {
         get
         {
            return _selectedStateIndex;
         }
         set
         {
            int prevI = _selectedStateIndex;

            if (prevI == value)
            {
               return;
            }

            if ((value == -1) || (value > ParentVm.Behaviour.States.Count - 1))
            {
               _selectedStateIndex = 0;
            }
            else
            {
               _selectedStateIndex = value;
            }

            _modelObject.State = ParentVm.Behaviour.States[_selectedStateIndex].StateName;

            OnPropertyChanged("SelectedStateIndex");
            OnPropertyChanged("State");
            CompoundObjectChild.OnPropertyChanged("");

            ParentVm.DeselectAllChildren();

            CompoundObjectViewModel p = ParentVm;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.ParentVm;
            }


         }
      }



      public string File
      {
         get { return _modelObject.Properties.File; }
         set
         {
            _modelObject.Properties.File = value;
            OnPropertyChanged("File");
         }
      }


      public double PosX
      {
         get { return _modelObject.Properties.PosX; }
         set
         {
            _modelObject.Properties.PosX = value;

            OnPropertyChanged("PosX");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = ParentVm;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.ParentVm;
            }
         }
      }

      public double PosY
      {
         get { return _modelObject.Properties.PosY; }
         set
         {
            _modelObject.Properties.PosY = value;

            OnPropertyChanged("PosY");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = ParentVm;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.ParentVm;
            }
         }
      }

      public ObservableCollection<CompoundObjectViewModel> SingleObjectCollectionChild
      {
         get { return _compoundObjectChild; }
         set
         {
            _compoundObjectChild = value;
            OnPropertyChanged("CompoundObjectChild");
         }
      }

      public CompoundObjectViewModel CompoundObjectChild
      {
         get
         {
            if ((_compoundObjectChild != null) && (_compoundObjectChild.Count > 0))
            {
               return _compoundObjectChild[0];
            }

            return null;
         }
      }

      #endregion

      #region Private Methods


      #endregion

      #region Public Methods


      #endregion

      #region Event handlers

      void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         OnPropertyChanged("CompoundObjectChild");

         ////Get the sender observable collection
         //ObservableCollection<string> obsSender = sender as ObservableCollection<string>;

         //List<string> editedOrRemovedItems = new List<string>();
         //foreach (string newItem in e.NewItems)
         //{
         //   editedOrRemovedItems.Add(newItem);
         //}

         //foreach (string oldItem in e.OldItems)
         //{
         //   editedOrRemovedItems.Add(oldItem);
         //}

         ////Get the action which raised the collection changed event
         //NotifyCollectionChangedAction action = e.Action;
      }
      #endregion
   }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapfrogEditor
{
    class LfDragablePointViewModel : MicroMvvm.ViewModelBase, IPositionInterface
   {
      #region Declarations

      private MainViewModel _mainVm;
      private LfDragablePoint _modelObject;
      private LfPolygonViewModel _parent;

      private bool _isSelected;


      #endregion

      #region Constructors

      public LfDragablePointViewModel(MainViewModel mainVm, LfPolygonViewModel parent, LfDragablePoint modelObject)
      {
         MainVm = mainVm;
         Parent = parent;
         ModelObject = modelObject;
         IsSelected = false;
      }

      #endregion

      #region Properties

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

      public LfDragablePoint ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public LfPolygonViewModel Parent
      {
         get { return _parent; }
         set
         {
            _parent = value;
            OnPropertyChanged("");
         }
      }

      public double PosX
      {
         get
         {
            if (ModelObject == null) return 0;

            return ModelObject.PosX;
         }
         set
         {
            if (ModelObject == null) return;

            ModelObject.PosX = value;

            if (Parent != null)
            {
               Parent.OnPropertyChanged("Points");
               Parent.OnPropertyChanged("ClosedPointVms");
               Parent.OnPropertyChanged("BoundingBox");

               CompoundObjectViewModel p = Parent.Parent;

               while (p != null)
               {
                  p.OnPropertyChanged("BoundingBox");
                  p = p.Parent;
               }
            }

            OnPropertyChanged("PosX");
         }
      }


      public double PosY
      {
         get
         {
            if (ModelObject == null) return 0;

            return ModelObject.PosY;
         }
         set
         {
            if (ModelObject == null) return;

            ModelObject.PosY = value;

            if (Parent != null)
            {
               Parent.OnPropertyChanged("Points");
               Parent.OnPropertyChanged("ClosedPointVms");
               Parent.OnPropertyChanged("BoundingBox");

               CompoundObjectViewModel p = Parent.Parent;

               while (p != null)
               {
                  p.OnPropertyChanged("BoundingBox");
                  p = p.Parent;
               }
            }

            OnPropertyChanged("PosY");
         }
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

      #endregion
   }
}
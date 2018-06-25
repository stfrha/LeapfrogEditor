﻿using System;
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
   class LfPolygonViewModel : LfShapeViewModel
   {
      #region Declarations

      private ObservableCollection<LfDragablePointViewModel> _pointVms = new ObservableCollection<LfDragablePointViewModel>();
      // TODO: What about triangles?

      #endregion

      #region Constructors

      public LfPolygonViewModel(MainViewModel mainVm, CompoundObjectViewModel parent) :
         base(mainVm, parent)
      {
      }

      #endregion

      #region Properties

      public LfPolygon LocalModelObject
      {
         get { return (LfPolygon)ModelObject; }
      }

      public PointCollection Points
      {
         get
         {
            PointCollection p = new PointCollection();

            foreach (LfDragablePointViewModel dp in PointVms)
            {
               p.Add(new Point(dp.PosX, dp.PosY));
            }

            return p;
         }
         set
         { }

      }

      public ObservableCollection<LfDragablePointViewModel> PointVms
      {
         get { return _pointVms; }
         set { _pointVms = value; }
      }

      #endregion

      #region public Methods

      public override void InvalidateAll()
      {
         OnPropertyChanged("");

         foreach (LfDragablePointViewModel dp in PointVms)
         {
            dp.OnPropertyChanged("");
         }
      }

      public LfDragablePointViewModel AddPoint(Point point)
      {
         if (LocalModelObject == null) return null;

         LfDragablePoint np = new LfDragablePoint(1, point.X, point.Y);
         LfDragablePointViewModel newPoint = new LfDragablePointViewModel(MainVm, this, np);
         PointVms.Add(newPoint);
         LocalModelObject.AddPoint(newPoint.ModelObject);

         InvalidateAll();
         Parent.OnPropertyChanged("");

         return newPoint;
      }

      public void RemovePoint(LfDragablePointViewModel point)
      {
         if (LocalModelObject == null) return;

         PointVms.Remove(point);
         LocalModelObject.RemovePoint(point.ModelObject);

         InvalidateAll();
         Parent.OnPropertyChanged("");
      }

      public LfDragablePointViewModel InsertPoint(Point insertMe, LfDragablePointViewModel insertBeforeMe)
      {
         if (LocalModelObject == null) return null;

         LfDragablePoint np = new LfDragablePoint(1, insertMe.X, insertMe.Y);
         LfDragablePointViewModel newPoint = new LfDragablePointViewModel(MainVm, this, np);

         int index = 0;

         if (insertBeforeMe != null)
         {
            index = PointVms.IndexOf(insertBeforeMe);
         }

         if (index >= 0)
         {
            PointVms.Insert(index, newPoint);
         }

         if (insertBeforeMe != null)
         {
            LocalModelObject.InsertPoint(newPoint.ModelObject, insertBeforeMe.ModelObject);
         }
         else
         {
            LocalModelObject.InsertPoint(newPoint.ModelObject, null);
         }

         InvalidateAll();
         Parent.OnPropertyChanged("");

         return newPoint;
      }

      public void DeselectAllPoints()
      {
         foreach (LfDragablePointViewModel dp in PointVms)
         {
            dp.IsSelected = false;
         }
      }

      #endregion

      #region protected Methods

      protected override Rect GetBoundingBox()
      {
         double l = double.MaxValue;
         double r = double.MinValue;
         double t = double.MaxValue;
         double b = double.MinValue;

         foreach (LfDragablePointViewModel dp in _pointVms)
         {
            // Convert point according to angle
            Point rtp = RotatedPointFromLocal(new Point(dp.PosX, dp.PosY));
            

            if (rtp.X < l)
            {
               l = rtp.X;
            }

            if (rtp.X > r)
            {
               r = rtp.X;
            }

            if (rtp.Y < t)
            {
               t = rtp.Y;
            }

            if (rtp.Y > b)
            {
               b = rtp.Y;
            }
         }
         Rect tr = new Rect(new Point(l, t), new Point(r, b));

         return tr;
      }

      #endregion

   }
}

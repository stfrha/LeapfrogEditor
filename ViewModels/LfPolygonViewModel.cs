using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LeapfrogEditor
{
   class LfPolygonViewModel : LfShapeViewModel
   {
      #region Declarations

      private ObservableCollection<DragablePointViewModel> _pointVms = new ObservableCollection<DragablePointViewModel>();
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

            foreach (DragablePointViewModel dp in PointVms)
            {
               p.Add(new Point(dp.PosX, dp.PosY));
            }

            return p;
         }
         set
         { }

      }

      public ObservableCollection<DragablePointViewModel> PointVms
      {
         get { return _pointVms; }
         set { _pointVms = value; }
      }

      public ObservableCollection<DragablePointViewModel> ClosedPointVms
      {
         get
         {
            // Copy points collection
            ObservableCollection<DragablePointViewModel> tc = new ObservableCollection<DragablePointViewModel>(_pointVms);

            // Add first item to end to get a closed path
            tc.Add(tc[0]);

            return tc;
         }
         set {}
      }

      #endregion

      #region public Methods

      public DragablePointViewModel AddPoint(Point point)
      {
         if (LocalModelObject == null) return null;

         DragablePoint np = new DragablePoint(1, point.X, point.Y);
         DragablePointViewModel newPoint = new DragablePointViewModel(MainVm, this, np);
         PointVms.Add(newPoint);
         LocalModelObject.AddPoint(newPoint.ModelObject);

         OnPropertyChanged("");
         Parent.OnPropertyChanged("");

         return newPoint;
      }

      public void RemovePoint(DragablePointViewModel point)
      {
         if (LocalModelObject == null) return;

         PointVms.Remove(point);
         LocalModelObject.RemovePoint(point.ModelObject);
         OnPropertyChanged("ClosedPointVms");
         OnPropertyChanged("Points");
         OnPropertyChanged("");
         Parent.OnPropertyChanged("");
      }

      public DragablePointViewModel InsertPoint(Point insertMe, DragablePointViewModel insertBeforeMe)
      {
         if (LocalModelObject == null) return null;

         DragablePoint np = new DragablePoint(1, insertMe.X - PosX, insertMe.Y - PosY);
         DragablePointViewModel newPoint = new DragablePointViewModel(MainVm, this, np);

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

         OnPropertyChanged("");
         Parent.OnPropertyChanged("");

         return newPoint;
      }

      public void DeselectAllPoints()
      {
         foreach (DragablePointViewModel dp in PointVms)
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

         foreach (DragablePointViewModel dp in _pointVms)
         {
            if (dp.PosX < l)
            {
               l = dp.PosX;
            }

            if (dp.PosX > r)
            {
               r = dp.PosX;
            }

            if (dp.PosY < t)
            {
               t = dp.PosY;
            }

            if (dp.PosY > b)
            {
               b = dp.PosY;
            }
         }
         Rect tr = new Rect(new Point(l, t), new Point(r, b));

         return tr;
      }


      #endregion

   }
}

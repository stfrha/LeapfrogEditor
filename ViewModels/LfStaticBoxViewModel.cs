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
   class LfStaticBoxViewModel : LfShapeViewModel, IWidthHeightInterface
   {
      #region Declarations

      #endregion

      #region Constructors

      public LfStaticBoxViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, LfStaticBox modelObject) :
         base(mainVm, parent)
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public LfStaticBox LocalModelObject
      {
         get { return (LfStaticBox)ModelObject; }
      }

      public double Width
      {
         get
         {
            if (LocalModelObject == null) return 0;

            return LocalModelObject.Width;
         }
         set
         {
            if (LocalModelObject == null) return;

            LocalModelObject.Width = value;
            OnPropertyChanged("Width");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = Parent;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.Parent;
            }
         }
      }

      public double Height
      {
         get
         {
            if (LocalModelObject == null) return 0;

            return LocalModelObject.Height;
         }
         set
         {
            if (LocalModelObject == null) return;

            LocalModelObject.Height = value;
            OnPropertyChanged("Height");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = Parent;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.Parent;
            }
         }
      }

      public List<LfPointViewModel> PointVms
      {
         get
         {
            // Create collection of points representing the corners of the Box
            List<LfPointViewModel> pl = new List<LfPointViewModel>();

            pl.Add(new LfPointViewModel(MainVm, this, RotatedPointFromLocal(new Point(-Width / 2, -Height / 2))));
            pl.Add(new LfPointViewModel(MainVm, this, RotatedPointFromLocal(new Point(Width / 2, -Height / 2))));
            pl.Add(new LfPointViewModel(MainVm, this, RotatedPointFromLocal(new Point(Width / 2, Height / 2))));
            pl.Add(new LfPointViewModel(MainVm, this, RotatedPointFromLocal(new Point(-Width / 2, Height / 2))));
            
            return pl;
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

         foreach (LfPointViewModel p in PointVms)
         {
            if (p.PosX < l)
            {
               l = p.PosX;
            }

            if (p.PosX > r)
            {
               r = p.PosX;
            }

            if (p.PosY < t)
            {
               t = p.PosY;
            }

            if (p.PosY > b)
            {
               b = p.PosY;
            }
         }
         Rect tr = new Rect(new Point(l, t), new Point(r, b));

         return tr;
      }

      #endregion

   }
}

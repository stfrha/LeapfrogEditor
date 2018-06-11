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
   class LfSpriteBoxViewModel : LfShapeViewModel
   {
      #region Declarations

      #endregion

      #region Constructors

      public LfSpriteBoxViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, LfSpriteBox modelObject) :
         base(mainVm, parent)
      {
         ModelObject = modelObject;
      }

      #endregion

      #region Properties

      public LfSpriteBox LocalModelObject
      {
         get { return (LfSpriteBox)ModelObject; }
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


      #endregion

      #region protected Methods

      protected override Rect GetBoundingBox()
      {
         Rect r = new Rect(0, 0, Width, Height);
         r.Offset(new Vector(-AnchorX * Width, -AnchorY * Height));
         return r;
      }

      #endregion

   }
}

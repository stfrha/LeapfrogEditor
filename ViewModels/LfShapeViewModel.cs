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
   class LfShapeViewModel : MicroMvvm.ViewModelBase, IPositionInterface
   {
      #region Declarations

      private MainViewModel _mainVm;
      private LfShape _modelObject;
      private CompoundObjectViewModel _parent;
      private bool _isSelected;

      #endregion

      #region Constructors

      public LfShapeViewModel(MainViewModel mainVm, CompoundObjectViewModel parent)
      {
         MainVm = mainVm;
         Parent = parent;
         ModelObject = null;
      }

      #endregion

      #region Properties

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

      public LfShape ModelObject
      {
         get
         {
            if (_modelObject == null) return null;
            return _modelObject;
         }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public CompoundObjectViewModel Parent
      {
         get { return _parent; }
         set
         {
            _parent = value;
            OnPropertyChanged("");
         }
      }

      public string Name
      {
         get
         {
            if (_modelObject == null) return "";

            return _modelObject.Name;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.Name = value;
            OnPropertyChanged("Name");
         }
      }

      public string Texture
      {
         get
         {
            if (_modelObject == null) return "";

            return _modelObject.Texture;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.Texture = value;
            OnPropertyChanged("Texture");
         }
      }

      public double PosX
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.PosX;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.PosX = value;
            OnPropertyChanged("PosX");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = Parent;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.Parent;
            }
         }
      }


      public double PosY
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.PosY;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.PosY = value;
            OnPropertyChanged("PosY");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = Parent;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.Parent;
            }
         }
      }

      public double Angle
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.Angle;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.Angle = value;
            OnPropertyChanged("Angle");
         }
      }

      //public double Width
      //{
      //   get { return _modelObject.Width; }
      //   set
      //   {
      //      _modelObject.Width = value;
      //      OnPropertyChanged("Width");
      //      OnPropertyChanged("BoundingBox");

      //      CompoundObjectViewModel p = Parent;

      //      while (p != null)
      //      {
      //         p.OnPropertyChanged("BoundingBox");
      //         p = p.Parent;
      //      }
      //   }
      //}
      
      //public double Height
      //{
      //   get { return _modelObject.Height; }
      //   set
      //   {
      //      _modelObject.Height = value;
      //      OnPropertyChanged("Height");
      //      OnPropertyChanged("BoundingBox");

      //      CompoundObjectViewModel p = Parent;

      //      while (p != null)
      //      {
      //         p.OnPropertyChanged("BoundingBox");
      //         p = p.Parent;
      //      }
      //   }
      //}

      //public double AnchorX
      //{
      //   get
      //   {
      //      if (_modelObject == null) return 0;

      //      return _modelObject.AnchorX;
      //   }
      //   set
      //   {
      //      if (_modelObject == null) return;

      //      _modelObject.AnchorX = value;
      //      OnPropertyChanged("AnchorX");
      //      OnPropertyChanged("BoundingBox");

      //      CompoundObjectViewModel p = Parent;

      //      while (p != null)
      //      {
      //         p.OnPropertyChanged("BoundingBox");
      //         p = p.Parent;
      //      }
      //   }
      //}

      //public double AnchorY
      //{
      //   get
      //   {
      //      if (_modelObject == null) return 0;

      //      return _modelObject.AnchorY;
      //   }
      //   set
      //   {
      //      if (_modelObject == null) return;

      //      _modelObject.AnchorY = value;
      //      OnPropertyChanged("AnchorY");
      //      OnPropertyChanged("BoundingBox");

      //      CompoundObjectViewModel p = Parent;

      //      while (p != null)
      //      {
      //         p.OnPropertyChanged("BoundingBox");
      //         p = p.Parent;
      //      }
      //   }
      //}

      public uint ZLevel
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.ZLevel;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.ZLevel = value;
            OnPropertyChanged("ZLevel");
         }
      }

      public string CollisionEntity
      {
         get
         {
            if (_modelObject == null) return "";

            return _modelObject.CollisionEntity;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.CollisionEntity = value;
            OnPropertyChanged("CollisionEntity");
         }
      }

      public uint CollisionCategory
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.CollisionCategory;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.CollisionCategory = value;
            OnPropertyChanged("CollisionCategory");
         }
      }

      public uint CollisionMask
      {
         get
         {
            if (_modelObject == null) return 0;

            return _modelObject.CollisionMask;
         }
         set
         {
            if (_modelObject == null) return;

            _modelObject.CollisionMask = value;
            OnPropertyChanged("CollisionMask");
         }
      }

      public Rect BoundingBox
      {
         get
         {
            return GetBoundingBox();
            //Rect r = new Rect(0, 0, Width, Height);
            //r.Offset(new Vector(-AnchorX * Width, -AnchorY * Height));
            //return r;
         }
         set
         { }
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

      #region protected Methods

      protected virtual Rect GetBoundingBox()
      {
         // Base class implementation returns default rect
         return new Rect(new Size(200, 200));
      }

      #endregion

      #region public Methods

      // Let's try to define some points here:
      // A shape point is a point expressed in the coordinate system
      // of the shape.
      // There are two types of shape points: local points and rotated points.
      // The local point is the point of the shape without it being rotated.
      // The rotated is the local points rotated to the Angle property.
      
      // A rotated point is expressed in the shape's coordinate system
      // i.e. using the shape PosX,PosY as origo but it is rotated
      // according to the Angle property
      public Point RotatedPointFromLocal(Point localPoint)
      {
         Point p2 = new Point();

         double cosa = Math.Cos(Angle);
         double sina = Math.Sin(Angle);

         p2.X = localPoint.X * cosa - sina * -localPoint.Y;
         p2.Y = -(localPoint.X * sina + cosa * -localPoint.Y);

         return p2;
      }

      // A local point is expressed in the shape's coordinate system
      // i.e. using the shape PosX,PosY as origo. It is the coordinate
      // if the shape has no rotation (As if the Angle property is zero)
      public Point LocalPointFromRotated(Point localPoint)
      {
         Point p2 = new Point();

         double cosa = Math.Cos(-Angle);
         double sina = Math.Sin(-Angle);

         p2.X = localPoint.X * cosa - sina * -localPoint.Y;
         p2.Y = -(localPoint.X * sina + cosa * -localPoint.Y);


         return p2;
      }

      // This method returns with the supplied point (expressed in the 
      // shape's coordinate system) converted to the coordinate system
      // of the scene (i.e. that of the top level CompoundObject).
      public Point ScenePointFromShape(Point shapePoint)
      {
         return Parent.GetScenePointFromCoPoint(Parent.ShapePointInCo(shapePoint, this));
      }

      public Point ShapePointFromScene(Point scenePoint)
      {
         Point coPoint = Parent.GetCoPointFromScenePoint(scenePoint);

         return Parent.CoPointInShape(coPoint, this);
      }

      #endregion
   }
}

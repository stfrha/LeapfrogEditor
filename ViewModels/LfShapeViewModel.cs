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

      #region private Methods

      protected virtual Rect GetBoundingBox()
      {
         // Base class implementation returns default rect
         return new Rect(new Size(200, 200));
      }

      #endregion
   }
}

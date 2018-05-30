using System;
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
   class CompoundObjectViewModel : MicroMvvm.ViewModelBase, IPositionInterface
   {
      #region Declarations

      private CompoundObject _modelObject;
      private CompoundObjectViewModel _parent;
      private CompositeCollection _shapes = new CompositeCollection()
      {
         new CollectionContainer { Collection = new ObservableCollection<StaticBoxViewModel>() },
         new CollectionContainer { Collection = new ObservableCollection<DynamicBoxViewModel>() },
         new CollectionContainer { Collection = new ObservableCollection<StaticPolygonViewModel>() },
         new CollectionContainer { Collection = new ObservableCollection<DynamicPolygonViewModel>() },
         new CollectionContainer { Collection = new ObservableCollection<BoxedSpritePolygonViewModel>() }
      };

      private bool _isSelected;

      //private ObservableCollection<StaticBoxViewModel> _staticBoxes = new ObservableCollection<StaticBoxViewModel>();
      //private ObservableCollection<DynamicBoxViewModel> _dynamicBoxes = new ObservableCollection<DynamicBoxViewModel>();
      //private ObservableCollection<StaticPolygonViewModel> _staticPolygons = new ObservableCollection<StaticPolygonViewModel>();
      //private ObservableCollection<DynamicPolygonViewModel> _dynamicPolygons = new ObservableCollection<DynamicPolygonViewModel>();
      //private ObservableCollection<BoxedSpritePolygonViewModel> _boxedSpritePolygons = new ObservableCollection<BoxedSpritePolygonViewModel>();
      private ObservableCollection<CompoundObjectViewModel> _childObjects = new ObservableCollection<CompoundObjectViewModel>();


      #endregion

      #region Constructors

      public CompoundObjectViewModel()
      {
         Parent = null;
         ModelObject = new CompoundObject();
      }

      #endregion

      #region Properties

      public CompoundObject ModelObject
      {
         get { return _modelObject; }
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
         get { return _modelObject.Name; }
         set
         {
            _modelObject.Name = value;
            OnPropertyChanged("Name");
         }
      }

      public string Type
      {
         get { return _modelObject.Type; }
         set
         {
            _modelObject.Type = value;
            OnPropertyChanged("Type");
         }
      }

      public string File
      {
         get { return _modelObject.File; }
         set
         {
            _modelObject.File = value;
            OnPropertyChanged("File");
         }
      }

      public double PosX
      {
         get
         {
            return _modelObject.PosX;
         }
         set
         {
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
            return _modelObject.PosY;
         }
         set
         {
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

      public CompositeCollection Shapes
      {
         get { return _shapes; }
         set { _shapes = value; }
      }

      public ObservableCollection<CompoundObjectViewModel> ChildObjects
      {
         get { return _childObjects; }
         set { _childObjects = value; }
      }

      public Rect BoundingBox
      {
         get
         {
            BoundingBoxRect bbr = new BoundingBoxRect();

            foreach (IShapeInterface shape in Shapes)
            {
               Rect cb = shape.BoundingBox;
               cb.Offset(new Vector(shape.PosX, shape.PosY));
               bbr.AddRect(cb);
            }

            foreach (CompoundObjectViewModel child in ChildObjects)
            {
               Rect cb = child.BoundingBox;
               cb.Offset(new Vector(child.PosX, child.PosY));
               bbr.AddRect(cb);
            }

            return bbr.BoundingBox;
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

      #region Public Methods

      public void BuildViewModel(CompoundObject co)
      {
         Shapes.Clear();
         ChildObjects.Clear();

         ModelObject = co;

         foreach (StaticBox sb in co.StaticBoxes)
         {
            StaticBoxViewModel shapevm = new StaticBoxViewModel();
            shapevm.ModelObject = sb;
            shapevm.Parent = this;
            Shapes.Add(shapevm);
         }

         foreach (DynamicBox db in co.DynamicBoxes)
         {
            DynamicBoxViewModel shapevm = new DynamicBoxViewModel();
            shapevm.ModelObject = db;
            shapevm.Parent = this;
            Shapes.Add(shapevm);
         }

         foreach (StaticPolygon sp in co.StaticPolygons)
         {
            StaticPolygonViewModel shapevm = new StaticPolygonViewModel();
            shapevm.ModelObject = sp;

            foreach (DragablePoint dragPoint in sp.Points)
            {
               DragablePointViewModel dragPointVm = new DragablePointViewModel();
               dragPointVm.Parent = shapevm;
               dragPointVm.ModelObject = dragPoint;
               shapevm.PointVms.Add(dragPointVm);
            }

            shapevm.Parent = this;
            Shapes.Add(shapevm);
         }

         foreach (DynamicPolygon dp in co.DynamicPolygons)
         {
            DynamicPolygonViewModel shapevm = new DynamicPolygonViewModel();
            shapevm.ModelObject = dp;

            foreach (DragablePoint dragPoint in dp.Points)
            {
               DragablePointViewModel dragPointVm = new DragablePointViewModel();
               dragPointVm.Parent = shapevm;
               dragPointVm.ModelObject = dragPoint;
               shapevm.PointVms.Add(dragPointVm);
            }

            shapevm.Parent = this;
            Shapes.Add(shapevm);
         }

         foreach (BoxedSpritePolygon bsp in co.BoxedSpritePolygons)
         {
            BoxedSpritePolygonViewModel shapevm = new BoxedSpritePolygonViewModel();
            shapevm.ModelObject = bsp;

            foreach (DragablePoint dragPoint in bsp.Points)
            {
               DragablePointViewModel dragPointVm = new DragablePointViewModel();
               dragPointVm.Parent = shapevm;
               dragPointVm.ModelObject = dragPoint;
               shapevm.PointVms.Add(dragPointVm);
            }

            shapevm.Parent = this;
            Shapes.Add(shapevm);
         }

         foreach (CompoundObject tco in co.ChildObjects)
         {
            CompoundObjectViewModel covm = new CompoundObjectViewModel();
            covm.BuildViewModel(tco);
            covm.Parent = this;
            ChildObjects.Add(covm);
         }

      }

      public void DeselectAllChildren()
      {
         foreach (IShapeInterface shape in Shapes)
         {
            shape.IsSelected = false;
         }

         foreach (CompoundObjectViewModel child in ChildObjects)
         {
            child.DeselectAllChildren();
            child.IsSelected = false;
         }
      }

      #endregion

   }
}

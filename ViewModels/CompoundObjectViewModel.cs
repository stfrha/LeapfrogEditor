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

      private CompoundObjectRef _refObject = new CompoundObjectRef();
      private CompoundObjectViewModel _parent = new CompoundObjectViewModel();

      private int _selectedStateIndex = 0;
      private ObservableCollection<string> _states = new ObservableCollection<string>();

      // Shapes and children collections are two dimensional to accomondate for all 
      // State properties
      private ObservableCollection<CompositeCollection> _shapes = new ObservableCollection<CompositeCollection>();

      //private CompositeCollection _shapes = new CompositeCollection()
      //{
      //   new CollectionContainer { Collection = new ObservableCollection<StaticBoxViewModel>() },
      //   new CollectionContainer { Collection = new ObservableCollection<DynamicBoxViewModel>() },
      //   new CollectionContainer { Collection = new ObservableCollection<StaticPolygonViewModel>() },
      //   new CollectionContainer { Collection = new ObservableCollection<DynamicPolygonViewModel>() },
      //   new CollectionContainer { Collection = new ObservableCollection<BoxedSpritePolygonViewModel>() }
      //};

      private ObservableCollection<ObservableCollection<CompoundObjectViewModel>> _childObjects = new ObservableCollection<ObservableCollection<CompoundObjectViewModel>>();

      private bool _isSelected;

      #endregion

      #region Constructors

      public CompoundObjectViewModel()
      {
      }

      #endregion

      #region Properties

      public CompoundObjectRef RefObject
      {
         get { return _refObject; }
         set
         {
            _refObject = value;
            OnPropertyChanged("");
         }
      }

      public CompoundObject ModelObject
      {
         get
         {
            return _refObject.StateProperties[_selectedStateIndex].CompObj;
         }
         set { }
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
         get { return _refObject.Name; }
         set
         {
            _refObject.Name = value;
            OnPropertyChanged("Name");
         }
      }

      public string Type
      {
         get { return ModelObject.Type; }
         set
         {
            ModelObject.Type = value;
            OnPropertyChanged("Type");
         }
      }

      public string File
      {
         get { return _refObject.StateProperties[_selectedStateIndex].File; }
         set
         {
            _refObject.StateProperties[_selectedStateIndex].File = value;
            OnPropertyChanged("File");
         }
      }

      public double PosX
      {
         get
         {
            return _refObject.StateProperties[_selectedStateIndex].PosX;
         }
         set
         {
            _refObject.StateProperties[_selectedStateIndex].PosX = value;
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
            return _refObject.StateProperties[_selectedStateIndex].PosY;
         }
         set
         {
            _refObject.StateProperties[_selectedStateIndex].PosY = value;
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

      public int SelectedStateIndex
      {
         get { return _selectedStateIndex; }
         set
         {
            _selectedStateIndex = value;
            DeselectAllChildren();
            OnPropertyChanged("");
         }
      }

      public ObservableCollection<string> States
      {
         get { return _states; }
         set { _states = value; }
      }

      public ObservableCollection<CompositeCollection> Shapes
      {
         get { return _shapes; }
         set { _shapes = value; }
      }

      public ObservableCollection<ObservableCollection<CompoundObjectViewModel>> ChildObjects
      {
         get { return _childObjects; }
         set { _childObjects = value; }
      }

      public Rect BoundingBox
      {
         get
         {
            BoundingBoxRect bbr = new BoundingBoxRect();

            foreach (IShapeInterface shape in Shapes[_selectedStateIndex])
            {
               Rect cb = shape.BoundingBox;
               cb.Offset(new Vector(shape.PosX, shape.PosY));
               bbr.AddRect(cb);
            }

            foreach (CompoundObjectViewModel child in ChildObjects[_selectedStateIndex])
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

      #region Private Methods

      private CompositeCollection SetShapes(CompoundObject co)
      {
         CompositeCollection shapes = new CompositeCollection()
         {
            new CollectionContainer { Collection = new ObservableCollection<StaticBoxViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<DynamicBoxViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<StaticPolygonViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<DynamicPolygonViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<BoxedSpritePolygonViewModel>() }
         };

         foreach (StaticBox sb in co.StaticBoxes)
         {
            StaticBoxViewModel shapevm = new StaticBoxViewModel();
            shapevm.ModelObject = sb;
            shapevm.Parent = this;
            shapes.Add(shapevm);
         }

         foreach (DynamicBox db in co.DynamicBoxes)
         {
            DynamicBoxViewModel shapevm = new DynamicBoxViewModel();
            shapevm.ModelObject = db;
            shapevm.Parent = this;
            shapes.Add(shapevm);
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
            shapes.Add(shapevm);
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
            shapes.Add(shapevm);
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
            shapes.Add(shapevm);
         }

         return shapes;

      }

      private ObservableCollection<CompoundObjectViewModel> SetChildren(CompoundObject co)
      {
         ObservableCollection<CompoundObjectViewModel> tempChildren = new ObservableCollection<CompoundObjectViewModel>();

         foreach (CompoundObjectRef tco in co.ChildObjectRefs)
         {
            CompoundObjectViewModel covm = new CompoundObjectViewModel();
            covm.RefObject = tco;
            covm.Parent = this;
            covm.BuildViewModel(tco);
            tempChildren.Add(covm);
         }

         return tempChildren;
      }

      #endregion

      #region Public Methods

      public void BuildViewModel(CompoundObjectRef cor)
      {
         Shapes.Clear();
         ChildObjects.Clear();

         foreach (ObjectRefStateProperties sp in cor.StateProperties)
         {
            Shapes.Add(SetShapes(sp.CompObj));
            ChildObjects.Add(SetChildren(sp.CompObj));
         }
      }

      public void DeselectAllChildren()
      {
         foreach (IShapeInterface shape in Shapes[_selectedStateIndex])
         {
            shape.IsSelected = false;
         }

         foreach (CompoundObjectViewModel child in ChildObjects[_selectedStateIndex])
         {
            child.DeselectAllChildren();
            child.IsSelected = false;
         }
      }

      #endregion

   }
}

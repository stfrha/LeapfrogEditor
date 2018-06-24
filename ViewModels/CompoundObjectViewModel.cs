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

      private CompoundObjectRef _refObject;
      private CompoundObjectViewModel _parent;
      private MainViewModel _mainVm;

      private int _selectedStateIndex = 0;

      private ObservableCollection<CompositeCollection> _shapes = new ObservableCollection<CompositeCollection>();

      // Children collection is two dimensional to accomondate for all State properties
      private ObservableCollection<ObservableCollection<CompoundObjectViewModel>> _childObjects = new ObservableCollection<ObservableCollection<CompoundObjectViewModel>>();

      private bool _isSelected;

      #endregion

      #region Constructors

      public CompoundObjectViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, CompoundObjectRef refObject)
      {
         MainVm = mainVm;
         Parent = parent;
         RefObject = refObject;
      }

      #endregion

      #region Properties

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

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
            if (_refObject != null)
            {
               if (_refObject.StateProperties.Count > 0)
               {
                  return _refObject.StateProperties[_selectedStateIndex].CompObj;
               }
            }
            return null;
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
            if (_refObject != null)
            {
               if (_refObject.StateProperties.Count > 0)
               {
                  return _refObject.StateProperties[_selectedStateIndex].PosX;
               }
            }
            return 0;
         }
         set
         {
            if (_refObject != null)
            {
               if (_refObject.StateProperties.Count > 0)
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
         }
      }

      public double PosY
      {
         get
         {
            if (_refObject != null)
            {
               if (_refObject.StateProperties.Count > 0)
               {
                  return _refObject.StateProperties[_selectedStateIndex].PosY;
               }
            }
            return 0;
         }
         set
         {
            if (_refObject != null)
            {
               if (_refObject.StateProperties.Count > 0)
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
         get
         {
            ObservableCollection<string> s = new ObservableCollection<string>();

            foreach (ObjectRefStateProperties sp in _refObject.StateProperties)
            {
               s.Add(sp.State);
            }            

            return s;
         }
         set { }
      }

      public CompositeCollection Shapes
      {
         get
         {
            if (_shapes.Count > 0)
            {
               return _shapes[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if (_shapes.Count > 0)
            {
               _shapes[_selectedStateIndex] = value;
            }
         }
      }

      public ObservableCollection<CompoundObjectViewModel> ChildObjects
      {
         get
         {
            if (_childObjects.Count > 0)
            {
               return _childObjects[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if (_childObjects.Count > 0)
            {
               _childObjects[_selectedStateIndex] = value;
            }
         }
      }

      public Rect BoundingBox
      {
         get
         {
            if (Shapes.Count == 11)
            {
               int a = 10;
            }
            if ((Shapes.Count == 0) && (ChildObjects.Count == 0))
            {
               return new Rect(0,0,0,0);
            }

            BoundingBoxRect bbr = new BoundingBoxRect();

            if (Shapes.Count > 0)
            {
               foreach (object o in Shapes)
               {
                  if (o is LfShapeViewModel)
                  {
                     LfShapeViewModel shape = (LfShapeViewModel)o;

                     Rect cb = shape.BoundingBox;
                     cb.Offset(new Vector(shape.PosX, shape.PosY));
                     bbr.AddRect(cb);
                  }
               }
            }

            if (ChildObjects.Count > 0)
            {
               foreach (CompoundObjectViewModel child in ChildObjects)
               {
                  Rect cb = child.BoundingBox;
                  cb.Offset(new Vector(child.PosX, child.PosY));
                  bbr.AddRect(cb);
               }
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
            new CollectionContainer { Collection = new ObservableCollection<LfSpriteBoxViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfSpritePolygonViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfStaticBoxViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfStaticCircleViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfStaticPolygonViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfStaticBoxedSpritePolygonViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfDynamicBoxViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfDynamicCircleViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfDynamicPolygonViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<LfDynamicBoxedSpritePolygonViewModel>() },
         };

         foreach (LfStaticBox sb in co.StaticBoxes)
         {
            LfStaticBoxViewModel shapevm = new LfStaticBoxViewModel(MainVm, this, sb);
            shapes.Add(shapevm);
         }

         foreach (LfStaticCircle sb in co.StaticCircles)
         {
            LfStaticCircleViewModel shapevm = new LfStaticCircleViewModel(MainVm, this, sb);
            shapes.Add(shapevm);
         }

         foreach (LfStaticPolygon sp in co.StaticPolygons)
         {
            LfStaticPolygonViewModel shapevm = new LfStaticPolygonViewModel(MainVm, this, sp);

            foreach (LfDragablePoint dragPoint in sp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Add(shapevm);
         }

         foreach (LfDynamicBox db in co.DynamicBoxes)
         {
            LfDynamicBoxViewModel shapevm = new LfDynamicBoxViewModel(MainVm, this, db);
            shapes.Add(shapevm);
         }

         foreach (LfDynamicCircle db in co.DynamicCircles)
         {
            LfDynamicCircleViewModel shapevm = new LfDynamicCircleViewModel(MainVm, this, db);
            shapes.Add(shapevm);
         }

         foreach (LfDynamicPolygon dp in co.DynamicPolygons)
         {
            LfDynamicPolygonViewModel shapevm = new LfDynamicPolygonViewModel(MainVm, this, dp);

            foreach (LfDragablePoint dragPoint in dp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Add(shapevm);
         }

         foreach (LfStaticBoxedSpritePolygon bsp in co.StaticBoxedSpritePolygons)
         {
            LfStaticBoxedSpritePolygonViewModel shapevm = new LfStaticBoxedSpritePolygonViewModel(MainVm, this, bsp);

            foreach (LfDragablePoint dragPoint in bsp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Add(shapevm);
         }

         foreach (LfDynamicBoxedSpritePolygon bsp in co.DynamicBoxedSpritePolygons)
         {
            LfDynamicBoxedSpritePolygonViewModel shapevm = new LfDynamicBoxedSpritePolygonViewModel(MainVm, this, bsp);

            foreach (LfDragablePoint dragPoint in bsp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Add(shapevm);
         }


         return shapes;

      }

      private ObservableCollection<CompoundObjectViewModel> SetChildren(CompoundObject co)
      {
         ObservableCollection<CompoundObjectViewModel> tempChildren = new ObservableCollection<CompoundObjectViewModel>();

         foreach (CompoundObjectRef tco in co.ChildObjectRefs)
         {
            CompoundObjectViewModel covm = new CompoundObjectViewModel(MainVm, this, tco);
            covm.BuildViewModel(tco);
            tempChildren.Add(covm);
         }

         return tempChildren;
      }

      #endregion

      #region Public Methods

      public void BuildViewModel(CompoundObjectRef cor)
      {
         _shapes.Clear();
         _childObjects.Clear();

         foreach (ObjectRefStateProperties sp in cor.StateProperties)
         {
            _shapes.Add(SetShapes(sp.CompObj));
            _childObjects.Add(SetChildren(sp.CompObj));
         }
      }

      public void DeselectAllChildren()
      {

         foreach (object o in Shapes)
         {
            if (o is LfShapeViewModel)
            {
               LfShapeViewModel shape = (LfShapeViewModel)o;

               shape.IsSelected = false;
            }
         }

         foreach (CompoundObjectViewModel child in ChildObjects)
         {
            child.DeselectAllChildren();
            child.IsSelected = false;
         }
      }

      // This method returns with the point of the shape's coordinate system
      // in this CompoundObject's coordinate system. 
      // The position of the shape is added to the shape point. The shape point
      // can be either rotated or not.
      public Point ShapePointInCo(Point shapePoint, LfShapeViewModel shape)
      {
         if (shape == null) return new Point(0, 0);

         Point coPoint = shapePoint;

         coPoint.Offset(shape.PosX, shape.PosY);

         return coPoint;
      }

      // This method returns with the point of the CO's coordinate system
      // in this Shape's coordinate system. 
      // The position of the shape is subtracted from the CO point. 
      public Point CoPointInShape(Point coPoint, LfShapeViewModel shape)
      {
         if (shape == null) return new Point(0, 0);

         Point shapePoint = coPoint;

         shapePoint.Offset(-shape.PosX, -shape.PosY);

         return shapePoint;
      }

      // This method returns with the supplied point (expressed in this CompoundObject's
      // coordinate system) converted to the parent CompoundObjects coordinate system.
      // This is done by adding the Position of this CompoundObject.
      public Point ParentCoPoint(Point coPoint)
      {
         Point parentPoint = coPoint;

         parentPoint.Offset(PosX, PosY);

         return parentPoint;
      }

      // This method returns with the supplied point (expressed in the parent of this 
      // CompoundObject's coordinate system) converted to this CompoundObjects coordinate system.
      // This is done by subtracting the parentPoint with the position of this CompoundObject.
      public Point CoPointFromParent(Point parentPoint)
      {
         Point coPoint = parentPoint;

         coPoint.Offset(-PosX, -PosY);

         return coPoint;
      }

      // This method returns with the supplied point in the scene's coordinate system
      // (Equal to the top level CompoundObject's coordinate system). 
      // The point is converted to the parent's coordinate system and the method is then
      // recursively called for the parent until the parent is null. In this case we are 
      // at the top level CompoundObject which is the scene. Then we returns the point
      public Point GetScenePointFromCoPoint(Point coPoint)
      {
         if (Parent != null)
         {
            Point parentPoint = ParentCoPoint(coPoint);
            return GetScenePointFromCoPoint(parentPoint);
         }
         else
         {
            return coPoint;
         }
      }

      public Point GetCoPointFromScenePoint(Point scenePoint)
      {
         if (Parent != null)
         {
            Point parentPoint = CoPointFromParent(scenePoint);
            return GetCoPointFromScenePoint(parentPoint);
         }
         else
         {
            return scenePoint;
         }
      }



      #endregion

   }
}

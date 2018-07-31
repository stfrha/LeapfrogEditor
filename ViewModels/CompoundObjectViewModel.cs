﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LeapfrogEditor
{
   public class CompoundObjectViewModel : MicroMvvm.ViewModelBase, IPositionInterface
   {
      #region Declarations

      private CompoundObjectRef _refObject;
      private CompoundObjectViewModel _parent;
      private MainViewModel _mainVm;

      private int _selectedStateIndex = 0;

      private ObservableCollection<CompositeCollection> _shapes = new ObservableCollection<CompositeCollection>();
      private ObservableCollection<CompositeCollection> _joints = new ObservableCollection<CompositeCollection>();

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
         SelectedStateIndex = 0;
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
         get
         {
            if ((_selectedStateIndex >= 0) && _refObject.StateProperties.Count > 0)
            {
               return _refObject.StateProperties[_selectedStateIndex].File;
            }
            return "";
         }
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
               if ((_selectedStateIndex >= 0) && (_refObject.StateProperties.Count > 0))
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
               if ((_selectedStateIndex >= 0) && (_refObject.StateProperties.Count > 0))
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
               if ((_selectedStateIndex >= 0) && (_refObject.StateProperties.Count > _selectedStateIndex))
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
               if ((_selectedStateIndex >= 0) && (_refObject.StateProperties.Count > 0))
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
         get
         {
            return _selectedStateIndex;
         }
         set
         {
            if (value == -1)
            {
               _selectedStateIndex = 0;
            }
            else
            {
               _selectedStateIndex = value;
            }

            DeselectAllChildren();
            OnPropertyChanged("");

            CompoundObjectViewModel p = Parent;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.Parent;
            }
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
            if ((_selectedStateIndex >= 0) && (_shapes.Count > _selectedStateIndex))
            {
               return _shapes[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((_selectedStateIndex >= 0) && (_shapes.Count > _selectedStateIndex))
            {
               _shapes[_selectedStateIndex] = value;
            }
         }
      }

      public CompositeCollection Joints
      {
         get
         {
            if ((_selectedStateIndex >= 0) && (_joints.Count > _selectedStateIndex))
            {
               return _joints[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((_selectedStateIndex >= 0) && (_joints.Count > _selectedStateIndex))
            {
               _joints[_selectedStateIndex] = value;
            }
         }
      }

      public ObservableCollection<CompoundObjectViewModel> ChildObjects
      {
         get
         {
            if ((_selectedStateIndex >= 0) && (_childObjects.Count > _selectedStateIndex))
            {
               return _childObjects[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((_selectedStateIndex >= 0) && (_childObjects.Count > _selectedStateIndex))
            {
               _childObjects[_selectedStateIndex] = value;
            }
         }
      }

      public Rect BoundingBox
      {
         get
         {
            if (Shapes == null) return new Rect(0, 0, 0, 0);

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
            new CollectionContainer { Collection = new ObservableCollection<AsteroidFieldViewModel>() },
         };

         foreach (LfSpriteBox sb in co.SpriteBoxes)
         {
            LfSpriteBoxViewModel shapevm = new LfSpriteBoxViewModel(MainVm, this, sb);
            shapes.Add(shapevm);
         }

         foreach (LfSpritePolygon sp in co.SpritePolygons)
         {
            LfSpritePolygonViewModel shapevm = new LfSpritePolygonViewModel(MainVm, this, sp);

            foreach (LfDragablePoint dragPoint in sp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Add(shapevm);
         }

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

         foreach (AsteroidFieldRef asf in co.AsteroidFields)
         {
            AsteroidFieldViewModel shapevm = new AsteroidFieldViewModel(MainVm, this, asf);
            shapes.Add(shapevm);
         }

         return shapes;

      }

      private CompositeCollection SetJoints(CompoundObject co)
      {
         CompositeCollection joints = new CompositeCollection()
         {
            new CollectionContainer { Collection = new ObservableCollection<WeldJointViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<RevoluteJointViewModel>() },
            new CollectionContainer { Collection = new ObservableCollection<PrismaticJointViewModel>() },
         };

         foreach (WeldJoint wj in co.WeldJoints)
         {
            WeldJointViewModel wjvm = new WeldJointViewModel(MainVm, this, wj);
            joints.Add(wjvm);
         }

         foreach (RevoluteJoint rj in co.RevoluteJoints)
         {
            RevoluteJointViewModel rjvm = new RevoluteJointViewModel(MainVm, this, rj);
            joints.Add(rjvm);
         }

         foreach (PrismaticJoint pj in co.PrismaticJoints)
         {
            PrismaticJointViewModel pjvm = new PrismaticJointViewModel(MainVm, this, pj);
            joints.Add(pjvm);
         }

         return joints;
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

      public void RemoveShape(LfShapeViewModel svm)
      {
         // Check if there are any joints connected to this svm, if so, removed them
         // We may remove joints so we need a for loop here:
         for (int i = Joints.Count - 1; i >= 0; i--)
         {
            // Below will take care of all joints since they
            // all inherit from WeldJoint
            if (Joints[i] is WeldJointViewModel)
            {
               WeldJointViewModel joint = (WeldJointViewModel)Joints[i];

               if ((joint.AName == svm.Name) || (joint.BName == svm.Name))
               {
                  // Remove the joint
                  ModelObject.RemoveJoint(joint.ModelObject);
                  Joints.RemoveAt(i);
               }
            }
         }

         // Remove the shape model
         ModelObject.RemoveShape(svm.ModelObject);

         // Remove the shape viewmodel from this
         Shapes.Remove(svm);

         // If there are no more shapes in the CO, remove the CO
         if (Shapes.Count == 0)
         {
            //Parent.ChildObjects.Remove(this);
            //Parent.ModelObject.ChildObjectRefs(this.RefObject)
         }

         OnPropertyChanged("");
      }

      public void InvalidateJoints()
      {
         foreach (object o in Joints)
         {
            // Below will take care of all joints since they
            // all inherit from WeldJoint
            if (o is WeldJointViewModel)
            {
               WeldJointViewModel joint = (WeldJointViewModel)o;

               joint.OnPropertyChanged("");
            }
         }
      }

      public LfShapeViewModel FindShape(string name, CompositeCollection shapes)
      {
         foreach (object o in shapes)
         {
            if (o is LfShapeViewModel)
            {
               LfShapeViewModel shape = (LfShapeViewModel)o;

               if (shape.Name == name)
               {
                  return shape;
               }
            }
         }

         return null;
      }

      public void BuildViewModel(CompoundObjectRef cor)
      {
         _shapes.Clear();
         _joints.Clear();
         _childObjects.Clear();

         foreach (ObjectRefStateProperties sp in cor.StateProperties)
         {
            CompositeCollection sc = SetShapes(sp.CompObj);
            _shapes.Add(sc);

            CompositeCollection jc = SetJoints(sp.CompObj);
            _joints.Add(jc);

            _childObjects.Add(SetChildren(sp.CompObj));

            // Only now is the Joints property valid for this state
            foreach (object o in jc)
            {
               if (o is WeldJointViewModel)
               {
                  WeldJointViewModel joint = (WeldJointViewModel)o;

                  joint.ConnectToShapes(sc);
               }
            }
         }
      }

      public void DeselectAllChildren()
      {
         if (Shapes != null)
         {
            foreach (object o in Shapes)
            {
               if (o is LfShapeViewModel)
               {
                  LfShapeViewModel shape = (LfShapeViewModel)o;

                  shape.IsSelected = false;
               }
            }
         }

         if (Joints != null)
         {
            foreach (object o in Joints)
            {
               if (o is WeldJointViewModel)
               {
                  WeldJointViewModel joint = (WeldJointViewModel)o;

                  joint.IsSelected = false;
               }
            }
         }

         if (ChildObjects != null)
         {
            foreach (CompoundObjectViewModel child in ChildObjects)
            {
               child.DeselectAllChildren();
               child.IsSelected = false;
            }
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

      public void GenerateTriangles()
      {
         foreach (object o in Shapes)
         {
            if (o is LfPolygonViewModel)
            {
               LfPolygonViewModel pvm = (LfPolygonViewModel)o;

               pvm.GenerateTriangles();
            }
         }

         foreach (CompoundObjectViewModel covm in ChildObjects)
         {
            covm.GenerateTriangles();
         }
      }

      #endregion

   }
}

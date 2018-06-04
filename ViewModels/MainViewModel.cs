using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   enum LeftClickState
   {
      none,
      staticPolygon,
      dynamicPolygon,
      boxedSpritePolygon,
      staticBox,
      dynamicBox,
      addPoint
   }

   class MainViewModel : MicroMvvm.ViewModelBase
   {

      #region Declarations

      private CompoundObjectRef _myCpRef = new CompoundObjectRef();
      private ObjectRefStateProperties _myStateProp = new ObjectRefStateProperties();
      private CompoundObject _myCP;
      private CompoundObjectViewModel _myCpVm;

      private CompoundObjectViewModel _selectedCompoundObject = null;
      private ObservableCollection<IShapeInterface> _selectedShapes = new ObservableCollection<IShapeInterface>();
      private ObservableCollection<DragablePointViewModel> _selectedPoints = new ObservableCollection<DragablePointViewModel>();

      private LeftClickState _LeftClickState = LeftClickState.none;

      #endregion

      #region Constructor

      public MainViewModel()
      {
         MyCP = CompoundObject.ReadFromFile("landing_scene.xml");
         MyStateProp.CompObj = MyCP;
         MyCpRef.StateProperties.Add(MyStateProp);

         MyCpVm = new CompoundObjectViewModel(this, null, MyCpRef);
         MyCpVm.BuildViewModel(MyCpRef);
      }

      #endregion

      #region Properties

      public CompoundObjectRef MyCpRef
      {
         get { return _myCpRef; }
         set { _myCpRef = value; }
      }

      public ObjectRefStateProperties MyStateProp
      {
         get { return _myStateProp; }
         set { _myStateProp = value; }
      }

      public CompoundObject MyCP
      {
         get { return _myCP; }
         set { _myCP = value; }
      }

      public CompoundObjectViewModel MyCpVm
      {
         get { return _myCpVm; }
         set { _myCpVm = value; }
      }

      #endregion

      #region Commands

      void ReloadExecute(Object parameter)
      {
         MyCP = CompoundObject.ReadFromFile("landing_scene.xml");
         MyStateProp.CompObj = MyCP;
         MyCpRef.StateProperties.Add(MyStateProp);

         MyCpVm.BuildViewModel(MyCpRef);

      }

      bool CanReloadExecute(Object parameter)
      {
         return true;
      }

      public ICommand Reload
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => ReloadExecute(parameter), parameter => CanReloadExecute(parameter));
         }
      }

      void SaveExecute(Object parameter)
      {
         MyCpVm.ModelObject.WriteToFile();
      }

      bool CanSaveExecute(Object parameter)
      {
         return true;
      }

      public ICommand Save
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => SaveExecute(parameter), parameter => CanSaveExecute(parameter));
         }
      }

      void StateExecute(Object parameter)
      {
         CompoundObjectViewModel covm = MyCpVm.ChildObjects[0];

         if (covm.SelectedStateIndex == 0)
         {
            covm.SelectedStateIndex = 1;
         }
         else
         {
            covm.SelectedStateIndex = 0;
         }
      }

      bool CanStateExecute(Object parameter)
      {
         return true;
      }

      public ICommand State
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => StateExecute(parameter), parameter => CanStateExecute(parameter));
         }
      }


      

      void NewStaticPolygonExecute(Object parameter)
      {
         // Deselect all other shapes when generating a new polygon
         foreach (IShapeInterface shape in _selectedShapes)
         {
            shape.IsSelected = false;
         }

         _selectedShapes.Clear();

         _LeftClickState = LeftClickState.staticPolygon;
      }

      bool CanNewStaticPolygonExecute(Object parameter)
      {
         return (_selectedCompoundObject != null);
      }

      public ICommand NewStaticPolygon
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => NewStaticPolygonExecute(parameter), parameter => CanNewStaticPolygonExecute(parameter));
         }
      }

      #endregion

      #region Public Methods

      // NOTE: FrameworkElement is used to communicate from the View. It can 
      // be used to obtain the ViewModel object from it's datacontext.
      // In some cases we need the type of FrameworkElement (to see if a 
      // Line or Rectangle).

      // All mouse handling function return true if the the data was used for some action
      // (if original event was handled)

      public bool MouseDown(FrameworkElement target, MouseButton button, Point clickPoint, bool shift, bool ctrl, bool alt)
      {
         if (button == MouseButton.Left)
         {
            // Decode target ViewModel and view oobject that was clicked
            if (target.DataContext is CompoundObjectViewModel)
            {
               // Mouse down on rectangle around CompoundObject
               CompoundObjectViewModel covm = (CompoundObjectViewModel)target.DataContext;

               //Debug.WriteLine("Clicked rectangle around CompoundObject");

               return true;
            }
            else if ((target is Rectangle) && (target.DataContext is DragablePointViewModel))
            {
               // Mouse down on rectangle of DragablePoint
               DragablePointViewModel dpvm = (DragablePointViewModel)target.DataContext;

               if (dpvm.IsSelected)
               {
                  // Could be the beginning of dragging

               }
               else
               {
                  if (!ctrl)
                  {
                     foreach (DragablePointViewModel selpoint in _selectedPoints)
                     {
                        selpoint.IsSelected = false;
                     }
                     _selectedPoints.Clear();
                  }

                  _selectedPoints.Add(dpvm);
                  dpvm.IsSelected = true;
               }

               //Debug.WriteLine("Clicked rectangle of DragablePoint");

               return true;
            }
            else if ((target is Line) && (target.DataContext is DragablePointViewModel))
            {
               // Mouse down on Line between DragablePoints 
               DragablePointViewModel dpvm = (DragablePointViewModel)target.DataContext;

               if (ctrl)
               {
                  DragablePointViewModel newPoint = dpvm.Parent.InsertPoint(clickPoint, dpvm);
                  foreach (DragablePointViewModel selpoint in _selectedPoints)
                  {
                     selpoint.IsSelected = false;
                  }
                  _selectedPoints.Clear();

                  _selectedPoints.Add(newPoint);
                  newPoint.IsSelected = true;
               }

               //Debug.WriteLine("Clicked line between DragablePoint");

               return true;
            }
            else if (target.DataContext is IShapeInterface)
            {
               // Mouse down on Shape
               IShapeInterface shvm = (IShapeInterface)target.DataContext;

               //Debug.WriteLine("Clicked on Shape");

               if (shvm.Parent.IsSelected)
               {
                  if (shvm.IsSelected)
                  {
                     // Could be the start of dragging the selection

                  }
                  else
                  {
                     if (!ctrl)
                     {
                        foreach (IShapeInterface selshape in _selectedShapes)
                        {
                           selshape.IsSelected = false;
                        }
                        _selectedShapes.Clear();
                     }

                     _selectedShapes.Add(shvm);
                     shvm.IsSelected = true;
                  }

               }
               else
               {
                  if (_selectedCompoundObject != null)
                  {
                     _selectedCompoundObject.IsSelected = false;
                  }
                  _selectedCompoundObject = shvm.Parent;
               }

               if (!shvm.IsSelected)
               {
                  shvm.Parent.IsSelected = true;
                  _selectedShapes.Add(shvm);
                  _selectedCompoundObject = shvm.Parent;

               }

               return true;
            }
            else if ((target is Rectangle) && (target.DataContext is IPositionInterface))
            {
               // Mouse down on rectangle around Shape
               IPositionInterface posvm = (IPositionInterface)target.DataContext;

               //Debug.WriteLine("Clicked rectangle around something that can be dragged");

               return true;
            }
         }

         return false;
      }

      public bool MouseMove(FrameworkElement target, Vector dragVector, bool shift, bool ctrl, bool alt)
      {
         // Decode target ViewModel and view oobject that was clicked
         if (target.DataContext is CompoundObjectViewModel)
         {
            // Mouse move on rectangle around CompoundObject
            CompoundObjectViewModel covm = (CompoundObjectViewModel)target.DataContext;

            covm.PosX += dragVector.X;
            covm.PosY += dragVector.Y;

            //Debug.WriteLine("Movbed rectangle around CompoundObject");
            return true;
         }
         else if ((target is Rectangle) && (target.DataContext is DragablePointViewModel))
         {
            // Mouse down on rectangle of DragablePoint
            DragablePointViewModel dpvm = (DragablePointViewModel)target.DataContext;

            foreach (DragablePointViewModel point in _selectedPoints)
            {
               point.PosX += dragVector.X;
               point.PosY += dragVector.Y;
            }

            //Debug.WriteLine("Move rectangle of DragablePoint");

            return true;
         }
         else if ((target is Line) && (target.DataContext is DragablePointViewModel))
         {
            // Mouse down on Line between DragablePoints 
            DragablePointViewModel dpvm = (DragablePointViewModel)target.DataContext;

            //Debug.WriteLine("Clicked line between DragablePoint");

            return true;
         }
         else if ((target is Rectangle) && (target.DataContext is IPositionInterface))
         {
            // Mouse move on rectangle around Shape
            IPositionInterface posvm = (IPositionInterface)target.DataContext;

            foreach (IPositionInterface shape in _selectedShapes)
            {
               shape.PosX += dragVector.X;
               shape.PosY += dragVector.Y;
            }


            //Debug.WriteLine("Moved rectangle around something that can be dragged");

            return true;
         }

         return false;
      }

      // clickPoint will be in coordinates of the parent CompoundObject
      public bool MouseUp(FrameworkElement targe, MouseButton button, Point clickPoint, bool shift, bool ctrl, bool alt)
      {

         return false;
      }

      public bool BackgroundMouseUp(Point clickPoint, MouseButton button, bool shift, bool ctrl, bool alt)
      {
         if (button == MouseButton.Left)
         {
            //Debug.WriteLine("Clicked on background");
            if (_LeftClickState == LeftClickState.none)
            {
               MyCpVm.DeselectAllChildren();
               MyCpVm.IsSelected = false;
            }
            else if (_LeftClickState == LeftClickState.staticPolygon)
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(_selectedCompoundObject.PosX, _selectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               StaticPolygon newPolygon = new StaticPolygon();
               newPolygon.PosX = localClickPoint.X;
               newPolygon.PosY = localClickPoint.Y;

               _selectedCompoundObject.ModelObject.StaticPolygons.Add(newPolygon);

               StaticPolygonViewModel newPolygonVm = new StaticPolygonViewModel(this, _selectedCompoundObject, newPolygon);
               _selectedCompoundObject.Shapes.Add(newPolygonVm);

               _selectedShapes.Add(newPolygonVm);
               newPolygonVm.IsSelected = true;

               // InsertPoint, perhaps wrongly, offset the point with PosX and PosY. 
               // Therefore the origin point is used to define the first point.
               // This should probably be changed so we insert point at 0,0 (local coordinate)
               // here.
               DragablePointViewModel newPoint = newPolygonVm.InsertPoint(localClickPoint, null);

               foreach (DragablePointViewModel selpoint in _selectedPoints)
               {
                  selpoint.IsSelected = false;
               }
               _selectedPoints.Clear();

               _selectedPoints.Add(newPoint);
               newPoint.IsSelected = true;

               _LeftClickState = LeftClickState.addPoint;

            }
            else if (_LeftClickState == LeftClickState.addPoint)
            {
               // When adding points to new polygon we require that the
               // shape is the only one selected
               if ((_selectedShapes.Count == 1) && (_selectedShapes[0] is EditablePolygonViewModel))
               {
                  EditablePolygonViewModel newPolygon = (EditablePolygonViewModel)_selectedShapes[0];
                  Point parentOrigo = new Point(newPolygon.PosX, newPolygon.PosY);
                  Point localClickPoint = new Point();
                  localClickPoint = (Point)(clickPoint - parentOrigo);

                  DragablePointViewModel newPoint = newPolygon.AddPoint(localClickPoint);
                  foreach (DragablePointViewModel selpoint in _selectedPoints)
                  {
                     selpoint.IsSelected = false;
                  }
                  _selectedPoints.Clear();

                  _selectedPoints.Add(newPoint);
                  newPoint.IsSelected = true;
               }

            }

            return true;
         }
         else if (button == MouseButton.Right)
         {
            _LeftClickState = LeftClickState.none;
            return true;
         }


         return false;
      }

      public bool KeyDownHandler(KeyEventArgs e)
      {
         if (e.Key == Key.Delete)
         {
            if (_selectedPoints.Count > 0)
            {
               foreach (DragablePointViewModel dp in _selectedPoints)
               {
                  EditablePolygonViewModel polyVm = dp.Parent;

                  // Is this the last point to be removed? If so, remove the shape
                  // first so there is no problem with updating something with zero
                  // points.
                  if (polyVm.PointVms.Count == 1)
                  {
                     // Polygon has no more points, delete the polygon Shape

                     polyVm.Parent.ModelObject.RemoveShape(polyVm.PolygonObject);
                     polyVm.Parent.Shapes.Remove(polyVm);
                  }

                  // Before we remove the point
                  polyVm.RemovePoint(dp);

               }
               _selectedPoints.Clear();
            }
         }

         return false;
      }

      #endregion
   }
}

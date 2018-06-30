using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
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
      spriteBox,
      spritePolygon,
      staticBox,
      staticCircle,
      staticPolygon,
      staticBoxedSpritePolygon,
      dynamicBox,
      dynamicCircle,
      dynamicPolygon,
      dynamicBoxedSpritePolygon,
      addPoint
   }

   enum MouseEventObjectType
   {
      none,                         // object is null
      shape,                        // object is the shape
      dragableBorder,               // object is the point (before or after?) the line
      dragablePoint,                // object is the point
      compoundObjectBoundaryBox     // object is the CompoundObject
   }

   class MainViewModel : MicroMvvm.ViewModelBase
   {

      #region Declarations

      private CompoundObjectRef _myCpRef = new CompoundObjectRef();
      private ObjectRefStateProperties _myStateProp = new ObjectRefStateProperties();
      private CompoundObject _myCP;
      private CompoundObjectViewModel _myCpVm;

      private CompoundObjectViewModel _selectedCompoundObject = null;
      private ObservableCollection<LfShapeViewModel> _selectedShapes = new ObservableCollection<LfShapeViewModel>();
      private ObservableCollection<LfDragablePointViewModel> _selectedPoints = new ObservableCollection<LfDragablePointViewModel>();

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

      public CompoundObjectViewModel SelectedCompoundObject
      {
         get { return _selectedCompoundObject; }
         set { _selectedCompoundObject = value; }
      }

      public ObservableCollection<LfShapeViewModel> SelectedShapes
      {
         get { return _selectedShapes; }
         set { _selectedShapes = value; }
      }

      public ObservableCollection<LfDragablePointViewModel> SelectedPoints
      {
         get { return _selectedPoints; }
         set { _selectedPoints = value; }
      }

      #endregion

      #region Commands

      void CreateTrianglesExecute(Object parameter)
      {
         MyCpVm.GenerateTriangles();
      }

      bool CanCreateTrianglesExecute(Object parameter)
      {
         return true;
      }

      public ICommand CreateTriangles
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => CreateTrianglesExecute(parameter), parameter => CanCreateTrianglesExecute(parameter));
         }
      }

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
         // Generate Triangles before saving
         MyCpVm.GenerateTriangles();

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
         foreach (LfShapeViewModel shape in _selectedShapes)
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

      void NewDynamicPolygonExecute(Object parameter)
      {
         // Deselect all other shapes when generating a new polygon
         foreach (LfShapeViewModel shape in _selectedShapes)
         {
            shape.IsSelected = false;
         }

         _selectedShapes.Clear();

         _LeftClickState = LeftClickState.dynamicPolygon;
      }

      bool CanNewDynamicPolygonExecute(Object parameter)
      {
         return (_selectedCompoundObject != null);
      }

      public ICommand NewDynamicPolygon
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => NewDynamicPolygonExecute(parameter), parameter => CanNewDynamicPolygonExecute(parameter));
         }
      }


      void NewShapeExecute(Object parameter)
      {
         // Deselect all other shapes when generating a new polygon
         foreach (LfShapeViewModel shape in _selectedShapes)
         {
            shape.IsSelected = false;
         }

         _selectedShapes.Clear();

         string param = parameter as string;

         if (!Enum.TryParse<LeftClickState>(param,  out _LeftClickState))
         {
            _LeftClickState = LeftClickState.none;
         }
      }

      bool CanNewShapeExecute(Object parameter)
      {
         return ((_selectedCompoundObject != null) && (parameter is string));
      }

      public ICommand NewShape
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => NewShapeExecute(parameter), parameter => CanNewShapeExecute(parameter));
         }
      }



      //<MenuItem Header = "Sprite Box" Command="{Binding NewSpriteBox}" />
      //<MenuItem Header = "Sprite Polygon" Command="{Binding NewSpritePolygon}" />
      //<MenuItem Header = "Static Circle" Command="{Binding NewStaticCircle}" />
      //<MenuItem Header = "Dynamic Circle" Command="{Binding NewDynamicCircle}" />
      //<MenuItem Header = "Static Box" Command="{Binding NewStaticBox}" />
      //<MenuItem Header = "Dynamic Box" Command="{Binding NewDynamicBox}" />
      //<MenuItem Header = "Static Polygon" Command="{Binding NewStaticPolygon}" />
      //<MenuItem Header = "Dynamic Polygon" Command="{Binding NewDynamicPolygon}" />
      //<MenuItem Header = "Static Boxed Sprite Polygon" Command="{Binding NewStaticBoxedSpritePolygon}" />
      //<MenuItem Header = "Dynamic Boxed Sprite Polygon" Command="{Binding NewDynamicBoxedSpritePolygon}" />



      #endregion



      #region Public Methods

      // NOTE: FrameworkElement is used to communicate from the View. It can 
      // be used to obtain the ViewModel object from it's datacontext.
      // In some cases we need the type of FrameworkElement (to see if a 
      // Line or Rectangle).

      // All mouse handling function return true if the the data was used for some action
      // (if original event was handled)

      
      public bool MouseDown(
         MouseEventObjectType objectType, 
         object sender, 
         MouseButton button, 
         Point clickPoint, 
         int clickCount, 
         bool shift, 
         bool ctrl, 
         bool alt)
      {
         if (button == MouseButton.Left)
         {

            if (clickCount > 1)
            {
               if (_LeftClickState == LeftClickState.addPoint)
               {
                  TerminatePointAdding();
                  return true;
               }
            }

            // If we are creating shapes or adding points to a polygon
            // we want to be able to click anywhere (even in an existing shape).
            // Therefore, we return false for any of those cases and hopefully
            // the mouse event will bubble up to the background canvas
            // and thus be handeled in that mouse handler.
            if (_LeftClickState != LeftClickState.none)
            {
               return false;
            }

            // Decode target ViewModel and view object that was clicked
            switch (objectType)
            {
               case MouseEventObjectType.shape:
                  // So far we do not do anything here
                  //LfShapeViewModel shvm = (LfShapeViewModel)sender;
                  return false;

               case MouseEventObjectType.dragableBorder:
                  break;

               case MouseEventObjectType.dragablePoint:

                  LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

                  if (dpvm.IsSelected)
                  {
                     // Could be the beginning of dragging

                  }
                  else
                  {
                     if (!ctrl)
                     {
                        foreach (LfDragablePointViewModel selpoint in _selectedPoints)
                        {
                           selpoint.IsSelected = false;
                        }
                        _selectedPoints.Clear();
                     }

                     _selectedPoints.Add(dpvm);
                     dpvm.IsSelected = true;
                  }

                  return true;
              
               case MouseEventObjectType.compoundObjectBoundaryBox:
                  // So far we do not do anything here
                  // CompoundObjectViewModel covm = (CompoundObjectViewModel)sender;
                  return false;

               case MouseEventObjectType.none:
                  BackgroundMouseDown(clickPoint, button, clickCount, shift, ctrl, alt);
                  return false;
            }


            //            if (target.DataContext is CompoundObjectViewModel)
            //            {
            //               // Mouse down on rectangle around CompoundObject
            //               CompoundObjectViewModel covm = (CompoundObjectViewModel)target.DataContext;

            //               if (_LeftClickState == LeftClickState.addPoint)
            //               {
            //                  TerminatePointAdding();
            //               }

            //               //Debug.WriteLine("Clicked rectangle around CompoundObject");

            ////               return true;
            //            }
            //            else if ((target is Rectangle) && (target.DataContext is LfDragablePointViewModel))
            //            {
            //               // Mouse down on rectangle of DragablePoint

            //               if (_LeftClickState == LeftClickState.addPoint)
            //               {
            //                  TerminatePointAdding();
            //               }

            //               LfDragablePointViewModel dpvm = (LfDragablePointViewModel)target.DataContext;

            //               if (dpvm.IsSelected)
            //               {
            //                  // Could be the beginning of dragging

            //               }
            //               else
            //               {
            //                  if (!ctrl)
            //                  {
            //                     foreach (LfDragablePointViewModel selpoint in _selectedPoints)
            //                     {
            //                        selpoint.IsSelected = false;
            //                     }
            //                     _selectedPoints.Clear();
            //                  }

            //                  _selectedPoints.Add(dpvm);
            //                  dpvm.IsSelected = true;
            //               }

            //               //Debug.WriteLine("Clicked rectangle of DragablePoint");

            //               return true;
            //            }
            //            else if ((target is Line) && (target.DataContext is LfDragablePointViewModel))
            //            {
            //               // Mouse down on Line between DragablePoints 

            //               if (_LeftClickState == LeftClickState.addPoint)
            //               {
            //                  TerminatePointAdding();
            //               }

            //               LfDragablePointViewModel dpvm = (LfDragablePointViewModel)target.DataContext;

            //               //Debug.WriteLine("Clicked line between DragablePoint");
            //            }
            //            else if (target.DataContext is LfShapeViewModel)
            //            {
            //               // Mouse down on Shape

            //               if (_LeftClickState == LeftClickState.addPoint)
            //               {
            //                  TerminatePointAdding();
            //               }

            //               LfShapeViewModel shvm = (LfShapeViewModel)target.DataContext;

            //               //Debug.WriteLine("Clicked on Shape");

            //            }
            //            else if ((target is Rectangle) && (target.DataContext is IPositionInterface))
            //            {
            //               // Mouse down on rectangle around Shape

            //               if (_LeftClickState == LeftClickState.addPoint)
            //               {
            //                  TerminatePointAdding();
            //               }

            //               IPositionInterface posvm = (IPositionInterface)target.DataContext;

            //               //Debug.WriteLine("Clicked rectangle around something that can be dragged");

            //               return true;
            //            }
         }

         return false;
      }

      public bool MouseMove(
         MouseEventObjectType objectType,
         object sender,
         Vector dragVector,
         bool shift,
         bool ctrl,
         bool alt)
      {
         // If we are creating shapes or adding points to a polygon
         // we want to be able to click anywhere (even in an existing shape).
         // Therefore, we return false for any of those cases and hopefully
         // the mouse event will bubble up to the background canvas
         // and thus be handeled in that mouse handler.
         if (_LeftClickState != LeftClickState.none)
         {
            return false;
         }

         // Decode target ViewModel and view oobject that was clicked
         switch (objectType)
         {
            case MouseEventObjectType.shape:
               return false;

            case MouseEventObjectType.dragableBorder:

               IPositionInterface posvm = (IPositionInterface)sender;

               foreach (IPositionInterface shape in _selectedShapes)
               {
                  shape.PosX += dragVector.X;
                  shape.PosY += dragVector.Y;
               }

               return true;

            case MouseEventObjectType.dragablePoint:

               // Mouse move on rectangle of DragablePoint
               LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

               // Before moving all vertices, we rotate the vector to match the shape rotation.
               Point vectPoint = new Point(dragVector.X, dragVector.Y);
               Point rotPoint = dpvm.Parent.LocalPointFromRotated(vectPoint);
               Vector rotatedDragVector = new Vector(rotPoint.X, rotPoint.Y);
               
               foreach (LfDragablePointViewModel point in _selectedPoints)
               {
                  point.PosX += rotatedDragVector.X;
                  point.PosY += rotatedDragVector.Y;
               }

               return true;

            case MouseEventObjectType.compoundObjectBoundaryBox:

               CompoundObjectViewModel covm = (CompoundObjectViewModel)sender;

               covm.PosX += dragVector.X;
               covm.PosY += dragVector.Y;

               return true;

            case MouseEventObjectType.none:
               break;
         }


//         if (target.DataContext is CompoundObjectViewModel)
//         {
//            // Mouse move on rectangle around CompoundObject
//            CompoundObjectViewModel covm = (CompoundObjectViewModel)target.DataContext;

//            covm.PosX += dragVector.X;
//            covm.PosY += dragVector.Y;

//            //Debug.WriteLine("Movbed rectangle around CompoundObject");
//            return true;
//         }
//         else if ((target is Rectangle) && (target.DataContext is LfDragablePointViewModel))
//         {
//            // Mouse move on rectangle of DragablePoint
//            LfDragablePointViewModel dpvm = (LfDragablePointViewModel)target.DataContext;

//            // Before moving all vertices, we rotate the vector to match the shape rotation.
//            Point vectPoint = new Point(dragVector.X, dragVector.Y);
//            Point rotPoint = dpvm.Parent.LocalPointFromRotated(vectPoint);
//            Vector rotatedDragVector = new Vector(rotPoint.X, rotPoint.Y);


//            foreach (LfDragablePointViewModel point in _selectedPoints)
//            {
//               point.PosX += rotatedDragVector.X;
//               point.PosY += rotatedDragVector.Y;
//            }

//            //Debug.WriteLine("Move rectangle of DragablePoint");

//            return true;
//         }
//         else if (((target is Line) || (target is Ellipse)) && (target.DataContext is IPositionInterface))
//         {
//            // Mouse move on Line between DragablePoints 
////            LfDragablePointViewModel dpvm = (LfDragablePointViewModel)target.DataContext;

//            IPositionInterface posvm = (IPositionInterface)target.DataContext;

//            foreach (IPositionInterface shape in _selectedShapes)
//            {
//               shape.PosX += dragVector.X;
//               shape.PosY += dragVector.Y;
//            }

//            //Debug.WriteLine("Clicked line between DragablePoint");

//            return true;
//         }
//         else if ((target is Rectangle) && (target.DataContext is IPositionInterface))
//         {
//            // Mouse move on rectangle around Shape
//            IPositionInterface posvm = (IPositionInterface)target.DataContext;

//            foreach (IPositionInterface shape in _selectedShapes)
//            {
//               shape.PosX += dragVector.X;
//               shape.PosY += dragVector.Y;
//            }


//            //Debug.WriteLine("Moved rectangle around something that can be dragged");

//            return true;
//         }

         return false;
      }

      // clickPoint will be in coordinates of the parent CompoundObject
      public bool MouseUp(
         MouseEventObjectType objectType,
         object sender,
         MouseButton button,
         Point clickPoint,
         int clickCount,
         bool shift,
         bool ctrl,
         bool alt)
      {
         if (button == MouseButton.Left)
         {
            // If we are creating shapes or adding points to a polygon
            // we want to be able to click anywhere (even in an existing shape).
            // Therefore, we return false for any of those cases and hopefully
            // the mouse event will bubble up to the background canvas
            // and thus be handeled in that mouse handler.
            if (_LeftClickState != LeftClickState.none)
            {
               return false;
            }

            // Decode target ViewModel and view oobject that was clicked
            switch (objectType)
            {
               case MouseEventObjectType.shape:

                  // Mouse up on Shape
                  LfShapeViewModel shvm = (LfShapeViewModel)sender;

                  if (shvm.Parent.IsSelected)
                  {
                     if (!ctrl)
                     {
                        foreach (LfShapeViewModel selshape in _selectedShapes)
                        {
                           selshape.IsSelected = false;
                        }
                        _selectedShapes.Clear();
                     }

                     _selectedShapes.Add(shvm);
                     shvm.IsSelected = true;
                  }
                  else
                  {
                     if (_selectedCompoundObject != null)
                     {
                        _selectedCompoundObject.DeselectAllChildren();
                        _selectedShapes.Clear();
                        _selectedCompoundObject.IsSelected = false;
                     }
                     _selectedCompoundObject = shvm.Parent;
                  }

                  if (!shvm.IsSelected)
                  {
                     shvm.Parent.IsSelected = true;
                     //_selectedShapes.Add(shvm);
                     _selectedCompoundObject = shvm.Parent;

                  }

                  return true;

               case MouseEventObjectType.dragableBorder:

                  LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

                  if (ctrl)
                  {
                     // What is the click-point in this case?
                     // It is said to be the closesed parenting canvas which
                     // should be in CompoundObject coordinates. Lets try to convert
                     // this into the rotated shape coordinates. 

                     Point coPoint = clickPoint;

                     Point unrotatedShapePoint = dpvm.Parent.Parent.CoPointInShape(clickPoint, dpvm.Parent);

                     Point rotShapePoint = dpvm.Parent.LocalPointFromRotated(unrotatedShapePoint);

                     LfDragablePointViewModel newPoint = dpvm.Parent.InsertPoint(rotShapePoint, dpvm);
                     foreach (LfDragablePointViewModel selpoint in _selectedPoints)
                     {
                        selpoint.IsSelected = false;
                     }
                     _selectedPoints.Clear();

                     _selectedPoints.Add(newPoint);
                     newPoint.IsSelected = true;
                  }

                  return true;

               case MouseEventObjectType.dragablePoint:
                  // So far nothing is done here
                  // LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

                  return true;

               case MouseEventObjectType.compoundObjectBoundaryBox:
                  // So far nothing is done here
                  // CompoundObjectViewModel covm = (CompoundObjectViewModel)sender;

                  return true;

               case MouseEventObjectType.none:
                  return BackgroundMouseUp(clickPoint, button, shift, ctrl, alt);
            }



         //   if (target.DataContext is CompoundObjectViewModel)
         //   {
         //      // Mouse up on rectangle around CompoundObject
         //      CompoundObjectViewModel covm = (CompoundObjectViewModel)target.DataContext;

         //      //Debug.WriteLine("Clicked rectangle around CompoundObject");

         //      return true;
         //   }
         //   else if ((target is Rectangle) && (target.DataContext is LfDragablePointViewModel))
         //   {
         //      // Mouse up on rectangle of DragablePoint
         //      LfDragablePointViewModel dpvm = (LfDragablePointViewModel)target.DataContext;

         //      //Debug.WriteLine("Clicked rectangle of DragablePoint");

         //      return true;
         //   }
         //   else if ((target is Line) && (target.DataContext is LfDragablePointViewModel))
         //   {
         //      // Mouse up on Line between DragablePoints 
         //      LfDragablePointViewModel dpvm = (LfDragablePointViewModel)target.DataContext;

         //      if (ctrl)
         //      {
         //         // What is the click-point in this case?
         //         // It is said to be the closesed parenting canvas which
         //         // should be in CompoundObject coordinates. Lets try to convert
         //         // this into the rotated shape coordinates. 

         //         Point coPoint = clickPoint;

         //         Point unrotatedShapePoint = dpvm.Parent.Parent.CoPointInShape(clickPoint, dpvm.Parent);

         //         Point rotShapePoint = dpvm.Parent.LocalPointFromRotated(unrotatedShapePoint);

         //         LfDragablePointViewModel newPoint = dpvm.Parent.InsertPoint(rotShapePoint, dpvm);
         //         foreach (LfDragablePointViewModel selpoint in _selectedPoints)
         //         {
         //            selpoint.IsSelected = false;
         //         }
         //         _selectedPoints.Clear();

         //         _selectedPoints.Add(newPoint);
         //         newPoint.IsSelected = true;
         //      }

         //      //Debug.WriteLine("Clicked line between DragablePoint");

         //      return true;
         //   }
         //   else if (target.DataContext is LfShapeViewModel)
         //   {
         //      // Mouse up on Shape
         //      LfShapeViewModel shvm = (LfShapeViewModel)target.DataContext;

         //      //Debug.WriteLine("Mouse up on Shape");

         //      if (shvm.Parent.IsSelected)
         //      {
         //         if (!ctrl)
         //         {
         //            foreach (LfShapeViewModel selshape in _selectedShapes)
         //            {
         //               selshape.IsSelected = false;
         //            }
         //            _selectedShapes.Clear();
         //         }

         //         _selectedShapes.Add(shvm);
         //         shvm.IsSelected = true;
         //      }
         //      else
         //      {
         //         if (_selectedCompoundObject != null)
         //         {
         //            _selectedCompoundObject.IsSelected = false;
         //         }
         //         _selectedCompoundObject = shvm.Parent;
         //      }

         //      if (!shvm.IsSelected)
         //      {
         //         shvm.Parent.IsSelected = true;
         //         _selectedShapes.Add(shvm);
         //         _selectedCompoundObject = shvm.Parent;

         //      }

         //      return true;

         //   }
         //   else if ((target is Rectangle) && (target.DataContext is IPositionInterface))
         //   {
         //      // Mouse up on rectangle around Shape
         //      IPositionInterface posvm = (IPositionInterface)target.DataContext;

         //      //Debug.WriteLine("Clicked rectangle around something that can be dragged");
         //      return true;
         //   }
         }

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
               _selectedCompoundObject = null;
               _selectedShapes.Clear();
            }
            else if ((_LeftClickState == LeftClickState.staticBox) ||
               (_LeftClickState == LeftClickState.dynamicBox) ||
               (_LeftClickState == LeftClickState.dynamicCircle) ||
               (_LeftClickState == LeftClickState.staticCircle) ||
               (_LeftClickState == LeftClickState.spriteBox))
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(_selectedCompoundObject.PosX, _selectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               LfShape newShape = null;
               LfShapeViewModel newShapeVm = null;

               if (_LeftClickState == LeftClickState.staticBox)
               {
                  newShape = new LfStaticBox();
                  newShapeVm = new LfStaticBoxViewModel(this, _selectedCompoundObject, (LfStaticBox)newShape);
                  _selectedCompoundObject.ModelObject.StaticBoxes.Add((LfStaticBox)newShape);
               }
               else if (_LeftClickState == LeftClickState.dynamicBox)
               {
                  newShape = new LfDynamicBox();
                  newShapeVm = new LfDynamicBoxViewModel(this, _selectedCompoundObject, (LfDynamicBox)newShape);
                  _selectedCompoundObject.ModelObject.DynamicBoxes.Add((LfDynamicBox)newShape);
               }
               else if (_LeftClickState == LeftClickState.staticCircle)
               {
                  newShape = new LfStaticCircle();
                  newShapeVm = new LfStaticCircleViewModel(this, _selectedCompoundObject, (LfStaticCircle)newShape);
                  _selectedCompoundObject.ModelObject.StaticCircles.Add((LfStaticCircle)newShape);
               }
               else if (_LeftClickState == LeftClickState.dynamicCircle)
               {
                  newShape = new LfDynamicCircle();
                  newShapeVm = new LfDynamicCircleViewModel(this, _selectedCompoundObject, (LfDynamicCircle)newShape);
                  _selectedCompoundObject.ModelObject.DynamicCircles.Add((LfDynamicCircle)newShape);
               }
               else if (_LeftClickState == LeftClickState.spriteBox)
               {
                  newShape = new LfSpriteBox();
                  newShapeVm = new LfSpriteBoxViewModel(this, _selectedCompoundObject, (LfSpriteBox)newShape);
                  _selectedCompoundObject.ModelObject.SpriteBoxes.Add((LfSpriteBox)newShape);
               }

               if ((newShape != null) && (newShapeVm != null))
               {
                  newShape.PosX = localClickPoint.X;
                  newShape.PosY = localClickPoint.Y;

                  _selectedCompoundObject.Shapes.Add(newShapeVm);

                  _selectedShapes.Add(newShapeVm);
                  newShapeVm.IsSelected = true;

                  foreach (LfDragablePointViewModel selpoint in _selectedPoints)
                  {
                     selpoint.IsSelected = false;
                  }
                  _selectedPoints.Clear();
               }

               _LeftClickState = LeftClickState.none;

            }
            else if ((_LeftClickState == LeftClickState.staticPolygon) || 
               (_LeftClickState == LeftClickState.dynamicPolygon) ||
               (_LeftClickState == LeftClickState.spritePolygon))
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(_selectedCompoundObject.PosX, _selectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               LfScalableTexturePolygon newPolygon;
               LfScalableTexturePolygonViewModel newPolygonVm;

               if (_LeftClickState == LeftClickState.staticPolygon)
               {
                  newPolygon = new LfStaticPolygon();
                  newPolygonVm = new LfStaticPolygonViewModel(this, _selectedCompoundObject, (LfStaticPolygon)newPolygon);
                  _selectedCompoundObject.ModelObject.StaticPolygons.Add((LfStaticPolygon)newPolygon);
               }
               else if(_LeftClickState == LeftClickState.dynamicPolygon)
               {
                  newPolygon = new LfDynamicPolygon();
                  newPolygonVm = new LfDynamicPolygonViewModel(this, _selectedCompoundObject, (LfDynamicPolygon)newPolygon);
                  _selectedCompoundObject.ModelObject.DynamicPolygons.Add((LfDynamicPolygon)newPolygon);
               }
               else
               {
                  newPolygon = new LfSpritePolygon();
                  newPolygonVm = new LfSpritePolygonViewModel(this, _selectedCompoundObject, (LfSpritePolygon)newPolygon);
                  _selectedCompoundObject.ModelObject.SpritePolygons.Add((LfSpritePolygon)newPolygon);
               }

               newPolygon.PosX = localClickPoint.X;
               newPolygon.PosY = localClickPoint.Y;

               _selectedCompoundObject.Shapes.Add(newPolygonVm);

               _selectedShapes.Add(newPolygonVm);
               newPolygonVm.IsSelected = true;

               LfDragablePointViewModel newPoint = newPolygonVm.InsertPoint(new Point(0,0), null);

               foreach (LfDragablePointViewModel selpoint in _selectedPoints)
               {
                  selpoint.IsSelected = false;
               }
               _selectedPoints.Clear();

               _selectedPoints.Add(newPoint);
               newPoint.IsSelected = true;

               _LeftClickState = LeftClickState.addPoint;

            }
            else if ((_LeftClickState == LeftClickState.staticBoxedSpritePolygon) || 
               (_LeftClickState == LeftClickState.dynamicBoxedSpritePolygon))
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(_selectedCompoundObject.PosX, _selectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               LfStaticBoxedSpritePolygon newPolygon = null;
               LfStaticBoxedSpritePolygonViewModel newPolygonVm = null;

               if (_LeftClickState == LeftClickState.staticBoxedSpritePolygon)
               {
                  newPolygon = new LfStaticBoxedSpritePolygon();
                  newPolygonVm = new LfStaticBoxedSpritePolygonViewModel(this, _selectedCompoundObject, (LfStaticBoxedSpritePolygon)newPolygon);
                  _selectedCompoundObject.ModelObject.StaticBoxedSpritePolygons.Add((LfStaticBoxedSpritePolygon)newPolygon);
               }
               else
               {
                  newPolygon = new LfDynamicBoxedSpritePolygon();
                  newPolygonVm = new LfDynamicBoxedSpritePolygonViewModel(this, _selectedCompoundObject, (LfDynamicBoxedSpritePolygon)newPolygon);
                  _selectedCompoundObject.ModelObject.DynamicBoxedSpritePolygons.Add((LfDynamicBoxedSpritePolygon)newPolygon);
               }
               
               newPolygon.PosX = localClickPoint.X;
               newPolygon.PosY = localClickPoint.Y;
               
               _selectedCompoundObject.Shapes.Add(newPolygonVm);

               _selectedShapes.Add(newPolygonVm);
               newPolygonVm.IsSelected = true;

               LfDragablePointViewModel newPoint = newPolygonVm.InsertPoint(new Point(0, 0), null);

               foreach (LfDragablePointViewModel selpoint in _selectedPoints)
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
               if ((_selectedShapes.Count == 1) && (_selectedShapes[0] is LfPolygonViewModel))
               {
                  LfPolygonViewModel newPolygon = (LfPolygonViewModel)_selectedShapes[0];
                  Point parentObjectOrigo = new Point(newPolygon.Parent.PosX, newPolygon.Parent.PosY);
                  Point shapeOrigo = new Point(newPolygon.PosX, newPolygon.PosY);
                  shapeOrigo.Offset(parentObjectOrigo.X, parentObjectOrigo.Y);
                  Point localClickPoint = new Point();
                  localClickPoint = (Point)(clickPoint - shapeOrigo);

                  LfDragablePointViewModel newPoint = newPolygon.AddPoint(localClickPoint);
                  foreach (LfDragablePointViewModel selpoint in _selectedPoints)
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

         return false;
      }

      public bool BackgroundMouseDown(Point clickPoint, MouseButton button, int clickCount, bool shift, bool ctrl, bool alt)
      {
         if (clickCount > 1)
         {

            if (button == MouseButton.Left)
            {
               if (_LeftClickState == LeftClickState.addPoint)
               {
                  _LeftClickState = LeftClickState.none;
                  return true;
               }
            }
         }

         return false;
      }

      public bool KeyDownHandler(KeyEventArgs e)
      {
         if (e.Key == Key.Delete)
         {
            if (_selectedPoints.Count > 0)
            {
               foreach (LfDragablePointViewModel dp in _selectedPoints)
               {
                  LfPolygonViewModel polyVm = dp.Parent;

                  // Is this the last point to be removed? If so, remove the shape
                  // first so there is no problem with updating something with zero
                  // points.
                  if (polyVm.PointVms.Count == 1)
                  {
                     // Polygon has no more points, delete the polygon Shape

                     polyVm.Parent.ModelObject.RemoveShape(polyVm.ModelObject);
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

      public void RotateSelectedShape(int delta)
      {
         foreach (LfShapeViewModel svm in _selectedShapes)
         {
            svm.RotateShape(delta);
         }
      }


      
      #endregion

      #region private Methods

      private void TerminatePointAdding()
      {
         SystemSounds.Beep.Play();

         _LeftClickState = LeftClickState.none;

         // Lets do a brute force invalidate of all points of the polygon
         if ((_selectedShapes.Count() == 1) && (_selectedShapes[0] is LfPolygonViewModel))
         {
            LfPolygonViewModel pvm = (LfPolygonViewModel)_selectedShapes[0];

            foreach (LfDragablePointViewModel dpvm in pvm.PointVms)
            {
               dpvm.OnPropertyChanged("");
            }
         }

      }

      #endregion
   }
}

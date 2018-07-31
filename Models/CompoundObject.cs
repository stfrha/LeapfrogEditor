using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   [Serializable]
   [XmlRoot("compoundObject")]
   public class CompoundObject
   {
      #region Declarations

      private string _type;

      private ObservableCollection<LfSpriteBox> _spriteBoxes = new ObservableCollection<LfSpriteBox>();
      private ObservableCollection<LfSpritePolygon> _spritePolygons = new ObservableCollection<LfSpritePolygon>();
      private ObservableCollection<LfStaticCircle> _staticCircles = new ObservableCollection<LfStaticCircle>();
      private ObservableCollection<LfStaticBox> _staticBoxes = new ObservableCollection<LfStaticBox>();
      private ObservableCollection<LfStaticPolygon> _staticPolygons = new ObservableCollection<LfStaticPolygon>();
      private ObservableCollection<LfDynamicBox> _dynamicBoxes = new ObservableCollection<LfDynamicBox>();
      private ObservableCollection<LfDynamicCircle> _dynamicCircles = new ObservableCollection<LfDynamicCircle>();
      private ObservableCollection<LfDynamicPolygon> _dynamicPolygons = new ObservableCollection<LfDynamicPolygon>();
      private ObservableCollection<LfStaticBoxedSpritePolygon> _staticBoxedSpritePolygons = new ObservableCollection<LfStaticBoxedSpritePolygon>();
      private ObservableCollection<LfDynamicBoxedSpritePolygon> _dynamicBoxedSpritePolygons = new ObservableCollection<LfDynamicBoxedSpritePolygon>();

      private ObservableCollection<WeldJoint> _weldJoints = new ObservableCollection<WeldJoint>();
      private ObservableCollection<RevoluteJoint> _revoluteJoints = new ObservableCollection<RevoluteJoint>();
      private ObservableCollection<PrismaticJoint> _prismaticJoints = new ObservableCollection<PrismaticJoint>();

      private ObservableCollection<PlanetActorRef> _planetActors = new ObservableCollection<PlanetActorRef>();
      private ObservableCollection<ClippedWindowRef> _clippedWindows = new ObservableCollection<ClippedWindowRef>();
      private ObservableCollection<AsteroidFieldRef> _asteroidFields = new ObservableCollection<AsteroidFieldRef>();

      private ObservableCollection<CompoundObjectRef> _childObjectRefs = new ObservableCollection<CompoundObjectRef>();

      #endregion

      #region Constructor

      public CompoundObject()
      {
      }

      #endregion

      #region Properties

      [XmlAttribute("type")]
      public string Type
      {
         get { return _type; }
         set { _type = value; }
      }

      [XmlElement("spriteBox")]
      public ObservableCollection<LfSpriteBox> SpriteBoxes
      {
         get { return _spriteBoxes; }
         set { _spriteBoxes = value; }
      }

      [XmlElement("spritePolygon")]
      public ObservableCollection<LfSpritePolygon> SpritePolygons
      {
         get { return _spritePolygons; }
         set { _spritePolygons = value; }
      }

      [XmlElement("staticBox")]
      public ObservableCollection<LfStaticBox> StaticBoxes
      {
         get { return _staticBoxes; }
         set { _staticBoxes = value; }
      }

      [XmlElement("staticCircle")]
      public ObservableCollection<LfStaticCircle> StaticCircles
      {
         get { return _staticCircles; }
         set { _staticCircles = value; }
      }

      [XmlElement("staticPolygon")]
      public ObservableCollection<LfStaticPolygon> StaticPolygons
      {
         get { return _staticPolygons; }
         set { _staticPolygons = value; }
      }

      [XmlElement("dynamicBox")]
      public ObservableCollection<LfDynamicBox> DynamicBoxes
      {
         get { return _dynamicBoxes; }
         set { _dynamicBoxes = value; }
      }

      [XmlElement("dynamicCircle")]
      public ObservableCollection<LfDynamicCircle> DynamicCircles
      {
         get { return _dynamicCircles; }
         set { _dynamicCircles = value; }
      }

      [XmlElement("dynamicPolygon")]
      public ObservableCollection<LfDynamicPolygon> DynamicPolygons
      {
         get { return _dynamicPolygons; }
         set { _dynamicPolygons = value; }
      }

      [XmlElement("staticBoxedSpritePolygonBody")]
      public ObservableCollection<LfStaticBoxedSpritePolygon> StaticBoxedSpritePolygons
      {
         get { return _staticBoxedSpritePolygons; }
         set { _staticBoxedSpritePolygons = value; }
      }

      [XmlElement("dynamicBoxedSpritePolygonBody")]
      public ObservableCollection<LfDynamicBoxedSpritePolygon> DynamicBoxedSpritePolygons
      {
         get { return _dynamicBoxedSpritePolygons; }
         set { _dynamicBoxedSpritePolygons = value; }
      }

      [XmlElement("weldJoint")]
      public ObservableCollection<WeldJoint> WeldJoints
      {
         get { return _weldJoints; }
         set { _weldJoints = value; }
      }

      [XmlElement("revoluteJoint")]
      public ObservableCollection<RevoluteJoint> RevoluteJoints
      {
         get { return _revoluteJoints; }
         set { _revoluteJoints = value; }
      }

      [XmlElement("prismaticJoint")]
      public ObservableCollection<PrismaticJoint> PrismaticJoints
      {
         get { return _prismaticJoints; }
         set { _prismaticJoints = value; }
      }

      [XmlElement("planetActor")]
      public ObservableCollection<PlanetActorRef> PlanetActors
      {
         get { return _planetActors; }
         set { _planetActors = value; }
      }

      [XmlElement("asteroidField")]
      public ObservableCollection<AsteroidFieldRef> AsteroidFields
      {
         get { return _asteroidFields; }
         set { _asteroidFields = value; }
      }

      // At serialisation of a parent CompoundObject this collection
      // is read. Once read, the spaceSceneFile is used to create 
      // a child CompoundObject.
      [XmlElement("clippedWindow")]
      public ObservableCollection<ClippedWindowRef> ClippedWindows
      {
         get { return _clippedWindows; }
         set { _clippedWindows = value; }
      }

      // At serialisation of a parent CompoundObject this collection
      // is read. Once read, it is used to create CompoundObjects as 
      // children.
      [XmlElement("ChildCompoundObjectRef")]
      public ObservableCollection<CompoundObjectRef> ChildObjectRefs
      {
         get { return _childObjectRefs; }
         set { _childObjectRefs = value; }
      }

      #endregion

      #region Public Methods

      public static CompoundObject ReadFromFile(string fileName)
      {
         string path = Path.GetDirectoryName(fileName);

         XmlSerializer ser = new XmlSerializer(typeof(CompoundObject));
         FileStream fs = new FileStream(fileName, FileMode.Open);
         XmlReader reader = XmlReader.Create(fs);
         CompoundObject co = (CompoundObject)ser.Deserialize(reader);

         // Iterate CompounfObjectRefs to load all child objects
         foreach (CompoundObjectRef cor in co.ChildObjectRefs)
         {
            // Iterate all state properties
            foreach (ObjectRefStateProperties sp in cor.StateProperties)
            {
               string newFile = System.IO.Path.Combine(path, sp.File);

               CompoundObject childCo = CompoundObject.ReadFromFile(newFile);

               sp.CompObj = childCo;
            }
         }

         // Iterate ClippedWindows to load all each child objects
         foreach (ClippedWindowRef cwr in co.ClippedWindows)
         {
            // Iterate all state properties
            foreach (TStateProperties<ClippedWindowProperties> sp in cwr.StateProperties)
            {
               string newFile = System.IO.Path.Combine(path, sp.Properties.SpaceSceneFile);

               CompoundObject childCo = CompoundObject.ReadFromFile(newFile);

               sp.Properties.CompObj = childCo;
            }
         }

         fs.Close();

         return co;
      }

      public void WriteToFile(string fileName)
      {
         XmlWriterSettings settings = new XmlWriterSettings();
         settings.Indent = true;
         settings.NewLineOnAttributes = true;

         string path = Path.GetDirectoryName(fileName);

         XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
         ns.Add("", "");

         XmlSerializer ser = new XmlSerializer(typeof(CompoundObject));
         FileStream fs = new FileStream(fileName, FileMode.Create);
         XmlWriter writer = XmlWriter.Create(fs, settings);
         ser.Serialize(writer, this, ns);

         // Iterate ChildObjectRefs to save all child objects
         foreach (CompoundObjectRef co in ChildObjectRefs)
         {
            foreach (ObjectRefStateProperties sp in co.StateProperties)
            {
               string newFile = System.IO.Path.Combine(path, sp.File);

               sp.CompObj.WriteToFile(newFile);

            }
         }

         // Iterate ClippedWindowProperties to save all space scenes as child objects
         foreach (ClippedWindowRef cw in ClippedWindows)
         {
            foreach (TStateProperties<ClippedWindowProperties> sp in cw.StateProperties)
            {
               string newFile = System.IO.Path.Combine(path, sp.Properties.SpaceSceneFile);

               sp.Properties.CompObj.WriteToFile(newFile);
            }
         }

         fs.Close();
      }

      public void RemoveShape(object shape)
      {
         if (shape is LfSpriteBox)
         {
            SpriteBoxes.Remove((LfSpriteBox)shape);
         }
         else if (shape is LfSpritePolygon)
         {
            SpritePolygons.Remove((LfSpritePolygon)shape);
         }
         else if (shape is LfStaticBox)
         {
            StaticBoxes.Remove((LfStaticBox)shape);
         }
         else if (shape is LfStaticCircle)
         {
            StaticCircles.Remove((LfStaticCircle)shape);
         }
         else if (shape is LfStaticPolygon)
         {
            StaticPolygons.Remove((LfStaticPolygon)shape);
         }
         else if (shape is LfDynamicBox)
         {
            DynamicBoxes.Remove((LfDynamicBox)shape);
         }
         else if (shape is LfDynamicCircle)
         {
            DynamicCircles.Remove((LfDynamicCircle)shape);
         }
         else if (shape is LfDynamicPolygon)
         {
            DynamicPolygons.Remove((LfDynamicPolygon)shape);
         }
         else if (shape is LfStaticBoxedSpritePolygon)
         {
            StaticBoxedSpritePolygons.Remove((LfStaticBoxedSpritePolygon)shape);
         }
         else if (shape is LfDynamicBoxedSpritePolygon)
         {
            DynamicBoxedSpritePolygons.Remove((LfDynamicBoxedSpritePolygon)shape);
         }
      }

      public void RemoveJoint(object shape)
      {
         if (shape is WeldJoint)
         {
            WeldJoints.Remove((WeldJoint)shape);
         }
         else if (shape is RevoluteJoint)
         {
            RevoluteJoints.Remove((RevoluteJoint)shape);
         }
         else if (shape is PrismaticJoint)
         {
            PrismaticJoints.Remove((PrismaticJoint)shape);
         }
      }

      #endregion
   }
}

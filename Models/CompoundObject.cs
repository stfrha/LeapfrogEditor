using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
         string relPath = @"..\..\..\leapfrog\data\" + fileName;

         string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relPath);

         XmlSerializer ser = new XmlSerializer(typeof(CompoundObject));
         FileStream fs = new FileStream(path, FileMode.Open);
         XmlReader reader = XmlReader.Create(fs);
         CompoundObject co = (CompoundObject)ser.Deserialize(reader);

         // Iterate CompounfObjectRefs to load all child objects
         foreach (CompoundObjectRef cor in co.ChildObjectRefs)
         {
            // Iterate all state properties
            foreach (ObjectRefStateProperties sp in cor.StateProperties)
            {
               CompoundObject childCo = CompoundObject.ReadFromFile(sp.File);

               sp.CompObj = childCo;
            }
         }

         return co;
      }

      public void WriteToFile()
      {
         //string relPath = @"..\..\..\leapfrog\data\" + File;

         //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relPath);

         //XmlSerializer ser = new XmlSerializer(typeof(CompoundObject));
         //FileStream fs = new FileStream(path, FileMode.Create);
         //XmlWriter writer = XmlWriter.Create(fs);
         //ser.Serialize(writer, this);

         //// Iterate COmpounfObjects to save all child objects
         //foreach (CompoundObject co in ChildObjects)
         //{
         //   co.WriteToFile();
         //}
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

      #endregion
   }
}

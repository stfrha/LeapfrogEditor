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

      private ObservableCollection<StaticBox> _staticBoxes = new ObservableCollection<StaticBox>();
      private ObservableCollection<DynamicBox> _dynamicBoxes = new ObservableCollection<DynamicBox>();
      private ObservableCollection<StaticPolygon> _staticPolygons = new ObservableCollection<StaticPolygon>();
      private ObservableCollection<DynamicPolygon> _dynamicPolygons = new ObservableCollection<DynamicPolygon>();
      private ObservableCollection<BoxedSpritePolygon> _boxedSpritePolygons = new ObservableCollection<BoxedSpritePolygon>();

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

      [XmlElement("staticBox")]
      public ObservableCollection<StaticBox> StaticBoxes
      {
         get { return _staticBoxes; }
         set { _staticBoxes = value; }
      }

      [XmlElement("dynamicBox")]
      public ObservableCollection<DynamicBox> DynamicBoxes
      {
         get { return _dynamicBoxes; }
         set { _dynamicBoxes = value; }
      }

      [XmlElement("staticPolygon")]
      public ObservableCollection<StaticPolygon> StaticPolygons
      {
         get { return _staticPolygons; }
         set { _staticPolygons = value; }
      }

      [XmlElement("dynamicPolygon")]
      public ObservableCollection<DynamicPolygon> DynamicPolygons
      {
         get { return _dynamicPolygons; }
         set { _dynamicPolygons = value; }
      }

      [XmlElement("boxedSpritePolygonBody")]
      public ObservableCollection<BoxedSpritePolygon> BoxedSpritePolygons
      {
         get { return _boxedSpritePolygons; }
         set { _boxedSpritePolygons = value; }
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
         if (shape is DynamicBox)
         {
            DynamicBoxes.Remove((DynamicBox)shape);
         }
         else if (shape is DynamicPolygon)
         {
            DynamicPolygons.Remove((DynamicPolygon)shape);
         }
         else if (shape is StaticBox)
         {
            StaticBoxes.Remove((StaticBox)shape);
         }
         else if (shape is StaticPolygon)
         {
            StaticPolygons.Remove((StaticPolygon)shape);
         }
         else if (shape is BoxedSpritePolygon)
         {
            BoxedSpritePolygons.Remove((BoxedSpritePolygon)shape);
         }
      }

      #endregion
   }
}

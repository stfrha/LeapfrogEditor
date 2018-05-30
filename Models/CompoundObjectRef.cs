using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   [Serializable]
   public class CompoundObjectRef
   {
      #region Declarations

      private string _name;
      private string _type;
      private string _file;
      private double _posY;
      private double _posX;

      #endregion

      #region Constructor

      public CompoundObjectRef()
      {

      }

      #endregion

      #region Properties

      [XmlAttribute("name")]
      public string Name
      {
         get { return _name; }
         set { _name = value; }
      }

      [XmlAttribute("type")]
      public string Type
      {
         get { return _type; }
         set { _type = value; }
      }

      [XmlAttribute("file")]
      public string File
      {
         get { return _file; }
         set { _file = value; }
      }

      [XmlAttribute("posX")]
      public double PosX
      {
         get { return _posX; }
         set { _posX = value; }
      }

      [XmlAttribute("posY")]
      public double PosY
      {
         get { return _posY; }
         set { _posY = value; }
      }

      #endregion
   }
}

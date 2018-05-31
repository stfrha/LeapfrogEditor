using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   [Serializable]
   public class ObjectRefStateProperties
   {
      #region Declarations

      private string _state = "default";
      private string _file = "undef_file.xml";
      private double _posX = 0;
      private double _posY = 0;
      private CompoundObject _compObj = null;

      #endregion

      #region Constructor

      public ObjectRefStateProperties()
      {

      }

      #endregion

      #region Properties

      [XmlAttribute("state")]
      public string State
      {
         get { return _state; }
         set { _state = value; }
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

      [XmlIgnore]
      public CompoundObject CompObj
      {
         get { return _compObj; }
         set { _compObj = value; }
      }

      #endregion
   }
}

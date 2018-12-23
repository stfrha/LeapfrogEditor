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
   public class ChildObject
   {
      #region Declarations

      private string _name;
      private string _type;
      private TStateProperties<ChildObjectStateProperties> _stateProperties = new TStateProperties<ChildObjectStateProperties>();

      #endregion

      #region Constructor

      public ChildObject()
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

      [XmlElement("stateProperties")]
      public TStateProperties<ChildObjectStateProperties> StateProperties
      {
         get { return _stateProperties; }
         set { _stateProperties = value; }
      }
         
      #endregion
   }
}

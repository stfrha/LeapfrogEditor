﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   [Serializable]
   public class LfDynamicBoxedSpritePolygon : LfStaticBoxedSpritePolygon
   {
      #region Declarations

      private double _density;

      #endregion

      #region Constructors

      public LfDynamicBoxedSpritePolygon()
      {
         _density = 1;
      }

      #endregion

      #region Properties

      [XmlAttribute("density")]
      public double Density
      {
         get { return _density; }
         set { _density = value; }
      }

      #endregion

   }
}
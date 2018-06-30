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
   public class PrismaticJoint : WeldJoint
   {
      #region Declarations

      private double _aAxisX;
      private double _aAxisY;
      private double _refAngle;
      private double _lowerLimit;
      private double _upperLimit;

      #endregion

      #region Constructors

      public PrismaticJoint() : base()
      {
         _aAxisX = 1;
         _aAxisY = 0;
         _refAngle = 0;
         _lowerLimit = 0;
         _upperLimit = 10;
      }

      #endregion

      #region Properties

      [XmlAttribute("objectALocalAxisX")]
      public double AAxisX
      {
         get { return _aAxisX; }
         set { _aAxisX = value; }
      }

      [XmlAttribute("objectALocalAxisY")]
      public double AAxisY
      {
         get { return _aAxisY; }
         set { _aAxisY = value; }
      }

      [XmlAttribute("refAngle")]
      public double RefAngle
      {
         get { return _refAngle; }
         set { _refAngle = value; }
      }

      [XmlAttribute("lowerLimit")]
      public double LowerLimit
      {
         get { return _lowerLimit; }
         set { _lowerLimit = value; }
      }

      [XmlAttribute("upperLimit")]
      public double UpperLimit
      {
         get { return _upperLimit; }
         set { _upperLimit = value; }
      }

      #endregion

   }
}

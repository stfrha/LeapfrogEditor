﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapfrogEditor
{
   interface IAnchorableInterface
   {
      double AnchorX
      {
         get;
         set;
      }

      double AnchorY
      {
         get;
         set;
      }

      double Width
      {
         get;
         set;
      }

      double Height
      {
         get;
         set;
      }

   }
}

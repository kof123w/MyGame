﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FixedMath;
using FixMath.NET;

namespace BEPUphysics.CollisionShapes
{
    public struct EntityShapeVolumeDescription
    {
        public FPMatrix3x3 VolumeDistribution;
        public Fix64 Volume;
    }
}

﻿using System.Collections.Generic;
using FixedMath;
using FixedMath.DataStructures;
using FixMath.NET;

namespace BEPUphysics.Paths
{
    /// <summary>
    /// Defines a 3D curve using hermite interpolation.
    /// </summary>
    public abstract class HermiteCurve3D : Curve<FPVector3>
    {
        /// <summary>
        /// Internal list of curve tangents.
        /// </summary>
        protected List<FPVector3> tangents = new List<FPVector3>();


        /// <summary>
        /// Gets the tangents used by the curve per control point.
        /// </summary>
        public ReadOnlyList<FPVector3> Tangents
        {
            get
            {
                return new ReadOnlyList<FPVector3>(tangents);
            }
        }

        /// <summary>
        /// Evaluates the curve section starting at the control point index using
        /// the weight value.
        /// </summary>
        /// <param name="controlPointIndex">Index of the starting control point of the subinterval.</param>
        /// <param name="weight">Location to evaluate on the subinterval from 0 to 1.</param>
        /// <param name="value">Value at the given location.</param>
        public override void Evaluate(int controlPointIndex, Fix64 weight, out FPVector3 value)
        {
            value = FPVector3.Hermite(
                ControlPoints[controlPointIndex].Value, tangents[controlPointIndex],
                ControlPoints[controlPointIndex + 1].Value, tangents[controlPointIndex + 1], weight);
        }

        /// <summary>
        /// Called when a control point is added.
        /// </summary>
        /// <param name="curveControlPoint">New control point.</param>
        /// <param name="index">Index of the control point.</param>
        protected internal override void ControlPointAdded(CurveControlPoint<FPVector3> curveControlPoint, int index)
        {
            tangents.Clear();
            ComputeTangents();
        }

        /// <summary>
        /// Called when a control point is removed.
        /// </summary>
        /// <param name="curveControlPoint">Removed control point.</param>
        /// <param name="oldIndex">Index of the control point before it was removed.</param>
        protected internal override void ControlPointRemoved(CurveControlPoint<FPVector3> curveControlPoint, int oldIndex)
        {
            tangents.Clear();
            ComputeTangents();
        }

        /// <summary>
        /// Called when a control point belonging to the curve has its time changed.
        /// </summary>
        /// <param name="curveControlPoint">Changed control point.</param>
        /// <param name="oldIndex">Old index of the control point.</param>
        /// <param name="newIndex">New index of the control point.</param>
        protected internal override void ControlPointTimeChanged(CurveControlPoint<FPVector3> curveControlPoint, int oldIndex, int newIndex)
        {
            tangents.Clear();
            ComputeTangents();
        }

        /// <summary>
        /// Called when a control point belonging to the curve has its value changed.
        /// </summary>
        /// <param name="curveControlPoint">Changed control point.</param>
        protected internal override void ControlPointValueChanged(CurveControlPoint<FPVector3> curveControlPoint)
        {
            tangents.Clear();
            ComputeTangents();
        }

        /// <summary>
        /// Computes the tangent entries in the curve according to some type of hermite curve.
        /// </summary>
        protected abstract void ComputeTangents();
    }
}
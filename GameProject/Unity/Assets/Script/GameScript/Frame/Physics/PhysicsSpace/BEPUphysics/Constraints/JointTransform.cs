﻿using System;
using FixedMath;
using FixMath.NET;

namespace BEPUphysics.Constraints
{
    /// <summary>
    /// Defines a three dimensional orthonormal basis used by a constraint.
    /// </summary>
    public class JointBasis3D
    {
        internal FPVector3 localPrimaryAxis = FPVector3.Backward;
        internal FPVector3 localXAxis = FPVector3.Right;
        internal FPVector3 localYAxis = FPVector3.Up;
        internal FPVector3 primaryAxis = FPVector3.Backward;
        internal FPMatrix3x3 rotationMatrix = FPMatrix3x3.Identity;
        internal FPVector3 xAxis = FPVector3.Right;
        internal FPVector3 yAxis = FPVector3.Up;

        /// <summary>
        /// Gets the primary axis of the transform in local space.
        /// </summary>
        public FPVector3 LocalPrimaryAxis
        {
            get { return localPrimaryAxis; }
        }

        /// <summary>
        /// Gets or sets the local transform of the basis.
        /// </summary>
        public FPMatrix3x3 LocalTransform
        {
            get
            {
                var toReturn = new FPMatrix3x3 {Right = localXAxis, Up = localYAxis, Backward = localPrimaryAxis};
                return toReturn;
            }
            set { SetLocalAxes(value); }
        }

        /// <summary>
        /// Gets the X axis of the transform in local space.
        /// </summary>
        public FPVector3 LocalXAxis
        {
            get { return localXAxis; }
        }

        /// <summary>
        /// Gets the Y axis of the transform in local space.
        /// </summary>
        public FPVector3 LocalYAxis
        {
            get { return localYAxis; }
        }

        /// <summary>
        /// Gets the primary axis of the transform.
        /// </summary>
        public FPVector3 PrimaryAxis
        {
            get { return primaryAxis; }
        }

        /// <summary>
        /// Gets or sets the rotation matrix used by the joint transform to convert local space axes to world space.
        /// </summary>
        public FPMatrix3x3 RotationMatrix
        {
            get { return rotationMatrix; }
            set
            {
                rotationMatrix = value;
                ComputeWorldSpaceAxes();
            }
        }

        /// <summary>
        /// Gets or sets the world transform of the basis.
        /// </summary>
        public FPMatrix3x3 WorldTransform
        {
            get
            {
                var toReturn = new FPMatrix3x3 {Right = xAxis, Up = yAxis, Backward = primaryAxis};
                return toReturn;
            }
            set { SetWorldAxes(value); }
        }

        /// <summary>
        /// Gets the X axis of the transform.
        /// </summary>
        public FPVector3 XAxis
        {
            get { return xAxis; }
        }

        /// <summary>
        /// Gets the Y axis of the transform.
        /// </summary>
        public FPVector3 YAxis
        {
            get { return yAxis; }
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix to use to transform the local axes into world space.</param>
        public void SetLocalAxes(FPVector3 primaryAxis, FPVector3 xAxis, FPVector3 yAxis, FPMatrix3x3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetLocalAxes(primaryAxis, xAxis, yAxis);
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        public void SetLocalAxes(FPVector3 primaryAxis, FPVector3 xAxis, FPVector3 yAxis)
        {
            if (Fix64.Abs(FPVector3.Dot(primaryAxis, xAxis)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(primaryAxis, yAxis)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(xAxis, yAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            localPrimaryAxis = FPVector3.Normalize(primaryAxis);
            localXAxis = FPVector3.Normalize(xAxis);
            localYAxis = FPVector3.Normalize(yAxis);
            ComputeWorldSpaceAxes();
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.
        /// The matrix's up vector is used as the y axis.</param>
        public void SetLocalAxes(FPMatrix3x3 matrix)
        {
            if (Fix64.Abs(FPVector3.Dot(matrix.Backward, matrix.Right)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(matrix.Backward, matrix.Up)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(matrix.Right, matrix.Up)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            localPrimaryAxis = FPVector3.Normalize(matrix.Backward);
            localXAxis = FPVector3.Normalize(matrix.Right);
            localYAxis = FPVector3.Normalize(matrix.Up);
            ComputeWorldSpaceAxes();
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix to use to transform the local axes into world space.</param>
        public void SetWorldAxes(FPVector3 primaryAxis, FPVector3 xAxis, FPVector3 yAxis, FPMatrix3x3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetWorldAxes(primaryAxis, xAxis, yAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        public void SetWorldAxes(FPVector3 primaryAxis, FPVector3 xAxis, FPVector3 yAxis)
        {
            if (Fix64.Abs(FPVector3.Dot(primaryAxis, xAxis)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(primaryAxis, yAxis)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(xAxis, yAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            this.primaryAxis = FPVector3.Normalize(primaryAxis);
            this.xAxis = FPVector3.Normalize(xAxis);
            this.yAxis = FPVector3.Normalize(yAxis);
            FPMatrix3x3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            FPMatrix3x3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
            FPMatrix3x3.TransformTranspose(ref this.yAxis, ref rotationMatrix, out localYAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.
        /// The matrix's up vector is used as the y axis.</param>
        public void SetWorldAxes(FPMatrix3x3 matrix)
        {
            if (Fix64.Abs(FPVector3.Dot(matrix.Backward, matrix.Right)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(matrix.Backward, matrix.Up)) > Toolbox.BigEpsilon ||
				Fix64.Abs(FPVector3.Dot(matrix.Right, matrix.Up)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            primaryAxis = FPVector3.Normalize(matrix.Backward);
            xAxis = FPVector3.Normalize(matrix.Right);
            yAxis = FPVector3.Normalize(matrix.Up);
            FPMatrix3x3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            FPMatrix3x3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
            FPMatrix3x3.TransformTranspose(ref this.yAxis, ref rotationMatrix, out localYAxis);
        }

        internal void ComputeWorldSpaceAxes()
        {
            FPMatrix3x3.Transform(ref localPrimaryAxis, ref rotationMatrix, out primaryAxis);
            FPMatrix3x3.Transform(ref localXAxis, ref rotationMatrix, out xAxis);
            FPMatrix3x3.Transform(ref localYAxis, ref rotationMatrix, out yAxis);
        }
    }

    /// <summary>
    /// Defines a two axes which are perpendicular to each other used by a constraint.
    /// </summary>
    public class JointBasis2D
    {
        internal FPVector3 localPrimaryAxis = FPVector3.Backward;
        internal FPVector3 localXAxis = FPVector3.Right;
        internal FPVector3 primaryAxis = FPVector3.Backward;
        internal FPMatrix3x3 rotationMatrix = FPMatrix3x3.Identity;
        internal FPVector3 xAxis = FPVector3.Right;

        /// <summary>
        /// Gets the primary axis of the transform in local space.
        /// </summary>
        public FPVector3 LocalPrimaryAxis
        {
            get { return localPrimaryAxis; }
        }

        /// <summary>
        /// Gets the X axis of the transform in local space.
        /// </summary>
        public FPVector3 LocalXAxis
        {
            get { return localXAxis; }
        }

        /// <summary>
        /// Gets the primary axis of the transform.
        /// </summary>
        public FPVector3 PrimaryAxis
        {
            get { return primaryAxis; }
        }

        /// <summary>
        /// Gets or sets the rotation matrix used by the joint transform to convert local space axes to world space.
        /// </summary>
        public FPMatrix3x3 RotationMatrix
        {
            get { return rotationMatrix; }
            set
            {
                rotationMatrix = value;
                ComputeWorldSpaceAxes();
            }
        }

        /// <summary>
        /// Gets the X axis of the transform.
        /// </summary>
        public FPVector3 XAxis
        {
            get { return xAxis; }
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix to use to transform the local axes into world space.</param>
        public void SetLocalAxes(FPVector3 primaryAxis, FPVector3 xAxis, FPMatrix3x3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetLocalAxes(primaryAxis, xAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        public void SetLocalAxes(FPVector3 primaryAxis, FPVector3 xAxis)
        {
            if (Fix64.Abs(FPVector3.Dot(primaryAxis, xAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");

            localPrimaryAxis = FPVector3.Normalize(primaryAxis);
            localXAxis = FPVector3.Normalize(xAxis);
            ComputeWorldSpaceAxes();
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.</param>
        public void SetLocalAxes(FPMatrix3x3 matrix)
        {
            if (Fix64.Abs(FPVector3.Dot(matrix.Backward, matrix.Right)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");
            localPrimaryAxis = FPVector3.Normalize(matrix.Backward);
            localXAxis = FPVector3.Normalize(matrix.Right);
            ComputeWorldSpaceAxes();
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix to use to transform the local axes into world space.</param>
        public void SetWorldAxes(FPVector3 primaryAxis, FPVector3 xAxis, FPMatrix3x3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetWorldAxes(primaryAxis, xAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        public void SetWorldAxes(FPVector3 primaryAxis, FPVector3 xAxis)
        {
            if (Fix64.Abs(FPVector3.Dot(primaryAxis, xAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");
            this.primaryAxis = FPVector3.Normalize(primaryAxis);
            this.xAxis = FPVector3.Normalize(xAxis);
            FPMatrix3x3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            FPMatrix3x3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.</param>
        public void SetWorldAxes(FPMatrix3x3 matrix)
        {
            if (Fix64.Abs(FPVector3.Dot(matrix.Backward, matrix.Right)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");
            primaryAxis = FPVector3.Normalize(matrix.Backward);
            xAxis = FPVector3.Normalize(matrix.Right);
            FPMatrix3x3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            FPMatrix3x3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
        }

        internal void ComputeWorldSpaceAxes()
        {
            FPMatrix3x3.Transform(ref localPrimaryAxis, ref rotationMatrix, out primaryAxis);
            FPMatrix3x3.Transform(ref localXAxis, ref rotationMatrix, out xAxis);
        }
    }
}
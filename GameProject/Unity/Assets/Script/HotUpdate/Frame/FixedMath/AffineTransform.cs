﻿

namespace FixedMath
{
    ///<summary>
    /// A transformation composed of a linear transformation and a translation.
    ///</summary>
    public struct AffineTransform
    {
        ///<summary>
        /// Translation in the affine transform.
        ///</summary>
        public FPVector3 Translation;
        /// <summary>
        /// Linear transform in the affine transform.
        /// </summary>
        public FPMatrix3x3 LinearTransform;

        ///<summary>
        /// Constructs a new affine transform.
        ///</summary>
        ///<param name="translation">Translation to use in the transform.</param>
        public AffineTransform(ref FPVector3 translation)
        {
            LinearTransform = FPMatrix3x3.Identity;
            Translation = translation;
        }

        ///<summary>
        /// Constructs a new affine transform.
        ///</summary>
        ///<param name="translation">Translation to use in the transform.</param>
        public AffineTransform(FPVector3 translation)
            : this(ref translation)
        {
        }

        ///<summary>
        /// Constructs a new affine tranform.
        ///</summary>
        ///<param name="orientation">Orientation to use as the linear transform.</param>
        ///<param name="translation">Translation to use in the transform.</param>
        public AffineTransform(ref FPQuaternion orientation, ref FPVector3 translation)
        {
            FPMatrix3x3.CreateFromQuaternion(ref orientation, out LinearTransform);
            Translation = translation;
        }

        ///<summary>
        /// Constructs a new affine tranform.
        ///</summary>
        ///<param name="orientation">Orientation to use as the linear transform.</param>
        ///<param name="translation">Translation to use in the transform.</param>
        public AffineTransform(FPQuaternion orientation, FPVector3 translation)
            : this(ref orientation, ref translation)
        {
        }

        ///<summary>
        /// Constructs a new affine transform.
        ///</summary>
        ///<param name="scaling">Scaling to apply in the linear transform.</param>
        ///<param name="orientation">Orientation to apply in the linear transform.</param>
        ///<param name="translation">Translation to apply.</param>
        public AffineTransform(ref FPVector3 scaling, ref FPQuaternion orientation, ref FPVector3 translation)
        {
            //Create an SRT transform.
            FPMatrix3x3.CreateScale(ref scaling, out LinearTransform);
            FPMatrix3x3 rotation;
            FPMatrix3x3.CreateFromQuaternion(ref orientation, out rotation);
            FPMatrix3x3.Multiply(ref LinearTransform, ref rotation, out LinearTransform);
            Translation = translation;
        }

        ///<summary>
        /// Constructs a new affine transform.
        ///</summary>
        ///<param name="scaling">Scaling to apply in the linear transform.</param>
        ///<param name="orientation">Orientation to apply in the linear transform.</param>
        ///<param name="translation">Translation to apply.</param>
        public AffineTransform(FPVector3 scaling, FPQuaternion orientation, FPVector3 translation)
            : this(ref scaling, ref orientation, ref translation)
        {
        }

        ///<summary>
        /// Constructs a new affine transform.
        ///</summary>
        ///<param name="linearTransform">The linear transform component.</param>
        ///<param name="translation">Translation component of the transform.</param>
        public AffineTransform(ref FPMatrix3x3 linearTransform, ref FPVector3 translation)
        {
            LinearTransform = linearTransform;
            Translation = translation;

        }

        ///<summary>
        /// Constructs a new affine transform.
        ///</summary>
        ///<param name="linearTransform">The linear transform component.</param>
        ///<param name="translation">Translation component of the transform.</param>
        public AffineTransform(FPMatrix3x3 linearTransform, FPVector3 translation)
            : this(ref linearTransform, ref translation)
        {
        }

        ///<summary>
        /// Gets or sets the 4x4 matrix representation of the affine transform.
        /// The linear transform is the upper left 3x3 part of the 4x4 matrix.
        /// The translation is included in the matrix's Translation property.
        ///</summary>
        public FPMatrix FpMatrix
        {
            get
            {
                FPMatrix toReturn;
                FPMatrix3x3.ToMatrix4X4(ref LinearTransform, out toReturn);
                toReturn.Translation = Translation;
                return toReturn;
            }
            set
            {
                FPMatrix3x3.CreateFromMatrix(ref value, out LinearTransform);
                Translation = value.Translation;
            }
        }


        ///<summary>
        /// Gets the identity affine transform.
        ///</summary>
        public static AffineTransform Identity
        {
            get
            {
                var t = new AffineTransform { LinearTransform = FPMatrix3x3.Identity, Translation = new FPVector3() };
                return t;
            }
        }

        ///<summary>
        /// Transforms a vector by an affine transform.
        ///</summary>
        ///<param name="position">Position to transform.</param>
        ///<param name="transform">Transform to apply.</param>
        ///<param name="transformed">Transformed position.</param>
        public static void Transform(ref FPVector3 position, ref AffineTransform transform, out FPVector3 transformed)
        {
            FPMatrix3x3.Transform(ref position, ref transform.LinearTransform, out transformed);
            FPVector3.Add(ref transformed, ref transform.Translation, out transformed);
        }

        ///<summary>
        /// Transforms a vector by an affine transform's inverse.
        ///</summary>
        ///<param name="position">Position to transform.</param>
        ///<param name="transform">Transform to invert and apply.</param>
        ///<param name="transformed">Transformed position.</param>
        public static void TransformInverse(ref FPVector3 position, ref AffineTransform transform, out FPVector3 transformed)
        {
            FPVector3.Subtract(ref position, ref transform.Translation, out transformed);
            FPMatrix3x3 inverse;
            FPMatrix3x3.Invert(ref transform.LinearTransform, out inverse);
            FPMatrix3x3.TransformTranspose(ref transformed, ref inverse, out transformed);
        }

        ///<summary>
        /// Inverts an affine transform.
        ///</summary>
        ///<param name="transform">Transform to invert.</param>
        /// <param name="inverse">Inverse of the transform.</param>
        public static void Invert(ref AffineTransform transform, out AffineTransform inverse)
        {
            FPMatrix3x3.Invert(ref transform.LinearTransform, out inverse.LinearTransform);
            FPMatrix3x3.Transform(ref transform.Translation, ref inverse.LinearTransform, out inverse.Translation);
            FPVector3.Negate(ref inverse.Translation, out inverse.Translation);
        }

        /// <summary>
        /// Multiplies a transform by another transform.
        /// </summary>
        /// <param name="a">First transform.</param>
        /// <param name="b">Second transform.</param>
        /// <param name="transform">Combined transform.</param>
        public static void Multiply(ref AffineTransform a, ref AffineTransform b, out AffineTransform transform)
        {
            FPMatrix3x3 linearTransform;//Have to use temporary variable just in case a or b reference is transform.
            FPMatrix3x3.Multiply(ref a.LinearTransform, ref b.LinearTransform, out linearTransform);
            FPVector3 translation;
            FPMatrix3x3.Transform(ref a.Translation, ref b.LinearTransform, out translation);
            FPVector3.Add(ref translation, ref b.Translation, out transform.Translation);
            transform.LinearTransform = linearTransform;
        }

        ///<summary>
        /// Multiplies a rigid transform by an affine transform.
        ///</summary>
        ///<param name="a">Rigid transform.</param>
        ///<param name="b">Affine transform.</param>
        ///<param name="transform">Combined transform.</param>
        public static void Multiply(ref RigidTransform a, ref AffineTransform b, out AffineTransform transform)
        {
            FPMatrix3x3 linearTransform;//Have to use temporary variable just in case b reference is transform.
            FPMatrix3x3.CreateFromQuaternion(ref a.Orientation, out linearTransform);
            FPMatrix3x3.Multiply(ref linearTransform, ref b.LinearTransform, out linearTransform);
            FPVector3 translation;
            FPMatrix3x3.Transform(ref a.Position, ref b.LinearTransform, out translation);
            FPVector3.Add(ref translation, ref b.Translation, out transform.Translation);
            transform.LinearTransform = linearTransform;
        }


        ///<summary>
        /// Transforms a vector using an affine transform.
        ///</summary>
        ///<param name="position">Position to transform.</param>
        ///<param name="affineTransform">Transform to apply.</param>
        ///<returns>Transformed position.</returns>
        public static FPVector3 Transform(FPVector3 position, AffineTransform affineTransform)
        {
            FPVector3 toReturn;
            Transform(ref position, ref affineTransform, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates an affine transform from a rigid transform.
        /// </summary>
        /// <param name="rigid">Rigid transform to base the affine transform on.</param>
        /// <param name="affine">Affine transform created from the rigid transform.</param>
        public static void CreateFromRigidTransform(ref RigidTransform rigid, out AffineTransform affine)
        {
            affine.Translation = rigid.Position;
            FPMatrix3x3.CreateFromQuaternion(ref rigid.Orientation, out affine.LinearTransform);
        }

        /// <summary>
        /// Creates an affine transform from a rigid transform.
        /// </summary>
        /// <param name="rigid">Rigid transform to base the affine transform on.</param>
        /// <returns>Affine transform created from the rigid transform.</returns>
        public static AffineTransform CreateFromRigidTransform(RigidTransform rigid)
        {
            AffineTransform toReturn;
            toReturn.Translation = rigid.Position;
            FPMatrix3x3.CreateFromQuaternion(ref rigid.Orientation, out toReturn.LinearTransform);
            return toReturn;
        }

    }
}
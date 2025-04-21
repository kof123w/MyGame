using FixMath.NET;
using System;

namespace FixedMath
{
    /// <summary>
    /// Provides XNA-like 4x4 matrix math.
    /// </summary>
    public struct FPMatrix
    {
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public Fix64 M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public Fix64 M12;

        /// <summary>
        /// Value at row 1, column 3 of the matrix.
        /// </summary>
        public Fix64 M13;

        /// <summary>
        /// Value at row 1, column 4 of the matrix.
        /// </summary>
        public Fix64 M14;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public Fix64 M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public Fix64 M22;

        /// <summary>
        /// Value at row 2, column 3 of the matrix.
        /// </summary>
        public Fix64 M23;

        /// <summary>
        /// Value at row 2, column 4 of the matrix.
        /// </summary>
        public Fix64 M24;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public Fix64 M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public Fix64 M32;

        /// <summary>
        /// Value at row 3, column 3 of the matrix.
        /// </summary>
        public Fix64 M33;

        /// <summary>
        /// Value at row 3, column 4 of the matrix.
        /// </summary>
        public Fix64 M34;

        /// <summary>
        /// Value at row 4, column 1 of the matrix.
        /// </summary>
        public Fix64 M41;

        /// <summary>
        /// Value at row 4, column 2 of the matrix.
        /// </summary>
        public Fix64 M42;

        /// <summary>
        /// Value at row 4, column 3 of the matrix.
        /// </summary>
        public Fix64 M43;

        /// <summary>
        /// Value at row 4, column 4 of the matrix.
        /// </summary>
        public Fix64 M44;

        /// <summary>
        /// Constructs a new 4 row, 4 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m13">Value at row 1, column 3 of the matrix.</param>
        /// <param name="m14">Value at row 1, column 4 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        /// <param name="m23">Value at row 2, column 3 of the matrix.</param>
        /// <param name="m24">Value at row 2, column 4 of the matrix.</param>
        /// <param name="m31">Value at row 3, column 1 of the matrix.</param>
        /// <param name="m32">Value at row 3, column 2 of the matrix.</param>
        /// <param name="m33">Value at row 3, column 3 of the matrix.</param>
        /// <param name="m34">Value at row 3, column 4 of the matrix.</param>
        /// <param name="m41">Value at row 4, column 1 of the matrix.</param>
        /// <param name="m42">Value at row 4, column 2 of the matrix.</param>
        /// <param name="m43">Value at row 4, column 3 of the matrix.</param>
        /// <param name="m44">Value at row 4, column 4 of the matrix.</param>
        public FPMatrix(Fix64 m11, Fix64 m12, Fix64 m13, Fix64 m14,
                      Fix64 m21, Fix64 m22, Fix64 m23, Fix64 m24,
                      Fix64 m31, Fix64 m32, Fix64 m33, Fix64 m34,
                      Fix64 m41, Fix64 m42, Fix64 m43, Fix64 m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;

            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        /// <summary>
        /// Gets or sets the translation component of the transform.
        /// </summary>
        public FPVector3 Translation
        {
            get
            {
                return new FPVector3()
                {
                    x = M41,
                    y = M42,
                    z = M43
                };
            }
            set
            {
                M41 = value.x;
                M42 = value.y;
                M43 = value.z;
            }
        }

        /// <summary>
        /// Gets or sets the backward vector of the matrix.
        /// </summary>
        public FPVector3 Backward
        {
            get
            {
#if !WINDOWS
                FPVector3 vector = new FPVector3();
#else
                Vector3 vector;
#endif
                vector.x = M31;
                vector.y = M32;
                vector.z = M33;
                return vector;
            }
            set
            {
                M31 = value.x;
                M32 = value.y;
                M33 = value.z;
            }
        }

        /// <summary>
        /// Gets or sets the down vector of the matrix.
        /// </summary>
        public FPVector3 Down
        {
            get
            {
#if !WINDOWS
                FPVector3 vector = new FPVector3();
#else
                Vector3 vector;
#endif
                vector.x = -M21;
                vector.y = -M22;
                vector.z = -M23;
                return vector;
            }
            set
            {
                M21 = -value.x;
                M22 = -value.y;
                M23 = -value.z;
            }
        }

        /// <summary>
        /// Gets or sets the forward vector of the matrix.
        /// </summary>
        public FPVector3 Forward
        {
            get
            {
#if !WINDOWS
                FPVector3 vector = new FPVector3();
#else
                Vector3 vector;
#endif
                vector.x = -M31;
                vector.y = -M32;
                vector.z = -M33;
                return vector;
            }
            set
            {
                M31 = -value.x;
                M32 = -value.y;
                M33 = -value.z;
            }
        }

        /// <summary>
        /// Gets or sets the left vector of the matrix.
        /// </summary>
        public FPVector3 Left
        {
            get
            {
#if !WINDOWS
                FPVector3 vector = new FPVector3();
#else
                Vector3 vector;
#endif
                vector.x = -M11;
                vector.y = -M12;
                vector.z = -M13;
                return vector;
            }
            set
            {
                M11 = -value.x;
                M12 = -value.y;
                M13 = -value.z;
            }
        }

        /// <summary>
        /// Gets or sets the right vector of the matrix.
        /// </summary>
        public FPVector3 Right
        {
            get
            {
#if !WINDOWS
                FPVector3 vector = new FPVector3();
#else
                Vector3 vector;
#endif
                vector.x = M11;
                vector.y = M12;
                vector.z = M13;
                return vector;
            }
            set
            {
                M11 = value.x;
                M12 = value.y;
                M13 = value.z;
            }
        }

        /// <summary>
        /// Gets or sets the up vector of the matrix.
        /// </summary>
        public FPVector3 Up
        {
            get
            {
#if !WINDOWS
                FPVector3 vector = new FPVector3();
#else
                Vector3 vector;
#endif
                vector.x = M21;
                vector.y = M22;
                vector.z = M23;
                return vector;
            }
            set
            {
                M21 = value.x;
                M22 = value.y;
                M23 = value.z;
            }
        }


        /// <summary>
        /// Computes the determinant of the matrix.
        /// </summary>
        /// <returns></returns>
        public Fix64 Determinant()
        {
            //Compute the re-used 2x2 determinants.
            Fix64 det1 = M33 * M44 - M34 * M43;
            Fix64 det2 = M32 * M44 - M34 * M42;
            Fix64 det3 = M32 * M43 - M33 * M42;
            Fix64 det4 = M31 * M44 - M34 * M41;
            Fix64 det5 = M31 * M43 - M33 * M41;
            Fix64 det6 = M31 * M42 - M32 * M41;
            return
                (M11 * ((M22 * det1 - M23 * det2) + M24 * det3)) -
                (M12 * ((M21 * det1 - M23 * det4) + M24 * det5)) +
                (M13 * ((M21 * det2 - M22 * det4) + M24 * det6)) -
                (M14 * ((M21 * det3 - M22 * det5) + M23 * det6));
        }

        /// <summary>
        /// Transposes the matrix in-place.
        /// </summary>
        public void Transpose()
        {
            Fix64 intermediate = M12;
            M12 = M21;
            M21 = intermediate;

            intermediate = M13;
            M13 = M31;
            M31 = intermediate;

            intermediate = M14;
            M14 = M41;
            M41 = intermediate;

            intermediate = M23;
            M23 = M32;
            M32 = intermediate;

            intermediate = M24;
            M24 = M42;
            M42 = intermediate;

            intermediate = M34;
            M34 = M43;
            M43 = intermediate;
        }

        /// <summary>
        /// Creates a matrix representing the given axis and angle rotation.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <returns>Matrix created from the axis and angle.</returns>
        public static FPMatrix CreateFromAxisAngle(FPVector3 axis, Fix64 angle)
        {
            FPMatrix toReturn;
            CreateFromAxisAngle(ref axis, angle, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates a matrix representing the given axis and angle rotation.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <param name="result">Matrix created from the axis and angle.</param>
        public static void CreateFromAxisAngle(ref FPVector3 axis, Fix64 angle, out FPMatrix result)
        {
            Fix64 xx = axis.x * axis.x;
            Fix64 yy = axis.y * axis.y;
            Fix64 zz = axis.z * axis.z;
            Fix64 xy = axis.x * axis.y;
            Fix64 xz = axis.x * axis.z;
            Fix64 yz = axis.y * axis.z;

            Fix64 sinAngle = Fix64.Sin(angle);
            Fix64 oneMinusCosAngle = F64.C1 - Fix64.Cos(angle);

            result.M11 = F64.C1 + oneMinusCosAngle * (xx - F64.C1);
            result.M21 = -axis.z * sinAngle + oneMinusCosAngle * xy;
            result.M31 = axis.y * sinAngle + oneMinusCosAngle * xz;
            result.M41 = F64.C0;

            result.M12 = axis.z * sinAngle + oneMinusCosAngle * xy;
            result.M22 = F64.C1 + oneMinusCosAngle * (yy - F64.C1);
            result.M32 = -axis.x * sinAngle + oneMinusCosAngle * yz;
            result.M42 = F64.C0;

            result.M13 = -axis.y * sinAngle + oneMinusCosAngle * xz;
            result.M23 = axis.x * sinAngle + oneMinusCosAngle * yz;
            result.M33 = F64.C1 + oneMinusCosAngle * (zz - F64.C1);
            result.M43 = F64.C0;

            result.M14 = F64.C0;
            result.M24 = F64.C0;
            result.M34 = F64.C0;
            result.M44 = F64.C1;
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="fpQuaternion">Quaternion to convert.</param>
        /// <param name="result">Rotation matrix created from the quaternion.</param>
        public static void CreateFromQuaternion(ref FPQuaternion fpQuaternion, out FPMatrix result)
        {
            Fix64 qX2 = fpQuaternion.x + fpQuaternion.x;
            Fix64 qY2 = fpQuaternion.y + fpQuaternion.y;
            Fix64 qZ2 = fpQuaternion.z + fpQuaternion.z;
            Fix64 XX = qX2 * fpQuaternion.x;
            Fix64 YY = qY2 * fpQuaternion.y;
            Fix64 ZZ = qZ2 * fpQuaternion.z;
            Fix64 XY = qX2 * fpQuaternion.y;
            Fix64 XZ = qX2 * fpQuaternion.z;
            Fix64 XW = qX2 * fpQuaternion.w;
            Fix64 YZ = qY2 * fpQuaternion.z;
            Fix64 YW = qY2 * fpQuaternion.w;
            Fix64 ZW = qZ2 * fpQuaternion.w;

            result.M11 = F64.C1 - YY - ZZ;
            result.M21 = XY - ZW;
            result.M31 = XZ + YW;
            result.M41 = F64.C0;

            result.M12 = XY + ZW;
            result.M22 = F64.C1 - XX - ZZ;
            result.M32 = YZ - XW;
            result.M42 = F64.C0;

            result.M13 = XZ - YW;
            result.M23 = YZ + XW;
            result.M33 = F64.C1 - XX - YY;
            result.M43 = F64.C0;

            result.M14 = F64.C0;
            result.M24 = F64.C0;
            result.M34 = F64.C0;
            result.M44 = F64.C1;
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="fpQuaternion">Quaternion to convert.</param>
        /// <returns>Rotation matrix created from the quaternion.</returns>
        public static FPMatrix CreateFromQuaternion(FPQuaternion fpQuaternion)
        {
            FPMatrix toReturn;
            CreateFromQuaternion(ref fpQuaternion, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Combined transformation.</param>
        public static void Multiply(ref FPMatrix a, ref FPMatrix b, out FPMatrix result)
        {
            Fix64 resultM11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41;
            Fix64 resultM12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42;
            Fix64 resultM13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43;
            Fix64 resultM14 = a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44;

            Fix64 resultM21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41;
            Fix64 resultM22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42;
            Fix64 resultM23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43;
            Fix64 resultM24 = a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44;

            Fix64 resultM31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41;
            Fix64 resultM32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42;
            Fix64 resultM33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43;
            Fix64 resultM34 = a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44;

            Fix64 resultM41 = a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41;
            Fix64 resultM42 = a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42;
            Fix64 resultM43 = a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43;
            Fix64 resultM44 = a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44;

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;
            result.M14 = resultM14;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;
            result.M24 = resultM24;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
            result.M34 = resultM34;

            result.M41 = resultM41;
            result.M42 = resultM42;
            result.M43 = resultM43;
            result.M44 = resultM44;
        }


        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Combined transformation.</returns>
        public static FPMatrix Multiply(FPMatrix a, FPMatrix b)
        {
            FPMatrix result;
            Multiply(ref a, ref b, out result);
            return result;
        }


        /// <summary>
        /// Scales all components of the matrix.
        /// </summary>
        /// <param name="fpMatrix">Matrix to scale.</param>
        /// <param name="scale">Amount to scale.</param>
        /// <param name="result">Scaled matrix.</param>
        public static void Multiply(ref FPMatrix fpMatrix, Fix64 scale, out FPMatrix result)
        {
            result.M11 = fpMatrix.M11 * scale;
            result.M12 = fpMatrix.M12 * scale;
            result.M13 = fpMatrix.M13 * scale;
            result.M14 = fpMatrix.M14 * scale;

            result.M21 = fpMatrix.M21 * scale;
            result.M22 = fpMatrix.M22 * scale;
            result.M23 = fpMatrix.M23 * scale;
            result.M24 = fpMatrix.M24 * scale;

            result.M31 = fpMatrix.M31 * scale;
            result.M32 = fpMatrix.M32 * scale;
            result.M33 = fpMatrix.M33 * scale;
            result.M34 = fpMatrix.M34 * scale;

            result.M41 = fpMatrix.M41 * scale;
            result.M42 = fpMatrix.M42 * scale;
            result.M43 = fpMatrix.M43 * scale;
            result.M44 = fpMatrix.M44 * scale;
        }

        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Combined transformation.</returns>
        public static FPMatrix operator *(FPMatrix a, FPMatrix b)
        {
            FPMatrix toReturn;
            Multiply(ref a, ref b, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static FPMatrix operator *(FPMatrix m, Fix64 f)
        {
            FPMatrix result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static FPMatrix operator *(Fix64 f, FPMatrix m)
        {
            FPMatrix result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref FPVector4 v, ref FPMatrix fpMatrix, out FPVector4 result)
        {
            Fix64 vX = v.x;
            Fix64 vY = v.y;
            Fix64 vZ = v.z;
            Fix64 vW = v.w;
            result.x = vX * fpMatrix.M11 + vY * fpMatrix.M21 + vZ * fpMatrix.M31 + vW * fpMatrix.M41;
            result.y = vX * fpMatrix.M12 + vY * fpMatrix.M22 + vZ * fpMatrix.M32 + vW * fpMatrix.M42;
            result.z = vX * fpMatrix.M13 + vY * fpMatrix.M23 + vZ * fpMatrix.M33 + vW * fpMatrix.M43;
            result.w = vX * fpMatrix.M14 + vY * fpMatrix.M24 + vZ * fpMatrix.M34 + vW * fpMatrix.M44;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 Transform(FPVector4 v, FPMatrix fpMatrix)
        {
            FPVector4 toReturn;
            Transform(ref v, ref fpMatrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref FPVector4 v, ref FPMatrix fpMatrix, out FPVector4 result)
        {
            Fix64 vX = v.x;
            Fix64 vY = v.y;
            Fix64 vZ = v.z;
            Fix64 vW = v.w;
            result.x = vX * fpMatrix.M11 + vY * fpMatrix.M12 + vZ * fpMatrix.M13 + vW * fpMatrix.M14;
            result.y = vX * fpMatrix.M21 + vY * fpMatrix.M22 + vZ * fpMatrix.M23 + vW * fpMatrix.M24;
            result.z = vX * fpMatrix.M31 + vY * fpMatrix.M32 + vZ * fpMatrix.M33 + vW * fpMatrix.M34;
            result.w = vX * fpMatrix.M41 + vY * fpMatrix.M42 + vZ * fpMatrix.M43 + vW * fpMatrix.M44;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 TransformTranspose(FPVector4 v, FPMatrix fpMatrix)
        {
            FPVector4 toReturn;
            TransformTranspose(ref v, ref fpMatrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref FPVector3 v, ref FPMatrix fpMatrix, out FPVector4 result)
        {
            result.x = v.x * fpMatrix.M11 + v.y * fpMatrix.M21 + v.z * fpMatrix.M31 + fpMatrix.M41;
            result.y = v.x * fpMatrix.M12 + v.y * fpMatrix.M22 + v.z * fpMatrix.M32 + fpMatrix.M42;
            result.z = v.x * fpMatrix.M13 + v.y * fpMatrix.M23 + v.z * fpMatrix.M33 + fpMatrix.M43;
            result.w = v.x * fpMatrix.M14 + v.y * fpMatrix.M24 + v.z * fpMatrix.M34 + fpMatrix.M44;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 Transform(FPVector3 v, FPMatrix fpMatrix)
        {
            FPVector4 toReturn;
            Transform(ref v, ref fpMatrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref FPVector3 v, ref FPMatrix fpMatrix, out FPVector4 result)
        {
            result.x = v.x * fpMatrix.M11 + v.y * fpMatrix.M12 + v.z * fpMatrix.M13 + fpMatrix.M14;
            result.y = v.x * fpMatrix.M21 + v.y * fpMatrix.M22 + v.z * fpMatrix.M23 + fpMatrix.M24;
            result.z = v.x * fpMatrix.M31 + v.y * fpMatrix.M32 + v.z * fpMatrix.M33 + fpMatrix.M34;
            result.w = v.x * fpMatrix.M41 + v.y * fpMatrix.M42 + v.z * fpMatrix.M43 + fpMatrix.M44;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector4 TransformTranspose(FPVector3 v, FPMatrix fpMatrix)
        {
            FPVector4 toReturn;
            TransformTranspose(ref v, ref fpMatrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref FPVector3 v, ref FPMatrix fpMatrix, out FPVector3 result)
        {
            Fix64 vX = v.x;
            Fix64 vY = v.y;
            Fix64 vZ = v.z;
            result.x = vX * fpMatrix.M11 + vY * fpMatrix.M21 + vZ * fpMatrix.M31 + fpMatrix.M41;
            result.y = vX * fpMatrix.M12 + vY * fpMatrix.M22 + vZ * fpMatrix.M32 + fpMatrix.M42;
            result.z = vX * fpMatrix.M13 + vY * fpMatrix.M23 + vZ * fpMatrix.M33 + fpMatrix.M43;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref FPVector3 v, ref FPMatrix fpMatrix, out FPVector3 result)
        {
            Fix64 vX = v.x;
            Fix64 vY = v.y;
            Fix64 vZ = v.z;
            result.x = vX * fpMatrix.M11 + vY * fpMatrix.M12 + vZ * fpMatrix.M13 + fpMatrix.M14;
            result.y = vX * fpMatrix.M21 + vY * fpMatrix.M22 + vZ * fpMatrix.M23 + fpMatrix.M24;
            result.z = vX * fpMatrix.M31 + vY * fpMatrix.M32 + vZ * fpMatrix.M33 + fpMatrix.M34;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformNormal(ref FPVector3 v, ref FPMatrix fpMatrix, out FPVector3 result)
        {
            Fix64 vX = v.x;
            Fix64 vY = v.y;
            Fix64 vZ = v.z;
            result.x = vX * fpMatrix.M11 + vY * fpMatrix.M21 + vZ * fpMatrix.M31;
            result.y = vX * fpMatrix.M12 + vY * fpMatrix.M22 + vZ * fpMatrix.M32;
            result.z = vX * fpMatrix.M13 + vY * fpMatrix.M23 + vZ * fpMatrix.M33;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector3 TransformNormal(FPVector3 v, FPMatrix fpMatrix)
        {
            FPVector3 toReturn;
            TransformNormal(ref v, ref fpMatrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformNormalTranspose(ref FPVector3 v, ref FPMatrix fpMatrix, out FPVector3 result)
        {
            Fix64 vX = v.x;
            Fix64 vY = v.y;
            Fix64 vZ = v.z;
            result.x = vX * fpMatrix.M11 + vY * fpMatrix.M12 + vZ * fpMatrix.M13;
            result.y = vX * fpMatrix.M21 + vY * fpMatrix.M22 + vZ * fpMatrix.M23;
            result.z = vX * fpMatrix.M31 + vY * fpMatrix.M32 + vZ * fpMatrix.M33;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="fpMatrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static FPVector3 TransformNormalTranspose(FPVector3 v, FPMatrix fpMatrix)
        {
            FPVector3 toReturn;
            TransformNormalTranspose(ref v, ref fpMatrix, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Transposes the matrix.
        /// </summary>
        /// <param name="m">Matrix to transpose.</param>
        /// <param name="transposed">Matrix to transpose.</param>
        public static void Transpose(ref FPMatrix m, out FPMatrix transposed)
        {
            Fix64 intermediate = m.M12;
            transposed.M12 = m.M21;
            transposed.M21 = intermediate;

            intermediate = m.M13;
            transposed.M13 = m.M31;
            transposed.M31 = intermediate;

            intermediate = m.M14;
            transposed.M14 = m.M41;
            transposed.M41 = intermediate;

            intermediate = m.M23;
            transposed.M23 = m.M32;
            transposed.M32 = intermediate;

            intermediate = m.M24;
            transposed.M24 = m.M42;
            transposed.M42 = intermediate;

            intermediate = m.M34;
            transposed.M34 = m.M43;
            transposed.M43 = intermediate;

            transposed.M11 = m.M11;
            transposed.M22 = m.M22;
            transposed.M33 = m.M33;
            transposed.M44 = m.M44;
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <param name="inverted">Inverted version of the matrix.</param>
        public static void Invert(ref FPMatrix m, out FPMatrix inverted)
        {
			FPMatrix4x8.Invert(ref m, out inverted);
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <returns>Inverted version of the matrix.</returns>
        public static FPMatrix Invert(FPMatrix m)
        {
            FPMatrix inverted;
            Invert(ref m, out inverted);
            return inverted;
        }
		
        /// <summary>
        /// Inverts the matrix using a process that only works for rigid transforms.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <param name="inverted">Inverted version of the matrix.</param>
        public static void InvertRigid(ref FPMatrix m, out FPMatrix inverted)
        {
            //Invert (transpose) the upper left 3x3 rotation.
            Fix64 intermediate = m.M12;
            inverted.M12 = m.M21;
            inverted.M21 = intermediate;

            intermediate = m.M13;
            inverted.M13 = m.M31;
            inverted.M31 = intermediate;

            intermediate = m.M23;
            inverted.M23 = m.M32;
            inverted.M32 = intermediate;

            inverted.M11 = m.M11;
            inverted.M22 = m.M22;
            inverted.M33 = m.M33;

            //Translation component
            var vX = m.M41;
            var vY = m.M42;
            var vZ = m.M43;
            inverted.M41 = -(vX * inverted.M11 + vY * inverted.M21 + vZ * inverted.M31);
            inverted.M42 = -(vX * inverted.M12 + vY * inverted.M22 + vZ * inverted.M32);
            inverted.M43 = -(vX * inverted.M13 + vY * inverted.M23 + vZ * inverted.M33);

            //Last chunk.
            inverted.M14 = F64.C0;
            inverted.M24 = F64.C0;
            inverted.M34 = F64.C0;
            inverted.M44 = F64.C1;
        }

        /// <summary>
        /// Inverts the matrix using a process that only works for rigid transforms.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <returns>Inverted version of the matrix.</returns>
        public static FPMatrix InvertRigid(FPMatrix m)
        {
            FPMatrix inverse;
            InvertRigid(ref m, out inverse);
            return inverse;
        }

        /// <summary>
        /// Gets the 4x4 identity matrix.
        /// </summary>
        public static FPMatrix Identity
        {
            get
            {
                FPMatrix toReturn;
                toReturn.M11 = F64.C1;
                toReturn.M12 = F64.C0;
                toReturn.M13 = F64.C0;
                toReturn.M14 = F64.C0;

                toReturn.M21 = F64.C0;
                toReturn.M22 = F64.C1;
                toReturn.M23 = F64.C0;
                toReturn.M24 = F64.C0;

                toReturn.M31 = F64.C0;
                toReturn.M32 = F64.C0;
                toReturn.M33 = F64.C1;
                toReturn.M34 = F64.C0;

                toReturn.M41 = F64.C0;
                toReturn.M42 = F64.C0;
                toReturn.M43 = F64.C0;
                toReturn.M44 = F64.C1;
                return toReturn;
            }
        }

        /// <summary>
        /// Creates a right handed orthographic projection.
        /// </summary>
        /// <param name="left">Leftmost coordinate of the projected area.</param>
        /// <param name="right">Rightmost coordinate of the projected area.</param>
        /// <param name="bottom">Bottom coordinate of the projected area.</param>
        /// <param name="top">Top coordinate of the projected area.</param>
        /// <param name="zNear">Near plane of the projection.</param>
        /// <param name="zFar">Far plane of the projection.</param>
        /// <param name="projection">The resulting orthographic projection matrix.</param>
        public static void CreateOrthographicRH(Fix64 left, Fix64 right, Fix64 bottom, Fix64 top, Fix64 zNear, Fix64 zFar, out FPMatrix projection)
        {
            Fix64 width = right - left;
            Fix64 height = top - bottom;
            Fix64 depth = zFar - zNear;
            projection.M11 = F64.C2 / width;
            projection.M12 = F64.C0;
            projection.M13 = F64.C0;
            projection.M14 = F64.C0;

            projection.M21 = F64.C0;
            projection.M22 = F64.C2 / height;
            projection.M23 = F64.C0;
            projection.M24 = F64.C0;

            projection.M31 = F64.C0;
            projection.M32 = F64.C0;
            projection.M33 = -1 / depth;
            projection.M34 = F64.C0;

            projection.M41 = (left + right) / -width;
            projection.M42 = (top + bottom) / -height;
            projection.M43 = zNear / -depth;
            projection.M44 = F64.C1;

        }

        /// <summary>
        /// Creates a right-handed perspective matrix.
        /// </summary>
        /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
        /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
        /// <param name="nearClip">Near clip plane of the perspective.</param>
        /// <param name="farClip">Far clip plane of the perspective.</param>
        /// <param name="perspective">Resulting perspective matrix.</param>
        public static void CreatePerspectiveFieldOfViewRH(Fix64 fieldOfView, Fix64 aspectRatio, Fix64 nearClip, Fix64 farClip, out FPMatrix perspective)
        {
            Fix64 h = F64.C1 / Fix64.Tan(fieldOfView / F64.C2);
            Fix64 w = h / aspectRatio;
            perspective.M11 = w;
            perspective.M12 = F64.C0;
            perspective.M13 = F64.C0;
            perspective.M14 = F64.C0;

            perspective.M21 = F64.C0;
            perspective.M22 = h;
            perspective.M23 = F64.C0;
            perspective.M24 = F64.C0;

            perspective.M31 = F64.C0;
            perspective.M32 = F64.C0;
            perspective.M33 = farClip / (nearClip - farClip);
            perspective.M34 = -1;

            perspective.M41 = F64.C0;
            perspective.M42 = F64.C0;
            perspective.M44 = F64.C0;
            perspective.M43 = nearClip * perspective.M33;

        }

        /// <summary>
        /// Creates a right-handed perspective matrix.
        /// </summary>
        /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
        /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
        /// <param name="nearClip">Near clip plane of the perspective.</param>
        /// <param name="farClip">Far clip plane of the perspective.</param>
        /// <returns>Resulting perspective matrix.</returns>
        public static FPMatrix CreatePerspectiveFieldOfViewRH(Fix64 fieldOfView, Fix64 aspectRatio, Fix64 nearClip, Fix64 farClip)
        {
            FPMatrix perspective;
            CreatePerspectiveFieldOfViewRH(fieldOfView, aspectRatio, nearClip, farClip, out perspective);
            return perspective;
        }

        /// <summary>
        /// Creates a view matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">Target of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <param name="viewFpMatrix">Look at matrix.</param>
        public static void CreateLookAtRH(ref FPVector3 position, ref FPVector3 target, ref FPVector3 upVector, out FPMatrix viewFpMatrix)
        {
            FPVector3 forward;
            FPVector3.Subtract(ref target, ref position, out forward);
            CreateViewRH(ref position, ref forward, ref upVector, out viewFpMatrix);
        }

        /// <summary>
        /// Creates a view matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">Target of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <returns>Look at matrix.</returns>
        public static FPMatrix CreateLookAtRH(FPVector3 position, FPVector3 target, FPVector3 upVector)
        {
            FPMatrix lookAt;
            FPVector3 forward;
            FPVector3.Subtract(ref target, ref position, out forward);
            CreateViewRH(ref position, ref forward, ref upVector, out lookAt);
            return lookAt;
        }


        /// <summary>
        /// Creates a view matrix pointing in a direction with a given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="forward">Forward direction of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <param name="viewFpMatrix">Look at matrix.</param>
        public static void CreateViewRH(ref FPVector3 position, ref FPVector3 forward, ref FPVector3 upVector, out FPMatrix viewFpMatrix)
        {
            FPVector3 z;
            Fix64 length = forward.Length();
            FPVector3.Divide(ref forward, -length, out z);
            FPVector3 x;
            FPVector3.Cross(ref upVector, ref z, out x);
            x.Normalize();
            FPVector3 y;
            FPVector3.Cross(ref z, ref x, out y);

            viewFpMatrix.M11 = x.x;
            viewFpMatrix.M12 = y.x;
            viewFpMatrix.M13 = z.x;
            viewFpMatrix.M14 = F64.C0;
            viewFpMatrix.M21 = x.y;
            viewFpMatrix.M22 = y.y;
            viewFpMatrix.M23 = z.y;
            viewFpMatrix.M24 = F64.C0;
            viewFpMatrix.M31 = x.z;
            viewFpMatrix.M32 = y.z;
            viewFpMatrix.M33 = z.z;
            viewFpMatrix.M34 = F64.C0;
            FPVector3.Dot(ref x, ref position, out viewFpMatrix.M41);
            FPVector3.Dot(ref y, ref position, out viewFpMatrix.M42);
            FPVector3.Dot(ref z, ref position, out viewFpMatrix.M43);
            viewFpMatrix.M41 = -viewFpMatrix.M41;
            viewFpMatrix.M42 = -viewFpMatrix.M42;
            viewFpMatrix.M43 = -viewFpMatrix.M43;
            viewFpMatrix.M44 = F64.C1;

        }

        /// <summary>
        /// Creates a view matrix pointing looking in a direction with a given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="forward">Forward direction of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <returns>Look at matrix.</returns>
        public static FPMatrix CreateViewRH(FPVector3 position, FPVector3 forward, FPVector3 upVector)
        {
            FPMatrix lookat;
            CreateViewRH(ref position, ref forward, ref upVector, out lookat);
            return lookat;
        }



        /// <summary>
        /// Creates a world matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the transform.</param>
        /// <param name="forward">Forward direction of the transformation.</param>
        /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
        /// <param name="worldFpMatrix">World matrix.</param>
        public static void CreateWorldRH(ref FPVector3 position, ref FPVector3 forward, ref FPVector3 upVector, out FPMatrix worldFpMatrix)
        {
            FPVector3 z;
            Fix64 length = forward.Length();
            FPVector3.Divide(ref forward, -length, out z);
            FPVector3 x;
            FPVector3.Cross(ref upVector, ref z, out x);
            x.Normalize();
            FPVector3 y;
            FPVector3.Cross(ref z, ref x, out y);

            worldFpMatrix.M11 = x.x;
            worldFpMatrix.M12 = x.y;
            worldFpMatrix.M13 = x.z;
            worldFpMatrix.M14 = F64.C0;
            worldFpMatrix.M21 = y.x;
            worldFpMatrix.M22 = y.y;
            worldFpMatrix.M23 = y.z;
            worldFpMatrix.M24 = F64.C0;
            worldFpMatrix.M31 = z.x;
            worldFpMatrix.M32 = z.y;
            worldFpMatrix.M33 = z.z;
            worldFpMatrix.M34 = F64.C0;

            worldFpMatrix.M41 = position.x;
            worldFpMatrix.M42 = position.y;
            worldFpMatrix.M43 = position.z;
            worldFpMatrix.M44 = F64.C1;

        }


        /// <summary>
        /// Creates a world matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the transform.</param>
        /// <param name="forward">Forward direction of the transformation.</param>
        /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
        /// <returns>World matrix.</returns>
        public static FPMatrix CreateWorldRH(FPVector3 position, FPVector3 forward, FPVector3 upVector)
        {
            FPMatrix lookat;
            CreateWorldRH(ref position, ref forward, ref upVector, out lookat);
            return lookat;
        }



        /// <summary>
        /// Creates a matrix representing a translation.
        /// </summary>
        /// <param name="translation">Translation to be represented by the matrix.</param>
        /// <param name="translationFpMatrix">Matrix representing the given translation.</param>
        public static void CreateTranslation(ref FPVector3 translation, out FPMatrix translationFpMatrix)
        {
            translationFpMatrix = new FPMatrix
            {
                M11 = F64.C1,
                M22 = F64.C1,
                M33 = F64.C1,
                M44 = F64.C1,
                M41 = translation.x,
                M42 = translation.y,
                M43 = translation.z
            };
        }

        /// <summary>
        /// Creates a matrix representing a translation.
        /// </summary>
        /// <param name="translation">Translation to be represented by the matrix.</param>
        /// <returns>Matrix representing the given translation.</returns>
        public static FPMatrix CreateTranslation(FPVector3 translation)
        {
            FPMatrix translationFpMatrix;
            CreateTranslation(ref translation, out translationFpMatrix);
            return translationFpMatrix;
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="scale">Scale to be represented by the matrix.</param>
        /// <param name="scaleFpMatrix">Matrix representing the given scale.</param>
        public static void CreateScale(ref FPVector3 scale, out FPMatrix scaleFpMatrix)
        {
            scaleFpMatrix = new FPMatrix
                {
                    M11 = scale.x,
                    M22 = scale.y,
                    M33 = scale.z,
                    M44 = F64.C1
			};
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="scale">Scale to be represented by the matrix.</param>
        /// <returns>Matrix representing the given scale.</returns>
        public static FPMatrix CreateScale(FPVector3 scale)
        {
            FPMatrix scaleFpMatrix;
            CreateScale(ref scale, out scaleFpMatrix);
            return scaleFpMatrix;
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="x">Scale along the x axis.</param>
        /// <param name="y">Scale along the y axis.</param>
        /// <param name="z">Scale along the z axis.</param>
        /// <param name="scaleFpMatrix">Matrix representing the given scale.</param>
        public static void CreateScale(Fix64 x, Fix64 y, Fix64 z, out FPMatrix scaleFpMatrix)
        {
            scaleFpMatrix = new FPMatrix
            {
                M11 = x,
                M22 = y,
                M33 = z,
                M44 = F64.C1
			};
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="x">Scale along the x axis.</param>
        /// <param name="y">Scale along the y axis.</param>
        /// <param name="z">Scale along the z axis.</param>
        /// <returns>Matrix representing the given scale.</returns>
        public static FPMatrix CreateScale(Fix64 x, Fix64 y, Fix64 z)
        {
            FPMatrix scaleFpMatrix;
            CreateScale(x, y, z, out scaleFpMatrix);
            return scaleFpMatrix;
        }

        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString()
        {
            return "{" + M11 + ", " + M12 + ", " + M13 + ", " + M14 + "} " +
                   "{" + M21 + ", " + M22 + ", " + M23 + ", " + M24 + "} " +
                   "{" + M31 + ", " + M32 + ", " + M33 + ", " + M34 + "} " +
                   "{" + M41 + ", " + M42 + ", " + M43 + ", " + M44 + "}";
        }
    }
}

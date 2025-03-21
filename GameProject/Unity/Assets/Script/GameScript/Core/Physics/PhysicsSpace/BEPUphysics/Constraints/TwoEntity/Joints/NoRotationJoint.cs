using System;
using BEPUphysics.Entities;
using FixedMath;
using FixMath.NET;

namespace BEPUphysics.Constraints.TwoEntity.Joints
{
    /// <summary>
    /// Constrains two entities so that they cannot rotate relative to each other.
    /// </summary>
    public class NoRotationJoint : Joint, I3DImpulseConstraintWithError, I3DJacobianConstraint
    {
        private FPVector3 accumulatedImpulse;
        private FPVector3 biasVelocity;
        private FPMatrix3x3 effectiveMassMatrix;
        private FPQuaternion _initialFpQuaternionConjugateA;
        private FPQuaternion _initialFpQuaternionConjugateB;
        private FPVector3 error;

        /// <summary>
        /// Constructs a new constraint which prevents relative angular motion between the two connected bodies.
        /// To finish the initialization, specify the connections (ConnectionA and ConnectionB) and the initial orientations
        /// (InitialOrientationA, InitialOrientationB).
        /// This constructor sets the constraint's IsActive property to false by default.
        /// </summary>
        public NoRotationJoint()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a new constraint which prevents relative angular motion between the two connected bodies.
        /// </summary>
        /// <param name="connectionA">First connection of the pair.</param>
        /// <param name="connectionB">Second connection of the pair.</param>
        public NoRotationJoint(Entity connectionA, Entity connectionB)
        {
            ConnectionA = connectionA;
            ConnectionB = connectionB;

            _initialFpQuaternionConjugateA = FPQuaternion.Conjugate(ConnectionA.orientation);
            _initialFpQuaternionConjugateB = FPQuaternion.Conjugate(ConnectionB.orientation);
        }

        /// <summary>
        /// Gets or sets the initial orientation of the first connected entity.
        /// The constraint will try to maintain the relative orientation between the initialOrientationA and initialOrientationB.
        /// </summary>
        public FPQuaternion InitialOrientationA
        {
            get { return FPQuaternion.Conjugate(_initialFpQuaternionConjugateA); }
            set { _initialFpQuaternionConjugateA = FPQuaternion.Conjugate(value); }
        }

        /// <summary>
        /// Gets or sets the initial orientation of the second connected entity.
        /// The constraint will try to maintain the relative orientation between the initialOrientationA and initialOrientationB.
        /// </summary>
        public FPQuaternion InitialOrientationB
        {
            get { return FPQuaternion.Conjugate(_initialFpQuaternionConjugateB); }
            set { _initialFpQuaternionConjugateB = FPQuaternion.Conjugate(value); }
        }

        #region I3DImpulseConstraintWithError Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        public FPVector3 RelativeVelocity
        {
            get
            {
                FPVector3 velocityDifference;
                FPVector3.Subtract(ref connectionB.angularVelocity, ref connectionA.angularVelocity, out velocityDifference);
                return velocityDifference;
            }
        }

        /// <summary>
        /// Gets the total impulse applied by this constraint.
        /// </summary>
        public FPVector3 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the current constraint error.
        /// </summary>
        public FPVector3 Error
        {
            get { return error; }
        }

        #endregion

        #region I3DJacobianConstraint Members

        /// <summary>
        /// Gets the linear jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
        /// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
        /// <param name="jacobianZ">Third linear jacobian entry for the first connected entity.</param>
        public void GetLinearJacobianA(out FPVector3 jacobianX, out FPVector3 jacobianY, out FPVector3 jacobianZ)
        {
            jacobianX = Toolbox.ZeroVector;
            jacobianY = Toolbox.ZeroVector;
            jacobianZ = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the linear jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
        /// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
        /// <param name="jacobianZ">Third linear jacobian entry for the second connected entity.</param>
        public void GetLinearJacobianB(out FPVector3 jacobianX, out FPVector3 jacobianY, out FPVector3 jacobianZ)
        {
            jacobianX = Toolbox.ZeroVector;
            jacobianY = Toolbox.ZeroVector;
            jacobianZ = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
        /// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
        /// <param name="jacobianZ">Third angular jacobian entry for the first connected entity.</param>
        public void GetAngularJacobianA(out FPVector3 jacobianX, out FPVector3 jacobianY, out FPVector3 jacobianZ)
        {
            jacobianX = Toolbox.RightVector;
            jacobianY = Toolbox.UpVector;
            jacobianZ = Toolbox.BackVector;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
        /// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
        /// <param name="jacobianZ">Third angular jacobian entry for the second connected entity.</param>
        public void GetAngularJacobianB(out FPVector3 jacobianX, out FPVector3 jacobianY, out FPVector3 jacobianZ)
        {
            jacobianX = Toolbox.RightVector;
            jacobianY = Toolbox.UpVector;
            jacobianZ = Toolbox.BackVector;
        }

        /// <summary>
        /// Gets the mass matrix of the constraint.
        /// </summary>
        /// <param name="outputMassMatrix">Constraint's mass matrix.</param>
        public void GetMassMatrix(out FPMatrix3x3 outputMassMatrix)
        {
            outputMassMatrix = effectiveMassMatrix;
        }

        #endregion

        /// <summary>
        /// Applies the corrective impulses required by the constraint.
        /// </summary>
        public override Fix64 SolveIteration()
        {
            FPVector3 velocityDifference;
            FPVector3.Subtract(ref connectionB.angularVelocity, ref connectionA.angularVelocity, out velocityDifference);
            FPVector3 softnessVector;
            FPVector3.Multiply(ref accumulatedImpulse, softness, out softnessVector);

            FPVector3 lambda;
            FPVector3.Add(ref velocityDifference, ref biasVelocity, out lambda);
            FPVector3.Subtract(ref lambda, ref softnessVector, out lambda);
            FPMatrix3x3.Transform(ref lambda, ref effectiveMassMatrix, out lambda);

            FPVector3.Add(ref lambda, ref accumulatedImpulse, out accumulatedImpulse);
            if (connectionA.isDynamic)
            {
                connectionA.ApplyAngularImpulse(ref lambda);
            }
            if (connectionB.isDynamic)
            {
                FPVector3 torqueB;
                FPVector3.Negate(ref lambda, out torqueB);
                connectionB.ApplyAngularImpulse(ref torqueB);
            }

            return Fix64.Abs(lambda.X) + Fix64.Abs(lambda.Y) + Fix64.Abs(lambda.Z);
        }

        /// <summary>
        /// Initializes the constraint for the current frame.
        /// </summary>
        /// <param name="dt">Time between frames.</param>
        public override void Update(Fix64 dt)
        {
            FPQuaternion fpQuaternionA;
            FPQuaternion.Multiply(ref connectionA.orientation, ref _initialFpQuaternionConjugateA, out fpQuaternionA);
            FPQuaternion fpQuaternionB;
            FPQuaternion.Multiply(ref connectionB.orientation, ref _initialFpQuaternionConjugateB, out fpQuaternionB);
            FPQuaternion.Conjugate(ref fpQuaternionB, out fpQuaternionB);
            FPQuaternion intermediate;
            FPQuaternion.Multiply(ref fpQuaternionA, ref fpQuaternionB, out intermediate);


            Fix64 angle;
            FPVector3 axis;
            FPQuaternion.GetAxisAngleFromQuaternion(ref intermediate, out axis, out angle);

            error.X = axis.X * angle;
            error.Y = axis.Y * angle;
            error.Z = axis.Z * angle;

            Fix64 errorReduction;
            springSettings.ComputeErrorReductionAndSoftness(dt, F64.C1 / dt, out errorReduction, out softness);
            errorReduction = -errorReduction;
            biasVelocity.X = errorReduction * error.X;
            biasVelocity.Y = errorReduction * error.Y;
            biasVelocity.Z = errorReduction * error.Z;

            //Ensure that the corrective velocity doesn't exceed the max.
            Fix64 length = biasVelocity.LengthSquared();
            if (length > maxCorrectiveVelocitySquared)
            {
                Fix64 multiplier = maxCorrectiveVelocity / Fix64.Sqrt(length);
                biasVelocity.X *= multiplier;
                biasVelocity.Y *= multiplier;
                biasVelocity.Z *= multiplier;
            }

            FPMatrix3x3.Add(ref connectionA.inertiaTensorInverse, ref connectionB.inertiaTensorInverse, out effectiveMassMatrix);
            effectiveMassMatrix.M11 += softness;
            effectiveMassMatrix.M22 += softness;
            effectiveMassMatrix.M33 += softness;
            FPMatrix3x3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);


           
        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //apply accumulated impulse
            if (connectionA.isDynamic)
            {
                connectionA.ApplyAngularImpulse(ref accumulatedImpulse);
            }
            if (connectionB.isDynamic)
            {
                FPVector3 torqueB;
                FPVector3.Negate(ref accumulatedImpulse, out torqueB);
                connectionB.ApplyAngularImpulse(ref torqueB);
            }
        } 
    }
}
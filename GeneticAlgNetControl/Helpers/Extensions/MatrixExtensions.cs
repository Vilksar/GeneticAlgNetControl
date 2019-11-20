using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.Extensions
{
    /// <summary>
    /// Provides extensions for matrices.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Checks if the given matrix is full rank or not.
        /// </summary>
        /// <param name="matrix">The matrix to check.</param>
        /// <returns>True if the matrix is full rank, false otherwise</returns>
        public static bool IsFullRank(this Matrix<double> matrix)
        {
            // Clone the given matrix.
            var clonedMatrix = matrix.Clone();
            // Check if there are more rows than columns.
            if (clonedMatrix.ColumnCount < clonedMatrix.RowCount)
            {
                // Create a new helper matrix.
                var helperMatrix = Matrix<double>.Build.Dense(clonedMatrix.RowCount, clonedMatrix.RowCount - clonedMatrix.ColumnCount);
                // Append the helper matrix to the cloned matrix.
                clonedMatrix = clonedMatrix.Append(helperMatrix);
            }
            // Transpose the matrix.
            clonedMatrix = clonedMatrix.Transpose();
            // If needed, Insert a number of all-zero columns such that to make the matrix square (only if it has less columns than rows).
            for (int index = 0; index < matrix.RowCount - matrix.ColumnCount; index++)
            {
                matrix = matrix.InsertColumn(matrix.ColumnCount, Vector<double>.Build.Dense(matrix.RowCount, 0.0));
            }
            // Check if it is full rank.
            return clonedMatrix.QR(QRMethod.Full).IsFullRank;
        }
    }
}

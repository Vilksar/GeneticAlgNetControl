using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

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
            // Check if there are more rows than columns.
            if (matrix.ColumnCount < matrix.RowCount)
            {
                // Return false, as the matrix can not have the rank equal to the number of rows.
                return false;
            }
            // Check if it is full rank.
            return matrix.Transpose().QR(QRMethod.Full).IsFullRank;
        }
    }
}

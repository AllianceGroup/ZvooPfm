using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Framework.Utils;

namespace Default.ViewModel.BusinessController.Reports
{
    public class Matrix : IMatrixRow
    {
        /// <summary>
        /// width without total column
        /// </summary>
        private readonly int _width;

        private readonly bool _needTotalColumn;

        private readonly List<IMatrixRow> _rows;

        public List<IMatrixRow> Rows
        {
            get { return _rows; }
        }

        public string Title { get; set; }

        public string Id { get; set; }

        public MatrixRow TotalRow { get; private set; }

        public Matrix(int width, string title, bool needTotalColumn)
        {
            _width = width;
            Title = title;
            _rows = new List<IMatrixRow>();
            _needTotalColumn = needTotalColumn;
        }

        public int GetWidthWithTotalColumn()
        {
            return _needTotalColumn ? _width + 1 : _width;
        }

        public void AddRow(string id, string title, params RowValue[] values)
        {
            if (values.Count() != _width)
                throw new ArgumentException("Invalid values count");

            var row = new MatrixRow(GetWidthWithTotalColumn()) { Id = id, Title = title };
            row.AddValues(values);
            var ranges = values.Select(x => x.Range);
            if (_needTotalColumn)
            {
                row[values.Length] = new RowValue();
                row[values.Length].Amount = row.GetTotal();
                row[values.Length].Range = new DateRange(ranges.Min(x => x.From), ranges.Max(x => x.To));
            }

            _rows.Add(row);
        }

        public void AddRow(Matrix matrix)
        {
            if (matrix.TotalRow == null)
                throw new ArgumentException("You should calculate TotalRow before add sub matrix");

            _rows.Add(matrix);
        }

        public void AddRow(MatrixRow row)
        {
            _rows.Add(row);
        }

        public void CalculateTotalRow(string id, string title, List<DateRange> ranges)
        {
            TotalRow = new MatrixRow(GetWidthWithTotalColumn());
            TotalRow.Id = id;
            TotalRow.Title = title;

            for (int i = 0; i < _width; i++)
            {
                TotalRow[i] = new RowValue();
                foreach (var row in _rows)
                {
                    TotalRow[i].Amount += row[i].Amount;
                }
                if (ranges != null && ranges.Count > 0)
                    TotalRow[i].Range = ranges[i];
            }

            if (_needTotalColumn)
            {
                TotalRow[_width] = new RowValue();
                TotalRow[_width].Amount = TotalRow.GetTotal();
                if (ranges != null && ranges.Count > 0)
                    TotalRow[_width].Range = new DateRange(ranges.Min(x => x.From), ranges.Max(x => x.To));
            }
        }

        public RowValue this[int index]
        {
            get { return TotalRow[index]; }
            set { TotalRow[index] = value; }
        }

        public bool IsMatrix
        {
            get { return true; }
        }

        public bool IsZeroBalanceMatrix()
        {
            return TotalRow.GetTotal() == 0;
        }
    }

    public interface IMatrixRow
    {
        RowValue this[int index] { get; set; }

        string Title { get; set; }

        string Id { get; set; }

        bool IsMatrix { get; }
    }

    public class MatrixRow : IMatrixRow
    {
        /// <summary>
        /// Calculate difference between rows
        /// </summary>
        /// <returns>Empty row, just with calculated difference in amounts</returns>
        public static MatrixRow operator -(MatrixRow first, MatrixRow second)
        {
            if (first.Width != second.Width)
                throw new ArgumentException("Matrix Rows length should be equal");
            var width = first.Width;

            var rowValues = new RowValue[width];

            for (int i = 0; i < width; i++)
            {
                rowValues[i] = new RowValue { Amount = first[i].Amount - second[i].Amount };
            }

            var result = new MatrixRow(width);
            result.AddValues(rowValues);

            return result;
        }

        public int Width
        {
            get { return _values.Count(); }
        }

        public MatrixRow(int width)
        {
            _values = new RowValue[width];
        }

        public string Id { get; set; }

        public bool IsMatrix
        {
            get { return false; }
        }

        public string Title { get; set; }

        public readonly RowValue[] _values;

        public RowValue this[int index]
        {
            get { return _values[index]; }
            set { _values[index] = value; }
        }

        public void AddValues(params RowValue[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                this[i] = values[i];
            }
        }

        public long GetTotal()
        {
            return _values.Sum(x => x.Amount);
        }
    }

    public class RowValue
    {
        public long Amount { get; set; }

        public DateRange Range { get; set; }
    }
}

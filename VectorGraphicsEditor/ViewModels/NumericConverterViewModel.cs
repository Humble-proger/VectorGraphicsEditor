using System;
using System.Collections.Generic;
using System.Globalization;

using Avalonia.Data.Converters;

namespace GraphicsApp.ViewModels
{
    public class NumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // �������������� �� �������� � ����� (OneWay)
            return value?.ToString() ?? string.Empty;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // �������������� �� ������ � �������� (TwoWay)
            string strValue = value as string ?? string.Empty;
            if (string.IsNullOrEmpty(strValue))
                return 0.0;

            // �������� ������������� � �����
            if (double.TryParse(strValue, out double numericValue))
            {
                return numericValue;
            }

            // ���� �� �����, ���������� null ��� ������� ��������
            return 0.0;
        }
    }

    public class NumericConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int result))
                return result;
            return 0;
        }
    }
}
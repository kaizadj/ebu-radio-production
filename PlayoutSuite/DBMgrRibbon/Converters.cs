using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using LogListPrj;
using DS;

namespace DBMgrRibbon
{
    public class DataConverter
    {


        public static String timespantostr(TimeSpan t)
        {
            String ret = "";
            if (Math.Abs(t.Hours) != 0)
                ret += Math.Abs(t.Hours) + ":";
            if (Math.Abs(t.Minutes) < 10)
                ret += "0";

            ret += Math.Abs(t.Minutes) + ":";

            if (Math.Abs(t.Seconds) < 10)
                ret += "0";

            ret += Math.Abs(t.Seconds) + "." + Math.Abs(t.Milliseconds).ToString().Substring(0, 1);
            return ret;
        }
    }
    public class CellValueColorConverter : IMultiValueConverter
    {

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DS.SlotStatus val = DS.SlotStatus.ERROR;
            if (values[0] != DependencyProperty.UnsetValue)
            {//SLICE
                val = (DS.SlotStatus)values[1];
            }
            else if (values[2] != DependencyProperty.UnsetValue)
            {
                val = (DS.SlotStatus)values[3];
            }

            if (val == DS.SlotStatus.WAITING)
            {
                return Brushes.Cyan;
            }
            else if (val == DS.SlotStatus.ERROR)
            {
                return Brushes.Red;
            }
            else if (val == DS.SlotStatus.SKIPPED)
            {
                return Brushes.Maroon;
            }
            else if (val == DS.SlotStatus.ONAIR)
            {
                return Brushes.LimeGreen;
            }
            else if (val == DS.SlotStatus.READY)
            {
                return Brushes.Yellow;
            }

            else
            {
                return Brushes.Gray;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class CellStringConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return values[0] + " " + values[1];
            if (values[0] != DependencyProperty.UnsetValue)
            {//SLICE
                return values[1].ToString();
            }
            else if (values[2] != DependencyProperty.UnsetValue)
            {
                return values[3].ToString();
            }
            else
            {
                return "???";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GapSchedConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {


            if (values[1] != DependencyProperty.UnsetValue)
            {//SLICE
                if (values[1] is TimeSpan && values[2] is TimeSpan)
                {
                    TimeSpan t = (TimeSpan)values[2];
                    String sign = (t.CompareTo(TimeSpan.Zero) == -1)? "- " :"+ ";
                    
                    TimeSpan t2 = (TimeSpan)values[1];
                    String sign2 = (t2.CompareTo(TimeSpan.Zero) == -1)? "- " :"+ ";
                    return sign + DataConverter.timespantostr(t) + " / " + sign2 + DataConverter.timespantostr(t2);
                }
            }

            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CellTimeConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {


            if (values[0] != DependencyProperty.UnsetValue)
            {
                if (values[1] is TimeSpan)
                    return DataConverter.timespantostr((TimeSpan)values[1]);
                else if (values[1] is DateTime)
                    return ((DateTime)values[1]).ToLongTimeString();
            }
            else if (values[2] != DependencyProperty.UnsetValue)
            {
                if (values[3] is TimeSpan)
                    return DataConverter.timespantostr((TimeSpan)values[3]);
                else if (values[3] is DateTime)
                    return ((DateTime)values[3]).ToLongTimeString();
            }

            return "???";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class RowBackgroundConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush background = Brushes.White;
            String type = values[0].ToString();

            if (type == "timemarkerhard")
            {
                background = Brushes.Red;
            }
            else if (type == "timemarkersoft")
            {
                background = Brushes.Yellow;
            }
            else if (type == "slice")
            {
                background = Brushes.Black;
                // e.Row.Foreground = Brushes.White;
            }
            else if (type == "warning")
            {
                background = Brushes.OrangeRed;
                //e.Row.Foreground = Brushes.White;
            }
            else if (values[0] != DependencyProperty.UnsetValue)
            {
                SlotItem slot = ((SlotItem)values[1]);
                if (slot.cutsong)
                {
                    background = Brushes.Cyan;
                }
                else if (slot.fillsong)
                {
                    background = Brushes.Blue;
                }


                SlotStatus status = slot.status;
                switch (status)
                {
                    case SlotStatus.PLAYED:
                        background = Brushes.SlateGray;
                        //e.Row.Foreground = Brushes.White;
                        break;
                    case SlotStatus.ONAIR:
                        background = Brushes.Green;
                        //e.Row.Foreground = Brushes.White;
                        break;
                    case SlotStatus.ERROR:
                        background = Brushes.Chocolate;
                        //e.Row.Foreground = Brushes.White;
                        break;
                }

            }

            return background;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
    public class RowForegroundConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush foreground = Brushes.Black;
            String type = values[0].ToString();

            if (type == "timemarkerhard")
            {
                foreground = Brushes.White;
            }
            else if (type == "timemarkersoft")
            {
            }
            else if (type == "slice")
            {
                foreground = Brushes.White;
            }
            else if (type == "warning")
            {
                foreground = Brushes.White;
            }
            else if (values[0] != DependencyProperty.UnsetValue)
            {
                SlotItem slot = ((SlotItem)values[1]);
                if (slot.cutsong)
                {
                }
                else if (slot.fillsong)
                {
                }

                SlotStatus status = slot.status;
                switch (status)
                {
                    case SlotStatus.PLAYED:
                        foreground = Brushes.White;
                        break;
                    case SlotStatus.ONAIR:
                        foreground = Brushes.White;
                        break;
                    case SlotStatus.ERROR:
                        foreground = Brushes.White;
                        break;
                }
            }

            return foreground;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}

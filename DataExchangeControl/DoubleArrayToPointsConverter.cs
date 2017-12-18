using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FileChangeChecker
{
    /// <summary>
    /// Value converter used for databinding Polyline.Points to
    /// a double[].
    /// </summary>
    //[ValueConversion(typeof(double[]), typeof(PointCollection))]
    //class DoubleArrayToPointsConverter : IValueConverter
    //{
    //    #region Fields

    //    /// <summary>
    //    /// Width property value
    //    /// </summary>
    //    private double widthValue = 100;


    //    /// <summary>
    //    /// Scale property value
    //    /// </summary>
    //    private double scaleValue = 1;


    //    /// <summary>
    //    /// Offset property value.
    //    /// </summary>
    //    private double offsetValue;

    //    #endregion

    //    #region Properties

    //    /// <summary>
    //    /// Width that the generated points collection should span.
    //    /// </summary>
    //    public double Width
    //    {
    //        get { return widthValue; }
    //        set { widthValue = value; }
    //    }


    //    /// <summary>
    //    /// Factor by which to scale Y values in generated points.
    //    /// </summary>
    //    public double Scale
    //    {
    //        get { return scaleValue; }
    //        set { scaleValue = value; }
    //    }

 
    //    /// <summary>
    //    /// Distance by which to offset Y values in generated points.
    //    /// </summary>
    //    public double Offset
    //    {
    //        get { return offsetValue; }
    //        set { offsetValue = value; }
    //    }

    //    #endregion

    //    #region IValueConverter

    //    /// <summary>
    //    /// Called by WPF to convert a double[] into a PointCollection
    //    /// </summary>
    //    /// <param name="value">Input value - must be a double[]</param>
    //    /// <returns>A PointCollection containing points based on the input double[],
    //    /// sized and positioned based on the Width, Scale, and Offset properties.</returns>
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        double[] values = value as double[];
    //        if (values == null)
    //        {
    //            throw new ArgumentException("value", "Must be double[]");
    //        }

    //        PointCollection points = new PointCollection(values.Length);
    //        for (int i = 0; i < values.Length; ++i)
    //        {
    //             double x = i * Width / values.Length;
    //             double y = 0;
    //            if ((i >= 1) && (i<=17))
    //            {  y = -1*i*3; }
    //            else
    //            {    
    //                 y = values[i] * Scale + Offset;
    //            }
    //            points.Add(new Point(x, y));

    //        }

    //        return points;
    //    }

    //    /// <summary>
    //    /// Not implemented - required by IValueConverter, but not used because
    //    /// we only support one-way bindings.
    //    /// </summary>
    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new Exception("The method or operation is not implemented.");
    //    }

    //    #endregion
    //}

    class HarmonicSeries : INotifyPropertyChanged
    {
        #region Fields


        /// <summary>
        /// Value of Outputs property
        /// </summary>
        private double[] outputValues;

        #endregion

        #region Constructor


        public HarmonicSeries()
        {
            int numberOfOutputValues = 50;
            outputValues = new double[numberOfOutputValues];
            //this.Test1.PropertyChanged += new PropertyChangedEventHandler(HarmonicSeries_PropertyChanged);
        }

        #endregion

        #region Change handling

        void HarmonicSeries_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Amplitudes");
            UpdateValues();
        }

        #endregion

        #region Properties


        /// <summary>
        /// The output waveform, generated from the current values in
        /// the Amplitudes collection.
        /// </summary>
        public double[] Output
        {
            get { return outputValues; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised whenever a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private methods


        /// <summary>
        /// Recalculate the Outputs based on the current Amplitudes.
        /// </summary>
        /// 
        //в данном методе создаются изгибы
        private void UpdateValues()
        {

            outputValues = new double[4];
            outputValues[0] = 1;
            outputValues[1] = 2;
            OnPropertyChanged("Output");
        }


        /// <summary>
        /// Raise a property change notification.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}

using RetroGameHandler.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace RetroGameHandler.Converter
{
    public static class ObjectConverter
    {
        public static void Convert(object objTo, object objFrom, bool SetToDefaultValue = false, string DateTimeFormat = null, IFormatProvider provider = null)
        {
            var typeTo = objTo.GetType();
            var typeObj = objFrom.GetType();
            foreach (var property in typeTo.GetProperties())
            {
                var tProperty = property.GetType();
                var objProp = typeObj.GetProperty(property.Name);
                if (objProp == null)
                {
                    if (SetToDefaultValue) property.SetValue(objTo, default);
                    continue;
                }
                if (tProperty.IsClass && typeof(IConvert).IsAssignableFrom(tProperty))
                {
                    var iConv = (IConvert)property;
                    iConv.Convert(typeObj);
                }
                else if (tProperty == objProp.GetType())
                {
                    property.SetValue(objTo, objProp.GetValue(objFrom));
                }
                else if (tProperty == typeof(string))
                {
                    var val = objProp.GetType() ==
                        typeof(DateTime) ?
                        ((DateTime)objProp.GetValue(objFrom)).ToString(DateTimeFormat, (provider ?? CultureInfo.CurrentCulture)) :
                        objProp.GetValue(objFrom).ToString();
                    property.SetValue(objTo, val);
                }
            }
        }
    }
}
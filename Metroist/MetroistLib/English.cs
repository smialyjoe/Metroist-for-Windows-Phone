using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace MetroistLib
{
    [KnownType(typeof(English))]
    [DataContract]
    public class English : Language
    {
        public string Error_ERROR_WRONG_DATE_SYNTAX()
        {
            return "Formato inválido para data/hora. Tente novamente.";
        }
    }
}

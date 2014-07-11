using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace GeneralLib
{
    public class Service
    {
        Dictionary<string, object> args;
        Action<object> handler;

        public void request(Uri uri, Dictionary<string, object> args, Action<object> handler, String requestType = "GET")
        {
            this.args = args;
            this.handler = handler;

            string requestUrl = requestType == "GET" ? uri.AbsoluteUri + "?" + DictionaryToString(args) : uri.AbsoluteUri;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                httpWebRequest.Method = requestType;

                Debug.WriteLine("Metroist [{0}] {1}?{2}", requestType, uri.AbsoluteUri, DictionaryToString(args));

                if (requestType == "POST")
                {
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    httpWebRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), httpWebRequest);
                }
                else
                {
                    httpWebRequest.BeginGetResponse(new AsyncCallback(ReadWebRequestCallBack), httpWebRequest);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream postStream = request.EndGetRequestStream(asynchronousResult);
            string postData = DictionaryToString(args);
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallBack), request);
        }

        void ReadWebRequestCallBack(IAsyncResult asynchronousResult)
        {
            String response = "";
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            try
            {
                WebResponse myResponse = (WebResponse)request.EndGetResponse(asynchronousResult);
                Stream encodingStream = myResponse.GetResponseStream();
                Encoding encode = Encoding.GetEncoding("iso-8859-1");
                using (StreamReader httpwebStreamReader = new StreamReader(encodingStream, encode))
                    response = httpwebStreamReader.ReadToEnd();
                myResponse.Close();

                Deployment.Current.Dispatcher.BeginInvoke(()=>
                {
                    handler(response);
                });
            }
            catch (WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    handler(e);
                });
            }
            
        }

        public void cancel()
        {
        }

        public string DictionaryToString(Dictionary<string, object> dictionary, Boolean needsEncode = true)
        {
            if (dictionary == null)
                return string.Empty;

            StringBuilder postData = new StringBuilder();
            foreach (var keyVal in dictionary)
            {
                var value = !needsEncode ? keyVal.Value.ToString() : HttpUtility.UrlEncode(keyVal.Value.ToString());
                postData.AppendFormat("{0}={1}", keyVal.Key, value);
                postData.AppendFormat("&");
            }
            postData.Remove(postData.Length - 1, 1);
            return postData.ToString();
        }

    }
}

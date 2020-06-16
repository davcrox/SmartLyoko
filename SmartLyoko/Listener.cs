using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using LyokoAPI.Events;
using LyokoAPI.Plugin;
using LyokoAPI.VirtualStructures;
using LyokoAPI.VirtualStructures.Interfaces;
using System.IO;

namespace SmartLyoko
{
    // static class to hold global variables, etc.
    static class Globals
    {
        // global int http://localhost:64195/
        public static string domain = "http://**********/";
        // global function
        public static string HelloWorld()
        {
            return "Hello World";
        }
    }
    public static class Listener
    {
        static System.Net.WebClient client = new System.Net.WebClient();

        public static bool _listening = false;
        /*
        * These variables are needed to unregister events later.
        * We dont really recommmend this in this situation
        */
        private static void onTowerdeactivation(ITower tower)
        {
            LyokoAPI.Events.LyokoLogger.Log("Test", "Tour désactivé");
            string json = "{\"secteur\": \""+ tower.Sector.Name + "\",\"nombre\": \""+ tower.Number + "\"}";
            sendEvent("tourdesactiver", json);
        }
        private static void onXanaAwaken()
        {
            LyokoAPI.Events.LyokoLogger.Log("Test", "Reveil de Xana");
            string json = "{\"xana\": \"true\"}";
            sendEvent("reveilxana", json);
        }
        private static void onHijack(ITower tower, APIActivator old, APIActivator newactivator)
        {
            LyokoAPI.Events.LyokoLogger.Log("Test", "Hijack");
            string json = "{\"secteur\": \"" + tower.Sector.Name + "\",\"nombre\": \"" + tower.Number + "\",\"old\": \"" + getActivator(old) + "\",\"new\": \"" + getActivator(newactivator) + "\"}";
            sendEvent("hijack", json);
        }

        public static void StartListening()
        {
            
            client.Headers.Add("content-type", "application/json");//set your header here, you can add multiple headers
            sendEvent("lyokoconnect", "{\"user\":\"davcrox\"}");
            if (_listening) { return; } //dont do anything if we're already listening
            LyokoAPI.Events.LyokoLogger.Log("Test", "Subscribe");
            TowerActivationEvent.Subscribe(OnTowerActivation); //Give the method name without '()'.
            XanaDefeatEvent.Subscribe(DoAThing); //the name of the method doesn't matter as long as the return value and parameters are the same

            TowerDeactivationEvent.Subscribe(onTowerdeactivation); //single statement lambda with one parameter

            XanaAwakenEvent.Subscribe(onXanaAwaken); //single statement lambda with no parameters
            TowerHijackEvent.Subscribe(onHijack);
            /*TowerHijackEvent.Subscribe((tower, oldactivator, newactivator) => //multi statement lambda with 3 parameters
            {
                //SoundPlayer.PlaySound(Sounds.Thief);
                Console.WriteLine("subscribe HIJACK");
                OnTowerActivation(tower); //you can re-use methods if you want, they're still methods.
            });*/
            _listening = true;
        }
        public static void StopListening()
        {
            if (!_listening) { return; } //dont stop listening if we've already stopped (unregistering events that haven't been registered is harmless though)
            TowerActivationEvent.Unsubscribe(OnTowerActivation);
            XanaDefeatEvent.Unsubscribe(DoAThing);
            sendEvent("lyokodisconnect", "{\"user\":\"davcrox\"}");
            /*
            * these unregister the listeners by using the delegate returned by Subscribe()
            */
            TowerDeactivationEvent.Unsubscribe(onTowerdeactivation);
            XanaAwakenEvent.Unsubscribe(onXanaAwaken);
            TowerHijackEvent.Unsubscribe(onHijack);

        }

        private static void OnTowerActivation(ITower tower)
        {
            LyokoAPI.Events.LyokoLogger.Log("Test", "OnTowerActivation");
            // LyokoAPI.Events.LyokoLogger.Log("Test", tower.Activator);
            switch (tower.Activator)
            {
                case APIActivator.XANA: LyokoAPI.Events.LyokoLogger.Log("Test", "XANA"); break;
                case APIActivator.JEREMIE: LyokoAPI.Events.LyokoLogger.Log("Test", "JEREMIE"); break;
                case APIActivator.HOPPER: LyokoAPI.Events.LyokoLogger.Log("Test", "HOPPER"); break;
            }
            //KeyboardRGB.SetColor(color); //fictional KeyboardRGB class
            LyokoAPI.Events.LyokoLogger.Log("Test", "Tour activé");
            string json = "{\"secteur\": \"" + tower.Sector.Name + "\",\"nombre\": \"" + tower.Number + "\",\"activateur\": \"" + getActivator(tower.Activator)+ "\"}";
            sendEvent("touractiver", json);
        }
        private static string getActivator(APIActivator activator)
        {
            switch (activator)
            {
                case APIActivator.XANA: return "XANA"; break;
                case APIActivator.JEREMIE: return "JEREMIE"; break;
                case APIActivator.HOPPER: return "HOPPER"; break;
            }
            return "ERROR";
        }

        private static void DoAThing()
        {
            LyokoLogger.Log("ExamplePlugin", "I did a thing!");
        }
        private static void sendEvent(string type,string json)
        {
            LyokoLogger.Log("POST", json);
            //string s = Encoding.ASCII.GetString(client.UploadData(Globals.domain + "/"+type, "POST", Encoding.Default.GetBytes(json)));
            WebRequest request = WebRequest.Create(Globals.domain + type);
            request.Method = "POST";
            string postData = json;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;


            //Here is the Business end of the code...
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            //and here is the response.
            WebResponse response = request.GetResponse();

            LyokoLogger.Log("GOT", ((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            //Console.WriteLine(responseFromServer);
            reader.Close();
            dataStream.Close();
            response.Close();
            LyokoLogger.Log("GOT", responseFromServer);
        }   



    }
}

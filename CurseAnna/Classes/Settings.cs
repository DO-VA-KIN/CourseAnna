using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CurseAnna.Classes
{
    public struct SettingsS
    {
        public UInt16 HorseCount;
        public UInt16 MaxSpeed;
        public UInt16 MinSpeed;        
        public List<UInt32> Bets;
        public long Balance;
    }
    public class Settings
    {
        private static readonly string FileName = Environment.CurrentDirectory + @"\Settings.xml";
        public SettingsS Values;
        
        public Settings() 
        {
            if(!Read())
                SetDefault();
        }


        public bool Read()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(SettingsS));
                using (FileStream fs = new FileStream(FileName, FileMode.Open))
                    Values = (SettingsS)ser.Deserialize(fs);
                return true;
            }
            catch { return false; }
        }


        public bool Save()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(SettingsS));
                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                    ser.Serialize(fs, Values);
                return true;
            }
            catch { return false; }
        }


        public void SetDefault()
        {
            Values = new SettingsS
            {
                HorseCount = 5,
                MaxSpeed = 20,
                MinSpeed = 10,
                Bets = new List<UInt32>()
                {
                    100,
                    200,
                    500,
                    1000,
                    2500,
                    5000,
                },
                Balance = 20000,
            };
        }
    }

}

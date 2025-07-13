
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkUnitFrameWork.LowLayer;

namespace MarkUnitFrameWork
{
    #region Interface
   

    internal class MarkUnitSettingsEntity:IMarkUnitSettingsEntity
    {
        readonly KeyValuePair<string, object>[] keys = new KeyValuePair<string, object>[]
        {
#if DEBUG
            new KeyValuePair<string, object>("Host", @"https://markirovka.sandbox.crptech.ru"),
#else
            new KeyValuePair<string, object>("Host", @"https://sdn.crpt.ru"),
#endif
            new KeyValuePair<string, object>("TimeOutUPDSDN", 30),
            new KeyValuePair<string, object>("TimeOutBlistSDN", 15),
            new KeyValuePair<string, object>("ActiveLocalModule", false),
            new KeyValuePair<string, object>("CountSDNConnetcFail", 2),
            new KeyValuePair<string, object>("X-API-KEY", ""),
        };
        private Dictionary<string, object> values;
        public MarkUnitSettingsEntity()
        {
            values = new Dictionary<string, object>();
            foreach (var key in keys)
            {               
                values.Add(key.Key, key.Value);
            }
        }
        public void Initial(Dictionary<string, object> valuePairs)
        {
            if (valuePairs == null || valuePairs.Count == 0)
                throw new ArgumentException("Value pairs cannot be null or empty");
            
            values = valuePairs;
        }
        public void Set(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty");
            values[key] = value;
        }
        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty");
            return values.ContainsKey(key) ? values[key] : null;
        }
        public bool Exist(string key)
        {
            return values.ContainsKey(key);
        }
        public bool Initialed()
        {           
            return validate();
        }
        bool validate()
        {
            bool flag = values != null && values.Count > 0;
            foreach (var keypair in keys)
            {
                if (!values.ContainsKey(keypair.Key)) return false;
            }
            return flag;
        }

        public Dictionary<string, object> GetValues()
        {
            return new Dictionary<string, object>(values);
        }
        public Dictionary<string, object> GetValuesDefault()
        {
            var defDictonary = new Dictionary<string, object>(values);
            foreach (var key in keys)
            {
                defDictonary.Add(key.Key, key.Value);
            }
            return defDictonary;
        }
    }

    
#endregion

    #region Models
    public class MarUnitBuilder
    {
        IMarkUnitSettingsEntity setting;
        public MarUnitBuilder(IMarkUnitSettingsEntity settingsEntity)
        {
            setting = settingsEntity;            
        }
        public IMarkUnitEntity markUnitBuild()
        {
           return new RREngene(setting);
        }
       

    }


    public class MarkUnitSettingBuilder
    {
        IMarkUnitSettingsEntity MarkUnitSettings;
        public MarkUnitSettingBuilder()
        {
            MarkUnitSettings = new MarkUnitSettingsEntity();
        }
        public MarkUnitSettingBuilder(IDictionary<string,object> valuePairs):this()
        {
            MarkUnitSettings.Initial(valuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        public void AddSetting(KeyValuePair<string, object> add)
        {            
            AddSetting(add.Key, add.Value);
        }
        public void AddSetting(KeyValuePair<string, object>[] add)
        {
            foreach (var item in add)
            {
                AddSetting(item);
            }
        }
        public void AddSetting(string key, object value)
        {
            if (MarkUnitSettings.Exist(key))
                throw new ArgumentException($"Key {key} already exists in settings.");
            MarkUnitSettings.Set(key, value);
        }
        public void InitialSettingTable(Dictionary<string, object> valuePairs)
        {
            MarkUnitSettings.Initial(valuePairs);
        }
        public Dictionary<string, object> GetSettingsValues()
        {
            return MarkUnitSettings.GetValues();
        }

        public IMarkUnitSettingsEntity markUnitSettingsBuild()
        {
            if(!MarkUnitSettings.Initialed())
                throw new ExeptonInitSetting("MarkUnitSettings not initialed correctly, check keys and values");
            return MarkUnitSettings;
        }
    }
    #endregion


    #region Exeptoin
    public class ExeptonInitSetting : Exception
    {
        public ExeptonInitSetting(string message) : base(message)
        {
        }
    }

    #endregion

}

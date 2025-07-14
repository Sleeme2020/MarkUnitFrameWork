using MarkUnitFrameWork;
using MarkUnitFrameWork.LowLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        static char GS = '\u001d';
        static string[] mark = new string[] {
        @"0104610011500039215QIsk6Sto:Wfx"+GS+"93j6RM",
        @"0104610011500039215Q?jgeg""Qhehs"+GS+"93J8eg"
        };
        static void Main(string[] args)
        {
            MarkUnitSettingBuilder markUnitSettingBuilder = new MarkUnitSettingBuilder();
            IMarkUnitSettingsEntity markUnitSettingsEntity = markUnitSettingBuilder.markUnitSettingsBuild();
            markUnitSettingsEntity.Set("X-API-KEY", @"79d58850-04fb-41cd-b4b8-58c0c3af6746");

            ////ПРИМЕР!!! список сдн при запуске из бд если им больше 2-3 ч. можно пропускать.
            //List<string> sdnlist = new List<string>() { 
            //"host1",
            //"host2"
            
            //};
            ////запись в настройки
            //markUnitSettingsEntity.Set("SDNListInit", sdnlist);
            //// дата последнего обновления
            //markUnitSettingsEntity.Set("DateExeptionSDN", DateTime.Now); 

            MarUnitBuilder marUnitBuilder = new MarUnitBuilder(markUnitSettingsEntity);
            IMarkUnitEntity markUnit = marUnitBuilder.markUnitBuild();

            ////ПРИМЕР!! получаем список сдн хостов после запуска движка(успешного)
            //var sdnlst = markUnit.GetSDN();
            ////дата последнего апгрейда в движке
            //var lstupddatesdn = markUnit.GetSDNDate(); 

            var send = new SenderRR(mark);

            var result = markUnit.GetSdnOnline(send);

        }
    }
}

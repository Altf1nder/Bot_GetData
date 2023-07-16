using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace UniqueArtwork
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth;
            Console.Write("   ______ _     _ _______  ______\r\n |  ____ |     | |_____| |_____/\r\n |_____| |_____| |     | |    \\_\r\n");
            //can be replaced with LokiPoe.Me.League
            Console.WriteLine("Unique art filename list. Enter league name (i.e. Archnemesis).\r\nThis doesn't work for replica uniques as they have the same art");


            string league = Console.ReadLine();
            Console.WriteLine("To get art name from worldItem: \r\nworldItem.Item.RenderArt\r\nComes in a from of \"Art/2DItems/Belts/ElderBelt.dds\"");
            List<string> ninjaUrlsList = new List<string>()
                        {
                                $"https://poe.ninja/api/data/ItemOverview?league={league}&type=UniqueArmour&language=en",
                                $"https://poe.ninja/api/data/ItemOverview?league={league}&type=UniqueAccessory&language=en",
                                $"https://poe.ninja/api/data/ItemOverview?league={league}&type=UniqueFlask&language=en",
                                $"https://poe.ninja/api/data/ItemOverview?league={league}&type=UniqueJewel&language=en",
                                $"https://poe.ninja/api/data/ItemOverview?league={league}&type=UniqueWeapon&language=en"
                        };

            //get ninja uniques information, exclude replicas
            List<ItemJson> items;
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    List<ItemJson> itemslist = new List<ItemJson>();
                    foreach (string url in ninjaUrlsList)
                    {
                        List<ItemJson> tempItems = Deserialize<ItemJson>(client, url);

                        itemslist.AddRange(tempItems.Where(x => !x.Name.Contains("Replica")));
                    }

                    items = itemslist;
                }
            }
            catch
            {
                return;
            }

            items = new List<ItemJson>(items.OrderByDescending(e => e.ExaltedValue).Where(c => c.ChaosValue >= 20.0));

            int k = 0;

            Console.WriteLine($"Total uniques: {items.Count()}");
            foreach (var item in items)
            {
                Console.WriteLine($"{k} {item.ToString()} ");
                k++;
            }

            Console.ReadLine();
        }

        private static List<T> Deserialize<T>(WebClient c, string url)
        {
            string download = c.DownloadString(url);
            return JsonConvert.DeserializeObject<NinjaLines<T>>(download)?.Lines;
        }

        internal class NinjaLines<T>
        {
            public List<T> Lines { get; set; }
        }

        public class ItemJson
        {
            private string _icon;
            public string Name { get; set; }
            public int Links { get; set; }
            public double ChaosValue { get; set; }
            public double ExaltedValue { get; set; }

            public string Icon
            {
                get => _icon;
                set
                {
                    var index = value.LastIndexOf('/');
                    var ico = index != -1 ? value.Substring(value.LastIndexOf('/')).Replace("/", "").Replace(".png", "") : value;
                    _icon = ico;
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{Name}, ");
                if (Links != 0)
                {
                    sb.Append($"[{Links}L], ");
                }

                sb.Append($":\t\t{ChaosValue}c/{ExaltedValue}ex\t\t");
                sb.Append($" {Icon}");
                return sb.ToString();
            }

        }
    }
}
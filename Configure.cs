using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OrangeBot {

    /// <summary>
    /// 設定クラス
    /// </summary>
    public class Configure {
        private Dictionary<string, string> Settings = new Dictionary<string, string>();

        /// <summary>
        /// 設定情報を読み込み、配列に格納します。
        /// </summary>
        public Configure(string dir) {
            var lines = File.ReadAllLines(dir);
            var data = lines.Select(x => x.Split(new char[] { '=' }, 2));
            Settings = data.ToDictionary(x => x[0], x => x[1]);
        }

        /// <summary>
        /// 設定の読込
        /// </summary>
        /// <param name="key">読み込みたい項目のキー</param>
        /// <returns>value</returns>
        public string Get(string key) {
            if (Settings.ContainsKey(key)) {
                return Settings
                    .Where(x => x.Key == key)
                    .Select(x => x.Value)
                    .FirstOrDefault();
            }
            else {
                return "";
            }
        }

        /// <summary>
        /// 設定の編集
        /// </summary>
        /// <param name="key">設定したい項目のキー</param>
        /// <param name="value">変更後の値</param>
        public void Set(string key, string value) {
            if (Settings.ContainsKey(key)) {
                if (value.Contains("\r\n")) {
                    Settings[key] = value.Replace("\r\n", "\\r\\n");
                }
                else {
                    Settings[key] = value;
                }
            }
            else {
                if (value.Contains("\r\n")) {
                    Settings.Add(key, value.Replace("\r\n", "\\r\\n"));
                }
                else {
                    Settings.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        public void Save() {
            using (var stream = new FileStream(Global.iniPath, FileMode.Create, FileAccess.Write)) {
                using (var sw = new StreamWriter(stream, Encoding.UTF8)) {
                    foreach (var item in Settings) {
                        sw.WriteLine("{0}={1}", item.Key, item.Value);
                    }
                }
            }
        }
    }
}
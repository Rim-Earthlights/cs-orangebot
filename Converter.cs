using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeBot {
    internal class Converter {
        private static readonly Dictionary<string, string> description = new() {
            { "thunderstorm with light rain", "弱い雷雨" },
            { "thunderstorm with rain", "雷雨" },
            { "thunderstorm with heavy rain", "強い雷雨" },
            { "light thunderstorm", "弱い雷" },
            { "thunderstorm", "雷" },
            { "heavy thunderstorm", "強い雷" },
            { "ragged thunderstorm", "ときどき雷" },
            { "thunderstorm with light drizzle", "弱い霧雨を伴う雷" },
            { "thunderstorm with drizzle", "霧雨を伴う雷" },
            { "thunderstorm with heavy drizzle", "強い霧雨を伴う雷" },
            { "light intensity drizzle", "弱い霧雨" },
            { "drizzle", "霧雨" },
            { "heavy intensity drizzle", "強い霧雨" },
            { "light intensity drizzle rain", "弱い雨と霧雨" },
            { "drizzle rain", "雨と霧雨" },
            { "heavy intensity drizzle rain", "強い雨と霧雨" },
            { "shower rain and drizzle", "にわか雨と霧雨" },
            { "heavy shower rain and drizzle", "強いにわか雨と霧雨" },
            { "shower drizzle", "にわか霧雨" },
            { "light rain", "小雨" },
            { "moderate rain", "雨" },
            { "heavy intensity rain", "強い雨" },
            { "very heavy rain", "非常に激しい雨" },
            { "extreme rain", "猛烈な雨" },
            { "freezing rain", "雨氷" },
            { "light intensity shower rain", "弱いにわか雨" },
            { "shower rain", "にわか雨" },
            { "heavy intensity shower rain", "強いにわか雨" },
            { "ragged shower rain", "ときどきにわか雨" },
            { "proximity shower rain", "すぐ近くでにわか雨" },
            { "light snow", "小雪" },
            { "snow", "雪" },
            { "heavy snow", "大雪" },
            { "sleet", "凍雨" },
            { "shower sleet", "にわか凍雨" },
            { "light rain and snow", "弱いみぞれ" },
            { "rain and snow", "みぞれ" },
            { "light shower snow", "弱いにわか雪" },
            { "shower snow", "にわか雪" },
            { "heavy shower snow", "強いにわか雪" },
            { "mist", "もや" },
            { "smoke", "煙っている" },
            { "haze", "煙霧" },
            { "sand, dust whirls", "砂塵旋風" },
            { "fog", "霧" },
            { "sand", "降砂" },
            { "dust", "降塵" },
            { "volcanic ash", "降灰" },
            { "squalls", "スコール" },
            { "tornado", "竜巻" },
            { "clear sky", "快晴" },
            { "Sky is Clear", "晴天" },
            { "few clouds", "晴れ" },
            { "scattered clouds", "千切れ雲" },
            { "broken clouds", "雲がち" },
            { "overcast clouds", "曇り" },
        };

        private static Dictionary<string, string> pref = new() {
            { "Hokkaido", "北海道" },
            { "Aomori", "青森県" },
            { "Iwate", "岩手県" },
            { "Miyagi", "宮城県" },
            { "Akita", "秋田県" },
            { "Yamagata", "山形県" },
            { "Fukushima", "福島県" },
            { "Ibaraki", "茨城県" },
            { "Tochigi", "栃木県" },
            { "Gunma", "群馬県" },
            { "Saitama", "埼玉県" },
            { "Chiba", "千葉県" },
            { "Tokyo", "東京都" },
            { "Kanagawa", "神奈川県" },
            { "Niigata", "新潟県" },
            { "Toyama", "富山県" },
            { "Ishikawa", "石川県" },
            { "Fukui", "福井県" },
            { "Yamanashi", "山梨県" },
            { "Nagano", "長野県" },
            { "Gifu", "岐阜県" },
            { "Shizuoka", "静岡県" },
            { "Aichi", "愛知県" },
            { "Mie", "三重県" },
            { "Shiga", "滋賀県" },
            { "Kyoto", "京都府" },
            { "Osaka", "大阪府" },
            { "Hyogo", "兵庫県" },
            { "Nara", "奈良県" },
            { "Wakayama", "和歌山県" },
            { "Tottori", "鳥取県" },
            { "Shimane", "島根県" },
            { "Okayama", "岡山県" },
            { "Hiroshima", "広島県" },
            { "Yamaguchi", "山口県" },
            { "Tokushima", "徳島県" },
            { "Kagawa", "香川県" },
            { "Ehime", "愛媛県" },
            { "Kochi", "高知県" },
            { "Fukuoka", "福岡県" },
            { "Saga", "佐賀県" },
            { "Nagasaki", "長崎県" },
            { "Kumamoto", "熊本県" },
            { "Oita", "大分県" },
            { "Miyazaki", "宮崎県" },
            { "Kagoshima", "鹿児島県" },
            { "Okinawa", "沖縄県" },
        };

        /// <summary>
        /// 天気予報を日本語にして返す
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string Description(string word) {
            return description.Where(x => x.Key == word).FirstOrDefault().Value;
        }

        public static string Pref(string word) {
            return pref.Where(x => x.Key == word).FirstOrDefault().Value;
        }
    }
}

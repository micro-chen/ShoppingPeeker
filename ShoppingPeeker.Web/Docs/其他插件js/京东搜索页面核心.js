define("search_new", ["jdf/1.0.0/unit/search/1.0.0/search", "jdf/1.0.0/unit/getjsonp/1.0.0/getjsonp", "product/module/clslog", "jdf/1.0.0/ui/lazyload/1.0.0/lazyload", "./search_plug.js?201710"], function(f, y, a) {
    if (!window.console) {
        window.console = {};
        window.console.log = window.console.error = function() {}
    }
    if (!window.SEARCH) {
        window.SEARCH = {}
    }
    window.json_city = {
        "0": {
            "1": "北京",
            "2": "上海",
            "3": "天津",
            "4": "重庆",
            "5": "河北",
            "6": "山西",
            "7": "河南",
            "8": "辽宁",
            "9": "吉林",
            "10": "黑龙江",
            "11": "内蒙古",
            "12": "江苏",
            "13": "山东",
            "14": "安徽",
            "15": "浙江",
            "16": "福建",
            "17": "湖北",
            "18": "湖南",
            "19": "广东",
            "20": "广西",
            "21": "江西",
            "22": "四川",
            "23": "海南",
            "24": "贵州",
            "25": "云南",
            "26": "西藏",
            "27": "陕西",
            "28": "甘肃",
            "29": "青海",
            "30": "宁夏",
            "31": "新疆",
            "32": "台湾",
            "52993": "港澳",
            "84": "钓鱼岛",
            "53283": "海外"
        },
        "1": {
            "72": "朝阳区",
            "2800": "海淀区",
            "2801": "西城区",
            "2802": "东城区",
            "2803": "崇文区",
            "2804": "宣武区",
            "2805": "丰台区",
            "2806": "石景山区",
            "2807": "门头沟",
            "2808": "房山区",
            "2809": "通州区",
            "2810": "大兴区",
            "2812": "顺义区",
            "2814": "怀柔区",
            "2816": "密云区",
            "2901": "昌平区",
            "2953": "平谷区",
            "3065": "延庆县"
        },
        "2": {
            "2813": "徐汇区",
            "2815": "长宁区",
            "2817": "静安区",
            "2820": "闸北区",
            "2822": "虹口区",
            "2823": "杨浦区",
            "2824": "宝山区",
            "2825": "闵行区",
            "2826": "嘉定区",
            "2830": "浦东新区",
            "2833": "青浦区",
            "2834": "松江区",
            "2835": "金山区",
            "2836": "南汇区",
            "2837": "奉贤区",
            "2841": "普陀区",
            "2919": "崇明县",
            "78": "黄浦区"
        },
        "3": {
            "51035": "东丽区",
            "51036": "和平区",
            "51037": "河北区",
            "51038": "河东区",
            "51039": "河西区",
            "51040": "红桥区",
            "51041": "蓟县",
            "51042": "静海县",
            "51043": "南开区",
            "51044": "塘沽区",
            "51045": "西青区",
            "51046": "武清区",
            "51047": "津南区",
            "51048": "汉沽区",
            "51049": "大港区",
            "51050": "北辰区",
            "51051": "宝坻区",
            "51052": "宁河县"
        },
        "4": {
            "113": "万州区",
            "114": "涪陵区",
            "115": "梁平县",
            "119": "南川区",
            "123": "潼南县",
            "126": "大足区",
            "128": "黔江区",
            "129": "武隆县",
            "130": "丰都县",
            "131": "奉节县",
            "132": "开县",
            "133": "云阳县",
            "134": "忠县",
            "135": "巫溪县",
            "136": "巫山县",
            "137": "石柱县",
            "138": "彭水县",
            "139": "垫江县",
            "140": "酉阳县",
            "141": "秀山县",
            "48131": "璧山县",
            "48132": "荣昌县",
            "48133": "铜梁县",
            "48201": "合川区",
            "48202": "巴南区",
            "48203": "北碚区",
            "48204": "江津区",
            "48205": "渝北区",
            "48206": "长寿区",
            "48207": "永川区",
            "50950": "江北区",
            "50951": "南岸区",
            "50952": "九龙坡区",
            "50953": "沙坪坝区",
            "50954": "大渡口区",
            "50995": "綦江区",
            "51026": "渝中区",
            "51027": "高新区",
            "51028": "北部新区",
            "4164": "城口县",
            "3076": "高新区"
        },
        "5": {
            "142": "石家庄市",
            "148": "邯郸市",
            "164": "邢台市",
            "199": "保定市",
            "224": "张家口市",
            "239": "承德市",
            "248": "秦皇岛市",
            "258": "唐山市",
            "264": "沧州市",
            "274": "廊坊市",
            "275": "衡水市"
        },
        "6": {
            "303": "太原市",
            "309": "大同市",
            "318": "阳泉市",
            "325": "晋城市",
            "330": "朔州市",
            "336": "晋中市",
            "350": "忻州市",
            "368": "吕梁市",
            "379": "临汾市",
            "398": "运城市",
            "3074": "长治市"
        },
        "7": {
            "412": "郑州市",
            "420": "开封市",
            "427": "洛阳市",
            "438": "平顶山市",
            "446": "焦作市",
            "454": "鹤壁市",
            "458": "新乡市",
            "468": "安阳市",
            "475": "濮阳市",
            "482": "许昌市",
            "489": "漯河市",
            "495": "三门峡市",
            "502": "南阳市",
            "517": "商丘市",
            "527": "周口市",
            "538": "驻马店市",
            "549": "信阳市",
            "2780": "济源市"
        },
        "8": {
            "560": "沈阳市",
            "573": "大连市",
            "579": "鞍山市",
            "584": "抚顺市",
            "589": "本溪市",
            "593": "丹东市",
            "598": "锦州市",
            "604": "葫芦岛市",
            "609": "营口市",
            "613": "盘锦市",
            "617": "阜新市",
            "621": "辽阳市",
            "632": "朝阳市",
            "6858": "铁岭市"
        },
        "9": {
            "639": "长春市",
            "644": "吉林市",
            "651": "四平市",
            "2992": "辽源市",
            "657": "通化市",
            "664": "白山市",
            "674": "松原市",
            "681": "白城市",
            "687": "延边州"
        },
        "10": {
            "727": "鹤岗市",
            "731": "双鸭山市",
            "737": "鸡西市",
            "742": "大庆市",
            "753": "伊春市",
            "757": "牡丹江市",
            "765": "佳木斯市",
            "773": "七台河市",
            "776": "黑河市",
            "782": "绥化市",
            "793": "大兴安岭地区",
            "698": "哈尔滨市",
            "712": "齐齐哈尔市"
        },
        "11": {
            "799": "呼和浩特市",
            "805": "包头市",
            "810": "乌海市",
            "812": "赤峰市",
            "823": "乌兰察布市",
            "835": "锡林郭勒盟",
            "848": "呼伦贝尔市",
            "870": "鄂尔多斯市",
            "880": "巴彦淖尔市",
            "891": "阿拉善盟",
            "895": "兴安盟",
            "902": "通辽市"
        },
        "12": {
            "904": "南京市",
            "911": "徐州市",
            "919": "连云港市",
            "925": "淮安市",
            "933": "宿迁市",
            "939": "盐城市",
            "951": "扬州市",
            "959": "泰州市",
            "965": "南通市",
            "972": "镇江市",
            "978": "常州市",
            "984": "无锡市",
            "988": "苏州市"
        },
        "13": {
            "2900": "济宁市",
            "1000": "济南市",
            "1007": "青岛市",
            "1016": "淄博市",
            "1022": "枣庄市",
            "1025": "东营市",
            "1032": "潍坊市",
            "1042": "烟台市",
            "1053": "威海市",
            "1058": "莱芜市",
            "1060": "德州市",
            "1072": "临沂市",
            "1081": "聊城市",
            "1090": "滨州市",
            "1099": "菏泽市",
            "1108": "日照市",
            "1112": "泰安市"
        },
        "14": {
            "1151": "黄山市",
            "1159": "滁州市",
            "1167": "阜阳市",
            "1174": "亳州市",
            "1180": "宿州市",
            "1201": "池州市",
            "1206": "六安市",
            "2971": "宣城市",
            "1114": "铜陵市",
            "1116": "合肥市",
            "1121": "淮南市",
            "1124": "淮北市",
            "1127": "芜湖市",
            "1132": "蚌埠市",
            "1137": "马鞍山市",
            "1140": "安庆市"
        },
        "15": {
            "1158": "宁波市",
            "1273": "衢州市",
            "1280": "丽水市",
            "1290": "台州市",
            "1298": "舟山市",
            "1213": "杭州市",
            "1233": "温州市",
            "1243": "嘉兴市",
            "1250": "湖州市",
            "1255": "绍兴市",
            "1262": "金华市"
        },
        "16": {
            "1303": "福州市",
            "1315": "厦门市",
            "1317": "三明市",
            "1329": "莆田市",
            "1332": "泉州市",
            "1341": "漳州市",
            "1352": "南平市",
            "1362": "龙岩市",
            "1370": "宁德市"
        },
        "17": {
            "1432": "孝感市",
            "1441": "黄冈市",
            "1458": "咸宁市",
            "1466": "恩施州",
            "1475": "鄂州市",
            "1477": "荆门市",
            "1479": "随州市",
            "3154": "神农架林区",
            "1381": "武汉市",
            "1387": "黄石市",
            "1396": "襄阳市",
            "1405": "十堰市",
            "1413": "荆州市",
            "1421": "宜昌市",
            "2922": "潜江市",
            "2980": "天门市",
            "2983": "仙桃市"
        },
        "18": {
            "4250": "耒阳市",
            "1482": "长沙市",
            "1488": "株洲市",
            "1495": "湘潭市",
            "1501": "衡阳市",
            "1511": "邵阳市",
            "1522": "岳阳市",
            "1530": "常德市",
            "1540": "张家界市",
            "1544": "郴州市",
            "1555": "益阳市",
            "1560": "永州市",
            "1574": "怀化市",
            "1586": "娄底市",
            "1592": "湘西州"
        },
        "19": {
            "1601": "广州市",
            "1607": "深圳市",
            "1609": "珠海市",
            "1611": "汕头市",
            "1617": "韶关市",
            "1627": "河源市",
            "1634": "梅州市",
            "1709": "揭阳市",
            "1643": "惠州市",
            "1650": "汕尾市",
            "1655": "东莞市",
            "1657": "中山市",
            "1659": "江门市",
            "1666": "佛山市",
            "1672": "阳江市",
            "1677": "湛江市",
            "1684": "茂名市",
            "1690": "肇庆市",
            "1698": "云浮市",
            "1704": "清远市",
            "1705": "潮州市"
        },
        "20": {
            "3168": "崇左市",
            "1715": "南宁市",
            "1720": "柳州市",
            "1726": "桂林市",
            "1740": "梧州市",
            "1746": "北海市",
            "1749": "防城港市",
            "1753": "钦州市",
            "1757": "贵港市",
            "1761": "玉林市",
            "1792": "贺州市",
            "1806": "百色市",
            "1818": "河池市",
            "3044": "来宾市"
        },
        "21": {
            "1827": "南昌市",
            "1832": "景德镇市",
            "1836": "萍乡市",
            "1842": "新余市",
            "1845": "九江市",
            "1857": "鹰潭市",
            "1861": "上饶市",
            "1874": "宜春市",
            "1885": "抚州市",
            "1898": "吉安市",
            "1911": "赣州市"
        },
        "22": {
            "2103": "凉山州",
            "1930": "成都市",
            "1946": "自贡市",
            "1950": "攀枝花市",
            "1954": "泸州市",
            "1960": "绵阳市",
            "1962": "德阳市",
            "1977": "广元市",
            "1983": "遂宁市",
            "1988": "内江市",
            "1993": "乐山市",
            "2005": "宜宾市",
            "2016": "广安市",
            "2022": "南充市",
            "2033": "达州市",
            "2042": "巴中市",
            "2047": "雅安市",
            "2058": "眉山市",
            "2065": "资阳市",
            "2070": "阿坝州",
            "2084": "甘孜州"
        },
        "23": {
            "3690": "三亚市",
            "3698": "文昌市",
            "3699": "五指山市",
            "3701": "临高县",
            "3702": "澄迈县",
            "3703": "定安县",
            "3704": "屯昌县",
            "3705": "昌江县",
            "3706": "白沙县",
            "3707": "琼中县",
            "3708": "陵水县",
            "3709": "保亭县",
            "3710": "乐东县",
            "3711": "三沙市",
            "2121": "海口市",
            "3115": "琼海市",
            "3137": "万宁市",
            "3173": "东方市",
            "3034": "儋州市"
        },
        "24": {
            "2144": "贵阳市",
            "2150": "六盘水市",
            "2155": "遵义市",
            "2169": "铜仁市",
            "2180": "毕节市",
            "2189": "安顺市",
            "2196": "黔西南州",
            "2205": "黔东南州",
            "2222": "黔南州"
        },
        "25": {
            "4108": "迪庆州",
            "2235": "昆明市",
            "2247": "曲靖市",
            "2258": "玉溪市",
            "2270": "昭通市",
            "2281": "普洱市",
            "2291": "临沧市",
            "2298": "保山市",
            "2304": "丽江市",
            "2309": "文山州",
            "2318": "红河州",
            "2332": "西双版纳州",
            "2336": "楚雄州",
            "2347": "大理州",
            "2360": "德宏州",
            "2366": "怒江州"
        },
        "26": {
            "3970": "阿里地区",
            "3971": "林芝地区",
            "2951": "拉萨市",
            "3107": "那曲地区",
            "3129": "山南地区",
            "3138": "昌都地区",
            "3144": "日喀则地区"
        },
        "27": {
            "2428": "延安市",
            "2442": "汉中市",
            "2454": "榆林市",
            "2468": "商洛市",
            "2476": "安康市",
            "2376": "西安市",
            "2386": "铜川市",
            "2390": "宝鸡市",
            "2402": "咸阳市",
            "2416": "渭南市"
        },
        "28": {
            "2525": "庆阳市",
            "2534": "陇南市",
            "2544": "武威市",
            "2549": "张掖市",
            "2556": "酒泉市",
            "2564": "甘南州",
            "2573": "临夏州",
            "3080": "定西市",
            "2487": "兰州市",
            "2492": "金昌市",
            "2495": "白银市",
            "2501": "天水市",
            "2509": "嘉峪关市",
            "2518": "平凉市"
        },
        "29": {
            "2580": "西宁市",
            "2585": "海东地区",
            "2592": "海北州",
            "2597": "黄南州",
            "2603": "海南州",
            "2605": "果洛州",
            "2612": "玉树州",
            "2620": "海西州"
        },
        "30": {
            "2628": "银川市",
            "2632": "石嘴山市",
            "2637": "吴忠市",
            "2644": "固原市",
            "3071": "中卫市"
        },
        "31": {
            "4110": "五家渠市",
            "4163": "博尔塔拉蒙古自治州阿拉山口口岸",
            "15945": "阿拉尔市",
            "15946": "图木舒克市",
            "2652": "乌鲁木齐市",
            "2654": "克拉玛依市",
            "2656": "石河子市",
            "2658": "吐鲁番地区",
            "2662": "哈密地区",
            "2666": "和田地区",
            "2675": "阿克苏地区",
            "2686": "喀什地区",
            "2699": "克孜勒苏州",
            "2704": "巴音郭楞州",
            "2714": "昌吉州",
            "2723": "博尔塔拉州",
            "2727": "伊犁州",
            "2736": "塔城地区",
            "2744": "阿勒泰地区"
        },
        "32": {
            "2768": "台湾市"
        },
        "52993": {
            "52994": "香港特别行政区",
            "52995": "澳门特别行政区"
        },
        "84": {
            "1310": "钓鱼岛"
        }
    };
    Array.prototype.unique = function() {
        var D = []
          , C = {};
        for (var B = 0, A = this.length; B < A; B++) {
            if (!C[this[B]]) {
                D.push(this[B]);
                C[this[B]] = 1
            }
        }
        return D
    }
    ;
    var g = window.history && window.history.pushState && window.history.replaceState && !navigator.userAgent.match(/((iPod|iPhone|iPad).+\bOS\s+[1-4]\D|WebApps\/.+CFNetwork)/);
    var s = function(A, C) {
        C = (((C || "") + "").toLowerCase().match(/<[a-z][a-z0-9]*>/g) || []).join("");
        var B = /<\/?([a-z][a-z0-9]*)\b[^>]*>/gi
          , D = /<!--[\s\S]*?-->|<\?(?:php)?[\s\S]*?\?>/gi;
        return A.replace(D, "").replace(B, function(F, E) {
            return C.indexOf("<" + E.toLowerCase() + ">") > -1 ? F : ""
        })
    };
    var d = function(B, A) {
        var C = new RegExp("(^|\\?|&)" + B + "=([^&]*)(\\s|&|$)","i");
        var D = A ? A : window.location.search;
        if (C.test(D)) {
            return RegExp.$2
        }
        return ""
    };
    var m = function(K, J, I) {
        var A, D, C;
        switch (arguments.length) {
        case 0:
            return "";
        case 1:
            A = window.location.pathname + window.location.search;
            D = K;
            C = "";
            break;
        case 2:
            A = K;
            D = J;
            C = "";
            break;
        case 3:
            A = K;
            D = J;
            C = I;
            break;
        default:
            break
        }
        if (typeof D == "string" && typeof C == "string") {
            var B = new RegExp("(^|\\?|&)" + D + "=([^&#]*)","gi");
            if (!C) {
                A = A.replace(B, "")
            } else {
                if (B.test(A)) {
                    A = A.replace(B, "$1" + D + "=" + C)
                } else {
                    var H = A.indexOf("#")
                      , G = "";
                    if (H != -1) {
                        G = A.substr(H);
                        A = A.substr(0, H)
                    }
                    A = A + "&" + D + "=" + C + G
                }
            }
        } else {
            if (D instanceof Array && typeof C == "string") {
                for (var F = 0, E = D.length; F < E; F++) {
                    A = m(A, D[F], C)
                }
            } else {
                if (D instanceof Array && C instanceof Array && D.length == C.length) {
                    for (var F = 0, E = D.length; F < E; F++) {
                        A = m(A, D[F], C[F])
                    }
                } else {
                    A = false
                }
            }
        }
        return A
    };
    var q = function(A, B, C) {
        $(A).unbind("click").bind("click", function(F) {
            var D = $(F.target)
              , E = D.attr("href");
            if (!E) {
                D = D.closest("a");
                E = D.attr("href")
            }
            if (E && E != "javascript:;") {
                window.location.href = m(E, B, C);
                return false
            }
        })
    };
    var j = function(A, B) {
        A = A.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
        return B == true ? A.replace(/'/g, "&#0*39;") : A
    };
    var x = function(A) {
        return !A ? "" : A.replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&amp;/g, "&").replace(/&quot;/g, '"').replace(/&#0*39;/g, "'")
    };
    var i = function(B, A, E) {
        var D = E
          , C = new Date;
        C.setTime(C.getTime() + 24 * D * 60 * 60 * 1000);
        document.cookie = B + "=" + escape(A) + ";expires=" + C.toGMTString() + ";path=/;domain=." + pageConfig.FN_getDomain()
    };
    var u = function(A, B) {
        if (typeof B != "object") {
            return ""
        }
        return A.replace(/{#(.*?)#}/g, function() {
            var C = arguments[1];
            if ("undefined" != typeof (B[C]) && B[C] != null) {
                return B[C]
            } else {
                return ""
            }
        })
    };
    var w = function(C, A, B) {
        B = B || "n7/";
        return "//img1" + (C % 5) + ".360buyimg.com/" + B + A
    };
    var e = function() {
        self == top ? window.scrollTo(0, $("#J_main").offset().top) : window.parent.scrollTo(0, $("#J_main").offset().top + 233)
    };
    var n = function(C, D) {
        var B = 0
          , A = function() {
            if (++B > 100) {
                return false
            }
            if (typeof (JA) == "object" && typeof (JA.tracker) == "object" && typeof (JA.tracker.ngloader) == "function") {
                JA.tracker.ngloader(C, D)
            } else {
                setTimeout(A, 50)
            }
        };
        A()
    };
    var z = function(A) {
        seajs.use(["jdf/1.0.0/unit/login/1.0.0/login.js"], function(B) {
            B({
                clstag1: "login|keycount|5|3",
                clstag2: "login|keycount|5|4",
                modal: true,
                complete: function() {
                    if (typeof (A) == "function") {
                        A()
                    }
                }
            })
        })
    };
    var t = {
        pop: function(A) {
            return A >= 1000000001 && A <= 1999999999 || A >= 10000000001 && A <= 99999999999 || A >= 200000000001 && A <= 209999999999
        },
        book: function(A) {
            return A >= 10000001 && A <= 19999999 || A >= 110000000001 && A <= 139999999999
        },
        mvd: function(A) {
            return A >= 20000001 && A <= 29999999 || A >= 140000000001 && A <= 149999999999
        }
    };
    var p = function() {
        var B = /^(https:)?\/\/(.*)\.(jd|360buy)\.com/i
          , A = [];
        $(".J_oneBoxFrame").each(function() {
            var C, D = $(this).attr("src") || $(this).attr("data-src");
            if (B.test(D)) {
                C = RegExp.$2;
                if (C == "life" && D.indexOf("initRestaurant") > -1) {
                    C = "dingzuo"
                } else {
                    if (C == "life" && D.indexOf("initTakeOut") > -1) {
                        C = "waimai"
                    }
                }
                A.push(C)
            } else {
                if (D.indexOf("//api.jd.yiche.com") > -1) {
                    A.push("yiche")
                }
            }
        });
        return A.join("$")
    };
    var r = function(C, A) {
        var B = "//gw.e.jd.com/downrecord/downrecord_insert.action?ebookId=" + C + "&key=" + A + "&callback=?";
        $.getJSON(B, function(D) {
            if (D.code == 1) {
                window.seajs.use(["jdf/1.0.0/ui/dialog/1.0.0/dialog"], function(E) {
                    $("body").dialog({
                        title: "下载",
                        hasButton: true,
                        source: "如您已安装京东LeBook客户端，请点击“确定”自动启动客户端<br /><br />如您尚未安装京东LeBook客户端，请点击“取消”将引导您免费安装客户端",
                        submitButton: "确定",
                        cancelButton: "取消",
                        onSubmit: function() {
                            $(".ui-dialog, .ui-mask").remove();
                            window.location = "LEBK:///Bought"
                        },
                        onCancel: function() {
                            $(".ui-mask").remove();
                            $("body").dialog({
                                title: "下载",
                                hasButton: true,
                                source: "是否安装？",
                                submitButton: "确定",
                                cancelButton: "取消",
                                onSubmit: function() {
                                    $(".ui-dialog, .ui-mask").remove();
                                    window.open("//e.jd.com/ebook/lebook_pc.aspx")
                                }
                            })
                        }
                    })
                })
            } else {
                alert(D.message)
            }
        })
    };
    var o = function(B, A) {
        window.seajs.use(["jdf/1.0.0/ui/dialog/1.0.0/dialog", "./script/digital_music_download_new"], function(D, C) {
            $("body").dialog({
                title: "下载",
                hasButton: true,
                source: "如您已安装京东LeMusic客户端，请点击“确定”自动启动客户端<br /><br />如您尚未安装京东LeMusic客户端，请点击“取消”将引导您免费安装客户端",
                submitButton: "确定",
                cancelButton: "取消",
                onSubmit: function() {
                    $(".ui-dialog, .ui-mask").remove();
                    var E = C.getProductType(A);
                    var F = "[a]user=" + B + "&productid=" + A + "&obtain=purchase&charset=gb2312&type=" + E + "[z]";
                    window.location = "LeMusic://" + C.encode64(F)
                },
                onCancel: function() {
                    $(".ui-mask").remove();
                    $("body").dialog({
                        title: "下载",
                        hasButton: true,
                        source: "是否安装？",
                        submitButton: "确定",
                        cancelButton: "取消",
                        onSubmit: function() {
                            $(".ui-dialog, .ui-mask").remove();
                            window.open("//app.music.jd.com/client_download.action")
                        }
                    })
                }
            })
        })
    };
    var k = function() {
        $("#J_oneboxTabs").find("a").click(function() {
            if ($(this).hasClass("selected")) {
                return
            }
            var A = $(this).index();
            var B = $(".onebox-tab-cnt").addClass("hide").eq(A).removeClass("hide").find(".J_oneBoxFrame");
            var C = B.attr("data-src");
            if (C) {
                B.removeAttr("data-src").attr("src", C)
            }
            $(this).addClass("selected").siblings().removeClass("selected")
        })
    };
    var b = function(C) {
        var B = location.pathname.match(/pinpai\/([12]-)?(\d+-)?(\d+)\.html$/);
        var A = location.pathname.match(/(writer|publish)\/(.+?)_\d+\.html$/);
        if (B) {
            if (C) {
                C = m(C, "brand_id", B[3])
            } else {
                C = "brand_id=" + B[3] + (B[1] && B[2] ? "&cid" + B[1].replace("-", "") + "=" + B[2].replace("-", "") : B[2] ? "&cid3=" + B[2].replace("-", "") : "")
            }
        } else {
            if (A) {
                C = m(C, ["keyword", "enc"], [$.browser.msie ? encodeURIComponent(decodeURIComponent(A[2])) : A[2], "utf-8"])
            }
        }
        return C
    };
    var v = function(A) {
        if (g) {
            window.history.pushState({}, "", window.location.pathname + "?" + A);
            SEARCH.load("s_new.php?" + b(A))
        } else {
            window.location.hash = A
        }
    };
    var h = function(D, B, A) {
        var A = A || ".J-picon-fix"
          , C = D.find(A);
        if (C.length) {
            C.last().after(B)
        } else {
            D.prepend(B)
        }
    };
    (function() {
        QUERY_KEYWORD = x(window.QUERY_KEYWORD);
        REAL_KEYWORD = x(window.REAL_KEYWORD);
        $("#key").val(QUERY_KEYWORD);
        if (typeof LogParm == "undefined") {
            LogParm = {
                ab: "0000",
                result_count: 0
            }
        }
        LogParm.rec_type = LogParm.rec_type || "0";
        LogParm.ev = LogParm.ev || 0;
        LogParm.cid = LogParm.cid || "";
        LogParm.psort = LogParm.psort || "";
        LogParm.page = LogParm.page || "1";
        LogParm.sig = LogParm.sig || "";
        LogParm.rel_cat2 = LogParm.rel_cat2 || "";
        LogParm.rel_cat3 = LogParm.rel_cat3 || "";
        LogParm.log_id = LogParm.log_id || "";
        LogParm.expand = LogParm.expand || "";
        LogParm.mtest = LogParm.mtest || ""
    })();
    window.searchlog = d("forcebot") ? function() {}
    : function() {
        var I = Array.prototype.slice.call(arguments, 0), A, K, F = I.length > 4 && I[0] == 1 && (I[3] == 52 || I[3] == 62) ? I.splice(4, 1, "")[0] : "";
        if (I[0] == "e") {
            K = LogParm.ekey;
            I.shift();
            I.splice(4, 1, QUERY_KEYWORD)
        } else {
            if (I[0] == 1 && window.REAL_KEYWORD) {
                K = window.REAL_KEYWORD;
                window.REAL_KEYWORD != QUERY_KEYWORD && I.splice(4, 1, QUERY_KEYWORD)
            } else {
                K = window.QUERY_KEYWORD;
                I[0] == 0 && window.REAL_KEYWORD && REAL_KEYWORD != QUERY_KEYWORD && I.splice(1, 1, REAL_KEYWORD)
            }
        }
        var C = "//sstat.jd.com/scslog?args="
          , D = I.length
          , J = "";
        var H = encodeURIComponent(K) + "^#psort#^#page#^#cid#^" + encodeURIComponent(window.location.href);
        var L = {
            keyword: K,
            ev: LogParm.ev,
            ab: LogParm.ab,
            mtest: LogParm.mtest,
            rel_ver: readCookie("rkv") || "",
            sig: LogParm.sig,
            rel_cat2: LogParm.rel_cat2,
            rel_cat3: LogParm.rel_cat3,
            logid: LogParm.log_id,
            loc: readCookie("ipLoc-djd") || "",
            referer: document.referrer,
            anchor: window.location.hash.substr(1)
        };
        if (D > 0) {
            if (I[0] == 0) {
                L.front_cost = LogParm.front_cost = LogParm.front_cost || "0";
                L.back_cost = LogParm.back_cost = LogParm.back_cost || "0";
                L.ip = LogParm.ip = LogParm.ip || "";
                L.rec_type = LogParm.rec_type;
                L.result_count = LogParm.result_count;
                L.word = I[1] || LogParm.word || "";
                A = C + LogParm.rec_type + "^" + H + "^^^" + LogParm.result_count + "^" + I[1] + "^" + LogParm.ev + "^" + LogParm.ab + "^" + LogParm.back_cost + "^" + LogParm.front_cost + "^" + LogParm.ip;
                A += "^" + encodeURIComponent(document.referrer);
                J += LogParm.expand
            } else {
                if (I[0] == 1) {
                    if (LogParm.rec_type != 10) {
                        A = C + "1^" + H + "^";
                        L.rec_type = 1
                    } else {
                        A = C + "11^" + H + "^";
                        L.rec_type = 11
                    }
                    for (var E = 1, G = Math.min(5, D); E < G; E++) {
                        A += encodeURI(I[E]) + "^"
                    }
                    if (D > 3) {
                        switch (parseInt(I[3])) {
                        case 51:
                            LogParm.cid = I[1];
                            break;
                        case 55:
                            LogParm.psort = I[1];
                            break;
                        case 56:
                            LogParm.page = I[1];
                            break;
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 81:
                        case 82:
                        case 83:
                            L.bhv = window.__behaivor__.getBehavior(String(I[1]), window.event || arguments.callee.caller.arguments[0]);
                        default:
                            L.wid = I[1];
                            break
                        }
                    }
                    if (D >= 5) {
                        L.word = I[4]
                    }
                    if (F) {
                        L.rel_key = F
                    }
                    L.pos = I[2];
                    L.type = I[3];
                    for (var E = 0, G = (5 - D); E < G; E++) {
                        A += "^"
                    }
                    A += LogParm.ev + "^" + LogParm.ab;
                    A += "^^^^" + encodeURIComponent(document.referrer);
                    J += D >= 6 && I[5] != "" ? LogParm.expand + "$" + I[5] : LogParm.expand
                }
            }
            A = A.replace("#cid#", LogParm.cid).replace("#psort#", LogParm.psort).replace("#page#", LogParm.page);
            A += "^" + LogParm.rel_cat2 + "^" + LogParm.rel_cat3 + "^" + LogParm.log_id + "^" + encodeURIComponent(J) + "^" + encodeURIComponent(LogParm.mtest);
            $.getScript(A + "&sig=" + encodeURIComponent(LogParm.sig));
            L.cid = LogParm.cid;
            L.sort = LogParm.psort;
            L.page = LogParm.page;
            J = J.split("$");
            for (var E = 0, D = J.length, B; E < D; E++) {
                B = J[E].indexOf("=");
                if (B > 0) {
                    L[J[E].substr(0, B)] = J[E].substr(B + 1)
                }
            }
            n("search.000001", L)
        }
    }
    ;
    window.call_free_download = function(C, A) {
        if (C && A) {
            z(function() {
                r(C, A)
            })
        } else {
            if (C && !A) {
                var B = readCookie("pin");
                if (B) {
                    z(function() {
                        o(B, C)
                    })
                }
            }
        }
    }
    ;
    searchUnit.userActionOnebox = function(A) {
        var B, C;
        switch (parseInt(A)) {
        case 1:
            B = "chongzhi";
            C = "order=1";
            break;
        case 2:
            B = "liuliang";
            C = "order=1";
            break;
        case 3:
            B = "jipiao";
            C = "order=0$foreign=0";
            break;
        case 4:
            B = "jipiao";
            C = "order=1";
            break;
        case 5:
            B = "jipiao";
            C = "order=0$foreign=1";
            break;
        case 6:
            B = "caipiao";
            C = "order=1";
            break;
        case 7:
            B = "caipiao";
            C = "order=0";
            break;
        case 8:
            B = "card";
            C = "order=1";
            break;
        case 9:
            B = "card";
            C = "order=0";
            break;
        default:
            B = "";
            C = "";
            break
        }
        if (B && C) {
            searchlog(1, 0, 0, 59, B, C)
        }
    }
    ;
    searchUnit.pageLoad = function(B, A) {
        window.location.href = "//search.jd.com/Search?keyword=" + encodeURIComponent(QUERY_KEYWORD) + "&enc=utf-8&qrst=1&accy_id=" + encodeURIComponent(B) + "&car_id=" + A;
        return false
    }
    ;
    searchUnit.shopFocus = function(C, D, B) {
        var A = B ? window.frames.shop_list.document : document;
        if (D == "do") {
            z(function() {
                $.getJSON("//follow-soa.jd.com/vender/follow?venderId=" + C + "&callback=?", function(E) {
                    typeof E == "object" && (E.data || E.code == "F0402") && $("#J_shop_focus_" + C, A).addClass("z-focused").mouseenter(function() {
                        $(this).addClass("z-focus-cancle").find("em").html("取消")
                    }).mouseleave(function() {
                        $(this).removeClass("z-focus-cancle").find("em").html("已关注")
                    }).find("em").html("已关注")
                })
            })
        } else {
            if (D == "undo") {
                z(function() {
                    window.seajs.use(["jdf/1.0.0/ui/dialog/1.0.0/dialog"], function(E) {
                        $("body").dialog({
                            title: "关注",
                            hasButton: true,
                            fixed: true,
                            width: 384,
                            extendMainClass: "dialog-confirm",
                            source: '<div class="m-tipbox2"><div class="tip-inner tip-warn"><div class="tip-title"><i class="tip-icon"></i><div class="title-main">是否取消关注该品牌店铺？</div></div></div></div>',
                            submitButton: "确定",
                            cancelButton: "取消",
                            onSubmit: function() {
                                $(".ui-dialog,.ui-mask").remove();
                                $.getJSON("//follow-soa.jd.com/vender/unfollow?venderId=" + C + "&callback=?", function(F) {
                                    typeof F == "object" && F.data == true && $("#J_shop_focus_" + C, A).removeClass("z-focused z-focus-cancle").unbind("mouseenter mouseleave").find("em").html("加关注")
                                })
                            },
                            onCancel: function() {
                                $(".ui-dialog,.ui-mask").remove()
                            }
                        })
                    })
                })
            } else {
                if (D == "check") {
                    $.getJSON("//follow-soa.jd.com/vender/batchIsFollow?venderIds=" + C + "&callback=?", function(F) {
                        if (typeof F == "object" && typeof F.data == "object" && F.success) {
                            for (var E in F.data) {
                                if (F.data[E]) {
                                    $("#J_shop_focus_" + E, A).addClass("z-focused").mouseenter(function() {
                                        $(this).addClass("z-focus-cancle").find("em").html("取消")
                                    }).mouseleave(function() {
                                        $(this).removeClass("z-focus-cancle").find("em").html("已关注")
                                    }).find("em").html("已关注")
                                }
                            }
                        }
                    })
                }
            }
        }
    }
    ;
    SEARCH.sync_iframe_height = function() {
        self != top && typeof parent.searchUnit == "object" && typeof parent.searchUnit.resizeOnebox == "function" && parent.searchUnit.resizeOnebox($(document.body).height() - 10, "promotion", function() {
            pageConfig.FN_ImgError(document);
            $("#J_main").lazyload({
                type: "img",
                placeholderClass: "err-product",
                delay: 20,
                space: 200
            })
        })
    }
    ;
    SEARCH.relate_search = {
        init: function() {
            var A = QUERY_KEYWORD
              , D = this
              , B = LogParm.ab
              , C = pageConfig.searchType == 9;
            if (A && location.hostname == "search.jd.com") {
                $.ajax({
                    url: "//qpsearch.jd.com/relationalSearch?keyword=" + encodeURIComponent(A).toLocaleLowerCase() + "&ver=" + (d("rel_ver") || "auto") + "&client=" + (C ? "cs" : "pc"),
                    async: true,
                    scriptCharset: "utf-8",
                    dataType: "jsonp",
                    success: function(E) {
                        D.callback(E, true, B, C)
                    }
                })
            } else {
                D.callback("", false, B, C)
            }
        },
        callback: function(F, L, N, J) {
            var F = typeof F == "string" ? F : ""
              , I = F.indexOf("^^")
              , K = I > -1 ? F.substr(0, I) : ""
              , G = F.substr(I > -1 ? I + 2 : 0).replace(/\*$/, "").split("*")
              , B = []
              , H = "";
            for (var E = 0, D = G.length; E < D; E++) {
                if ("" == G[E]) {
                    continue
                }
                B.push(G[E])
            }
            for (var E = 0, D = B.length; E < D; E++) {
                var A = B[E]
                  , C = E == 0 ? ' class="fore"' : ""
                  , M = E == D - 1 ? "" : "<b>|</b>";
                H += '<a onclick="searchlog(1,0,' + E + ",#{type},'" + A + '\')" href="Search?keyword=' + encodeURIComponent(A) + "&enc=utf-8" + (J ? "&market=1" : "") + "&spm=2.1." + E + '"' + C + ">" + A + "</a>" + M
            }
            if (H) {
                $("#hotwords").html(H.replace(/#{type}/g, 52));
                $(".search-ext .fg-line-value").html(H.replace(/#{type}/g, 62))
            } else {
                $.ajax({
                    url: "//dc.3.cn/cathot/get2",
                    dataType: "jsonp",
                    scriptCharset: "gbk",
                    success: function(O) {
                        if (typeof O != "object" || !O.data) {
                            return
                        }
                        for (var T = 0, S = Math.min(O.data.length, 8), R = "", Q = [], P; T < S; T++) {
                            P = O.data[T];
                            if (P.n && P.u) {
                                R += '<a href="' + P.u + '" target="_blank"' + (T == 0 ? ' style="color:#f30213;"' : "") + ">" + P.n + "</a><b>|</b>"
                            }
                        }
                        $("#hotwords").html(R.substr(0, R.length - 8))
                    }
                });
                $(".search-ext").hide()
            }
            $("#hotwords").addClass("haveline");
            $("#key").addClass("blurcolor").bind("focus", function() {
                $(this).addClass("defcolor").removeClass("blurcolor")
            }).bind("blur", function() {
                $(this).addClass("blurcolor").removeClass("defcolor")
            });
            if (L) {
                n("search.000008", {
                    keyword: QUERY_KEYWORD,
                    ab: N,
                    from: K,
                    num: D,
                    word: B.join("*")
                });
                document.cookie = "rkv=" + K + ";path=/;domain=.search.jd.com"
            }
        }
    };
    SEARCH.get_shop_info = function() {
        var A = [];
        $("span.m-focus").not('[data-done="1"]').each(function() {
            this.setAttribute("data-done", "1");
            A.push(this.getAttribute("data-shopid"))
        });
        A.length && $.getJSON("shop_new.php?ids=" + A.join(","), function(F) {
            if (typeof F == "object" && F.length) {
                for (var E = 0, D = F.length, G, B, C; E < D; E++) {
                    G = $("#J_shop_focus_" + F[E].shop_id).parent().parent().parent();
                    B = "";
                    C = F[E].shop_brief || F[E].summary;
                    G.find("img").attr("src", F[E].shop_logo ? F[E].shop_logo : "//misc.360buyimg.com/product/search/0.0.9/css/i/shop-def.png").removeClass();
                    G.find(".shop-name a").html(F[E].shop_name);
                    G.find(".shop-infor").eq(0).html("主营品牌：" + F[E].main_brand).next().html(C ? "店铺简介：" + C : "");
                    if (F[E].icon == 1) {
                        B += '<a class="shop-tag-img" title="京东品质认证商家"><img src="//img11.360buyimg.com/uba/jfs/t3319/144/278979502/1525/f89d43fe/580484b5Ncf61e7b9.png" alt="品质认证" width="72" height="18"></a>'
                    }
                    if (F[E].vender_type == 1) {
                        B += '<em class="shop-act-tag tag-jd">京东自营</em>'
                    }
                    B && G.find(".shop-name").append(B);
                    F[E].vender_total_score == 0 ? G.find(".J_total_score").html("-") : G.find(".J_total_score").html(F[E].vender_total_score / 100);
                    F[E].vender_ware_score == 0 ? G.find(".J_ware_score").html("-") : G.find(".J_ware_score").html(F[E].vender_ware_score / 100).after(F[E].vender_ware_score >= F[E].industry_total_score ? '<i class="i-up"></i>' : '<i class="i-down"></i>');
                    F[E].vender_service_score == 0 ? G.find(".J_service_score").html("-") : G.find(".J_service_score").html(F[E].vender_service_score / 100).after(F[E].vender_service_score >= F[E].industry_service_score ? '<i class="i-up"></i>' : '<i class="i-down"></i>');
                    F[E].vender_effective_score == 0 ? G.find(".J_effective_score").html("-") : G.find(".J_effective_score").html(F[E].vender_effective_score / 100).after(F[E].vender_effective_score >= F[E].industry_effective_score ? '<i class="i-up"></i>' : '<i class="i-down"></i>');
                    G.find(".m-focus").click(function() {
                        searchUnit.shopFocus($(this).attr("data-shopid"), $(this).hasClass("z-focused") ? "undo" : "do", 0)
                    })
                }
            }
        });
        A.length && searchUnit.shopFocus(A.join(","), "check", 0)
    }
    ;
    SEARCH.get_digital_price = function(B, A) {
        this.enable_price && seajs.use("product/module/tools", function(C) {
            C.getPrice(B.replace(/J_/g, "").split(","), pageConfig.price_pdos_off, function(F) {
                if (typeof F == "object") {
                    for (var E = 0, D = F.length, G = ""; E < D; E++) {
                        if (F[E].p < 0) {
                            G = "<i>暂无报价</i>"
                        } else {
                            if (F[E].p == 0) {
                                G = "<i>免费</i>"
                            } else {
                                G = "<em>￥</em><i>" + F[E].p + "</i>"
                            }
                        }
                        if (A) {
                            $("em." + F[E].id).html(s(G))
                        } else {
                            $("strong." + F[E].id).html(G)
                        }
                    }
                }
            }, {
                ext: "11",
                pin: decodeURIComponent(readCookie("pin") || "")
            })
        })
    }
    ;
    SEARCH.get_ware_stock = function(A, H, K) {
        if (!this.enable_stock) {
            return
        }
        var I = readCookie("__jda")
          , I = I && I.indexOf(".") > -1 ? I.split(".")[1] : ""
          , B = readCookie("pin") || ""
          , E = (readCookie("ipLoc-djd") || "").replace(/\..*$/, "").split("-")
          , G = E.slice(0, 4).join("-")
          , J = this.enable_stock == 2
          , L = [];
        A = H != -1 && A ? A.substr(2).split(",J_") : A;
        if (A) {
            C(A, "sku", 30)
        } else {
            F([], "sku")
        }
        function F(V, T) {
            var N = $("#J_main").find("a[data-stock]"), X, W, O, Q, M, S = [], U;
            for (var R = 0, P = N.length; R < P; R++) {
                X = N.eq(R),
                W = X.attr("data-stock"),
                O = V[W],
                Q = t.book(W) || t.mvd(W);
                (T == "sku" || O) && X.removeAttr("data-stock");
                if (T == "sku" && X.attr("data-sv") == 34) {
                    (H == "1" || H == "2") && X.hasClass("J_notification") && Q && S.push(W);
                    H == 3 && J && (U = X.closest("li").attr("data-spu")) && L.push(U) && X.attr("data-stock", U)
                } else {
                    if (T == "spu" && O && O.a == 34) {
                        X.parent().siblings(".p-stock").css("display", "block")
                    }
                }
            }
            if (T == "sku") {
                L.length && C(L, "spu", 5);
                S.length && $.getJSON("coupon.php?type=sale&sku=" + S.join(","), function(Z) {
                    if (typeof Z != "object") {
                        return
                    }
                    for (var Y in Z) {
                        if (Z[Y] != 0) {
                            continue
                        }
                        if (H == "2") {
                            $("#J_store_" + Y).replaceWith('<a class="p-o-btn addcart disabled"><i></i>加入购物车</a>')
                        } else {
                            $("#J_store_" + Y).before('<a href="javascript:;" class="' + (H == "1" ? "p-o-btn addcart disabled" : "disabled") + '"><i></i>加入购物车</a>').remove()
                        }
                    }
                })
            }
        }
        function D(M) {
            if (!M || typeof M != "object" || typeof K != "function") {
                return false
            }
            var Q = E[0] ? window.json_city[0][E[0]] : ""
              , N = [];
            for (var O in M) {
                var P = M[O]["u"] == 1 ? 33 : parseInt(M[O]["a"]);
                if (P == 0 || P == 18 || P == 34) {
                    N.push(O)
                }
            }
            K(N, Q)
        }
        function C(N, M, O) {
            do {
                $.ajax({
                    url: "//ss.3.cn/ss/" + (M == "sku" ? "areaStockState" : "areaStockSpuState") + "/mget?app=search_pc&ch=1&" + (M == "sku" ? "skuNum" : "spuNum") + "=" + N.splice(0, O).join(";") + "&area=" + G.replace(/-/g, ",") + "&pduid=" + I + "&pdpin=" + B,
                    async: true,
                    dataType: "jsonp",
                    success: function(P) {
                        H == -1 ? D(P) : F(P, M)
                    }
                })
            } while (N.length)
        }
    }
    ;
    SEARCH.get_icon_info = function() {
        var G = []
          , B = []
          , C = {}
          , E = $("#J_goodsList").find(".p-img div[data-catid]").not('[data-done="1"]');
        E.each(function() {
            var I = $(this).closest("li").attr("data-sku")
              , H = $(this).data();
            H.venid && G.push(H.venid);
            H.presale == "1" && B.push(I) && (C[I] = $(this).parent().parent());
            this.setAttribute("data-done", "1")
        });
        G.length && $.getJSON("//baozhang.jd.com/service/getAllInsure?venderIds=" + G.unique().join("-") + "&callback=?", function(J) {
            if (typeof J == "object") {
                for (var I = 0, H = J.length; I < H; I++) {
                    J[I].yFX == "1" && E.filter('[data-venid="' + J[I].venderId + '"]').each(function() {
                        $(this).parent().parent().find(".p-icons").append('<i class="goods-icons2 J-picon-tips" data-tips="退换货免运费">险</i>')
                    })
                }
            }
        });
        if (B.length) {
            var F = false
              , A = function(I, H) {
                I = I.toString();
                return I.length >= H ? I : A("0" + I, H)
            }
              , D = function(K) {
                var I = Math.floor(K / 86400);
                K -= I * 86400;
                var H = Math.floor(K / 3600);
                K -= H * 3600;
                H = A(H, 2);
                var L = Math.floor(K / 60);
                K -= L * 60;
                L = A(L, 2);
                var J = A(K, 2);
                return I > 0 ? "剩余" + I + "天" + H + "时" + L + "分" : "剩余" + H + "时" + L + "分" + J + "秒"
            };
            $.getJSON("//yushou.jd.com/youshouinfoList.action?sku=" + B.join(",") + "&callback=?", function(U) {
                if (typeof U == "object") {
                    for (var S in U) {
                        var O = $.parseJSON(U[S]);
                        if (typeof (O) != "object" || O.type != 1 && O.type != 2) {
                            continue
                        }
                        var R, I, M, T, J, P = C[S];
                        if (O.type == 1) {
                            I = O.d;
                            switch (parseInt(O.state)) {
                            case 1:
                                M = "预约未开始";
                                break;
                            case 2:
                                M = "预约中";
                                break;
                            case 3:
                                M = "抢购未开始";
                                break;
                            case 4:
                                M = "抢购中";
                                break;
                            case 5:
                                M = "抢购结束";
                                break;
                            default:
                                return false
                            }
                        } else {
                            R = O.ret;
                            I = R.d;
                            M = R.s == "0" ? "预售未开始" : "预售中";
                            if (R.t == "2" && R.sa) {
                                var H = '<div class="p-presell-stage clearfix">';
                                for (var N = 0, L = R.sa.length, K; N < L; N++) {
                                    K = (N + 1) < R.cs ? " timeout" : (N + 1) == R.cs ? " curr" : "";
                                    H += '<span class="item' + K + '"><a href="javascript:void(0)"><em>满' + R.sa[N].c + "人</em>";
                                    H += "<strong>￥" + R.sa[N].m + '</strong></a><i class="bottom"><em></em></i></span>'
                                }
                                H += "</div>";
                                P.find(".p-name").after(H)
                            }
                            J = P.find(".p-price").find("strong i");
                            if (R.hideRealPrice == 1 || R.hidePrice == 1 && R.cs == 3) {
                                J.html("待发布")
                            } else {
                                if (R.cp) {
                                    J.html(R.expAmount > 0 && R.depositWorth > 0 && R.oriPrice > 0 ? R.oriPrice : R.cp)
                                }
                            }
                            delete C[S]
                        }
                        T = '<div class="p-presell-time" data-time="' + I + '"><i></i><span>' + M + "</span><em>" + D(I) + "</em></div>";
                        P.append(T).parent().addClass("gl-item-presell");
                        F = true
                    }
                }
                for (var N in C) {
                    var Q = $("strong.J_" + N).filter("[data-price]");
                    Q.html("<em>￥</em><i>" + Q.attr("data-price") + "</i>").removeAttr("data-price")
                }
            })
        }
    }
    ;
    SEARCH.get_prompt_adwords = function(A) {
        this.enable_prom_adwords && $.getJSON("//ad.3.cn/ads/mgets?source=search_pc&skuids=" + A.replace(/J_/g, "AD_") + "&callback=?", function(E) {
            if (!E) {
                return
            }
            for (var D = 0, B = E.length; D < B; D++) {
                var G = E[D].id || ""
                  , F = $("#J_" + G)
                  , C = s(E[D].ad);
                if (F.length && C !== "") {
                    F.html(C).parent().attr("title", C).closest("li").find(".p-img>a").attr("title", C)
                }
            }
        })
    }
    ;
    SEARCH.get_prompt_flag = function(A) {
        this.enable_prom_flag && $.getJSON("//pf.3.cn/flags/mgets?source=search_pc&skuids=" + A + "&callback=?", function(E) {
            if (!E || typeof E !== "object") {
                return
            }
            for (var F = 0, C = E.length, B = [], J = []; F < C; F++) {
                var G = E[F]
                  , D = $("#J_pro_" + G.pid)
                  , H = I(G.pf, G.pfi, D, G.pid);
                h(D, H)
            }
            if (B.length || J.length) {
                n("search.000002", {
                    logid: LogParm.log_id,
                    key: QUERY_KEYWORD,
                    err55: B.join(","),
                    err59: J.join(",")
                })
            }
            function I(X, O, T, U) {
                if (!X) {
                    return
                }
                var K = T.data("promotion"), P, Q;
                if (K && O) {
                    P = true;
                    K = K.split("^");
                    Q = K[1].substr(0, 3);
                    for (var M = 0, L = O.length; M < L; M++) {
                        if (O[M].indexOf(K[1] + "_") == 0) {
                            P = false;
                            break
                        }
                    }
                    P && (Q == "55-" ? B.push(U) : Q == "59-" ? J.push(U) : 0)
                }
                for (var S = 0, N = X.length, V = [], W, Y; S < N; S++) {
                    switch (X[S]) {
                    case 5:
                        V[1] = '<i class="goods-icons4 J-picon-tips" data-tips="购买本商品送赠品">赠</i>';
                        break;
                    case 55:
                        if (!V[0]) {
                            W = "本商品参与满减促销";
                            Y = "满减";
                            if (P == false && Q == "55-") {
                                if (K[0].length > 10) {
                                    W = K[0]
                                } else {
                                    Y = K[0]
                                }
                            } else {
                                if (P == undefined && T.siblings(".p-promo-flag").length == 0) {
                                    B.push(U);
                                    P = true
                                }
                            }
                            V[0] = '<i class="goods-icons4 J-picon-tips" data-tips="' + W + '">' + Y + "</i>"
                        }
                        break;
                    case 58:
                        if (!V[1]) {
                            V[1] = '<i class="goods-icons4 J-picon-tips" data-tips="满指定金额即赠热销商品">满赠</i>'
                        }
                        break;
                    case 59:
                        if (P == false && Q == "59-") {
                            V[0] = '<i class="goods-icons4 J-picon-tips" data-tips="本商品参与满件促销">' + K[0] + "</i>"
                        } else {
                            if (P == undefined && T.siblings(".p-promo-flag").length == 0) {
                                J.push(U);
                                P = true
                            }
                        }
                        break;
                    default:
                        break
                    }
                }
                for (var S = 0, R = []; S < 2; S++) {
                    V[S] && R.push(V[S])
                }
                return R.join("")
            }
        })
    }
    ;
    SEARCH.get_comment_nums = function(A) {
        $.getJSON("//club.jd.com/comment/productCommentSummaries.action?referenceIds=" + A.replace(/J_/g, "") + "&callback=?", function(E) {
            if (typeof (E) != "object" || typeof (E.CommentsCount) != "object") {
                return false
            }
            for (var D = 0, C = E.CommentsCount, B = C.length; D < B; D++) {
                if (typeof (C[D].CommentCountStr) != "undefined") {
                    $("#J_comment_" + C[D].SkuId).html(C[D].CommentCountStr)
                }
            }
        })
    }
    ;
    SEARCH.get_im_info = function(B, A) {
        if (!B || !A) {
            return false
        }
        $.ajax({
            url: "//chat1.jd.com/api/checkChat?pidList=" + A,
            dataType: "jsonp",
            jsonp: "callback",
            scriptCharset: "utf-8",
            success: function(K) {
                if (typeof (K) != "object") {
                    return false
                }
                var D = $("#store-selector").find(".text").text()
                  , E = function(U, T, R) {
                    U = U == undefined ? "" : $.trim(U);
                    var S = U.match(T);
                    R = R == undefined ? 1 : R;
                    return S === null ? "" : S[R]
                };
                for (var I = 0, H = K.length; I < H; I++) {
                    var Q = K[I]
                      , M = ""
                      , O = B[Q.pid]
                      , F = O.attr("data-selfware") == "1";
                    if (Q.code == 1) {
                        M = '<b class="' + (F ? "im-02" : "im-01") + '" title="联系' + (F ? "供应商" : "第三方卖家") + '进行咨询" onclick="searchlog(1,' + Q.shopId + ',0,61)"></b>'
                    } else {
                        if (Q.code == 3) {
                            M = '<b class="im-offline" title="' + (F ? "供应商" : "第三方卖家") + '客服不在线，可留言" onclick="searchlog(1,' + Q.shopId + ',0,61)"></b>'
                        } else {
                            continue
                        }
                    }
                    var G = {};
                    var L = $.trim(O.siblings(".p-stock").html());
                    L = L == "暂不支持配送" ? L : !L ? "有货" : L.substr(L.length - 2);
                    G.stock = D + "（" + L + "）";
                    G.pid = Q.pid;
                    G.score = O.attr("data-score");
                    G.evaluationRate = O.attr("data-reputation");
                    G.commentNum = O.siblings(".p-commit").find("a").html();
                    var P = O.siblings(".p-img").find("img").eq(0);
                    var J = P.attr("src");
                    if (J == undefined || J == "//misc.360buyimg.com/lib/img/e/blank.gif") {
                        J = P.attr("data-lazy-img")
                    }
                    G.imgUrl = E(J, /http\S+?\.com\/\w+?\/(\S+)/);
                    G.wname = O.siblings(".p-name").find("em").html().replace(/<span[\s\S]+?<\/span>|<font class="skcolor_ljg">|<\/font>/g, "");
                    G.advertiseWord = $.trim(O.siblings(".p-name").find("i.promo-words").html());
                    G.seller = $.trim(Q.seller);
                    G.venderId = Q.venderId;
                    G.entry = "jd_search";
                    var C = "//" + Q.chatDomain + "/index.action?";
                    for (var N in G) {
                        C += N + "=" + encodeURI(encodeURI(G[N])) + "&"
                    }
                    O.find("span.J_im_icon,a.curr-shop").append(M).find("b").click((function(R) {
                        return function() {
                            window.open(R);
                            return false
                        }
                    })(C))
                }
            }
        })
    }
    ;
    SEARCH.get_shop_name = function(I) {
        var H = this, G = $("#J_main").find("div.p-shop,div.p-shopnum").not('[data-done="1"]'), B = G.length, J = [], A, F = {};
        if (!B) {
            return false
        }
        for (var C = 0, E = [], D; C < B; C++) {
            A = G[C].getAttribute("data-shopid");
            A && E.push(A);
            D = G.eq(C).closest("li[data-sku]").attr("data-sku");
            J.push(D);
            F[D] = G.eq(C);
            G[C].setAttribute("data-done", "1")
        }
        if (E.length) {
            $.getJSON("shop_new.php", {
                ids: E.unique().join(",")
            }, function(M) {
                if (typeof M != "object") {
                    return
                }
                for (var L = 0, K = M.length, O = {}; L < K; L++) {
                    O[M[L].shop_id] = M[L]
                }
                for (var L = 0; L < B; L++) {
                    var P = G.eq(L)
                      , N = O[P.attr("data-shopid")];
                    if (!N) {
                        continue
                    }
                    I == "2" && P.removeAttr("data-shopid").find("a.curr-shop").replaceWith('<a class="curr-shop" target="_blank" onclick="searchlog(1,' + N.shop_id + ',0,58)" href="//mall.jd.com/index-' + N.shop_id + '.html" title="' + N.shop_name + '">' + N.shop_name + "</a>");
                    (I == "1" || I == "3") && P.removeAttr("data-shopid").html('<span class="J_im_icon"><a target="_blank" onclick="searchlog(1,' + N.shop_id + ',0,58)" href="//mall.jd.com/index-' + N.shop_id + '.html" title="' + N.shop_name + '">' + N.shop_name + "</a></span>")
                }
                H.get_im_info(F, J.unique().join(","))
            })
        } else {
            H.get_im_info(F, J.unique().join(","))
        }
    }
    ;
    SEARCH.get_ware_info = function() {
        var B = []
          , D = []
          , E = $("#J_main")
          , A = E.find("ul.gl-warp").attr("data-tpl")
          , C = [];
        E.find("strong[class^='J_']").not('[data-done="1"]').each(function() {
            this.setAttribute("data-done", "1");
            B.push(this.className)
        });
        E.find("em[class^='J_']").not('[data-done="1"]').each(function() {
            this.setAttribute("data-done", "1");
            C.push(this.className)
        });
        E.find("div.p-icons").not('[data-done="1"]').each(function() {
            this.setAttribute("data-done", "1");
            D.push(this.id.replace("pro_", ""))
        });
        B.length && this.get_digital_price(B.join(","), 0);
        this.get_ware_stock("", A);
        this.get_shop_info();
        if (D.length) {
            D = D.join(",");
            this.get_prompt_adwords(D);
            !this.is_exchange_list && this.get_prompt_flag(D);
            !this.is_exchange_list && this.get_comment_nums(D)
        }
        this.get_icon_info();
        this.get_shop_name(A);
        C.length && this.get_digital_price(C.join(","), 1)
    }
    ;
    SEARCH.get_diviner_ware = function() {
        var C = []
          , G = this
          , F = G.cid
          , D = $("#J_goodsList")
          , B = (readCookie("ipLoc-djd") || "").split("-")
          , E = B[0] ? window.json_city[0][B[0]] : ""
          , A = $(window).width() >= 1390 ? 10 : 8;
        D.find("li[data-sku]").slice(0, 4).each(function() {
            C.push(this.getAttribute("data-sku"))
        });
        $.ajax({
            url: "//diviner.jd.com/diviner?p=907006&skus=" + C.join(",") + "&uuid=&pin=" + (readCookie("pin") || "") + "&c3=" + F + "&lid=" + B[0] + "&lim=" + A + "&ec=utf-8",
            dataType: "jsonp",
            success: function(Q) {
                if (typeof (Q) != "object" || !Q.success) {
                    return
                }
                var I = '<div class="m-tipbox"><div class="tip-inner"><div class="tip-text">根据上面的商品结果，为您推荐的相似商品。</div></div></div><ul class="gl-warp clearfix J_diviner">', N = D.find("ul[data-tpl]").attr("data-tpl"), O, K = function(T) {
                    var S = new Image();
                    T = T + "&m=UA-J2011-1&ref=" + encodeURIComponent(document.referrer) + "&random=" + Math.random();
                    S.setAttribute("src", T)
                };
                var H = '<div class="p-scroll"><span class="ps-prev">&lt;</span><span class="ps-next">&gt;</span><div class="ps-wrap"><ul class="ps-main"><li class="ps-item"><a href="javascript:;" class="curr"><img data-sku="{#sku#}" width="25" height="25" data-lazy-img="{#img_url#}"></a></li></ul></div></div>'
                  , P = '<div class="p-shop" data-selfware="0" data-score="5" data-reputation="100" data-done="1">{#shop#}</div>'
                  , J = '<div class="p-shopnum" data-selfware="1" data-score="5" data-reputation="100" data-done="1"><span class="curr-shop">{#shop#}</span></div>';
                O = '<li data-sku="{#sku#}" class="gl-item" data-clk="{#clk#}" data-pos="{#pos#}"><div class="gl-i-wrap"><div class="p-img"><a target="_blank" title="{#t#}" href="//item.jd.com/{#sku#}.html"><img data-img="1" data-lazy-img="{#img_url#}"></a></div>' + (N == "3" ? H : "") + '<div class="p-price"><strong class="J_{#sku#}"><em>￥</em><i>{#jp#}</i></strong></div><div class="p-name"><a target="_blank" title="{#t#}" href="//item.jd.com/{#sku#}.html"><em>{#t#}</em><i class="promo-words" id="J_AD_{#sku#}"></i></a></div><div class="p-commit"><strong>已有<a id="J_comment_{#sku#}" target="_blank" href="//item.jd.com/{#sku#}.html#comment">0</a>人评价</strong></div>' + (N == "3" ? '<div class="p-focus"><a class="J_focus" data-sku="{#sku#}" href="javascript:;" title="点击关注"><i></i>关注</a></div>' + P : "") + '<div class="p-icons" id="J_pro_{#sku#}"></div>' + (N == "2" ? J : "");
                if (N == "1" || N == "2") {
                    O += '<div class="p-operate">' + (N == "1" ? '<a class="p-o-btn contrast J_contrast" data-sku="{#sku#}" href="javascript:;"><i></i>对比</a>' : "") + '<a class="p-o-btn focus J_focus" data-sku="{#sku#}" href="javascript:;"><i></i>关注</a><a class="p-o-btn addcart" data-stock="{#sku#}" data-sv="1" data-disable-notice="0" data-presale="0" href="//gate.jd.com/InitCart.aspx?pid={#sku#}&pcount=1&ptype=1" target="_blank"><i></i>加入购物车</a></div>'
                }
                O += "</div></li>";
                var R = Q.data.length;
                for (var M = 0; M < R; M++) {
                    var L = Q.data[M];
                    L.index = L.sku % 5;
                    L.pos = M;
                    L.shop = t.pop(L.sku) ? " 第三方商家" : " 京东自营";
                    if (N == "2") {
                        L.img_url = w(L.sku, L.img, "cms/s200x200_")
                    } else {
                        L.img_url = w(L.sku, L.img, D.hasClass("gl-type-2") ? "n8/" : "n7/")
                    }
                    I += u(O, L)
                }
                D.append(I + "</ul>");
                window.searchUnit && window.searchUnit.setImgLazyload && window.searchUnit.setImgLazyload("ul.J_diviner");
                window.seajs.use("product/search/" + G.ui_ver + "/js/goodsList", function(S) {
                    S.init()
                });
                G.get_ware_info();
                D.find("ul.J_diviner>li").click(function(T) {
                    var U = $(this)
                      , S = T.target.nodeName;
                    if ((S == "A" || S == "IMG") && !$(T.target).parents(".p-scroll").length || S == "FONT" || S == "EM" || S == "I") {
                        K(U.attr("data-clk"));
                        JA.tracker.ngloader("search.000003", {
                            logid: LogParm.log_id,
                            logtype: 1,
                            key: QUERY_KEYWORD,
                            reco_type: "tj",
                            result_count: R,
                            pos: U.attr("data-pos")
                        })
                    }
                });
                K(Q.impr);
                n("search.000003", {
                    logid: LogParm.log_id,
                    logtype: 0,
                    key: QUERY_KEYWORD,
                    reco_type: "tj",
                    result_count: R,
                    pos: 0
                })
            }
        })
    }
    ;
    SEARCH.load = function(C, A) {
        if (this.loading) {
            return false
        } else {
            this.loading = true
        }
        var E = this
          , D = d("click", C)
          , B = A ? 1 : D == E.click ? 2 : 3;
        if ($("#J_viewType").attr("data-ref") != "1") {
            C = m(C, "vt", E.view_type)
        }
        $.ajax({
            url: m(C, "cs", B == 3 ? "y" : "").replace(/[\s&]*$/g, ""),
            timeout: 15000,
            error: function() {
                E.load_error = true;
                if (B == 1) {
                    $("#J_scroll_loading").addClass("notice-loading-error").find("span").html('加载失败，请<a href="javascript:void(0)" onclick="SEARCH.load(\'' + C + '\',true)"><font color="blue">重试</font></a>')
                } else {
                    $("#J_loading").find("span").css("background", "none").html("加载失败，请<a href=\"javascript:void(0)\" onclick=\"$('#J_loading').remove();SEARCH.load('" + C + '\')"><font color="blue">重试</font></a>')
                }
            },
            beforeSend: function() {
                if (B == 1) {
                    $("#J_scroll_loading").removeClass("notice-loading-error").find("span").html("正在加载中，请稍后~~")
                } else {
                    $("#J_filter").after('<div id="J_loading" class="notice-filter-loading"><div class="nf-l-wrap"><span>正在加载中，请稍后~</span></div></div>');
                    B == 3 && e()
                }
            },
            success: function(G) {
                if (B == 1) {
                    $("#J_scroll_loading").remove();
                    var F = E.is_shop ? $("#J_shopList") : $("#J_goodsList").find("ul.gl-warp");
                    F.append(G)
                } else {
                    if (B == 2) {
                        $("#J_filter").nextAll().remove(),
                        $("#J_filter").after(G)
                    } else {
                        var H = $("#J_main").find(".m-aside").find(".J_adv_tuiguang_exposal").remove().end().html();
                        $("#J_searchWrap").html(G).find(".m-aside").html(H);
                        e();
                        window.seajs.use(["product/search/" + E.ui_ver + "/js/selector"], function(I) {
                            I.init()
                        });
                        E.bind_events.init()
                    }
                }
                E.success_js(B)
            },
            complete: function() {
                E.loading = false
            }
        })
    }
    ;
    SEARCH.success_js = function(A) {
        delete this.load_error;
        this.get_ware_info();
        window.searchUnit.setImgLazyload("#J_main");
        window.seajs.use("product/search/" + this.ui_ver + "/js/goodsList", function(B) {
            B.init()
        });
        if (this.is_exchange_list && window.searchUnit && window.searchUnit.setGoodsChecked) {
            window.searchUnit.setGoodsChecked()
        }
        if (A != 1) {
            window.searchUnit.initAside()
        }
        searchlog(0, 0)
    }
    ;
    SEARCH.page_html = function(F, N, K, M, I, D, J, E) {
        this.current_page = F;
        this.next_start = I;
        this.prev_start = D;
        this.advware_count = J;
        this.promotion_count = E;
        if (M) {
            this.enable_twice_loading = 1;
            F = Math.ceil(F / 2);
            N = Math.ceil(N / 2);
            var C = 2 * F - 3
              , H = 2 * F + 1
        } else {
            this.enable_twice_loading = 0;
            var C = F - 1
              , H = F + 1
        }
        var G = '<span class="fp-text"><b>' + F + "</b><em>/</em><i>" + N + "</i></span>";
        if (F <= 1) {
            G += '<a class="fp-prev disabled" href="javascript:;">&lt;</a>'
        } else {
            G += '<a class="fp-prev" onclick="SEARCH.page(' + C + ')" href="javascript:;" title="使用方向键左键也可翻到上一页哦！">&lt;</a>'
        }
        if (F >= N) {
            G += '<a class="fp-next disabled" href="javascript:;">&gt;</a>'
        } else {
            G += '<a class="fp-next" onclick="SEARCH.page(' + H + ')" href="javascript:;" title="使用方向键右键也可翻到下一页哦！">&gt;</a>'
        }
        $("#J_topPage").html(G);
        $("#J_resCount").html(K);
        if (N <= 1) {
            return ""
        }
        var A = F - 2
          , B = Math.min(N, F + 2)
          , G = '<span class="p-num">';
        if (B < 7) {
            B = Math.min(7, N)
        } else {
            A = B - 4
        }
        if (F <= 1) {
            G += '<a class="pn-prev disabled"><i>&lt;</i><em>上一页</em></a>'
        } else {
            G += '<a class="pn-prev" onclick="SEARCH.page(' + C + ', true)" href="javascript:;" title="使用方向键左键也可翻到上一页哦！"><i>&lt;</i><em>上一页</em></a>'
        }
        for (var L = 1; L <= N; L++) {
            if (L <= 2 || L >= A && L <= B) {
                G += L == F ? '<a href="javascript:;" class="curr">' + L + "</a>" : '<a onclick="SEARCH.page(' + (M ? 2 * L - 1 : L) + ', true)" href="javascript:;">' + L + "</a>"
            } else {
                if (L < A) {
                    G += '<b class="pn-break">...</b>';
                    L = A - 1
                } else {
                    if (L > B) {
                        G += '<b class="pn-break">...</b>';
                        break
                    }
                }
            }
        }
        if (F >= N) {
            G += '<a class="pn-next disabled"><em>下一页</em><i>&gt;</i></a>'
        } else {
            G += '<a class="pn-next" onclick="SEARCH.page(' + H + ', true)" href="javascript:;" title="使用方向键右键也可翻到下一页哦！"><em>下一页</em><i>&gt;</i></a>'
        }
        G += '</span><span class="p-skip"><em>共<b>' + N + '</b>页&nbsp;&nbsp;到第</em><input class="input-txt" type="text" value="' + F + '" onkeydown="javascript:if(event.keyCode==13){SEARCH.page_jump(' + N + "," + M + ');return false;}"><em>页</em><a class="btn btn-default" onclick="SEARCH.page_jump(' + N + "," + M + ')" href="javascript:;">确定</a></span>';
        $("#J_bottomPage").html(G)
    }
    ;
    SEARCH.page = function(F, C) {
        F = parseInt(F, 10);
        if (F < 1) {
            F = 1
        }
        C && e();
        var H, E = Math.min, B = this.enable_twice_loading ? 1 : 2, D = 30 * B, G = 4 * B, A = 2 * B;
        if (F == 1) {
            H = 1
        } else {
            if (F < this.current_page) {
                H = this.prev_start - (this.current_page - F) * D + E(this.advware_count, (this.current_page - 1) * G) - E(this.advware_count, (F - 1) * G) + E(this.promotion_count, (this.current_page - 1) * A) - E(this.promotion_count, (F - 1) * A)
            } else {
                if (F == this.current_page) {
                    H = this.prev_start
                } else {
                    H = this.next_start + (F - this.current_page - 1) * D - E(this.advware_count, (F - 1) * G) + E(this.advware_count, this.current_page * G) - E(this.promotion_count, (F - 1) * A) + E(this.promotion_count, this.current_page * A)
                }
            }
        }
        v(this.base_url + "&page=" + F + "&s=" + H + "&click=" + this.click);
        searchlog(1, F, 0, 56)
    }
    ;
    SEARCH.sort_html = function(C) {
        C = C || "";
        if (C == "0") {
            C = ""
        }
        var B = ""
          , A = '<a href="javascript:;" class="#class#" onclick="#click#">#name#</a>'
          , D = class_name = "";
        if (C == "") {
            class_name = "curr"
        } else {
            D = "SEARCH.sort('')"
        }
        B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", "综合<i></i>");
        class_name = D = "";
        if (C == "3") {
            class_name = "curr"
        } else {
            D = "SEARCH.sort('3')"
        }
        B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", "销量<i></i>");
        class_name = D = "";
        if (C == "4" || C == "11") {
            class_name = "curr"
        } else {
            D = "SEARCH.sort('" + (this.comment_6m ? 11 : 4) + "')"
        }
        B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", "评论数<i></i>");
        if ($("ul.gl-warp").attr("data-tpl") == "2") {
            class_name = D = "";
            if (C == "6") {
                class_name = "curr"
            } else {
                D = "SEARCH.sort('6')"
            }
            B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", "出版时间<i></i>")
        } else {
            class_name = D = "";
            if (C == "5") {
                class_name = "curr"
            } else {
                D = "SEARCH.sort('5')"
            }
            B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", "新品<i></i>")
        }
        if (this.is_promotion) {
            class_name = D = "";
            if (C == "9") {
                class_name = "curr"
            } else {
                D = "SEARCH.sort('9')"
            }
            B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", "降价幅度<i></i>");
            class_name = D = "";
            if (C == "10") {
                class_name = "curr"
            } else {
                D = "SEARCH.sort('10')"
            }
            B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", "降价金额<i></i>")
        }
        if (C == "2") {
            class_name = "curr";
            D = "SEARCH.sort('1')"
        } else {
            if (C == "1") {
                class_name = "curr";
                D = "SEARCH.sort('2')"
            } else {
                class_name = "";
                D = "SEARCH.sort('2')"
            }
        }
        B += A.replace("#class#", class_name).replace("#click#", D).replace("#name#", '<span class="fs-tit">价格</span><em class="fs-' + (C == "1" ? "down" : "up") + '"><i class="arrow-top"></i><i class="arrow-bottom"></i></em>');
        B = B.replace(/>(.+?)<i><\/i><\/a>/g, '><span class="fs-tit">$1</span><em class="fs-down"><i class="arrow"></i></em></a>');
        $("#J_filter").find("div.f-sort").html(B)
    }
    ;
    SEARCH.sort = function(A) {
        A = A || "";
        if (A == "0") {
            A = ""
        }
        v(m(this.base_url, "psort", A) + "&click=" + this.click);
        searchlog(1, A, 0, 55);
        q("#J_selector", "psort", A)
    }
    ;
    SEARCH.exchange_filter = function(D, C) {
        var A = window.event || arguments.callee.caller.arguments[0]
          , B = $(A.target ? A.target : A.srcElement).parent();
        if (B.hasClass("selected")) {
            return false
        }
        v(m(this.base_url, D ? "cid3" : ["cid2", "cid3"], D ? "" + D : ""));
        B.addClass("selected").siblings().removeClass("selected");
        searchlog(1, D, C, 51)
    }
    ;
    SEARCH.page_jump = function(B, A) {
        var C = parseInt($("#J_bottomPage").find("input").val(), 10);
        if (isNaN(C)) {
            C = 1
        }
        if (C > B) {
            C = B
        }
        this.page(A ? 2 * C - 1 : C, true)
    }
    ;
    SEARCH.scroll = function() {
        var E = this.current_page + 1, D = $("#J_goodsList"), C = [], B, F, A = "s_new.php?" + b(this.base_url) + "&page=" + E + "&s=" + this.next_start + "&scrolling=y&log_id=" + LogParm.log_id + "&tpl=";
        if (this.is_shop) {
            B = "3_M"
        } else {
            B = D.find("ul.gl-warp").attr("data-tpl") + (D.hasClass("gl-type-2") ? "_L" : "_M")
        }
        A += B;
        if (E % 2 == 0) {
            F = B.indexOf("3_") == 0 ? "data-pid" : "data-sku";
            D.find("ul.gl-warp > li[" + F + "]").each(function() {
                C.push(this.getAttribute(F))
            });
            A += "&show_items=" + C.join(",")
        }
        this.load(A, true);
        searchlog(1, E, 0, 56)
    }
    ;
    SEARCH.bind_events = {
        iplocation: function(A) {
            seajs.use(["jdf/1.0.0/ui/switchable/1.0.0/switchable", "jdf/1.0.0/ui/area/1.0.0/area"], function() {
                $("#J_store_selector").area({
                    scopeLevel: 4,
                    hasCommonAreas: 0,
                    hasOversea: 1,
                    repLevel: 0,
                    hasCssLink: 0,
                    onChange: function(C, B) {
                        window.location.href = window.location.pathname + "?" + m(A.base_url, ["stock", "dt"]) + "&" + Math.random() + "#J_main";
                        return false
                    }
                })
            })
        },
        async_category: function(A) {
            var B = b(A.base_url);
            if (A.c_category) {
                $.get("category.php?" + m(B, ["ev", "psort"]) + (A.p_category ? "&c=all" : ""), function(C) {
                    $("#J_crumbsBar").find('ul[data-level="c"]').append(C).find("li").length > 30 && $("#J_crumbsBar").find('ul[data-level="c"]').parent().addClass("menu-drop-xl")
                })
            }
            if (A.p_category) {
                $.get("category.php?" + m(B, ["ev", "psort", "cid1", "cid2", "cid3"]) + "&cid2=" + A.p_category, function(C) {
                    $("#J_crumbsBar").find('ul[data-level="p"]').append(C)
                })
            }
        },
        price_select: function(D) {
            var B = $("#J_selectorPrice")
              , A = B.find("input")
              , C = A.eq(0).val()
              , H = A.eq(1).val()
              , F = "#ccc"
              , E = "#333"
              , G = "¥";
            if (!A.length) {
                return
            }
            A.keypress(function(J) {
                var I = J.keyCode || J.charCode;
                if (I && (I < 48 || I > 57) && I != 46 && I != 8 && I != 37 && I != 39) {
                    J.preventDefault()
                }
            }).focus(function() {
                if ($(this).val() == G) {
                    $(this).val("").css("color", E)
                }
            }).blur(function(L) {
                var J = $(this)
                  , I = $.trim(J.val())
                  , K = new RegExp("^[0-9]+(.[0-9]{2})?$","g");
                if (!K.test(I)) {
                    J.val(G).css("color", F)
                }
                L.stopPropagation()
            });
            B.find(".J-price-confirm").click(function(M, N) {
                var L = parseInt(A.eq(0).val(), 10)
                  , I = parseInt(A.eq(1).val(), 10)
                  , J = $(this).attr("data-url");
                if (N == "cancle") {
                    J = J.replace("exprice_min-max%40", "").replace("exprice_min-max%5E", "")
                } else {
                    if (!isNaN(L) && !isNaN(I)) {
                        if (L > I) {
                            var K = L;
                            L = I;
                            I = K
                        }
                        searchlog(1, 0, 0, 22, "价格::" + L + "-" + I);
                        J = J.replace("min", L).replace("max", I)
                    } else {
                        if (!isNaN(L)) {
                            searchlog(1, 0, 0, 22, "价格::" + L + "gt");
                            J = J.replace("min", L).replace("-max", "gt")
                        } else {
                            if (!isNaN(I)) {
                                searchlog(1, 0, 0, 22, "价格::0-" + I);
                                J = J.replace("min", 0).replace("max", I)
                            } else {
                                return false
                            }
                        }
                    }
                }
                window.location.href = J;
                return false
            });
            B.find(".J-price-cancle").click(function() {
                A.val(G).css("color", F)
            });
            $("#J_filter").find(".fdg-item").mouseenter(function() {
                var K = $(this).attr("data-range").match(/(\d+)-(\d*)/)
                  , J = K[1]
                  , I = K[2];
                A.eq(0).val(J);
                A.eq(1).val(I);
                A.css("color", E)
            }).mouseleave(function() {
                A.eq(0).val(C);
                A.eq(1).val(H);
                C == G && A.eq(0).css("color", F);
                H == G && A.eq(1).css("color", F)
            }).click(function() {
                B.find(".J-price-confirm").trigger("click", [$(this).hasClass("fdg-item-curr") ? "cancle" : ""])
            });
            B.filter(".f-price").mouseenter(function() {
                $(this).addClass("f-price-focus")
            }).mouseleave(function() {
                $(this).removeClass("f-price-focus")
            })
        },
        condition_filter: function(A) {
            $("#J_feature,#J_location").find("a").click(function(C) {
                var B = $(this).attr("data-field");
                if (!B) {
                    return false
                }
                if ($(this).hasClass("selected")) {
                    $(this).removeClass("selected");
                    v(m(A.base_url, B) + "&click=" + (A.click + 1))
                } else {
                    $(this).addClass("selected");
                    v(m(A.base_url, B, $(this).attr("data-val")) + "&click=" + (A.click + 1))
                }
                return false
            })
        },
        view_type: function(A) {
            A.view_type = d("vt", A.base_url);
            $("#J_viewType").find("a").click(function() {
                if ($(this).hasClass("selected")) {
                    return false
                }
                $(this).addClass("selected").siblings().removeClass("selected");
                var B = A.view_type = $(this).attr("data-value");
                if ($(this).parent().attr("data-ref") == "1") {
                    v(m(A.base_url, "vt", B) + "&click=" + (A.click + 1))
                } else {
                    if (B == "1") {
                        $("#J_goodsList").attr("class", $("#J_goodsList").attr("class").replace("gl-type-4", "gl-type-5")).find('li[data-type="activity"]').hide()
                    } else {
                        $("#J_goodsList").attr("class", $("#J_goodsList").attr("class").replace("gl-type-5", "gl-type-4")).find('li[data-type="activity"]').show()
                    }
                }
                return false
            })
        },
        research: function(C) {
            var A = "在结果中搜索"
              , D = d("exp_key", C.base_url)
              , B = function(E) {
                var F = $.trim(E.val());
                if ((F != "" || D) && F != A) {
                    searchlog(1, 0, 0, 27);
                    window.location.href = "?" + m(C.base_url, "exp_key", encodeURIComponent(F)) + "#J_crumbsBar"
                }
            };
            $("#J_filter").find(".f-search a").click(function(E) {
                B($(this).prev())
            }).prev().focus(function() {
                if ($.trim($(this).val()) == A) {
                    $(this).val("")
                }
            }).blur(function() {
                if ($.trim($(this).val()) == "" && D == "") {
                    $(this).val(A)
                }
            }).keydown(function(E) {
                if (E.keyCode == 13) {
                    B($(this))
                }
            })
        },
        init: function() {
            var B = window.SEARCH;
            for (var A in this) {
                A != "init" && this[A](B)
            }
        }
    };
    SEARCH.init = function(H, J, G, D, I, B, E, C, F, A) {
        B && this.get_diviner_ware(),
        this.get_ware_info(),
        this.page_html(H, J, G, I, E, C, F, A),
        this.sort_html(D),
        this.bind_events.init(),
        this.sync_iframe_height(),
        k()
    }
    ;
    (function(A, B) {
        if (g) {
            A.onpopstate = function(D) {
                var C = b(A.location.search.substr(1));
                C && B.load("s_new.php?" + C)
            }
        } else {
            if (typeof (B.is_correct_hash) == "function") {
                $(A).hashchange(function() {
                    var C = B.get_real_hash();
                    if (!C || C == "J_searchWrap") {
                        C = A.location.search.substr(1)
                    }
                    C = b(C);
                    B.is_correct_hash(C) && B.load("s_new.php?" + C)
                })
            }
        }
    })(window, SEARCH);
    var l = null;
    $(window).scroll(function() {
        clearTimeout(l);
        l = setTimeout(function() {
            var A = $("#J_scroll_loading");
            if (SEARCH.loading || SEARCH.load_error || !SEARCH.enable_twice_loading || !A.length || A.offset().top - 600 > $(window).height() + $(window).scrollTop()) {
                return false
            }
            SEARCH.scroll()
        }, 20)
    }).load(function() {
        var A = JSTiming.getTimes();
        A.oReady = c;
        A.oLoad = new Date().getTime() - jdpts._st;
        n("search.000010", A)
    });
    var c;
    $(document).keyup(function(D) {
        var E = document.activeElement.tagName.toLowerCase();
        if (E == "input" || E == "textarea") {
            return
        }
        var A = 0
          , D = D || event
          , C = $("#J_filter")
          , B = 0;
        A = D.keyCode || D.which || D.charCode;
        if (C.length) {
            B = C.offset().top
        }
        switch (A) {
        case 37:
            $("#J_bottomPage a.pn-prev").trigger("click");
            break;
        case 39:
            $("#J_bottomPage a.pn-next").trigger("click");
            break;
        default:
            break
        }
    }).ready(function() {
        c = new Date().getTime() - jdpts._st;
        SEARCH.relate_search.init();
        searchlog(0, p());
        log(2, 1, QUERY_KEYWORD)
    });
    a.exports = SEARCH
});

//验证数字(整数、浮点数都可以通过)
checkfloat = function (oNum) {
    if (!oNum) return false;
    var strP = /^(-)?\d+(\.\d+)?$/;
    if (!strP.test(oNum)) return false;
    try {
        if (parseFloat(oNum) != oNum) return false;
    } catch (ex) {
        return false;
    }
    return true;
};
JSToStringV = function JSToStringV(val) {

    if (!val) { return "" }
    if (val == "undefined") { return "" }
    if (val == "null") { return "" }
    if (val) { return val.replace(/^\s+|\s+$/gm, ''); }
    return val;

};
JSToIntV = function JSToIntV(val) {

    if (!val) { return 0 }
    val = parseInt(val)
    return val;

};
JSToFloatV = function JSToIntV(val) {

    if (!val) { return 0 }
    if (val == "undefined") { return 0 }
    val = parseFloat(val)
    if (!val) { return 0 }
    return val;

};

JSAddToJson = function JSAddToJson(jsonstr, id, value) {
    if (typeof value === "undefined") {
        // 删除属性
        delete jsonstr[id];
    }
    else {
        // 添加 或 修改
        jsonstr[id] = value;
    }
};
//存储Json对象
var SxJsonSession = {
    set: function (key, value) {
        window.sessionStorage.setItem(key, JSON.stringify(value));
    },
    get: function (key) {
        var temp;
        //console.log(window.sessionStorage.getItem(key));
        if (window.sessionStorage.getItem(key))
            temp = JSON.parse(window.sessionStorage.getItem(key));
        else
            temp = null;
        window.sessionStorage.removeItem(key);//取值后清理掉
        return temp;
    }
};

//只提取汉字  
getChinese = function getChinese(strValue) {
    if (strValue != null && strValue != "") {
        var reg = /[\u4e00-\u9fa5]/g;
        return strValue.match(reg).join("");
    }
    else
        return "";
}
//去掉汉字  
removeChinese = function removeChinese(strValue) {
    if (strValue != null && strValue != "") {
        var reg = /[\u4e00-\u9fa5]/g;
        return strValue.replace(reg, "");
    }
    else
        return "";
}
//去掉a-z
removeAZ = function removeAZ(strValue) {
    if (strValue != null && strValue != "") {
        var reg = /[a-zA-Z]/g;
        return strValue.replace(reg, "");
    }
    else return "";
}
//去掉特殊字符
removeStringT = function removeStringT(strValue) {
    if (strValue != null && strValue != "") {
        strValue = strValue.replace(/[\'\"\\\/\b\f\n\r\t]/g, '');// 去掉转义字符 
        strValue = strValue.replace(/[\-\_\,\!\|\~\`\(\)\#\$\%\^\&\*\{\}\:\;\"\L\<\>\?]/g, '');// 去掉特殊字符
        return strValue;
    }
    else return "";
}
//去掉特殊字符汉字、a-z 只剩下数字信息
getOnlyNumber = function getOnlyNumber(strValue) {
    if (strValue != null && strValue != "") {
        strValue = removeAZ(strValue);
        strValue = removeChinese(strValue);
        strValue = removeStringT(strValue);
        //strValue = strValue.replace("-", "");
        //strValue = strValue.replace("+", "");
        return strValue;
    }
    else return "";
}
getUUID = function getUUID() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}
//求两个数字之和
getSum = function getSum(str1, str2) {
    return parseFloat(str1) + parseFloat(str2);
}

//关闭
closeUIForm = function closeUIForm() {
    parent.$('.page-tabs-content').find('.active i').trigger("click");
}
/////////////////////////////业务通用JS////////////////////////////////////////////////////////////


//加载数据字典通用方法
LoadDataItemValue = function LoadDataItemValue(inputName, EnCode,fnChange) {
    var obj = $("#" + inputName).ComboBox({
        url: "../../SystemManage/DataItemDetail/GetDataItemListJson",
        param: { EnCode: EnCode },
        id: "ItemValue",
        text: "ItemName",
        description: "==请选择==",
        height: "200px"
    });
    if (fnChange) {
        obj.on("change", function (e) {
            fnChange(e);
        });
    }
}
//数据字典，select加载
LoadDataItemValue_Select = function LoadDataItemValue_Select(inputName, EnCode) {
    $.get("/SystemManage/DataItemDetail/GetDataItemListJson?EnCode=" + EnCode, function (re) {
        var html = "<option value=''>==请选择==</option>";
        $.each(re, function (i, row) {
            html += "<option value='" + row.ItemValue + "'>" + row.ItemName + "</option>";
        });
        $("#" + inputName).html(html);
    }, "json");
}
//加载车间数据
LoadFactoryValue = function LoadFactoryValue(inputName) {
    $("#" + inputName).ComboBox({
        url: "../../BaseManage/Department/GetFactoryList",
        param: { organizeId: "" },
        id: "EnCode",
        text: "EnCode",
        description: "==请选择==",
        height: "200px"
    });
}
//车间数据：select加载
LoadFactoryValue_Select = function LoadFactoryValue_Select(inputName) {
    $.get("/BaseManage/Department/GetFactoryList", { organizeId: "" }, function (re) {
        var html = "<option value=''>==请选择==</option>";
        $.each(re, function (i, row) {
            html += "<option value='" + row.EnCode + "'>" + row.EnCode + "</option>";
        });
        $("#" + inputName).html(html);
    }, "json");
}
//加载 供应商信息
LoadSpuValue = function LoadSpuValue(inputName, itemId) {
    $("#" + inputName).ComboBox({
        url: "../../BaseManage/Supplier/GetDataGridJsonIng",
        param: { itemId: itemId },
        id: "spuCode",
        text: "spuName",
        description: "==请选择==",
        height: "200px"
    });
}
//加载数据字典数据
LoadDataItemJsonData = function LoadDataItemJsonData(EnCode, fn) {
    $.get("/SystemManage/DataItemDetail/GetDataItemListJson?EnCode=" + EnCode, function (re) {
        fn(re);
    }, "json");
}
///////////////////////////////平板通用JS///////////////////////////////////
//加载数据字典数据
LoadDataItemJsonData_Pad=function LoadDataItemJsonData_Pad(EnCode, fn) {
    $.get("/ProductionManager/PadBanCi/GetDataItemListJson?EnCode=" + EnCode, function (re) {
        fn(re);
    }, "json");
}
//加载车间数据
LoadFactoryValue_Pad=function LoadFactoryValue_Pad(inputName) {
    $("#" + inputName).ComboBox({
        url: "/ProductionManager/PadBanCi/GetFactoryList",
        param: { organizeId: "" },
        id: "EnCode",
        text: "EnCode",
        description: "==请选择==",
        height: "200px"
    });
}
//车间数据：select加载
LoadFactoryValue_Select_Pad = function LoadFactoryValue_Select_Pad(inputName) {
    $.get("/ProductionManager/PadBanCi/GetFactoryList", { organizeId: "" }, function (re) {
        var html = "<option value=''>==请选择==</option>";
        $.each(re, function (i, row) {
            html += "<option value='" + row.EnCode + "'>" + row.EnCode + "</option>";
        });
        $("#" + inputName).html(html);
    }, "json");
}
//加载数据字典通用方法
LoadDataItemValue_Pad=function LoadDataItemValue_Pad(inputName, EnCode, fnChange) {
    var obj = $("#" + inputName).ComboBox({
        url: "/ProductionManager/PadBanCi/GetDataItemListJson",
        param: { EnCode: EnCode },
        id: "ItemValue",
        text: "ItemName",
        description: "==请选择==",
        height: "200px"
    });
    if (fnChange) {
        obj.on("change", function (e) {
            fnChange(e);
        });
    }
}
//数据字典，select加载
LoadDataItemValue_Select_Pad = function LoadDataItemValue_Select_Pad(inputName, EnCode) {
    $.get("/ProductionManager/PadBanCi/GetDataItemListJson?EnCode=" + EnCode, function (re) {
        var html = "<option value=''>==请选择==</option>";
        $.each(re, function (i, row) {
            html += "<option value='" + row.ItemValue + "'>" + row.ItemName + "</option>";
        });
        $("#" + inputName).html(html);
    }, "json");
}